using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class MoveSettings
    {
        
        [Header("[X,Z] Velocity")]
        public float forwardVel = 0.0f;
        public float rotateVel = 0.0f;
        [Header("[Y] Velocity")]
        public float vertVel = 0.0f;
        [Header("Movement Speeds")]
        public float walkSpd = 7.0f;
        public float runSpd = 15.0f;
        public float rotationSpd = 5.0f;
        public float jumpSpd = 6.0f;
        [Header("Layer Mask")]
        public LayerMask ground;
        [Header("Collisions")]
        public float height = 3.5f;
        public float width = 0.5f;
        public float heightPadding = 0.05f;
        public float maxGroundAngle = 120;
        [Header("Debugging")]
        public bool debug;
    }

    [System.Serializable]
    public class PhysSettings
    {
        [Header("Gravity")]
        public float downAccel = -10.0f;
        public float termVel = -10.0f;
        public float xtraForce = -300.0f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.1f;

        public string FORWARD_AXIS = "Vertical";
        public string SIDEWARD_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";
        public string RUN_AXIS = "Run";
    }

    public enum PlayerState
    {
        Idle,
        Jump,
        Walk,
        Run,
        Swim
    }
    //Raycast
    RaycastHit groundHitInfo;
    RaycastHit wallHitInfo;

    // Instance of move settings
    public MoveSettings moveSettings = new MoveSettings();
    // Instance of physic settings
    public PhysSettings physSettings = new PhysSettings();
    // Instance of input settings
    public InputSettings inputSettings = new InputSettings();

    // Vector3 Structs
    Vector3 forwardDirection, forward, camForwardDirection, velocity;

    // Variables
    bool canJump, isGrounded, resetTilt;

    float forwardInput, sidewardInput, jumpInput, runInput, groundAngle, orginTilt;
    // Objects & Components
    GameObject playerCamera = null;
    Animator animator = null;
    #region Properties
    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }




    #endregion
    void Start()
    {
        // Grab Camera Object
        playerCamera = GameObject.Find("Player Camera");
        animator = GetComponent<Animator>();
        isGrounded = true;
        orginTilt = transform.eulerAngles.x;
    }

    void Update()
    {
        GetInput();

        
        GetForward(); //Get The Player Forward Based On Camera Position And Slope Angle
        CalculateGroundAngle(); //Calculate The Angle Of THe Player Based On Ground Slope
        CheckGround(); //Check If The Player Is On The Ground
        CheckWall(); //Check If The Player Is On A Wall

        
        ApplyGravity();
        ApplySpeed();
        Jump();
        DrawDebugLines();

        Rotate();
        Move();
    }

    /// <summary>
    /// Input based on Horizontal(a,d), Vertical (w,s), Jump (space), and Run (shift) keys
    /// </summary>
    void GetInput()
    {
        forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS); //interpolated
        sidewardInput = Input.GetAxis(inputSettings.SIDEWARD_AXIS); //interpolated
        jumpInput = Input.GetAxis(inputSettings.JUMP_AXIS); //non-interpolated
        runInput = Input.GetAxis(inputSettings.RUN_AXIS); //non-interpolated
    }

    /// <summary>
    /// Direction relative to the camera object's rotation
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCamForward()
    {
        return Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
    }

    /// <summary>
    /// Rotate toward the calculated angle
    /// </summary>
    void Rotate()
    {
        // Get Euler angles To 
        float turnAmount = Mathf.Atan2(forwardDirection.x, forwardDirection.z) * Mathf.Rad2Deg;
   
        moveSettings.rotateVel = turnAmount * moveSettings.rotationSpd;

        transform.Rotate(0, moveSettings.rotateVel * Time.deltaTime, 0);
        
        if (moveSettings.rotateVel > 100.0f || moveSettings.rotateVel < -100.0f)
        {
            float tiltVal = Mathf.Clamp(Mathf.Abs(moveSettings.forwardVel / 8), 0, 1) ;
            transform.Rotate(-10 * tiltVal * Time.deltaTime, 0, 0);
            return;
        }
        restoreRotation();
        //transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(moveSettings.forwardVel / 10 * tiltVal, 0, forwardDirection.z / 14 * tiltVal)), Time.deltaTime * 2.0f);
    }

    void restoreRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(orginTilt, transform.eulerAngles.y, 0), Time.deltaTime * 5);
        return;
    }

    /// <summary>
    /// Apply Velocity to the Player's position
    /// </summary>
    void Move()
    {
        velocity.x *= moveSettings.forwardVel;
        velocity.z *= moveSettings.forwardVel;
        velocity.y += moveSettings.vertVel;

        transform.position += velocity * Time.deltaTime;
        animator.SetFloat("Forward Speed", moveSettings.forwardVel);
    }
    /// <summary>
    /// Apply Speed to velocity
    /// </summary>
    void ApplySpeed()
    {
        if (!isGrounded)
        {
            moveSettings.forwardVel = MinSpeed(moveSettings.forwardVel, 5.0f, 0.0f);
            return;
        }

        if (forwardInput == 0 && sidewardInput == 0)
        {
            moveSettings.forwardVel = 0;
            return;
        }

        float setSpeed = (runInput == 0.0f) ? moveSettings.walkSpd : moveSettings.runSpd;

        moveSettings.forwardVel = MaxSpeed(moveSettings.forwardVel, 15.0f, setSpeed);

        if (forward.y != 0)
        {
            velocity.y *= moveSettings.forwardVel;
        }

        //Have player Speed decrease rapidly if climbing steep slope
        //Debug.Log(groundAngle);
        if (groundAngle >= moveSettings.maxGroundAngle && moveSettings.forwardVel > -5f)
        {
            moveSettings.forwardVel -= 30 * Time.deltaTime;
        }

        

    }
    /// <summary>
    /// Build Player speed gradually until reaches max.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="max"></param>
    /// <returns>"current"</returns>
    float MaxSpeed(float current, float speed, float max)
    {
        current += speed * Time.deltaTime;
        if (current > max) { current = max; }

        return current;
    }
    float MinSpeed(float current, float speed, float min)
    {
        current -= speed * Time.deltaTime;
        if (current < min) { current = min; }

        return current;
    }
    void Jump()
    {
        if (!isGrounded) return;

        if (jumpInput == 0)
        {
            canJump = true;
        }
        if (jumpInput != 0 && canJump)
        {
            moveSettings.vertVel = moveSettings.jumpSpd;
            animator.SetBool("Is Jumping", true);
            canJump = false;
        }
        //velocity.y += moveSettings.vertVel;
    }

    /// <summary>
    /// Calculate the player object's forward direction
    /// </summary>
    void CalculateForward()
    {
        if (!isGrounded)
        {
            forward = transform.forward;
            return;
        }

        forward = Vector3.Cross(groundHitInfo.normal, -transform.right);
    }

    void GetForward()
    {
        CalculateForward();

        forwardDirection = forwardInput * GetCamForward() + sidewardInput * playerCamera.transform.right;

        if (forwardDirection.magnitude > 1.0f) forwardDirection.Normalize();

        forwardDirection = transform.InverseTransformDirection(forwardDirection);
        if (groundAngle < moveSettings.maxGroundAngle)
        {
            velocity = (forward * forwardDirection.magnitude);
        }
        else
        {
            velocity = forward;
        }
    }

    void CalculateGroundAngle()
    {
        
        if (!isGrounded)
        {
            groundAngle = 90;
            return;
        }
        
        groundAngle = Vector3.Angle(groundHitInfo.normal, transform.forward);
    }
    void CheckGround()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out groundHitInfo, moveSettings.height + moveSettings.heightPadding, moveSettings.ground))
        {
            isGrounded = true;
            moveSettings.vertVel = 0.0f;
            animator.SetBool("Is Jumping", false);
            
            if (Vector3.Distance(transform.position, groundHitInfo.point) < moveSettings.height)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * (moveSettings.height + 3), 5 * Time.deltaTime);
            }
        }
        else { isGrounded = false; } 
    }
    void ApplyGravity()
    {
        
        if (isGrounded) return;
        moveSettings.vertVel += (physSettings.downAccel) * Time.deltaTime;
        if (moveSettings.vertVel < physSettings.termVel) moveSettings.vertVel = physSettings.termVel;
        // Amplify Vertical Velocity
        
        //velocity[1] = moveSettings.vertVel;

    }
    void CheckWall()
    {
        /*
        bool forwardCheck = Physics.Raycast(transform.position, transform.forward, out wallHitInfo, moveSettings.width + moveSettings.heightPadding, moveSettings.ground);
        bool backwardCheck = Physics.Raycast(transform.position, -transform.forward, out wallHitInfo, moveSettings.width + moveSettings.heightPadding, moveSettings.ground);
        if (forwardCheck || backwardCheck)
        {
            if (Vector3.Distance(transform.position, wallHitInfo.point) > moveSettings.width )
            {
                //transform.position = Vector3.Lerp(transform.position, transform.position - transform.forward * (moveSettings.width - 1), 5 * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, transform.position - transform.forward * (moveSettings.width - 1), 5 * Time.deltaTime);
            }
            else if (Vector3.Distance(transform.position, wallHitInfo.point) < moveSettings.width)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * (moveSettings.width + 1), 10 * Time.deltaTime);
            }
        

            moveSettings.forwardVel = 0;
        }
        */
        float distanceToPoints = moveSettings.height / 2 - moveSettings.width;
        Vector3 pointOne = transform.position + Vector3.up * (distanceToPoints + 3);
        Vector3 pointTwo = transform.position - Vector3.up * (distanceToPoints);
        float radius = moveSettings.width * 0.95f;
        float castDistance = 0.5f;
        RaycastHit[] hits = Physics.CapsuleCastAll(pointOne, pointTwo, radius, forward, castDistance);

        foreach (RaycastHit objectHit in hits)
        {
            if (objectHit.transform.tag == "PlayerColGround") Debug.Log("Ground");
            if (objectHit.transform.tag == "PlayerColWall") 
            {


                moveSettings.forwardVel = 0; Debug.Log(objectHit.transform.position); 
            
            }
        }
    }
    private void DrawDebugLines()
    {
        if (!moveSettings.debug) return;

        Debug.DrawLine(transform.position, transform.position + forward * moveSettings.height * 2, Color.yellow);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * moveSettings.height, Color.green);
        Debug.DrawLine(transform.position, transform.position + playerCamera.transform.forward * 10, Color.red);
        Debug.DrawLine(transform.position - transform.forward * moveSettings.width, transform.position + transform.forward * moveSettings.width, Color.blue);
        Debug.DrawLine(transform.position - transform.right, transform.position + transform.right, Color.red);
    }
}