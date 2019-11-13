using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updatableData : ScriptableObject
{
    // Adds in the options for the HeightmapSettings, meshSettings and TextureData script to be auto updated or not

    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    // THIS ONLY WORKS IN THE EDITOR

    #if UNITY_EDITOR

    protected virtual void OnValidate()
    {
        // If auto update is selected
        if(autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }
    }

    public void NotifyOfUpdatedValues()
    {
        // Resets the unity editor update variable
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        if (OnValuesUpdated != null)
        {
            // updates the values if the validation is not null
            OnValuesUpdated();
        }
    }
    #endif
}
