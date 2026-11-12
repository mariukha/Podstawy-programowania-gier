using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class TopDownCamera : MonoBehaviour
{
    public PlayerMovement player;
    public Camera playerCamera;
    public float moveSpeed = 10f;
    public float zoomSpeed = 2f;
    public float minHeight = 5f;
    public float maxHeight = 30f;
    public float angle = 60f;

    Camera cam;
    PlayerCamera playerCameraScript;
    Transform marker;
    bool active;
    float height = 14f;

    void Start()
    {
        cam = GetComponent<Camera>();
        playerCameraScript = playerCamera.GetComponent<PlayerCamera>();
        transform.rotation = Quaternion.Euler(angle, 0f, 0f);

        GameObject m = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m.name = "TargetMarker";
        Destroy(m.GetComponent<Collider>());
        m.transform.localScale = new Vector3(0.6f, 0.02f, 0.6f);
        m.GetComponent<Renderer>().material.color = new Color(1f, 0.55f, 0.1f);
        marker = m.transform;
        marker.gameObject.SetActive(false);

        SetMode(false);
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (kb.vKey.wasPressedThisFrame) SetMode(!active);
        if (!active) return;

        Vector3 dir = Vector3.zero;
        if (kb.wKey.isPressed) dir.z += 1f;
        if (kb.sKey.isPressed) dir.z -= 1f;
        if (kb.dKey.isPressed) dir.x += 1f;
        if (kb.aKey.isPressed) dir.x -= 1f;
        transform.position += dir.normalized * moveSpeed * Time.deltaTime;

        float scroll = mouse.scroll.ReadValue().y;
        float newHeight = Mathf.Clamp(height - scroll * zoomSpeed, minHeight, maxHeight);
        transform.position += transform.forward * (height - newHeight) / Mathf.Sin(angle * Mathf.Deg2Rad);
        height = newHeight;

        if (mouse.rightButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 300f) && hit.normal.y > 0.7f)
            {
                player.SetTarget(hit.point);
                marker.position = hit.point + Vector3.up * 0.02f;
            }
        }
        marker.gameObject.SetActive(player.HasTarget);
    }

    void SetMode(bool topDown)
    {
        active = topDown;
        cam.enabled = topDown;
        playerCamera.enabled = !topDown;
        playerCameraScript.enabled = !topDown;
        player.topDownMode = topDown;
        player.ClearTarget();
        marker.gameObject.SetActive(false);

        Cursor.lockState = topDown ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = topDown;

        if (topDown)
        {
            Vector3 p = player.transform.position;
            float back = height / Mathf.Tan(angle * Mathf.Deg2Rad);
            transform.position = new Vector3(p.x, p.y + height, p.z - back);
        }
    }
}
