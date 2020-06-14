using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    World world;

    public float distance = 100.0f;
    public float xSpeed = 5.0f;
    public float ySpeed = 100.0f;

    private float yMinLimit = 0.0f;
    private float yMaxLimit = 80.0f;

    private float distanceMin = 25.0f;
    private float distanceMax = 100.0f;

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
        
        if (target)
        {
            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {

                x += 90.0f;

            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                x -= 90.0f;
            }

            
            // Reset The Value Of Angle X To 0 When Exceeding A 360 Degree Angle.
            if (x > 360.0f || x < -360.0f)
            {
                x = 0;

            }


            y = ClampAngle(y, yMinLimit, yMaxLimit);

            
            
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            if (Input.GetMouseButton(1))
            {
                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 40, distanceMin, distanceMax);
            }
            RaycastHit hit;

            if (Physics.Linecast(target.position, transform.position, out hit))
            {

                distance -= hit.distance;

            }

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