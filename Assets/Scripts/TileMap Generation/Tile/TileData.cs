using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public string type;
    public float height;
    public bool isWater;
    public Mesh mesh;

    public Tile(string type, float height, bool isWater)
    {
        this.type = type;
        this.height = height;
        this.isWater = isWater;
    }

    public Tile(Tile tile)
    {
        this.type = tile.type;
        this.height = tile.height;
        this.isWater = tile.isWater;
    }
}

    