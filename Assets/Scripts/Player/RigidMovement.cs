using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMovement : MonoBehaviour
{
    // Class wrapping up settings concerning the character movement
    [System.Serializable]
    public class MoveSettings
    {
        // How fast the player moves
        public float forwardVel = 4;
        // How fast the player turns
        public float rotateVel = 1000.0f;
        // How high the player jumps
        public float jumpVel = 6;

        // The distance to the ground to count the player as fully grounded
        public float distToGround = 0.0f;
        // The minimal distance to the ground to count the player falling (distToGround + eps)
        public float minFallingDistToGround = 0.1f;
        // The offset from the transform position to cast the down ray from
        public Vector3 pivotOffset = new Vector3(0.0f, 0.5f, 0.0f);
        // The mask defines what objects are counted as ground
        public LayerMask ground;
    }
    // Class containing all input fields
    class CharacterInput
    {
        // The input in the forward direction
        public float forward = 0;
        // The input in the sideward direction
        public float sideward = 0;
        // The input for jumping
        public float jump = 0;
    }
    public float lerpSmoothingTime = 0.1f;
    // Instance of move settings
    public MoveSettings moveSettings = new MoveSettings();
    // Gravity acceleration
    public float downAccel = 18.0f;
    // Used for storing velocity before applying to the rigid body
    Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    // Store if the player is grounded
    bool grounded = false;
    // Stores the raycast hit to the ground
    RaycastHit groundHit = new RaycastHit();
    // Is set if the player is during a jump
    bool onJump = false;
    // Instance of character input
    CharacterInput input = new CharacterInput();
    // Reference to camera
    GameObject playerCamera = null;
    // Rigidbody component
    Rigidbody rigidBody = null;
    // Reference for animator component
    //Animator animator = null;
    Vector3 forwardDirection = Vector3.zero;
    // Unity Methods
    void Start()
    {
        // Grab components
        playerCamera = GameObject.Find("Player Camera");
        rigidBody = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
    }
    void Update()
    {
        // Get player input
        GetInput();
        // Process player turning
        Turn();


        // Check Grounded once per FixedUpdate()
        velocity = (transform.forward * forwardDirection.magnitude);

        CheckGrounded();

        bool isWalking = Mathf.Abs(input.forward) + Mathf.Abs(input.sideward) > 0;
        Jump();
        Run();
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / lerpSmoothingTime);

    }
    void FixedUpdate()
    {
        
        /*
        // Adjust movement to move along the surface normal of the ground (reduces falling when walking on slopes)
        if (grounded)
        {
            playerRotation = Quaternion.FromToRotation(Vector3.up, groundHit.normal) * transform.rotation;
        }
        // Set velocity in player look direction (rotation)
        */
        //rigidBody.velocity = playerRotation * new Vector3(0.0f, 0.0f, velocity.z) + new Vector3(0.0f, velocity.y, 0.0f);

    }
    // Gets control input
    void GetInput()
    {
        input.forward = Input.GetAxis("Vertical");
        input.sideward = Input.GetAxis("Horizontal");
        input.jump = Input.GetAxis("Jump");
    }
    void Run()
    {
        // Player can only move forward in his look direction -> take length of both input movement parameters to get speed
        // Adjusting the look direction is done in the Turn() function
        velocity.z *= moveSettings.forwardVel;
        velocity.x *= moveSettings.forwardVel;
        if (input.forward != 0 || input.sideward != 0)
        {
            targetPosition += (new Vector3(velocity.x, 0.0f, velocity.z)) * Time.deltaTime;
        }
        
        
    }
    // Method processing the player turning
    void Turn()
    {
        // Forward direction: camPos -> playerPos
        Vector3 cameraVector = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        forwardDirection = input.forward * cameraVector + input.sideward * playerCamera.transform.right;

        if (forwardDirection.magnitude > 1.0f) { forwardDirection.Normalize(); };

        forwardDirection = transform.InverseTransformDirection(forwardDirection);
        // Get Euler angles To 
        float turnAmount = Mathf.Atan2(forwardDirection.x, forwardDirection.z);

        // Rotate Along The Y-Axis
        transform.Rotate(0, turnAmount * moveSettings.rotateVel * Time.deltaTime, 0);
    }
    // Method for processing the jump action
    void Jump()
    {
        // Check if player is standing on ground or is mid air
        if (grounded)
        {
            // Check for jump input and if the player is currently not jumping
            if (input.jump > 0 && !onJump)
            {
                // Set y-velocity accordingly
                targetPosition.y = Mathf.Sqrt(moveSettings.jumpVel * -1.2f * Physics.gravity.y);
                //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / lerpSmoothingTime);
                onJump = true;
            }
            else if (grounded)
            {
                // Zero out y-velocity if player is standing on the ground and not jumping
                //targetPosition.y = 0;
            }
        }
        else
        {
            // Falling -> Apply gravity
            targetPosition.y -= downAccel * Time.fixedDeltaTime;
        }
        // Reset jump input parameter
        input.jump = 0.0f;
    }
    // Checks if the player is standing on a ground
    void CheckGrounded()
    {
        bool lastGrounded = grounded;
        // Cast a ray downwards in y-axis with a max distance of pivotOffset.y + minFallingDistToGround to see if it hit the ground layer
        bool hit = Physics.Raycast(transform.position + moveSettings.pivotOffset,
                        Vector3.down, out groundHit, moveSettings.pivotOffset.y + moveSettings.minFallingDistToGround, moveSettings.ground);
        // Subtract the pivotOffset.y from the distance passed
        groundHit.distance -= moveSettings.pivotOffset.y;
        // The player is counted as grounded if 
        // his distance to the ground is smaller than the set distance to ground 
        // or
        // his distance to the ground smaller is smaller than the minimum falling distance set and
        // he was standing on the ground the last time checked and he is not during a jump
        if (hit && (groundHit.distance < moveSettings.distToGround || (lastGrounded && !onJump)))
        { // Player is grounded -> place him exactly on the ground with a distance of distToGround
            //transform.position = new Vector3(groundHit.point.x, groundHit.point.y + moveSettings.distToGround, groundHit.point.z) * Time.deltaTime;
            targetPosition.y = groundHit.point.y + moveSettings.distToGround;
            grounded = true;
            // Set jump as finished
            onJump = false;
        }
        else
        {
            // Not grounded
            grounded = false;
        }
    }
}
