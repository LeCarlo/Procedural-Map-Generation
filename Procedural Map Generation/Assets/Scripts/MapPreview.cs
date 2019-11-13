using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    // Texture data and mesh data variables
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public meshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public textureData TextureData;
    public Material terrainMaterial;


    // Different drawing for the preview of the mesh and noise map for a singular chunk, not the entire map
    public enum DrawMode { NoiseMap, Mesh };
    public DrawMode drawMode;

    // LOD to see on the preview mesh, only in the editor
    [Range(0, meshSettings.numSupportLODs - 1)]
    public int editorPreviewLOD;

    // Any value changes will auto update the preview mesh
    public bool autoUpdate;
    
    // Draws the preview mesh and noise map in the editor
    public void DrawMapInEditior()
    {

        // Applies the material and heights to the mesh
        TextureData.ApplyToMaterial(terrainMaterial);
        TextureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        // Generates a height map
        HeightMap heightMap = heightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);
        

        if (drawMode == DrawMode.NoiseMap)
        {
            DrawTexture(textureGenerator.textureFromHeightMap(heightMap));
        }

        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(meshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
        }
        
    }

    public void DrawTexture(Texture2D texture)
    {
        
        // Draws the texture for the noise map preview
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height)/10f;

        textureRender.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshdata)
    {
        // Draws the mesh for the mesh preview
        meshFilter.sharedMesh = meshdata.createMesh();

        textureRender.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }


    void OnValuesUpdated()
    {
        // If values are updated and the game is not running everthing drawn in the map editor will be updated
        if (!Application.isPlaying)
        {
            DrawMapInEditior();
        }
    }

    void OnTextureValuesUpdated()
    {
        // aplies the texture to the mesh material
        TextureData.ApplyToMaterial(terrainMaterial);
    }

    // Validates the values when updated

    void OnValidate()
    {
        
        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (TextureData != null)
        {
            TextureData.OnValuesUpdated -= OnTextureValuesUpdated;
            TextureData.OnValuesUpdated += OnTextureValuesUpdated;
        }

    }
}
