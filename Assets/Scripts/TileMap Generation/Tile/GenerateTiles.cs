using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTiles : MonoBehaviour
{
    public GameObject[] roads;
    public int width;
    public int height;
    private GameObject[,] prefab;
    bool needDestroy = false;
    public int lastWidth;
    public int lastHeight;
    public float setHeight;

    void Awake()
    {
        
    }

    public void GenNoiseMap(float[,] noiseMap)
    {
        width = noiseMap.GetLength(0);
        height = noiseMap.GetLength(1);

        prefab = new GameObject[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject makeObj = roads[0];
                setHeight = 0;
                if (noiseMap[x, y] > .25) { makeObj = roads[1]; setHeight = 1; }//SAND
                if (noiseMap[x, y] > .3) { makeObj = roads[2]; setHeight = 2; }//GRASS
                if (noiseMap[x, y] > .4) { makeObj = roads[2]; setHeight = 3; }//GRASS
                if (noiseMap[x, y] > .6) { makeObj = roads[2]; setHeight = 4; }//GRASS
                if (noiseMap[x, y] > .8) { makeObj = roads[3]; setHeight = 5; }//ROCK
                prefab[x,y] = Instantiate(makeObj, new Vector3(x * 48, 3 * setHeight, y * 48), makeObj.transform.rotation);
                prefab[x, y].transform.parent = this.transform;
            }
        }
        needDestroy = true;
        lastWidth = width;
        lastHeight = height;

}

    public void RemoveMap()
    {
        if(needDestroy)
        {
            for (int y = 0; y < lastHeight; y++)
            {
                for (int x = 0; x < lastWidth; x++)
                {
                    if (prefab[x, y] == null) return;
                    Destroy(prefab[x, y]);
                }
            }
            needDestroy = false;
        }
        
    }
}
