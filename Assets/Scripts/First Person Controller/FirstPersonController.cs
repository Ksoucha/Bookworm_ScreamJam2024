using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : GenericSingleton<FirstPersonController>
{
    [Header("Components")]
    [SerializeField] private FirstPersonCharacterController controller;
    [SerializeField] private Animator animator;

    [Header("Input")]
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference lookInput;
    [SerializeField] private InputActionReference jumpInput;
    [SerializeField] private InputActionReference sprintInput;


    [Header("Settings")]
    [SerializeField] private float gravity = 10;
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float runSpeed = 5;
    [SerializeField] private float accel = 5;
    [SerializeField] private float deccel = 20;
    [SerializeField] private float jumpHeight = 2;
    [SerializeField,Range(0.01f,0.5f)] private float cameraSmoothingSpeed = 0.1f;
    [SerializeField] private Vector2 cameraAngleLimits = new Vector2(-90, 90);
    public Transform GetCamera() => controller.Camera;

    public void ResetCameraOffset() => controller.Camera.transform.localPosition = initialCameraOffset;

    public bool canSprint = true;
    public bool canMove = true;
    private Vector3 initialCameraOffset;

    public bool isSprinting;
    public bool isJumping;
    public Vector2 move;

    public float interactionDistance = 1f;
    private Vector3 originalPosition;

    public override void Awake()
    {
        base.Awake();   
        Cursor.lockState = CursorLockMode.Locked;
        initialCameraOffset = controller.Camera.transform.localPosition;
    }

    private void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        isSprinting = sprintInput.action.IsPressed();
        isJumping = jumpInput.action.triggered;
        move = moveInput.action.ReadValue<Vector2>();
        Vector2 look = lookInput.action.ReadValue<Vector2>();
        float speed = isSprinting ? runSpeed : walkSpeed;

        if (!canMove)
        {
            controller.ResetVelocity();
            animator.SetFloat("velocity", 0);
            return;
        }

        if (isJumping)
            controller.Jump(jumpHeight);

        controller.Move(move, speed, new Vector3(accel, deccel));
        controller.MoveCamera(look, cameraAngleLimits, cameraSmoothingSpeed);
        controller.ApplyGravity(gravity);

        if (!animator)
            return;

        float velocity = controller.Velocity.magnitude * 0.75f;
        animator.SetBool("grounded", controller.IsGrounded);
        animator.SetFloat("velocity", velocity);

        if (Input.GetMouseButtonDown(0))
        {
            InteractWithAnyObject();
        }
    }

    internal void InteractWithAnyObject()
    {
        var camera = GetCamera();
        RaycastHit hitInfo;

        // Start slightly infront to prevent hitting player
        UnityEngine.Vector3 rayCastStartPosition = camera.transform.position + (camera.transform.forward * 1.0f);
        
        if (Physics.Raycast(rayCastStartPosition, camera.transform.forward, out hitInfo, interactionDistance))
        {
            Debug.DrawLine(rayCastStartPosition, hitInfo.point);

            if (hitInfo.collider != null)
            {
                Debug.Log(hitInfo.collider.name);
                var interactable = hitInfo.collider.gameObject.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    Debug.Log("ahhhha");
                }
            }
        }
    }

    internal void Reset()
    {
        transform.position = originalPosition;
    }
}
