using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followTemp : MonoBehaviour
{

    public GameObject wayPoint;
    public Transform attachPoint;
    void Start()
    {
    }

    void LateUpdate()
    {
        if (wayPoint != null)
        {
            transform.position = attachPoint.position;
            transform.rotation = attachPoint.rotation;
        }
        else Destroy(gameObject);
    }
}
