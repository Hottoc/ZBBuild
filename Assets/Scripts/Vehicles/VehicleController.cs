using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
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
    }
    #endregion

    #region Option Instances
    public MoveOptions moveOption = new MoveOptions();
    public PhysOptions physOption = new PhysOptions();
    public InputOptions inputOption = new InputOptions();
    public PlayerOptions playerOption = new PlayerOptions();
    #endregion 

    #region Inputs
    float forwardInput, sidewardInput;

    Vector3 velocity;
    Vector3 forwardDirection;
    Vector3 lastPos;
    #region Main Methods

    private void Start()
    {
        physOption.playerCamera = GameObject.Find("Player Camera");
        moveOption.orginTilt = transform.eulerAngles.x;
        lastPos = transform.position;
    }
    private void Update()
    {
        GetInput();

        Rotate();
        SimpleMove();
        
        SetSpeed();
        FinalMove();
        }

    #endregion

    private void GetInput()
    {
        forwardInput = Input.GetAxis(inputOption.FORWARD_AXIS); //interpolated
        sidewardInput = Input.GetAxis(inputOption.SIDEWARD_AXIS); //interpolated
    }
    #endregion

    private void SimpleMove()
    {
        //forwardDirection = new Vector3(0, 0, forwardInput); // -Z axis

        //if (forwardDirection.magnitude > 1.0f) forwardDirection.Normalize();

        //forwardDirection = transform.InverseTransformDirection(forwardDirection);
        velocity = transform.forward; // Vector 3 (x = 0, y = 0, z = current z)
        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //velocity += move;
    }

    private void FinalMove()
    {
        velocity.x *= physOption.ForwardVelocity;
        velocity.z *= physOption.ForwardVelocity;

        transform.position += velocity * Time.deltaTime;
        velocity = Vector3.zero;
    }

    private void Rotate()
    {
        //remove 65 to fix rapid spinning
        physOption.RotateVelocity = sidewardInput * (65 - (physOption.ForwardVelocity / 60)) * moveOption.rotationSpeed;

        this.transform.rotation *= Quaternion.Euler(0, physOption.RotateVelocity * Time.deltaTime, 0) ;
        
        if (physOption.RotateVelocity > 99.0f || physOption.RotateVelocity < -99.0f)
        {
            float tiltVal = Mathf.Clamp(Mathf.Abs(physOption.ForwardVelocity / 8), 0, 1);
            transform.Rotate(-6 * tiltVal * Time.deltaTime, 0, 0);
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

    #region Speed
    private void SetSpeed()
    {
            float setSpeed = 40.0f;
        if (forwardInput > 0)
        {
            physOption.ForwardVelocity = MaxSpeed(physOption.ForwardVelocity, 15.0f, setSpeed);
            return;
        }
        else if (forwardInput < 0)
        {
            physOption.ForwardVelocity = MinSpeed(physOption.ForwardVelocity, 25.0f, -20.0f);
            return;
        }

        // FV = 10 (MATH) 1
        if (physOption.ForwardVelocity > 0)
        {
            physOption.ForwardVelocity = MinSpeed(physOption.ForwardVelocity, 10.0f, 0);
        }
        else
        {
            physOption.ForwardVelocity = MaxSpeed(physOption.ForwardVelocity, 10.0f, 0);
        }
    }
    private float MaxSpeed(float current, float speed, float max)
    {
        current += speed * Time.deltaTime;
        if (current > max) { current = max; }

        return current;
    }
    private float MinSpeed(float current, float speed, float min) //-10, 0, 0
    {
        current -= speed * Time.deltaTime;
        Debug.Log(current);
        if (current < min) { current = min; }

        return current;
    }

    #endregion
}
