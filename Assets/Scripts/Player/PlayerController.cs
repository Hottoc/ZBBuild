using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Option Classes
    [System.Serializable]
    public class MoveOptions
    {
        [Header("Movement Speeds")]
        public float walkSpeed = 7.0f;
        public float runSpeed = 15.0f;
        public float rotationSpeed = 3.0f;
        public float jumpSpeed = 6.0f;
        public float smoothSpeed = 10.0f;
        [Header("Other")]
        public float maxGroundAngle = 120;
        public bool smooth;
        public float orginTilt;
    }

    [System.Serializable]
    public class PlayerOptions
    {
        public float playerHeight = 3.8f;
    }

    [System.Serializable]
    public class PhysOptions
    {
        [Header("[X,Z] Velocity")]
        private float _forwardVelocity = 0.0f;
        [Header("[Y] Velocity")]
        private float _vertVelocity;
        private float _termVelocity = 50.0f;
        private float _rotateVelocity = 0.0f;
        [Header("Jump")]
        public float jumpForce = 6.0f;
        public float jumpDecrease = 0.5f;
        [Header("Gravity")]
        public float gravity = 2.5f;
        public LayerMask discludePlayer;
        [Header("References")]
        public SphereCollider sphereCol;
        public GameObject playerCamera;

        public float ForwardVelocity { get { return _forwardVelocity; } set { _forwardVelocity = value; } }
        public float VertVelocity { get { return _vertVelocity; } set { _vertVelocity = value; } }
        public float TermVelocity { get { return _termVelocity; } set { _termVelocity = value; } }
        public float RotateVelocity { get { return _rotateVelocity; } set { _rotateVelocity = value; } }
    }

    [System.Serializable]
    public class InputOptions
    {
        public float inputDelay = 0.1f;

        public string FORWARD_AXIS = "Vertical";
        public string SIDEWARD_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";
        public string RUN_AXIS = "Run";
    }
    #endregion

    #region Option Instances
    public MoveOptions moveOption = new MoveOptions();
    public PhysOptions physOption = new PhysOptions();
    public InputOptions inputOption = new InputOptions();
    public PlayerOptions playerOption = new PlayerOptions();
    #endregion 

    #region Variables


    [Header("Movement Options")]
    public bool smooth;
    public float smoothSpeed;


    Animator animator = null;

    //Private Variables

    //Movement Vectors
    private Vector3 velocity;
    private Vector3 forwardDirection;
    private Vector3 lastPos;
    private Vector3 follow;
    private Transform lastCheck;

    #endregion

    #region Main Methods

    private void Start()
    {
        physOption.playerCamera = GameObject.Find("Player Camera");
        animator = GetComponent<Animator>();
        moveOption.orginTilt = transform.eulerAngles.x;
        lastPos = transform.position;
        lastCheck = transform;
    }
    private void Update()
    {
        GetInput();

        Gravity();

        SimpleMove();

        Rotate();

        Jump();

        SetSpeed();

        FinalMove();

        GroundChecking();

        CollisionCheck();
    }

    #endregion

    #region Inputs
    float forwardInput, sidewardInput, jumpInput, runInput;
    private void GetInput()
    {
        forwardInput = Input.GetAxis(inputOption.FORWARD_AXIS); //interpolated
        sidewardInput = Input.GetAxis(inputOption.SIDEWARD_AXIS); //interpolated
        jumpInput = Input.GetAxis(inputOption.JUMP_AXIS); //non-interpolated
        runInput = Input.GetAxis(inputOption.RUN_AXIS); //non-interpolated
    }
    #endregion

    #region Movement Methods

    private void SimpleMove()
    {
        forwardDirection = forwardInput * GetCamForward() + sidewardInput * physOption.playerCamera.transform.right;

        if (forwardDirection.magnitude > 1.0f) forwardDirection.Normalize();

        forwardDirection = transform.InverseTransformDirection(forwardDirection);
        velocity += follow + (transform.forward * forwardDirection.magnitude);
        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //velocity += move;
    }

    private void FinalMove()
    {

        //Vector3 vel = new Vector3(velocity.x, velocity.y, velocity.z) * movementSpeed;
        //velocity = (new Vector3 (move.x, -currentGravity, move.z)+vel)*movementSpeed;
        //velocity = transform.TransformDirection (velocity);
        //vel = transform.TransformDirection(vel);

        velocity.y -= physOption.VertVelocity * 7;

        velocity.x *= physOption.ForwardVelocity;
        velocity.z *= physOption.ForwardVelocity;

        transform.position += follow + velocity * Time.deltaTime;
        animator.SetFloat("Forward Speed", physOption.ForwardVelocity);
        velocity = Vector3.zero;

    }

    private Vector3 GetCamForward()
    {
        return Vector3.Scale(physOption.playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
    }

    /// <summary>
    /// Rotate toward the calculated angle
    /// </summary>
    private void Rotate()
    {
        // Get Euler angles To 
        float turnAmount = Mathf.Atan2(forwardDirection.x, forwardDirection.z) * Mathf.Rad2Deg;

        physOption.RotateVelocity = turnAmount * moveOption.rotationSpeed;

        transform.Rotate(0, physOption.RotateVelocity * Time.deltaTime, 0);

        if (physOption.RotateVelocity > 100.0f || physOption.RotateVelocity < -100.0f)
        {
            float tiltVal = Mathf.Clamp(Mathf.Abs(physOption.ForwardVelocity / 8), 0, 1);
            transform.Rotate(-10 * tiltVal * Time.deltaTime, 0, 0);
            return;
        }
        restoreRotation();
        //transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(moveSettings.forwardVel / 10 * tiltVal, 0, forwardDirection.z / 14 * tiltVal)), Time.deltaTime * 2.0f);
    }

    private void restoreRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(moveOption.orginTilt, transform.eulerAngles.y, 0), Time.deltaTime * 5);
        return;
    }

    #endregion

    #region Speed
    private void SetSpeed()
    {
        if (!grounded)
        {
            physOption.ForwardVelocity = MinSpeed(physOption.ForwardVelocity, 2.0f, 0.0f);
            return;
        }

        if (forwardInput == 0 && sidewardInput == 0)
        {
            physOption.ForwardVelocity = 0;
            return;
        }

        float setSpeed = (runInput == 0.0f) ? moveOption.walkSpeed : moveOption.runSpeed;

        physOption.ForwardVelocity = MaxSpeed(physOption.ForwardVelocity, 15.0f, setSpeed);
    }
    private float MaxSpeed(float current, float speed, float max)
    {
        current += speed * Time.deltaTime;
        if (current > max) { current = max; }

        return current;
    }
    private float MinSpeed(float current, float speed, float min)
    {
        current -= speed * Time.deltaTime;
        if (current < min) { current = min; }

        return current;
    }

    #endregion

    #region Gravity/Grounding
    //Gravity Private Variables
    private bool grounded;

    //Grounded Private Variables
    private Vector3 liftPoint = new Vector3(0, 1.2f, 0);
    private RaycastHit groundHit;
    private Vector3 groundCheckPoint = new Vector3(0, -0.87f, 0);

    private void Gravity()
    {
        if (grounded) { return; }
        
        physOption.VertVelocity += physOption.gravity * 5 * Time.deltaTime;

        if (physOption.VertVelocity > physOption.TermVelocity) physOption.VertVelocity = physOption.TermVelocity;
    }

    private void GroundChecking()
    {
        Ray ray = new Ray(transform.TransformPoint(liftPoint), Vector3.down);
        RaycastHit tempHit = new RaycastHit();

        if (Physics.SphereCast(ray, 0.17f, out tempHit, playerOption.playerHeight, physOption.discludePlayer))
        {
            GroundConfirm(tempHit, ray);
        }
        else
        {
            follow = Vector3.zero;
            grounded = false;
        }

    }
    private void GroundConfirm(RaycastHit tempHit, Ray ray)
    {
        Collider[] col = new Collider[3];
        int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(groundCheckPoint), playerOption.playerHeight / 2, col, physOption.discludePlayer);

        grounded = false;
        follow = Vector3.zero;

        for (int i = 0; i < num; i++)
        {
            if (col[i].transform == tempHit.transform)
            {
                groundHit = tempHit;
                grounded = true;
                physOption.VertVelocity = 0;
                //Snapping 
                if (inputJump == false)
                {
                    if (!smooth)
                    {
                        transform.position = new Vector3(transform.position.x, (groundHit.point.y + playerOption.playerHeight / 2), transform.position.z);
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, (groundHit.point.y + playerOption.playerHeight / 2), transform.position.z), smoothSpeed * Time.deltaTime);
                    }
                    Vector3 offset = col[i].transform.position - lastPos;
                    if ((offset != Vector3.zero ) && groundHit.transform == lastCheck)
                    {
                        follow.x = offset.x;
                        follow.z = offset.z;
                        follow.y = offset.y;
                    }
                    lastPos = col[i].transform.position;
                    lastCheck = groundHit.transform;
                }

                break;
            }
        }
        
        if (num <= 1 && tempHit.distance <= 3.1f && inputJump == false)
        {
            if (col[0])
            {
                //Ray ray = new Ray(transform.TransformPoint(liftPoint), Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 3.1f, physOption.discludePlayer))
                {
                    if (hit.transform != col[0].transform)
                    {
                        grounded = false;
                        follow = Vector3.zero;
                        return;
                    }
                }
            }
        }
    }
    #endregion

    #region Collision
    Collider[] overlaps = new Collider[4];
    private void CollisionCheck()
    {
        int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(physOption.sphereCol.center), physOption.sphereCol.radius, overlaps, physOption.discludePlayer, QueryTriggerInteraction.UseGlobal);
        for (int i = 0; i < num; i++)
        {
            Transform t = overlaps[i].transform;
            Vector3 dir;
            float dist;
            
            if (Physics.ComputePenetration(physOption.sphereCol, transform.position, transform.rotation, overlaps[i], t.position, t.rotation, out dir, out dist))
            {
                Vector3 penetrationVector = dir * dist;
                Vector3 velocityProjected = Vector3.Project(velocity, -dir);
                //transform.position = transform.position + penetrationVector;
                transform.position = Vector3.Lerp(transform.position, transform.position + penetrationVector, (20 + physOption.ForwardVelocity) * Time.deltaTime);
                physOption.ForwardVelocity = MinSpeed(physOption.ForwardVelocity, 10.0f, 0.0f);
                //physOption.ForwardVelocity -= velocityProjected.x;
                velocity -= velocityProjected;
            }
        }
    }

    #endregion

    #region Jumping

    private bool inputJump = false;
    private bool onJump;
    private float lastInput = 0;
    private void Jump()
    {
        if (!grounded)
        {
            physOption.VertVelocity -= physOption.jumpDecrease * Time.deltaTime;
            onJump = false;
            return;
        }

        if (physOption.VertVelocity > 0.2f || physOption.VertVelocity <= 0.2f)
        {
            physOption.VertVelocity = 0;
            inputJump = false;
        }

        if (jumpInput == 1 && lastInput == 0) 
        {
            onJump = true;
        }

        if (onJump)
        {
            inputJump = true;
            physOption.VertVelocity -= physOption.jumpForce;
        }
        lastInput = jumpInput;
    }
    #endregion
    private void OnDrawGizmos()
    {
        Vector3 boxPos = new Vector3(transform.position.x, transform.position.y - playerOption.playerHeight / 2 - (Vector3.one / 2).y, transform.position.z);
        Vector3 boxSize = Vector3.one;
        if (!grounded)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireCube(boxPos, boxSize);
    } 
} 
  