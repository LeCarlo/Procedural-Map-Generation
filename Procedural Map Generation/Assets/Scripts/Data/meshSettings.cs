using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class meshSettings : updatableData
{
    // The const and static variables of supported chunk and terrain data
    
    public const int numSupportLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatShadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float meshScale = 2.5f;
    public bool useFlatShading;

    // Ranges for the chunk size indexs
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
            // number of vertices per line is equal to the supported chunk size indexes, depending if we're using flat shading or not, plus 5
            return supportedChunkSizes[(useFlatShading) ? flatShadedChunkSizeIndex : chunkSizeIndex] + 5;
        }
    }

    // Mesh world size is calculated by the num of vertices per line - 3 multiplied by the scale of the mesh
    public float meshWorldSize
    {
        get { return (numVertsPerLine - 3) * meshScale; }
    }

}
