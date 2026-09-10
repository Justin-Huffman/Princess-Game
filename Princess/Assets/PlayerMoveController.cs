using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveController : MonoBehaviour
{
    [SerializeField] private float WalkSpeed = 5f;
    [SerializeField] private float RunSpeed = 8f;

    [SerializeField] private float JumpHeight = 6f;
    [SerializeField] private float Gravity = 20f;

    [SerializeField] private float LookSensitivity = 0.3f;
    [SerializeField] private float LookAngleLimit = 90f;

    private Camera playerCamera;
    private CharacterController characterController;

    private InputAction moveInput;
    private InputAction jumpInput;
    private bool jumped = false;
    private InputAction runInput;

    private float currentMoveSpeed = 0f;
    private Vector3 moveDirection = Vector3.zero;
    private float lookAngle = 0f;


    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();

        moveInput = InputSystem.actions.FindAction("Move");

        jumpInput = InputSystem.actions.FindAction("Jump");
        jumpInput.started += Jumped;

        runInput = InputSystem.actions.FindAction("Sprint");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentMoveSpeed = WalkSpeed;

    }


    private void Update()
    {
        Vector2 moveVector = moveInput.ReadValue<Vector2>();
        Vector2 mouseDelta = new Vector2(Mouse.current.delta.x.ReadValue(), Mouse.current.delta.y.ReadValue());

        if (!characterController.isGrounded)
        jumped = false;

        currentMoveSpeed = runInput.IsPressed() ? RunSpeed : WalkSpeed;
        HandleMovement(moveVector);
        HandleLooking(mouseDelta);
    }

    private void HandleMovement(Vector2 moveVector)
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float oldY = moveDirection.y;

        Vector2 newSpeed = new Vector2(moveVector.y * currentMoveSpeed, moveVector.x * currentMoveSpeed);

        moveDirection = (forward * newSpeed.x) + (right * newSpeed.y);
        if (jumped && characterController.isGrounded)
        moveDirection.y = JumpHeight;
        else
        moveDirection.y = oldY;

        if (!characterController.isGrounded)
        moveDirection.y -= Gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void Jumped(InputAction.CallbackContext _)
    {
        jumped = true;
    }

    private void HandleLooking(Vector2 mouseDelta)
    {
        lookAngle += -mouseDelta.y * LookSensitivity;
        lookAngle = Mathf.Clamp(lookAngle, -LookAngleLimit, LookAngleLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(lookAngle, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseDelta.x * LookSensitivity, 0);
    }
}
