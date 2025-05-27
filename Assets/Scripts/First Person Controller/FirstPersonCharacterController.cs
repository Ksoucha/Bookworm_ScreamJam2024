using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonCharacterController : MonoBehaviour
{
    // Reference to the camera's transform
    [SerializeField] private new Transform camera;

    // PRIVATE VARIABLES
    private CharacterController controller;   
    private Transform surface;                
    private Transform lastSurface;            
    private Vector3 lastSurfacePosition;     
    private Vector2 cameraRotationValues;
    private Vector2 smoothRotationInput;
    private Vector3 movementVector;
    private Vector3 gravityVector;
    private Vector3 linearSurfaceVector;      
    private RaycastHit surfaceRaycast;        
    private bool isGrounded;

    // CONSTANTS
    const float TAU = 6.283185307179586476925287f;  // Constant for 2π (full rotation in radians)
    const int GROUNDED_CHECK_DETAIL = 8;            // Number of points to check for ground detection

    // PUBLIC GETTERS
    public bool IsGrounded => isGrounded;           // Returns if the character is grounded
    public Transform Surface => isGrounded ? surface : null;  // Returns the current surface if grounded, otherwise null
    public Vector3 Velocity => controller.velocity; // Returns the velocity of the character
    public Transform Camera => camera;              // Returns the camera's transform

    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        InitializeCollisionBody();
    }

    // Unity method called once per frame
    private void Update()
    {
        isGrounded = ProperGroundCheck();           // Check if the character is grounded
        CalculateSurfaceVelocity();                 // Calculate surface movement if the character is on a moving platform
        controller.Move(movementVector + gravityVector + linearSurfaceVector);
    }

    #region public methods

    // Method to move the character based on input, speed, and acceleration/deceleration
    public void Move(Vector2 input, float speed, Vector2 accelDeccel)
    {
        input = Vector2.ClampMagnitude(input, 1);

        Vector3 ver_dir = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
        Vector3 hor_dir = Vector3.ProjectOnPlane(camera.transform.right, Vector3.up);

        Vector3 movement = hor_dir * input.x + ver_dir * input.y;
        movement *= speed * Time.deltaTime;

        // Determine whether the character is accelerating or decelerating
        bool accel = input.magnitude > 0;
        float accelValue = accel ? accelDeccel.x : accelDeccel.y;
        accelValue *= Time.deltaTime;

        Vector3 direction = (movement - movementVector) * accelValue;
        movementVector += direction;
    }

    // Applies gravity to the character when they are not grounded
    public void ApplyGravity(float gravity)
    {
        if (isGrounded)
            return;

        Vector3 falling = Vector3.down * Mathf.Abs(gravity) * Time.deltaTime;
        gravityVector += falling * Time.deltaTime;
    }

    // Method to make the character jump if grounded
    public void Jump(float force)
    {
        if (!isGrounded)
            return;

        gravityVector = Vector3.up * force * Time.fixedDeltaTime;
    }

    // Method to rotate the camera based on input and apply smoothing
    public void MoveCamera(Vector2 input, Vector2 cameraAngleLimits, float smoothingSpeed = 15)
    {
        // Smoothly interpolate between current and target input for smooth camera movement
        smoothRotationInput = Vector2.Lerp(
            smoothRotationInput,
            input,
            1 - Mathf.Exp(-Mathf.Abs(smoothingSpeed)));

        cameraRotationValues.x -= smoothRotationInput.y;  // Adjust vertical camera angle
        cameraRotationValues.y += smoothRotationInput.x;  // Adjust horizontal camera angle
        cameraRotationValues.x %= 360;
        cameraRotationValues.y %= 360;

        // Clamp vertical rotation to prevent excessive looking up or down
        cameraRotationValues.x = Mathf.Clamp(
            cameraRotationValues.x,
            cameraAngleLimits.x,
            cameraAngleLimits.y);

        camera.eulerAngles = cameraRotationValues;
    }

    // Resets the movement and gravity vectors to stop the character's motion
    public void ResetVelocity()
    {
        linearSurfaceVector = Vector3.zero;
        movementVector = Vector3.zero;
        gravityVector = Vector3.zero;
        controller.Move(Vector3.zero);  // Stop any current movement
    }
    #endregion

    #region private methods

    // Calculates the surface movement when standing on a moving platform
    private void CalculateSurfaceVelocity()
    {
        surface = surfaceRaycast.transform; 

        if (!surface)
        {
            linearSurfaceVector += (Vector3.zero - linearSurfaceVector) * Time.deltaTime;
            lastSurface = null;
            return;
        }

        if (lastSurface != surface)  // If character moved to a different surface
        {
            linearSurfaceVector = Vector3.zero;
            lastSurfacePosition = surface.position;
            lastSurface = surface;
        }

        if (lastSurfacePosition != surface.position)  // If the surface has moved
        {
            linearSurfaceVector = surface.position - lastSurfacePosition;
            lastSurfacePosition = surface.position;
        }

        linearSurfaceVector.y = Mathf.Min(linearSurfaceVector.y, 0);
    }

    // Method to check if the character is grounded
    private bool ProperGroundCheck()
    {
        float smallOffset = 0.1f;
        float radius = controller.radius;
        float height = controller.height;
        float skinWidth = controller.skinWidth;
        Vector3 center = controller.center;

        // Calculate the length of the ray for ground checking
        float length = height / 2 + skinWidth + smallOffset;

        int checks = 0;
        // Perform a series of raycasts around the character to check for ground
        for (int i = GROUNDED_CHECK_DETAIL; i > 0; i--)
        {
            float distance = ((float)i - 1) / GROUNDED_CHECK_DETAIL;
            for (int j = 0; j < i; j++)
            {
                float radians = (float)j / i;
                float x = Mathf.Cos(radians * TAU) * radius * distance;
                float y = Mathf.Sin(radians * TAU) * radius * distance;
                Vector3 vec = transform.position + center + new Vector3(x, 0, y);

                // Perform a raycast to check for the ground
                bool hit = Physics.Raycast(
                    vec, Vector3.down,
                    out surfaceRaycast, length, -1,
                    QueryTriggerInteraction.Ignore);

                if (!hit)
                    continue;

                checks++;
            }
        }

        // Return true if any ground was detected or if the character controller is grounded
        return checks > 0 || controller.isGrounded;
    }

    // Initializes the collision body for the character
    private void InitializeCollisionBody()
    {
        GameObject collisionBody = new GameObject("Collision Body");  // Create a new GameObject for the collision body
        CapsuleCollider collider = collisionBody.AddComponent<CapsuleCollider>();  // Add a CapsuleCollider to the collision body
        collider.direction = 1;  // Set the collider's direction (1 is vertical)
        collider.radius = controller.radius + 0.1f;  // Slightly increase the radius to match the CharacterController
        collider.height = controller.height;  // Set the height to match the CharacterController
        collider.center = controller.center;  // Set the center to match the CharacterController

        Rigidbody rigidbody = collisionBody.AddComponent<Rigidbody>();  // Add a Rigidbody to the collision body
        rigidbody.interpolation = RigidbodyInterpolation.None;  // Set interpolation mode to None
        rigidbody.isKinematic = true;  // Make the Rigidbody kinematic (not affected by physics)
        rigidbody.useGravity = false;  // Disable gravity for the Rigidbody

        collisionBody.transform.parent = transform;  // Set the collision body as a child of the character
        collisionBody.transform.localPosition = Vector3.zero;  // Set local position to zero to match the character's position
    }
    #endregion
}
