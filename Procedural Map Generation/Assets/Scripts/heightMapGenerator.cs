using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class heightMapGenerator 
{
    public static HeightMap GenerateHeightMap(int p_width, int p_height, HeightMapSettings p_settings, Vector2 p_sampleCentre)
    {
        // initializes the values based on the generated noise map
        float[,] values = Noise.GenerateNoiseMap(p_width, p_height, p_settings.noiseSettings, p_sampleCentre);

        // gets the data from the height map settings script's height curve
        AnimationCurve heightCurve_threadSafe = new AnimationCurve(p_settings.heightCurve.keys);

        // Default min and max value
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        // Sets the min and max values based on the height and width of the values variable
        for (int i = 0; i < p_width; i++)
        {
            for (int j = 0; j < p_height; j++)
            {
                // values is set to be multiplied by the heightcuve data which is also multiplied by the height multiplier
                values[i, j] *= heightCurve_threadSafe.Evaluate(values[i, j]) * p_settings.heightMultiplier;
                if (values[i,j] > maxValue)
                {
                    maxValue = values[i, j];
                }
                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }

        // Returns the data
        return new HeightMap(values, minValue, maxValue);
    }
}


public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] p_Values, float p_MinValue, float p_MaxValue)
    {
        this.values = p_Values;
        this.minValue = p_MinValue;
        this.maxValue = p_MaxValue;
    }
}
