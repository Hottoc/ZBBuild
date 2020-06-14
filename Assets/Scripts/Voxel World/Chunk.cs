using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    public ChunkCoord coord;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    GameObject chunkObject;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    World world;

    public Chunk(ChunkCoord _coord, World _world)
    {

        coord = _coord;
        world = _world;
        chunkObject = new GameObject();
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.chunkWidth, 0f, coord.z * VoxelData.chunkWidth);

        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        

        chunkObject.transform.SetParent(world.transform);
        meshRenderer.material = world.material;

        chunkObject.name = coord.x + ", " + coord.z;

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();

    }

    void PopulateVoxelMap()
    {

        for (int y = 0; y < VoxelData.chunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    if (y == VoxelData.chunkHeight - 1)
                    {
                        voxelMap[x, y, z] = 0;
                    }
                    else
                    {
                        voxelMap[x, y, z] = 1;
                    }
                }
            }
        }

    }

    void CreateMeshData()
    {

        for (int y = 0; y < VoxelData.chunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {

                    AddVoxelDataToChunk(new Vector3(x, y, z));

                }
            }
        }
    }

    bool CheckVoxel(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.chunkWidth - 1 || y < 0 || y > VoxelData.chunkHeight - 1 || z < 0 || z > VoxelData.chunkWidth - 1)
            return false;

        return world.blockTypes[voxelMap[x, y, z]].isSolid;

    }

    void AddVoxelDataToChunk(Vector3 pos)
    {

        for (int p = 0; p < 6; p++)
        {

            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
                Vector3 adjust = new Vector3(pos.x * VoxelData.voxelScaleWidth, pos.y * VoxelData.voxelScaleHeight, pos.z * VoxelData.voxelScaleWidth);

                vertices.Add(adjust + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(adjust + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(adjust + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(adjust + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);
                AddTexture(world.blockTypes[blockID].GetTextureID(p));
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;

            }
        }
    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void AddTexture (int textureID)
    {
        float y = textureID / VoxelData.textureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1.0f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }

}

public class ChunkCoord
{

    public float x;
    public float z;

    public ChunkCoord(int _x, int _z)
    {
        x = _x * VoxelData.voxelScaleWidth;
        z = _z * VoxelData.voxelScaleWidth;
    }
}