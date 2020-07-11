using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    World world;

    public float xSpeed = 5.0f; //Speed of Camera Yaw
    public float ySpeed = 100.0f; //Speed of Camera Pitch

    //Camera Pitch Variables
    private float yMinLimit = 0.0f; //Min Distance on Camera Pitch
    private float yMaxLimit = 80.0f; //Max Distance on Camera Pitch

    //Camera Distance/Zoom Variables
    private float distance = 30.0f;
    private float distanceMin = 10.0f; //Min Distance on Camera Zoom
    private float distanceMax = 30.0f; //Max Distance on Camera Zoom

    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    private void Start()
    {
        Vector3 angles = transform.eulerAngles;

        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        //Update This with Switch to improve performance.
        
        if (target)
        {
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 40, distanceMin, distanceMax);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }


            // Reset The Value Of Rotation Angle To 0 When Exceeding A 360 Degree Angle.
            if (x > 360.0f || x < -360.0f)
            {
                x = 0;
            }


            y = ClampAngle(y, yMinLimit, yMaxLimit);

            
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            /***********
            //Uncomment to prevent camera from moving through objects with colliders
            RaycastHit hit;

            
            if (Physics.Linecast(target.position, transform.position, out hit))
            {

                distance -= hit.distance;

            }
            */

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);

            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;

            transform.position = position;

        }
        
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
        {
            angle += 360F;
        } 
        if (angle > 360F)
        {
            angle -= 360F;
        }
            
        return Mathf.Clamp(angle, min, max);
    }
}