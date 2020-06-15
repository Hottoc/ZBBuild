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

    // Public Floats
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float rotationSpeed = 240.0f;
    public float jumpHeight = 7.0f;

    // Private Floats
    private float horizontal, vertical;
    private float gravity = -20.0f;
    
    // Vector3s
    private Vector3 velocity, moveDirection, camForwardDirection;

    // Bools
    [HideInInspector]
    public bool playerCanMove, jumpRequest, isSprinting, isGrounded;

    private void Start ()
    {
        character = GetComponent<CharacterController>();

        world = GameObject.Find("World").GetComponent<World>();
        playerCamera = GameObject.Find("Player Camera");

        headBone = GameObject.Find("Player/HeadBone").transform;
        rArmBone = GameObject.Find("Player/RootBone/TorsoBone/RightArmBone").transform;
    }

    private void LateUpdate()
    {
 
    }
    private void Update()
    {
        var origin = headBone.position;
        // Call Player Movement Function
        MovePlayer();

        // Call Animations Function
        //PlayAnimation();

        // Move the Character Controller
        character.Move(velocity * Time.deltaTime);
    }

    private void GetPlayerInputs ()
    {
        // Get Input for axis
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
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
        // Open Inventory When Pressing The "I" Key
        if (Input.GetKeyDown(KeyCode.I))
        {
            world.inUI = !world.inUI;

            Debug.Log("Inventory Access: " + world.inUI);
        }
        // Disable The Player's Ability To Move If Opening Inventory.
        if (!world.inUI)
        {
            playerCanMove = true;
        }
        else
        {
            playerCanMove = false;
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
        if (playerCanMove)
        {
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

            if (character.isGrounded)
            {
                // Set Vector3 "velocity" To The Player's Forward Vector3 Multiplied By The Length Of Vector3 "moveDirection" 
                velocity = transform.forward * moveDirection.magnitude;

                if (isSprinting)
                {
                    // Multiply Vector3 "velocity" By The Player's Sprint Speed 
                    velocity *= sprintSpeed;
                }
                else
                {
                    // Multiply Vector3 "velocity" By The Player's Walk Speed 
                    velocity *= walkSpeed;
                }

                // Check For Jump Input Before Jumping And Player Can Jump Again.
                if (jumpRequest)
                {
                    //GetComponent<Animator>().SetBool("IsJumping", true);
                    // Get The Jump Height On The Y Axis
                    velocity.y = jumpHeight;
                    jumpRequest = false;
                }
                else
                {
                    //GetComponent<Animator>().SetBool("IsJumping", false);
                }
            }
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        // Apply Gravity
        if (!character.isGrounded)
        {
            ApplyGravity();
        }
    }

    private void ApplyGravity()
    {
        if (character.isGrounded)
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
    }
/*
    private void PlayAnimation()
    {
        if (moveDirection.magnitude > 0f && isSprinting == true)
        {
            //GetComponent<Animator>().Play("Player|Run");
        }
        else if (moveDirection.magnitude > 0f && !isSprinting && !jumpRequest && playerCanMove)
        {
            GetComponent<Animator>().SetBool("IsWalking", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("IsWalking", false);
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
*/
}
