using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;
    public float crouchSpeed = 1.5f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;
    public float standHeight = 1.8f;
    public float crouchHeight = 1.1f;

    CharacterController controller;
    Animator animator;
    float verticalVelocity;
    bool crouch;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;

        float x = 0f, z = 0f;
        if (kb.wKey.isPressed) z += 1f;
        if (kb.sKey.isPressed) z -= 1f;
        if (kb.dKey.isPressed) x += 1f;
        if (kb.aKey.isPressed) x -= 1f;
        Vector3 move = (transform.right * x + transform.forward * z).normalized;

        if (kb.cKey.wasPressedThisFrame) crouch = !crouch;
        controller.height = crouch ? crouchHeight : standHeight;
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        bool grounded = controller.isGrounded;
        if (grounded && verticalVelocity < 0f) verticalVelocity = -2f;
        if (grounded && !crouch && kb.spaceKey.wasPressedThisFrame)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * (crouch ? crouchSpeed : speed);
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        animator.SetBool("IsMoving", move != Vector3.zero);
        animator.SetBool("IsCrouching", crouch);
        animator.SetBool("IsGrounded", grounded);
    }
}
