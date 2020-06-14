using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{

    public static readonly int chunkWidth = 20;
    public static readonly int chunkHeight = 1;
    public static float voxelScaleWidth = 2.0f;
    public static float voxelScaleHeight = 0.25f;
    public static readonly int worldSizeInChunks = 5;
    public static readonly int ViewDistanceInChunks = 8;

    public static int worldSizeInBlocks
    {

        get { return worldSizeInChunks * chunkWidth; }

    }

    public static readonly int textureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {
        get { return 1.0f / (float)textureAtlasSizeInBlocks; }
    }

    public static readonly Vector3[] voxelVerts = new Vector3[8] {

        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(voxelScaleWidth, 0.0f, 0.0f),
        new Vector3(voxelScaleWidth, voxelScaleHeight, 0.0f),
        new Vector3(0.0f, voxelScaleHeight, 0.0f),
        new Vector3(0.0f, 0.0f, voxelScaleWidth),
        new Vector3(voxelScaleWidth, 0.0f, voxelScaleWidth),
        new Vector3(voxelScaleWidth, voxelScaleHeight, voxelScaleWidth),
        new Vector3(0.0f, voxelScaleHeight, voxelScaleWidth),

    };

    public static readonly Vector3[] faceChecks = new Vector3[6] {

        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f)

    };

    public static readonly int[,] voxelTris = new int[6, 4] {

        // Back, Front, Top, Bottom, Left, Right

		// 0 1 2 2 1 3
		{0, 3, 1, 2}, // Back Face
		{5, 6, 4, 7}, // Front Face
		{3, 7, 2, 6}, // Top Face
		{1, 5, 0, 4}, // Bottom Face
		{4, 7, 0, 3}, // Left Face
		{1, 2, 5, 6} // Right Face

	};

    public static readonly Vector2[] voxelUvs = new Vector2[4] {

        new Vector2 (0.0f, 0.0f),
        new Vector2 (0.0f, 1.0f),
        new Vector2 (1.0f, 0.0f),
        new Vector2 (1.0f, 1.0f)

    };


}