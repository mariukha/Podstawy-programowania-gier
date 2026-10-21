using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitivity = 0.15f;
    public float zoomSpeed = 0.5f;
    public float minDistance = 1.5f;
    public float maxDistance = 8f;

    float distance = 3.5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;

        float mouseX = mouse.delta.ReadValue().x;
        transform.parent.Rotate(0f, mouseX * mouseSensitivity, 0f);

        float scroll = mouse.scroll.ReadValue().y;
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);
        transform.localPosition = new Vector3(0f, 1f + distance * 0.3f, -distance);
        transform.LookAt(transform.parent.position + Vector3.up * 1.5f);
    }
}
