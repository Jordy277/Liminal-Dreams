using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Refereneces")]
    private CharacterController controller;
    [SerializeField] private Transform playerCamera;

    [Header("Looking")]
    [SerializeField] private float sensitivity = 2f;
    private float xRotation = 0f;

    [Header("Input")]
    private InputActions inputActions;
    private Vector2 moveAction;
    private Vector2 mouseLookAction;
    private Vector2 scrollZoomAction;
    private bool isCrouching;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float gravity = 9.8f;
    private float verticalVelocity;

    void Awake()
    {
        inputActions = new InputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Look.performed += OnLookInput;
        inputActions.Player.Look.canceled += OnLookInput;
        inputActions.Player.Zoom.performed += OnZoomInput;
        inputActions.Player.Crouch.performed += OnCrouchInput;
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Look.performed += OnLookInput;
        inputActions.Player.Look.canceled += OnLookInput;
        inputActions.Player.Zoom.performed -= OnZoomInput;
        inputActions.Player.Crouch.performed -= OnCrouchInput;
        inputActions.Player.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleCameraLook();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        moveAction = context.ReadValue<Vector2>();
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        mouseLookAction = context.ReadValue<Vector2>();
    }

    private void OnZoomInput(InputAction.CallbackContext context)
    {
        scrollZoomAction = context.ReadValue<Vector2>();
    }

    private void OnCrouchInput(InputAction.CallbackContext context)
    {
        isCrouching = !isCrouching;
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveAction.x, 0, moveAction.y).normalized;
        move = transform.TransformDirection(move);

        move *= walkSpeed;
        move.y = VerticalForceCalculation();
        controller.Move(move * Time.deltaTime);
    }

    private void HandleCameraLook()
    {
        float mouseX = mouseLookAction.x * sensitivity * Time.deltaTime;
        float mouseY = mouseLookAction.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private float VerticalForceCalculation()
    {
        if(controller.isGrounded)
        {
            verticalVelocity = -1;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        return verticalVelocity;
    }
}
