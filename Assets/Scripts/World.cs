using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Awake()
    {
        Instantiate(playerPrefab, new Vector3(100, 4, -60), Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            DestroyImmediate(playerPrefab, true);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Instantiate(playerPrefab, new Vector3(100, 4, -60), Quaternion.identity);
        }
    }
}
