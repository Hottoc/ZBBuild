using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public GameObject container;

    public int rows;
    public int columns;
    public int tileFootprint = 24;

    public int numOfSeeds;

    List<Vector3> seeds = new List<Vector3>();
    List<int> tilePrefabIndex = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        CreateRandomPoints();
        GenerateMap();
    }
    public void GenerateMap ()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 point = new Vector3(row * tileFootprint, 0, column * tileFootprint);

                if (!seeds.Contains(point))
                {
                    int closestPointIndex = FindClosestPoint(point);

                    GameObject tile = Instantiate(tilePrefabs[tilePrefabIndex[closestPointIndex]], point, Quaternion.identity);
                    tile.transform.parent = container.transform;
                }
            }
        }
    }

    private void CreateRandomPoints()
    {
        for (int i = 0; i < numOfSeeds; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(0, rows) * tileFootprint, 0, Random.Range(0, columns) * tileFootprint);
            seeds.Add(randomPosition);

            int RandomTileNumber = Random.Range(0, tilePrefabs.Length);
            tilePrefabIndex.Add(RandomTileNumber);

            GameObject tile = Instantiate(tilePrefabs[RandomTileNumber], randomPosition, Quaternion.identity);
            tile.transform.parent = container.transform;
        }
    }

    private int FindClosestPoint(Vector3 point)
    {
        int closestPointIndex = 0;
        var distance = Vector3.Distance(point, seeds[0]);

        for (int i = 0; i < seeds.Count; i++)
        {
            var tempDistance = Vector3.Distance(point, seeds[i]);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                closestPointIndex = i;
            }

        }
        return closestPointIndex;
    }
}
