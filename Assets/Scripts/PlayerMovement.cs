using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;
    public float sprintSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float aimSpeed = 1.5f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;
    public float standHeight = 1.8f;
    public float crouchHeight = 1.1f;
    public float turnSpeed = 10f;

    [HideInInspector] public bool topDownMode;
    [HideInInspector] public bool locked;

    CharacterController controller;
    Animator animator;
    float verticalVelocity;
    bool crouch;

    bool hasTarget;
    Vector3 target;
    float stuckTime;

    public bool HasTarget => hasTarget;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    public void SetTarget(Vector3 point)
    {
        target = point;
        hasTarget = true;
        stuckTime = 0f;
    }

    public void ClearTarget()
    {
        hasTarget = false;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        Vector3 move = Vector3.zero;

        if (topDownMode)
        {
            if (hasTarget)
            {
                Vector3 toTarget = target - transform.position;
                toTarget.y = 0f;
                if (toTarget.magnitude < 0.15f)
                {
                    hasTarget = false;
                }
                else
                {
                    move = toTarget.normalized;
                    Quaternion look = Quaternion.LookRotation(move);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            float x = 0f, z = 0f;
            if (kb.wKey.isPressed) z += 1f;
            if (kb.sKey.isPressed) z -= 1f;
            if (kb.dKey.isPressed) x += 1f;
            if (kb.aKey.isPressed) x -= 1f;
            move = (transform.right * x + transform.forward * z).normalized;
        }

        if (locked) move = Vector3.zero;

        if (kb.cKey.wasPressedThisFrame) crouch = !crouch;
        controller.height = crouch ? crouchHeight : standHeight;
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        bool grounded = controller.isGrounded;
        if (grounded && verticalVelocity < 0f) verticalVelocity = -2f;
        if (grounded && !crouch && !locked && kb.spaceKey.wasPressedThisFrame)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        verticalVelocity += gravity * Time.deltaTime;

        bool aiming = !topDownMode && Mouse.current.rightButton.isPressed;
        float currentSpeed = crouch ? crouchSpeed : speed;
        if (aiming) currentSpeed = Mathf.Min(currentSpeed, aimSpeed);
        bool sprint = kb.leftShiftKey.isPressed && !crouch && !aiming && move != Vector3.zero;
        if (sprint) currentSpeed = sprintSpeed;

        Vector3 velocity = move * currentSpeed;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        if (hasTarget)
        {
            Vector3 real = controller.velocity;
            real.y = 0f;
            stuckTime = real.magnitude < 0.1f ? stuckTime + Time.deltaTime : 0f;
            if (stuckTime > 0.5f) hasTarget = false;
        }

        animator.SetBool("IsMoving", move != Vector3.zero);
        animator.SetBool("IsCrouching", crouch);
        animator.SetBool("IsGrounded", grounded);
        animator.speed = sprint ? 1.7f : 1f;
    }
}
