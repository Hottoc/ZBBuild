using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    public List<Tile> tiles = new List<Tile>();

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public GenerateTiles genTiles;

    public int octaves;
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    private void Awake()
    {
        BuildTileTypes();
    }

    void BuildTileTypes()
    {
        tiles = new List<Tile>()
        {
            new Tile("WATER", 1, true),
            new Tile("SAND", 3, false),
            new Tile("GRASS", 5, false),
            new Tile("ROCK", 8, false)
        };

        //Debug.Log(tiles.Find(tile => tile.type == "SAND"));
    }


    public void MapGeneration()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        genTiles.GenNoiseMap(noiseMap);


    }

    public void MapDestroy()
    {

        genTiles.RemoveMap();
    }
}
