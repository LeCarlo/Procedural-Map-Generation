using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    // threshold for the player to move before the chunks are updated;
    const float viewerMoveThresholdCU = 25f;
    const float sqrVMTCU = viewerMoveThresholdCU * viewerMoveThresholdCU; // Squared threshold

    // variables for the LOD colliders
    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    // settings for the meshes
    public meshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public textureData textureSettings;

    // Viewer, ideally the player
    public Transform viewer;

    // The mesh material for the map
    public Material mapMaterial;

    // The positions of the player
    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    // Size of the world and chunks in the visible viewing distance
    float meshWorldSize;
    int chunksVisibleInViewDst;

    // Dictionary and list for the terrain chunks
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    void Start()
    {
        // Applies the material to the terrain chunks on startup
        textureSettings.ApplyToMaterial(mapMaterial);
        // Updates the mesh heights
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        // Sets the max viewing distance based the on detail levels visibble distance threshold
        float maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;

        // Sets the mesh world size
        meshWorldSize = meshSettings.meshWorldSize;

        // sets the amount of chunks in the visible viewing distance
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        // Updates the visible chunks so that the chunks will spawn at startup
        UpdateVisibleChunks();
    }
    void Update()
    {
        // Updates the viewer position
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        // updates the collision mesh
        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }
        // Updates the visible chunks
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrVMTCU)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }
    void UpdateVisibleChunks()
    {
        // Using a hashset over a list because we are using more data types
        // This hashset is for chunks that have already been updated and are visible to the player
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            // Adds the visible terrain chunk coord to the hashset and then updates the terrain chunk
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        // Sets the current chunk coords based on the viewer's position divided by the mesh world size
        // If it's set to be multiplied by the mesh world size, no new chunks will be generated
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffSet = -chunksVisibleInViewDst; yOffSet <= chunksVisibleInViewDst; yOffSet++)
        {
            for (int xOffSet = -chunksVisibleInViewDst; xOffSet <= chunksVisibleInViewDst; xOffSet++)
            {
                // Sets the view chunk coord to the current positions + the respected x and y offsets
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffSet, currentChunkCoordY + yOffSet);

                // Checks to see if the view chunk coord is not in the already updated chunk hashset
                // If it doesn't it then checks to see if the terrain chunk dictionary has it
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        // if so, it updates the terrain chunk
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();

                    }
                    else
                    {
                        // Otherwise it generates a new terrain chunk and then adds it to the dictionary and then loads it
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                        newChunk.onVisibilityChanged += onTerrainChunkVisibilityChanged;
                        newChunk.Load();
                    }
                }
            }
        }
    }
    
    // If the terrain is visible or not, it gets added or removed from the list of visible chunks
    void onTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if(isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }

}

// LOD settings for the terrain chunks
[System.Serializable]
public struct LODInfo
{
    [Range(0, meshSettings.numSupportLODs - 1)]
    public int lod;
    public float visibleDistanceThreshold;

    public float sqrVisibleDstThreshold
    {
        get
        {
            return visibleDistanceThreshold * visibleDistanceThreshold;
        }
    }
}
