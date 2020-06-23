using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWorld : MonoBehaviour
{

    World world;
    public GameObject[] buildings;
    public GameObject[] roads;
    public Vector2 mapGridSize;
    private GameObject[,] prefab;
    int[,] mapGrid;
    int buildingFootprint = 24;
    public int borderX = 4;
    public int borderY = 4;
    public int seedoffset;

    void Start()
    {
        prefab = new GameObject[(int) mapGridSize.x, (int) mapGridSize.y];
        world = GameObject.Find("World").GetComponent<World>();

        ApplyMapNoise();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            
        }
        if (Input.GetKeyUp(KeyCode.F4))
        {
            DestroyTile();
            ApplyMapNoise();
            GenerateRoads();
            StartCoroutine(GenerateTiles());
        }
    }

    void ApplyMapNoise()
    {
        mapGrid = new int[(int)mapGridSize.x, (int)mapGridSize.y];
        //
        //generate map data
        for (int h = 0; h < mapGridSize.y; h++)
        {
            for (int w = 0; w < mapGridSize.x; w++)
            {
                mapGrid[w, h] = (int)(Mathf.PerlinNoise(seedoffset + (w / Random.Range(50.0f, 100.0f)), seedoffset + (h / Random.Range(50.0f, 100.0f))) * Random.Range(10.0f, 30.0f));
            }
        }


    }

    void GenerateBorder(int newBorderXStart, int newBorderXEnd, int newBorderYStart, int newBorderYEnd)
    {
        int borderXStartOffset = (newBorderXStart + 1);
        int borderYStartOffset = (newBorderYStart + 1);
        int borderXEndOffset = (newBorderXEnd + 1);
        int borderYEndOffset = (newBorderYEnd + 1);

        // Build Border
        for (int h = newBorderYStart; h < (mapGridSize.y - newBorderYEnd); h++) // Run Along X Axis
        {
            for (int w = newBorderXStart; w < (mapGridSize.x - newBorderXEnd); w++) // Run Along Y Axis
            {
                // Generate Corners First
                // Apply Bottom Left | Apply Top Right | Apply Bottom Right | Apply Top Right
                
                if ((w == newBorderXStart && h == newBorderYStart) || (w == newBorderXStart && h == (mapGridSize.y - borderYEndOffset)) || (w == (mapGridSize.x - borderXEndOffset) && h == newBorderYStart) || (w == (mapGridSize.x - borderXEndOffset) && h == (mapGridSize.y - borderYEndOffset)))
                {
                    mapGrid[w, h] = -2;
                }
                else
                {
                    // Check If Tile Is Not A Corner
                    if (mapGrid[w, h] != -3)
                    {
                        if ((w == newBorderXStart || w == (mapGridSize.x - borderXEndOffset)) && (mapGrid[w + 1, h] >= 0 && mapGrid[w - 1, h] >= 0)) // RUn Straight Lines
                        {
                            // && mapGrid[w, h - 1] > -3 && mapGrid[w, h - 2] > -3
                            if (h > (borderXStartOffset + 1) && h < (mapGridSize.y - (borderYEndOffset + 1)) && GetRandom(1, 100) <= 10 && (mapGrid[w, h + 1] != -4 && mapGrid[w, h - 1] != -4 && mapGrid[w, h - 2] != -4 && mapGrid[w + 1, h] >= 0 && mapGrid[w - 1, h] >= 0 && mapGrid[w + 1, h + 1] >= 0 && mapGrid[w - 1, h + 1] >= 0 && mapGrid[w + 1, h - 1] >= 0 && mapGrid[w - 1, h - 1] >= 0))
                            {
                                mapGrid[w, h] = -4; //T-Road THIS NEEDS TO BE IMPROVED FOR BOTH NORTH & SOUTH FACE
                            }
                            else
                            {
                                mapGrid[w, h] = -1; //Straight Road Facing North-South
                            }
                        }

                        else if ((h == newBorderYStart || h == (mapGridSize.y - borderYEndOffset)) && (mapGrid[w, h + 1] >= 0 && mapGrid[w, h - 1] >= 0))
                        {
                            // && mapGrid[w - 1, h] > -3 && mapGrid[w - 2, h] > -3
                            if (w > (borderYStartOffset + 1) && w < (mapGridSize.x - (borderXEndOffset + 1)) && GetRandom(1, 100) <= 10  && (mapGrid[w + 1, h] != -4 && mapGrid[w - 1, h] != -4 && mapGrid[w, h - 2] != -4 && mapGrid[w, h + 1] >= 0 && mapGrid[w, h - 1] >= 0 && mapGrid[w + 1, h + 1] >= 0 && mapGrid[w + 1, h - 1] >= 0 && mapGrid[w - 1, h + 1] >= 0 && mapGrid[w - 1, h - 1] >= 0))
                            {
                                mapGrid[w, h] = -4; //T-Road THIS NEEDS TO BE IMPROVED FOR BOTH EAST & WEST FACE
                            }
                            else
                            {
                                mapGrid[w, h] = -1; //Straight Road Facing East-West
                            }
                        }
                    }
                }
            }
        } 
    } // End Of Border Generator

    void GeneratePath(int currentPathX, int currentPathY, int directionX, int directionY)
    {
        bool pathFinished = false;

        int pathDirectionX = directionX;
        int pathDirectionY = directionY;

        // Loop Until The Path Is Finished
        do
        {
            // Generate A Straight Path Between 5 To 30 Tiles From Origin 
            for (int path = 1; path < GetRandom(5, 30); path++)
            {
                if (pathDirectionY == 0 && (mapGrid[currentPathX, currentPathY + pathDirectionX] < 0 || mapGrid[currentPathX, currentPathY - pathDirectionX] < 0)) // If Moving In The X Direction
                {
                    if (mapGrid[currentPathX + pathDirectionX, currentPathY + pathDirectionY] >= 0 && mapGrid[currentPathX + pathDirectionX + pathDirectionY, currentPathY + pathDirectionY + pathDirectionX] >= 0 && mapGrid[currentPathX + pathDirectionX - pathDirectionY, currentPathY + pathDirectionY - pathDirectionX] >= 0)
                    {
                        mapGrid[currentPathX, currentPathY] = -1;
                        //mapGrid[currentPathX + pathDirectionX, currentPathY] = -4;
                    }
                    pathFinished = true;
                    break;
                }
                else if (pathDirectionX == 0 && (mapGrid[currentPathX + pathDirectionY, currentPathY] < 0 || mapGrid[currentPathX - pathDirectionY, currentPathY] < 0)) // If Moving In The Y Direction
                {
                    if (mapGrid[currentPathX + pathDirectionX, currentPathY + pathDirectionY] >= 0 && mapGrid[currentPathX + pathDirectionX + pathDirectionY, currentPathY + pathDirectionY + pathDirectionX] >= 0 && mapGrid[currentPathX + pathDirectionX - pathDirectionY, currentPathY + pathDirectionY - pathDirectionX] >= 0)
                    {
                        mapGrid[currentPathX, currentPathY] = -1;
                        //mapGrid[currentPathX, currentPathY + pathDirectionY] = -4;
                    }
                    pathFinished = true;
                    break;
                }
                else if (mapGrid[currentPathX + pathDirectionX, currentPathY + pathDirectionY] >= 0) // If Next Tile In Direction Is Not A Road
                {
                    mapGrid[currentPathX, currentPathY] = -1;
                    currentPathX += pathDirectionX;
                    currentPathY += pathDirectionY;
                }
                else
                {
                    break;
                }
            }
            if (mapGrid[currentPathX + pathDirectionX, currentPathY + pathDirectionY] < 0 && !pathFinished) // If The Next Tile In Front Of The Path Is A Road
            {
                mapGrid[currentPathX, currentPathY] = -1;
                mapGrid[currentPathX + pathDirectionX, currentPathY + pathDirectionY] = -4;
                pathFinished = true;
            }
            else if (!pathFinished)
            {
                mapGrid[currentPathX, currentPathY] = -2;

                if (GetRandom(1, 100) <= 50)
                {
                    if (pathDirectionY != 0)
                    {
                        pathDirectionY = 0;
                        pathDirectionX = 1;
                    }
                    else
                    {
                        pathDirectionX = 0;
                        pathDirectionY = 1;
                    }
                }
                else
                {
                    if (pathDirectionY == 0)
                    {
                        pathDirectionY = -1;
                        pathDirectionX = 0;
                    }
                    else
                    {
                        pathDirectionX = -1;
                        pathDirectionY = 0;
                    }
                }
            }
            currentPathX += pathDirectionX;
            currentPathY += pathDirectionY;
        }
        while (!pathFinished);
    }

    void GenerateRoads()
    {
        // Build Border
        // Left Width | Right Width | Bottom Height | Top Height 
        GenerateBorder(borderX, borderX, borderY, borderY);
        // Build Interior Borders Based On Random Values
        // Left Width | Right Width | Bottom Height | Top Height
        for (int b = 1; b <= GetRandom(1,12); b++)
        {
            GenerateBorder(borderX + GetRandom(((b * 10) / GetRandom(5, 10)), (b * 10)), borderX + GetRandom(((b * 10) / GetRandom(5, 10)), (b * 10)), borderY + GetRandom(((b * 10) / GetRandom(5, 10)), (b * 10)), borderY + GetRandom(((b * 10) / GetRandom(5, 10)), (b * 10)));
        }
        int borderXOffset = (borderX + 1);
        int borderYOffset = (borderY + 1);
        ///*
        // Cycle Through Inside The City Grid
        for (int h = borderYOffset; h < (mapGridSize.y - borderYOffset); h++) // Run Along X Axis
        {
            for (int w = borderXOffset; w < (mapGridSize.x - borderXOffset); w++) // Run Along Y Axis
            {
                // Look for T Road
                // Check Left | Check Right | Check Bottom | Check Top
                if (mapGrid[w, h] > 0)
                {
                    if (mapGrid[w, h - 1] == -4 && mapGrid[w + 1, h - 1] == -1 && mapGrid[w - 1, h - 1] == -1 && mapGrid[w, h - 2] > 0)
                    {
                        GeneratePath(w, h, 0, 1);
                    }
                    else if (mapGrid[w, h + 1] == -4 && mapGrid[w + 1, h + 1] == -1 && mapGrid[w - 1, h + 1] == -1 && mapGrid[w, h + 2] > 0)
                    {
                        GeneratePath(w, h, 0, -1);
                    }
                    else if (mapGrid[w - 1, h] == -4 && mapGrid[w - 1, h + 1] == -1 && mapGrid[w - 1, h - 1] == -1 && mapGrid[w - 2, h] > 0)
                    {
                        GeneratePath(w, h, 1, 0);
                    }
                    else if (mapGrid[w + 1, h] == -4 && mapGrid[w + 1, h + 1] == -1 && mapGrid[w + 1, h - 1] == -1 && mapGrid[w + 2, h] > 0)
                    {
                        GeneratePath(w, h, -1, 0);
                    }
                }
            }
        }
        PolishRoads(borderX, borderX, borderY, borderY);
        //*/
    }

    void PolishRoads(int newBorderXStart, int newBorderXEnd, int newBorderYStart, int newBorderYEnd)
    {
        for (int h = newBorderYStart; h < (mapGridSize.y - newBorderYEnd); h++) // Run Along X Axis
        {
            for (int w = newBorderXStart; w < (mapGridSize.x - newBorderXEnd); w++) // Run Along Y Axis
            {
                
                if (mapGrid[w, h] < 0)
                {
                    if (mapGrid[w, h + 1] < 0 && mapGrid[w, h - 1] < 0 && mapGrid[w + 1, h] < 0 && mapGrid[w - 1, h] < 0) //Find Centers
                    {
                        mapGrid[w, h] = -3; //CENTER
                    }
                    if (((mapGrid[w, h + 1] < 0 && mapGrid[w, h - 1] < 0) && (mapGrid[w + 1, h] >= 0 && mapGrid[w - 1, h] >= 0)) || ((mapGrid[w + 1, h] < 0 && mapGrid[w - 1, h] < 0) && (mapGrid[w, h + 1] >= 0 && mapGrid[w, h - 1] >= 0)))
                    {
                        mapGrid[w, h] = -1; //STRAIGHT
                    }
                    if (mapGrid[w, h + 1] < 0 && mapGrid[w, h - 1] < 0 && ((mapGrid[w - 1, h] < 0 && mapGrid[w + 1, h] >= 0) || (mapGrid[w + 1, h] < 0 && mapGrid[w - 1, h] >= 0)))
                    {
                        mapGrid[w, h] = -4; //T 
                    }
                    if (mapGrid[w + 1, h] < 0 && mapGrid[w - 1, h] < 0 && ((mapGrid[w, h - 1] < 0 && mapGrid[w, h + 1] >= 0) || (mapGrid[w, h + 1] < 0 && mapGrid[w, h - 1] >= 0)))
                    {
                        mapGrid[w, h] = -4; // T
                    }
                }
            }
        }
    }

    IEnumerator GenerateTiles()
    {
        
        // Populate Tiles
        for (int h = 0; h < mapGridSize.y; h++)
        {
            for (int w = 0; w < mapGridSize.x; w++)
            {
                yield return new WaitForSeconds(.05f);

                string newTileError = "-";
                int result = mapGrid[w, h];
                Vector3 pos = new Vector3(w * buildingFootprint, 0,  h * buildingFootprint);

                if (result == -3) // Tile Is Center Road
                {
                    if (mapGrid[w, h + 1] < 0 && mapGrid[w, h - 1] < 0 && mapGrid[w - 1, h] < 0 && mapGrid[w + 1, h] < 0)
                    {
                        prefab[w, h] = Instantiate(roads[0], pos, roads[0].transform.rotation);
                    }
                    else
                    {
                        Debug.Log("Error At GenerateBuildings: Could Not Find Center Road Placement For Tile [" + w + "," + h + "]");
                        newTileError += " Center Tile Gen Error";
                        prefab[w, h] = Instantiate(buildings[2], pos, Quaternion.identity);
                    }   
                }
                else if (result == -4) // Tile Is A T Road
                {
                    if (mapGrid[w, h + 1] < 0 && mapGrid[w, h - 1] < 0 && mapGrid[w + 1, h] < 0 && mapGrid[w - 1, h] >= 0) // Right Face
                    {
                        prefab[w, h] = Instantiate(roads[3], pos, Quaternion.Euler(new Vector3(-90, 0, -90)));
                    }
                    else if (mapGrid[w + 1, h] < 0 && mapGrid[w - 1, h] < 0 && mapGrid[w, h - 1] < 0 && mapGrid[w, h + 1] >= 0) // Bottom Face
                    {
                        prefab[w, h] = Instantiate(roads[3], pos, Quaternion.Euler(new Vector3(-90, 0, 0)));
                    }
                    else if (mapGrid[w, h + 1] < 0 && mapGrid[w, h - 1] < 0 && mapGrid[w - 1, h] < 0  && mapGrid[w + 1, h] >= 0) // Left Face
                    {
                        prefab[w, h] = Instantiate(roads[3], pos, Quaternion.Euler(new Vector3(-90, 0, 90)));
                    }
                    else if (mapGrid[w + 1, h] < 0 && mapGrid[w - 1, h] < 0 && mapGrid[w, h + 1] < 0 && mapGrid[w, h - 1] >= 0) // Top Face
                    {
                        prefab[w, h] = Instantiate(roads[3], pos, Quaternion.Euler(new Vector3(-90, 0, -180)));
                    }
                    else
                    {
                        Debug.Log("Error At GenerateBuildings: Could Not Find T Road Placement For Tile [" + w + "," + h + "]");
                        newTileError += " T Tile Gen Error";
                        prefab[w, h] = Instantiate(buildings[2], pos, Quaternion.identity);
                    }
                }
                else if (result == -2) // Tile Is A Corner Road
                {
                    if (mapGrid[w, h + 1] < 0 && mapGrid[w + 1, h] < 0 && mapGrid[w - 1, h] >= 0 && mapGrid[w, h - 1] >= 0) // Right To Up
                    {
                        prefab[w, h] = Instantiate(roads[2], pos, Quaternion.Euler(new Vector3(-90, 0, -90)));
                    }
                    else if (mapGrid[w, h - 1] < 0 && mapGrid[w + 1, h] < 0 && mapGrid[w - 1, h] >= 0 && mapGrid[w, h + 1] >= 0) // Bottom To Right
                    {
                        prefab[w, h] = Instantiate(roads[2], pos, roads[2].transform.rotation);
                        // Road: {Bottom} {Right} 
                        // Terrain: {Top} {Left}
                    }
                    else if (mapGrid[w, h - 1] < 0 && mapGrid[w - 1, h] < 0 && mapGrid[w + 1, h] >= 0 && mapGrid[w, h + 1] >= 0) // Left To Bottom
                    {
                        prefab[w, h] = Instantiate(roads[2], pos, Quaternion.Euler(new Vector3(-90, 0, 90)));
                    }
                    else if (mapGrid[w - 1, h] < 0 && mapGrid[w, h + 1] < 0 && mapGrid[w + 1, h] >= 0 && mapGrid[w, h - 1] >= 0)
                    {
                        prefab[w, h] = Instantiate(roads[2], pos, Quaternion.Euler(new Vector3(-90, 0, 180)));
                    }
                    else
                    {
                        Debug.Log("Error At GenerateBuildings: Could Not Find Corner Road Placement For Tile [" + w + "," + h + "]");
                        newTileError += " Corner Tile Gen Error";
                        prefab[w, h] = Instantiate(buildings[2], pos, Quaternion.identity);
                    }
                }
                else if (result == -1) // Tile Is A Straight Road
                {
                    if (mapGrid[w, h + 1] < 0 && mapGrid[w, h - 1] < 0 && mapGrid[w + 1, h] >= 0 && mapGrid[w - 1, h] >= 0)
                    {
                        prefab[w, h] = Instantiate(roads[1], pos, Quaternion.Euler(new Vector3(-90, 0, -90))); 
                    }
                    else if ((mapGrid[w + 1, h] < 0 || mapGrid[w - 1, h] < 0) && mapGrid[w, h + 1] >= 0 && mapGrid[w, h - 1] >= 0)
                    {
                        prefab[w, h] = Instantiate(roads[1], pos, roads[1].transform.rotation);
                    }
                    else
                    {
                        Debug.Log("Error At GenerateBuildings: Could Not Find Straight Road Placement For Tile [" + w + "," + h + "]");
                        newTileError += " Straight Tile Gen Error";
                        prefab[w, h] = Instantiate(buildings[2], pos, Quaternion.identity);
                    }
                }
                else if (result >= 0)
                {
                    /*
                    if ((w >= (borderX + 1) && h >= (borderY + 1) && w < (mapGridSize.x - (borderX + 1)) && h < (mapGridSize.y - (borderY + 1))) && (mapGrid[w + 1, h] < 0 || mapGrid[w - 1, h] < 0 || mapGrid[w, h + 1] < 0 || mapGrid[w, h - 1] < 0))
                    {
                    */
                        if (result < 6)
                        prefab[w, h] = Instantiate(buildings[0], pos, Quaternion.identity);
                        else
                        prefab[w, h] = Instantiate(buildings[1], pos, Quaternion.identity);
                        /*
                    }
                    else
                    {
                        prefab[w, h] = Instantiate(buildings[1], pos, Quaternion.identity);
                    }
                    */
                }
                else
                {
                    prefab[w, h] = Instantiate(buildings[2], pos, Quaternion.identity);
                }

                //Assign Tile GameObject As Parent
                prefab[w, h].transform.parent = this.transform;
                // Assign Title Name
                prefab[w, h].name = "Tile: [" + w + " / " + h + "] | Value: [" + mapGrid[w, h] + "]" + newTileError;
            }
        }
    }

    int GetRandom(int min, int max)
    {
        int newRandom = Random.Range(min, max);

        return newRandom;
    }

    public void DestroyTile()
    {
        for (int h = 0; h < mapGridSize.y; h++)
        {
            for (int w = 0; w < mapGridSize.x; w++)
            {
                Destroy(prefab[w, h]);
            }
        }
    }

}