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
    public float minPitch = -20f;
    public float maxPitch = 60f;

    float distance = 3.5f;
    float current = 3.5f;
    float side;
    float pitch = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;

        Vector2 delta = mouse.delta.ReadValue();
        transform.parent.Rotate(0f, delta.x * mouseSensitivity, 0f);
        pitch = Mathf.Clamp(pitch - delta.y * mouseSensitivity, minPitch, maxPitch);

        float scroll = mouse.scroll.ReadValue().y;
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

        bool aiming = mouse.rightButton.isPressed;
        current = Mathf.Lerp(current, aiming ? aimDistance : distance, 10f * Time.deltaTime);
        side = Mathf.Lerp(side, aiming ? aimSide : 0f, 10f * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(pitch, 0f, 0f);
        Vector3 pivot = new Vector3(side, 1.5f, 0f);
        transform.localRotation = rotation;
        transform.localPosition = pivot + rotation * new Vector3(0f, 0f, -current);
    }
}
