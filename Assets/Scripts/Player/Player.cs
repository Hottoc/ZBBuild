using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Transform headBone;
    Transform rArmBone;
    // Components
    private CharacterController character;

    // Scripts
    private World world;

    // Objects
    private GameObject playerCamera, mouseCursor;

    private ControllerColliderHit _contact;
    RaycastHit hit;

    // Public Floats
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float rotationSpeed = 240.0f;
    //public float jumpHeight = 7.0f;
    public float pushPower = 2.0f;
    public float weight = 6.0f;

    //Test
    [SerializeField] private float jumpSpeed = 15.0f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f;
    public float vertSpeed;


    // Private Floats
    private float horizontal, vertical;
    public float gravity = -20.0f;
    
    // Vector3s
    private Vector3 velocity, moveDirection, camForwardDirection;

    // Bools
    [HideInInspector]
    public bool jumpRequest, isSprinting, isGrounded;

    private void Start ()
    {
        character = GetComponent<CharacterController>();

        world = GameObject.Find("World").GetComponent<World>();
        playerCamera = GameObject.Find("Player Camera");

        vertSpeed = minFall;
    }

    private void LateUpdate()
    {
 
    }
    private void Update()
    {
        // Call Player Movement Function
        MovePlayer();

        // Call Animations Function
        PlayAnimation();

        // Move the Character Controller
        character.Move(velocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            Vector3 force = new Vector3(0, -0.5f, 0) * gravity * weight;
            return;
        }
        
        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
        //body.AddForceAtPosition(force, hit.point);

        _contact = hit;
    }

    public enum myTest
    {
        Idle,
        Jump,
        Walk,
        Run
    }

    private void GetPlayerInputs ()
    {
        // Get Input for axis
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Run"))
        {
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Run"))
        {
            isSprinting = false;
        }
        // Request For Player To Jump When Pressing The "Jump" key
        if (Input.GetButtonDown("Jump"))
        {
            jumpRequest = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpRequest = false;
        }

        
    }
    private void MovePlayer()
    {
        // CHARACTER MOVEMENT

        // Call Function To Get Player Inputs
        GetPlayerInputs();

        // Multiply The Forward Vector Of The Camera Object By A Constant Vector To Get Player Desired Forward
        // Y Coordinate Will Always Equal Zero
        camForwardDirection = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        // Check If Player Can Move And Disable X And Z Movement If False
            // Calculate the forward vector 
            moveDirection = vertical * camForwardDirection + horizontal * playerCamera.transform.right;

        // Normalize Player Speed When Strafing.
        if (moveDirection.magnitude > 1.0f)
        {
            moveDirection.Normalize();
        }

        // Calculate The Forward Rotation Of The Player
        // Transform The Vector3 "moveDirection" From World Space To Local Space
        moveDirection = transform.InverseTransformDirection(moveDirection);

        // Get Euler angles To 
        float turnAmount = Mathf.Atan2(moveDirection.x, moveDirection.z);

        // Rotate The Player Along The Y-Axis
        transform.Rotate(0, turnAmount * rotationSpeed * Time.deltaTime, 0);

        bool hitGround = false;

        if (vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (character.height + character.radius) / 1.43f;
            hitGround = hit.distance <= check;
        }

        

        if (hitGround)
        {
            // Set Vector3 "velocity" To The Player's Forward Vector3 Multiplied By The Length Of Vector3 "moveDirection" 
            velocity = transform.forward * moveDirection.magnitude;

            if (isSprinting)
            {
                // Multiply Vector3 "velocity" By The Player's Sprint Speed 
                velocity *= sprintSpeed;
                //GetComponent<Animator>().SetBool("IsRunning", true);
            }
            else
            {
                // Multiply Vector3 "velocity" By The Player's Walk Speed 
                velocity *= walkSpeed;
                //GetComponent<Animator>().SetBool("IsRunning", false);
            }

            // Check For Jump Input Before Jumping And Player Can Jump Again.
            if (jumpRequest)
            {
                GetComponent<Animator>().SetBool("IsJumping", true);
                // Get The Jump Height On The Y Axis
                ;
                vertSpeed = jumpSpeed;
                jumpRequest = false;
            }
            else
            {
                GetComponent<Animator>().SetBool("IsJumping", false);
                vertSpeed = minFall;
            }
        }
        else
        {
            vertSpeed += gravity * 5 * Time.deltaTime;
            if (vertSpeed < terminalVelocity)
            {
                vertSpeed = terminalVelocity;
            }
        }

        velocity.y = vertSpeed;
    }

    private void PlayAnimation()
    {
        if (moveDirection.magnitude > 0f && !isSprinting && !jumpRequest)
        {
            //GetComponent<Animator>().SetBool("IsWalking", true);
        }
        else
        {
            //GetComponent<Animator>().SetBool("IsWalking", false);
        }
        

        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<Animator>().SetBool("IsEmote", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("IsEmote", false);
        }
    }

}
