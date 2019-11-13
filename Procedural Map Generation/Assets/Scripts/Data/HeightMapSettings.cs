using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HeightMapSettings : updatableData
{
    // Passes in the settings for the noise generation
    public NoiseSettings noiseSettings;
    
    public float heightMultiplier;
    public AnimationCurve heightCurve;

    // Sets the min and max height values based on the height curve multiplied by the height muliplier
    public float minHeight
    {
        get { return heightMultiplier * heightCurve.Evaluate(0); }
    }
    public float maxHeight
    {
        get { return heightMultiplier * heightCurve.Evaluate(1); }
    }

    // Checks the validation of the values
#if UNITY_EDITOR

    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();
        base.OnValidate();
    }
    #endif
}
