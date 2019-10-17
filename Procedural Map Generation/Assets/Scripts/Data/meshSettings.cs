using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class meshSettings : updatableData
{
    public const int numSupportLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatShadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float meshScale = 2.5f;
    public bool useFlatShading;

    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;
    [Range(0, numSupportedFlatShadedChunkSizes - 1)]
    public int flatShadedChunkSizeIndex;



    // number of vertices per line of mesh rendered at LOD = 0.
    // Includes the extra 2 verts that are excluded from the final mesh but are used for calculating normals
    public int numVertsPerLine
    {
        get
        {
            return supportedChunkSizes[(useFlatShading) ? flatShadedChunkSizeIndex : chunkSizeIndex] - 1;
        }
    }

    public float meshWorldSize
    {
        get { return (numVertsPerLine - 3) * meshScale; }
    }

}
