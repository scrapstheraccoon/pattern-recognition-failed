using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerAnimator animator;
    [SerializeField] private Transform vCam;
    [SerializeField] private Transform raycastOrigin;

    [Header("Player variables")]
    [SerializeField] private float speed;

    [Header("Physics Parameters")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedStickForce = -2f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float pushPower = 2f;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float controllerSensitivity = 100f;

    [Header("Camera Settings")]
    [SerializeField] private float upPitch = 15f;
    [SerializeField] private float downPitch = 20f;
    [SerializeField] private bool invertY;

    [Header("Input Action References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference lookAction;

    private bool isGrounded;
    private bool wasGrounded;

    private bool isMoving;
    private bool canMove = true;

    private Vector2 movementInput;
    private Vector3 velocity;

    private float pitch;

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        lookAction.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        lookAction.action.Disable();
    }

    void Update()
    {
        handlePlayerMovement();
        handlePlayerLook();
        updateAnimator();
    }

    private void handlePlayerMovement()
    {
        // grounded check
        wasGrounded = isGrounded;

        //movement check
        isMoving = movementInput.sqrMagnitude > 0.01f;

        // horizontal movement
        Vector3 move =
            transform.right * movementInput.x +
            transform.forward * movementInput.y;

        // gravity
        if (isGrounded)
        {
            if (velocity.y < 0f)
                velocity.y = groundedStickForce;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // jump
        if (jumpAction.action.WasPressedThisFrame() && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.TriggerJump();
        }

        // landing
        if (!wasGrounded && controller.isGrounded)
        {
            animator.TriggerLand();

            velocity.x = 0f;
            velocity.z = 0f;
        }

        if (canMove)
        {
            // input
            movementInput = moveAction.action.ReadValue<Vector2>();

            // movement
            Vector3 finalMove = move * speed + Vector3.up * velocity.y;
            controller.Move(finalMove * Time.deltaTime);
        }

        // grounded check Move
        isGrounded = controller.isGrounded;

        // landing
        if (!wasGrounded && isGrounded)
        {
            animator.TriggerLand();
            velocity.y = groundedStickForce;
        }

    }

    private void handlePlayerLook()
    {
        // detect system to change sensitivity
        float sensitivity;
        if (Gamepad.current != null)
            sensitivity = controllerSensitivity;
        else 
            sensitivity = mouseSensitivity;

        if (canMove)
        {
            // look
            Vector2 look = lookAction.action.ReadValue<Vector2>();
            float mouseX = look.x * sensitivity * Time.deltaTime;
            float mouseY = look.y * sensitivity * Time.deltaTime;            

            pitch += mouseY * (invertY ? -1 : 1);
            pitch = Mathf.Clamp(pitch, upPitch, downPitch);

            transform.Rotate(Vector3.up * mouseX);
            vCam.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            raycastOrigin.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void updateAnimator()
    {
        animator.SetMoving(isMoving);
        animator.SetGrounded(isGrounded);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body != null && !body.isKinematic)
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.AddForce(pushDir * pushPower, ForceMode.Impulse);
        }
    }

    public void toggleMovement()
    {
        canMove = !canMove;
    }
}
