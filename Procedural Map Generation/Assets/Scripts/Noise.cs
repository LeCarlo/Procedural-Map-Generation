using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{ 
    public static float[,] GenerateNoiseMap(int p_mapWidth, int p_mapHeight, NoiseSettings p_settings, Vector2 p_sampleCentre)
    {
        // sets the noise Map to a float the size of the map width and height
        float[,] noiseMap = new float[p_mapWidth, p_mapHeight];

        // sets the random variable to the seed
        System.Random prng = new System.Random(p_settings.seed);

        // sets the octave offsets to the octaves of the noise settings
        Vector2[] octaveOffSets = new Vector2[p_settings.octaves];

        float MaxPosibleHeight = 0; // Defaul maximum height
        float amplitude = 1;        // Default amplitude
        float frequency = 1;        // Default frequency

        // loops through the octaves and sets the offsets
        for (int i = 0; i < p_settings.octaves; i++)
        {
            // offset x is set by random value between -100000 and 100000 + the offset x of the noise settings + the x of sample centre
            // offset y is the same except instead of + we set to -
            float offSetX = prng.Next(-100000, 100000) + p_settings.offset.x + p_sampleCentre.x;
            float offSetY = prng.Next(-100000, 100000) - p_settings.offset.y - p_sampleCentre.y;

            // applies the above offsets
            octaveOffSets[i] = new Vector2(offSetX, offSetY);

            // Sets the max possible height to the amplitude and multiplying the amplitude by the persistance
            MaxPosibleHeight += amplitude;
            amplitude *= p_settings.persistance;
        }
        
        // sets the half width and height of the map
        float halfWidth = p_mapWidth / 2f;
        float halfHeight = p_mapHeight / 2f;

        for (int y = 0; y < p_mapHeight; y++)
        {
            for (int x = 0; x < p_mapWidth; x++)
            {
                amplitude = 1;          // Resets the amplitude to 1
                frequency = 1;          // Resets the frequency to 1
                float noiseHeight = 0;
                for (int i = 0; i < p_settings.octaves; i++)
                {
                    // Sets the sample X and Y values for the perlin noise value
                    float sampleX = (x - halfWidth + octaveOffSets[i].x) / p_settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffSets[i].y) / p_settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;     // Applies the perlin noise to the noiseHeight

                    // Sets the amplitude and frequency to lacunarity and persistance value
                    amplitude *= p_settings.persistance;
                    frequency *= p_settings.lacunarity;
                }
                
                // Sets the noise map to the noise height
                noiseMap[x, y] = noiseHeight;

                // normalizes the noise map data
                float normalizedHeight = (noiseMap[x, y] + 1) / (MaxPosibleHeight / 0.9f);
                noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);

            }
        }
        
        return noiseMap;
    }
}

// Settings and data for the noise map
[System.Serializable]
public class NoiseSettings
{
    public float scale = 50;

    [Range(2,8)]
    public int octaves = 6;
    [Range(0, 1)]
    public float persistance = 0.6f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    // Checks and validates the values
    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}