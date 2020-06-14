using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public int openingDirection;
    // 1 --> Needs Bottom Tile
    // 2 --> Needs Top Tile
    // 3 --> Needs Left Tile
    // 4 --> Needs Right Tile

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(openingDirection == 1)
        {
            // Needs To Spawn A Road With A Bottom Connection
        }
        else if
            (openingDirection == 2)
        {
            // Needs To Spawn A Road With A Top Connection
        }
        else if
            (openingDirection == 3)
        {
            // Needs To Spawn A Road With A Left Connection
        }
        else if
            (openingDirection == 4)
        {
            // Needs To Spawn A Road With A Right Connection
        }
    }
}
