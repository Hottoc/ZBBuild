using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private bool _inUI = false;

    public Material material;

    public BlockType[] blockTypes;

    public GameObject debugScreen;
    public Transform player;
    public Vector3 spawn;
    public Transform camera;

    // Start is called before the first frame update
    private void Start()
    {
        debugScreen.SetActive(false);
        // Create Player Object

        // Generate The Chunks
        // Spawn The Player
        //GenerateWorld();


    }


    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F3))
        {
            debugScreen.SetActive(!debugScreen.activeSelf);
        }

    }

    void GenerateWorld()
    {
        for (int x = 0; x < VoxelData.worldSizeInChunks; x++)
        {
            for (int z = 0; z < VoxelData.worldSizeInChunks; z++)
            {

                Chunk newChunk = new Chunk(new ChunkCoord(x, z), this);

            }
        }
        //spawn = new Vector3(VoxelData.worldSizeInBlocks / 2, VoxelData.chunkHeight + 2, VoxelData.worldSizeInBlocks / 2);
        //player.position = spawn;            
    }

    public bool inUI
    {
        get { return _inUI; }
        set { _inUI = value; }
    }
}

[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    // Back, Front, Top, Bottom, Left, Right
    public int GetTextureID (int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error In GetTextureID; Invalid Face Index");
                return 0;
        }
    }
}
