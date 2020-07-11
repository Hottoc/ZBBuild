using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [System.Serializable]
    public class MoveSettings
    {
        [Header("[X,Z] Velocity")]
        public float forwardVel = 0.0f;
        public float rotateVel = 200.0f;
        [Header("[Y] Velocity")]
        public float vertVel = 0.0f;
        public float jumpVel = 6.0f;
        public float termVel = -10.0f;
        
        [Header("Gravity")]
        public float minFall = -1.5f;
        public float downAccel = -10.0f;
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
        Run,
        Swim
    }

    [Header("Movement Speeds")]
    [Range(0.0f, 100.0f)]
    public float walkSpd = 0.0f;
    [Range(0.0f, 100.0f)]
    public float runSpd = 0.0f;

    // Instance of move settings
    public MoveSettings moveSettings = new MoveSettings();

    // Used for storing velocity before applying to the character controller
    Vector3 velocity = Vector3.zero;

    // Used for storing the player forward direction before applying to the character controller
    Vector3 forwardDirection = Vector3.zero;

    // Used for storing camera forward direction before applying to the character controller
    Vector3 camForwardDirection = Vector3.zero;

    // Store if the player is grounded
    bool grounded = false;
    
    ControllerColliderHit _contact;

    // Stores the raycast hit to the ground
    RaycastHit groundHit;

    bool canJump = false;

    // Instance of character input
    CharacterInput input = new CharacterInput();

    //Character Controller component
    CharacterController character = null;

    //Camera object
    GameObject playerCamera = null;

    // Reference for animator component
    Animator animator = null;

    // Start is called before the first frame update
    void Start()
    {
        // Grab components
        playerCamera = GameObject.Find("Player Camera");
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        moveSettings.vertVel = moveSettings.minFall;
    }

    // Update is called once per frame
    void Update()
    {
        // Get player input
        input.jump = 0.0f;
        //moveSettings.forwardVel = 0.0f;
        GetInput();
        
        forwardDirection = Vector3.zero;

        forwardDirection = input.forward * GetCamForward() + input.sideward * playerCamera.transform.right;

        if (forwardDirection.magnitude > 1.0f) { forwardDirection.Normalize(); };

        forwardDirection = transform.InverseTransformDirection(forwardDirection);

        // Process player turning
        Turn();

        velocity = (transform.forward * forwardDirection.magnitude);
        
        CheckGround();
        Run();
        Jump();
        animator.SetFloat("test", moveSettings.forwardVel);

        character.Move(velocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit groundHit)
    {
        _contact = groundHit;
    }

    void GetInput()
    {
        input.forward = Input.GetAxis("Vertical");
        input.sideward = Input.GetAxis("Horizontal");

        input.jump = Input.GetAxis("Jump");
        input.run = Input.GetAxis("Run"); 
    }

    void Run()
    {
        if (input.forward == 0 && input.sideward == 0) { moveSettings.forwardVel = 0.0f; return; }
        float setSpeed = (input.run == 0.0f) ? walkSpd : runSpd;
        moveSettings.forwardVel = MaxSpeed(moveSettings.forwardVel, setSpeed);
        velocity *= moveSettings.forwardVel;
        /*
        if (input.forward != 0 || input.sideward != 0)
        {
            if (input.run == 0.0f)
            {
                moveSettings.forwardVel = walkSpd;

                // Apply animations
                PlayState(CurrentState.Run, false);
                PlayState(CurrentState.Walk, true);
            }
            else
            {
                moveSettings.forwardVel = runSpd;

                // Apply animations
                PlayState(CurrentState.Walk, false);
                PlayState(CurrentState.Run, true);
            }
        }
        else
        {
            // Apply animations
            PlayState(CurrentState.Walk, false);
            PlayState(CurrentState.Run, false);
        }
        input.run = 0.0f;
        velocity.x *= moveSettings.forwardVel;
        velocity.z *= moveSettings.forwardVel;
        */
    }

    float MaxSpeed(float current, float max)
    {
        current += 15.0f * Time.deltaTime;
        if (current > max) { current = max; }

        return current;
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
        if (grounded)
        {
            if (input.jump > 0.0f && canJump && moveSettings.vertVel <= moveSettings.minFall)
            {
                moveSettings.vertVel = moveSettings.jumpVel;
                moveSettings.forwardVel /= 1.5f;
                velocity.x *= moveSettings.forwardVel;
                velocity.z *= moveSettings.forwardVel;
                canJump = false;
                // Apply animations
                //PlayState(CurrentState.Jump, true);
            }
        }
        else
        {
            moveSettings.vertVel += moveSettings.downAccel * 5 * Time.deltaTime;
            if (moveSettings.vertVel < moveSettings.termVel) { moveSettings.vertVel = moveSettings.termVel; }

            // Apply animations
            //PlayState(CurrentState.Jump, false);
        }
        velocity.y = moveSettings.vertVel;
    }

    public Vector3 GetCamForward()
    {
        return Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
    }

    void CheckGround()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, out groundHit, 3.7f);
        bool waterHit = Physics.Raycast(transform.position, Vector3.down, out groundHit, 3.7f, LayerMask.GetMask("Water"));
        Debug.DrawRay(transform.position, Vector3.down * groundHit.distance, Color.blue);
        if (hit && !waterHit)
        {
            grounded = true;
            canJump = true;
            //PlayState(CurrentState.Swim, false);
            
            return;
        }
        grounded = false;
        //PlayState(CurrentState.Swim, true);
    }

    private void PlayState(CurrentState state, bool active)
    {
        string animName = string.Empty;
        switch (state)
        {
            case CurrentState.Walk:
                animName = "IsWalking";
                break;
            case CurrentState.Run:
                animName = "IsRunning";
                break;
            case CurrentState.Swim:
                animName = "IsSwimming";
                break;
        }
        animator.SetBool(animName, active);
    }
}