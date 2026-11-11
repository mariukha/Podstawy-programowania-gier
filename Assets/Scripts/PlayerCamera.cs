using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitivity = 0.15f;
    public float zoomSpeed = 0.5f;
    public float minDistance = 1.5f;
    public float maxDistance = 8f;
    public float aimDistance = 1.8f;
    public float aimSide = 0.8f;

    float distance = 3.5f;
    float current = 3.5f;
    float side;

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

        bool aiming = mouse.rightButton.isPressed;
        current = Mathf.Lerp(current, aiming ? aimDistance : distance, 10f * Time.deltaTime);
        side = Mathf.Lerp(side, aiming ? aimSide : 0f, 10f * Time.deltaTime);

        transform.localPosition = new Vector3(side, 1f + current * 0.3f, -current);
        transform.LookAt(transform.parent.position + transform.parent.right * side + Vector3.up * 1.5f);
    }
}
