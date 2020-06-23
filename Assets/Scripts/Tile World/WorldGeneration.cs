using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference to Script design and implementation: http://devmag.org.za/2009/04/25/perlin-noise/

public class WorldGeneration : MonoBehaviour
{
    public Vector2 mapGridSize;
    int[,] mapGrid;
    // Start is called before the first frame update
    void Start()
    {
        GenerateNoiseMap(100, 100, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapWidth, mapHeight];

        for (int xIndex = 0; xIndex < mapWidth; xIndex++)
        {
            for (int yIndex = 0; yIndex < mapHeight; yIndex++)
            {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = xIndex / scale;
                float sampleY = yIndex / scale;

                // generate noise value using PerlinNoise
                noiseMap[xIndex,yIndex] = Mathf.PerlinNoise(sampleY * yIndex, sampleX * xIndex);
                Debug.Log("[" + xIndex + ":" + yIndex + "]" + noiseMap[xIndex, yIndex]);
            }
        }
    }
    /*************************************************************************
     - Create an array with random values between 0 and 1
    *************************************************************************/
    /*
    private void GenerateWhiteNoise()
    {
        Debug.Log("Script running");
        mapGrid = new int[(int)mapGridSize.x, (int)mapGridSize.y];

        for (int i = 0; i < mapGridSize.x; i++)
        {
            for (int j = 0; j < mapGridSize.y; j++)
            {
                Debug.Log("Tile: " + i + "," + j);
                mapGrid[i,j] = Random.Range(0, 1);
                Debug.Log("Test: " + mapGrid[i, j]);
            }
        }
    }
    */
    /*

    public static float[][] GenerateSmoothNoise(float[][] baseNoise, int octave)
    {
        int width = baseNoise.Length;
        int height = baseNoise[0].Length;

        float[][] smoothNoise = GetEmptyArray<float>(width, height);

        int samplePeriod = 1 << octave; // calculates 2 ^ k
        float sampleFrequency = 1.0f / samplePeriod;

        for (int i = 0; i < width; i++)
        {
            //calculate the horizontal sampling indices
            int sample_i0 = (i / samplePeriod) * samplePeriod;
            int sample_i1 = (sample_i0 + samplePeriod) % width; //wrap around
            float horizontal_blend = (i - sample_i0) * sampleFrequency;

            for (int j = 0; j < height; j++)
            {
                //calculate the vertical sampling indices
                int sample_j0 = (j / samplePeriod) * samplePeriod;
                int sample_j1 = (sample_j0 + samplePeriod) % height; //wrap around
                float vertical_blend = (j - sample_j0) * sampleFrequency;

                //blend the top two corners
                float top = Interpolate(baseNoise[sample_i0][sample_j0],
                    baseNoise[sample_i1][sample_j0], horizontal_blend);

                //blend the bottom two corners
                float bottom = Interpolate(baseNoise[sample_i0][sample_j1],
                    baseNoise[sample_i1][sample_j1], horizontal_blend);

                //final blend
                smoothNoise[i][j] = Interpolate(top, bottom, vertical_blend);
            }
        }

        return smoothNoise;
    }

    public static float Interpolate(float x0, float x1, float alpha)
    {
        return x0 * (1 - alpha) + alpha * x1;
    }

    public static float[][] GeneratePerlinNoise(float[][] baseNoise, int octaveCount)
    {
        int width = baseNoise.Length;
        int height = baseNoise[0].Length;

        float[][][] smoothNoise = new float[octaveCount][][]; //an array of 2D arrays containing

        float persistance = 0.7f;

        //generate smooth noise
        for (int i = 0; i < octaveCount; i++)
        {
            smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
        }

        float[][] perlinNoise = GetEmptyArray<float>(width, height); //an array of floats initialised to 0

        float amplitude = 1.0f;
        float totalAmplitude = 0.0f;

        //blend noise together
        for (int octave = octaveCount - 1; octave >= 0; octave--)
        {
            amplitude *= persistance;
            totalAmplitude += amplitude;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i][j] += smoothNoise[octave][i][j] * amplitude;
                }
            }
        }

        //normalisation
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                perlinNoise[i][j] /= totalAmplitude;
            }
        }

        return perlinNoise;
    }

    public static float[][] GeneratePerlinNoise(int width, int height, int octaveCount)
    {
        float[][] baseNoise = GenerateWhiteNoise(width, height);

        return GeneratePerlinNoise(baseNoise, octaveCount);
    }
    */
}
