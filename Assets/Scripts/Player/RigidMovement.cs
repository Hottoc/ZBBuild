using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMovement : MonoBehaviour
{
    [System.Serializable]
    public class MoveSettings
    {
        [Header("[X,Z] Velocity")]
        public float forwardVel = 0.0f;
        public float rotateVel = 250.0f;
        [Header("[Y] Velocity")]
        public float vertVel = 0.0f;
        public float jumpVel = 6.0f;
        public float termVel = -10.0f;

        [Header("Gravity")]
        //public float minFall = -1.5f;
        public float distToGround = 0.0f;

        public float minFallingDistToGround = 0.1f;

        public Vector3 pivotOffset = new Vector3(0.0f, 0.5f, 0.0f);

        public LayerMask ground;
    }

    class CharacterInput
    {
        // The input in the forward direction
        public float forward = 0.0f;
        // The input in the sideward direction
        public float sideward = 0.0f;
        // The input for jumping
        public float jump = 0.0f;
        // The input for running
        public float run = 0.0f;
    }

    public enum CurrentState
    {
        Idle,
        Jump,
        Walk,
        Run
    }

    [Header("Movement Speeds")]
    [Range(0.0f, 100.0f)]
    public float walkSpd = 0.0f;
    [Range(0.0f, 100.0f)]
    public float runSpd = 0.0f;

    // Instance of move settings
    public MoveSettings moveSettings = new MoveSettings();

    // Gravity acceleration
    public float downAccel = 18.0f;

    // Used for storing velocity before applying to the character controller
    Vector3 velocity = Vector3.zero;

    // Used for storing the player forward direction before applying to the character controller
    Vector3 forwardDirection = Vector3.zero;

    // Used for storing camera forward direction before applying to the character controller
    Vector3 camForwardDirection = Vector3.zero;

    // Store if the player is grounded
    bool grounded = false;

    //ControllerColliderHit _contact;

    // Stores the raycast hit to the ground
    RaycastHit groundHit = new RaycastHit();

    // Is set if the player is during a jump
    bool onJump = false;

    // Instance of character input
    CharacterInput input = new CharacterInput();

    //Character Controller component
    //CharacterController character = null;
    public float distToGround = 0.0f;
    public Rigidbody rb = null;

    //Camera object
    GameObject playerCamera = null;

    // Reference for animator component
    Animator animator = null;

    // Start is called before the first frame update
    void Start()
    {
        // Grab components
        playerCamera = GameObject.Find("Player Camera");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        distToGround = GetComponent<Collider>().bounds.extents.y;
        //moveSettings.vertVel = moveSettings.minFall;
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Get player input
        GetInput();

        forwardDirection = Vector3.zero;

        forwardDirection = input.forward * GetCamForward() + input.sideward * playerCamera.transform.right;

        if (forwardDirection.magnitude > 1.0f) { forwardDirection.Normalize(); };

        forwardDirection = transform.InverseTransformDirection(forwardDirection);

        // Process player turning
        Turn();

        velocity = (transform.forward * forwardDirection.magnitude);
        CheckGround();
        

    }

    void LateUpdate()
    {
        Run();
        Jump();




        rb.MovePosition(transform.position + velocity);
        //character.Move(velocity * Time.deltaTime);

    }
    void GetInput()
    {
        input.forward = Input.GetAxis("Vertical");
        input.sideward = Input.GetAxis("Horizontal");

        if (input.jump == 0.0f) { input.jump = Input.GetAxis("Jump"); }
        if (input.run == 0.0f) { input.run = Input.GetAxis("Run"); }
    }

    void Run()
    {
        if (input.forward != 0 || input.sideward != 0)
        {
            if (input.run == 0.0f)
            {
                moveSettings.forwardVel = walkSpd;

                // Apply animations
                //PlayState(CurrentState.Run, false);
                //PlayState(CurrentState.Walk, true);
            }
            else
            {
                moveSettings.forwardVel = runSpd;

                // Apply animations
                //PlayState(CurrentState.Walk, false);
                //PlayState(CurrentState.Run, true);
            }
        }
        else
        {
            // Apply animations
            //PlayState(CurrentState.Walk, false);
            //PlayState(CurrentState.Run, false);
        }
        input.run = 0.0f;
        velocity.x *= moveSettings.forwardVel;
        velocity.z *= moveSettings.forwardVel;
    }
        void Turn()
    {
        // Get Euler angles To 
        float turnAmount = Mathf.Atan2(forwardDirection.x, forwardDirection.z);

        // Rotate Along The Y-Axis
        transform.Rotate(0, turnAmount * moveSettings.rotateVel * Time.deltaTime, 0);
    }
    void Jump()
    {
        Debug.Log(onJump);
        if (grounded)
        {
            if (input.jump > 0 && !onJump)
            {
                Debug.Log("HELLO");
                onJump = true;
                velocity.y =  1 * moveSettings.jumpVel;
                
            }
            else if (grounded) {
                //velocity.y = 0;
            }
        }
        else
        {
            //moveSettings.vertVel += (-1 * downAccel) * Time.deltaTime;
            //if (moveSettings.vertVel < moveSettings.termVel) { moveSettings.vertVel = moveSettings.termVel; }
            velocity.y -= downAccel * Time.deltaTime;
        }
        //velocity.y = moveSettings.vertVel;
        input.jump = 0.0f;
    }

    public Vector3 GetCamForward()
    {
        return Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
    }

    void CheckGround()
    {
        //bool hit = Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f);
        bool lastGrounded = grounded;

        bool hit = Physics.Raycast(transform.position, Vector3.down, out groundHit, moveSettings.pivotOffset.y + moveSettings.minFallingDistToGround);
        //Debug.Log(hit);
        Debug.DrawRay(transform.position, Vector3.down * groundHit.distance, Color.blue);
        //groundHit.distance -= moveSettings.pivotOffset.y;
        if (hit)
        {
            velocity.y = 0;
            grounded = true;
            onJump = false;
        }
        else
        {
            grounded = false;
        }
        /*
        bool lastGrounded = grounded;

        bool hit = Physics.Raycast(transform.position + moveSettings.pivotOffset, Vector3.down, out groundHit, moveSettings.pivotOffset.y + moveSettings.minFallingDistToGround, moveSettings.ground);
        Debug.DrawRay(transform.position + moveSettings.pivotOffset, Vector3.down * groundHit.distance, Color.blue);
        groundHit.distance -= moveSettings.pivotOffset.y;

        if(hit && (groundHit.distance < moveSettings.distToGround || (lastGrounded && !onJump)))
        {
            //Debug.Log("Found ground");
            transform.position = new Vector3(transform.position.x, groundHit.point.y + moveSettings.distToGround, transform.position.z);

            grounded = true;

            onJump = false;
        }
        else
        {
            //Debug.Log("NO ground");
            grounded = false;
        }
        */
        /*
        if ((Physics.Raycast(transform.position, Vector3.down, out groundHit)))
        {
            Debug.DrawRay(transform.position, Vector3.down * groundHit.distance, Color.blue);
            //float check = (character.height + character.radius) / 1.43f;
            //grounded = groundHit.distance <= check;
        }
        */
    }
}
