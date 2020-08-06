using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    World world;

    public float xSpeed = 5.0f; //Speed of Camera Yaw
    public float ySpeed = 100.0f; //Speed of Camera Pitch
    public float zSpeed = 2000.0f;

    //Camera Pitch Variables
    private float yMinLimit = 30.0f; //Min Distance on Camera Pitch
    private float yMaxLimit = 80.0f; //Max Distance on Camera Pitch

    //Camera Distance/Zoom Variables
    private float distance = 30.0f;
    private float distanceMin = 10.0f; //Min Distance on Camera Zoom
    private float distanceMax = 40.0f; //Max Distance on Camera Zoom
    public Vector3 velocity = Vector3.zero;
    float x = 0.0f;
    float y = 0.0f;
    float newDistanceMin;
    // Use this for initialization
    private void Start()
    {
        Vector3 angles = transform.eulerAngles;

        x = angles.y;
        y = angles.x;

        newDistanceMin = distanceMin;
    }

    void LateUpdate()
    {
        //Update This with Switch to improve performance.
        if (target)
        {
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 5 * Time.deltaTime;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 5 * Time.deltaTime;

                distance = Mathf.Clamp(distance - (Input.GetAxis("Mouse ScrollWheel") * zSpeed * Time.deltaTime), newDistanceMin, distanceMax);
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

            //Uncomment to prevent camera from moving through objects with colliders
            RaycastHit hit;

            float test = 0;
            
            if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
            {
                newDistanceMin = hit.transform.position.z;
                if (distance > newDistanceMin && distance < distanceMax)
                {
                    distance += hit.distance * 5 * Time.deltaTime;
                }
                else
                {
                    test += 2;
                }
                
            }
            else
            {
                newDistanceMin = distanceMin;
            }
            
            Quaternion rotation = Quaternion.Euler(y + test, x, 0);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);

            Vector3 position = rotation * negDistance + target.position;

            //transform.rotation = rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 8f * Time.deltaTime);

            //transform.position = position;

            //transform.position = Vector3.SmoothDamp(transform.position, position, ref velocity, .3f);
            transform.position = Vector3.Lerp(transform.position, position, 10f * Time.deltaTime);
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