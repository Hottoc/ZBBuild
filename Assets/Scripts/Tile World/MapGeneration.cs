using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public Vector3 mapGridSize;
    public float noiseScale;
    public GameObject[] buildings;
    private GameObject[,,] prefab;
    int [,,] mapGrid;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;



    void Start()
    {
        GenerateMap((int)mapGridSize.y, (int)mapGridSize.x, (int)mapGridSize.z);
        
        for (int y = 0; y < mapGridSize.y; y++) //0
        {
            for (int z = 0; z < mapGridSize.z; z++)//0
            {
                for (int x = 0; x < mapGridSize.x; x++) //0
                {
                    Vector3 pos = new Vector3(x * 48, y * 1, z * 48);
                    Instantiate(buildings[0], pos, Quaternion.identity);

                    //prefab[x, y, z].transform.parent = this.transform;
                }
            }
        }
        
    }

    public void GenerateMap(int height, int width, int length)
    {
        mapGrid = new int[(int)mapGridSize.x, (int)mapGridSize.y, (int)mapGridSize.z];

        for (int y = 0; y < height; y++) //1
        {
            for (int x = 0; x < width; x++) //0
            {
                for (int z = 0; z < length; z++) //0-4
                {
                    mapGrid[x, y, z] = 0;
                    Debug.LogFormat("{0}|{1}|{2} - {3}", x, y, z, mapGrid[x, y, z]);
                }
            }
        }
    }
}
