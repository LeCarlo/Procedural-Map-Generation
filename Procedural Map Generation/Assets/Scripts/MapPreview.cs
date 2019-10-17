using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public enum DrawMode { NoiseMap, FalloffMap, Mesh };
    public DrawMode drawMode;

    public meshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public textureData TextureData;

    public Material terrainMaterial;



    [Range(0, meshSettings.numSupportLODs - 1)]
    public int editorPreviewLOD;

    public bool autoUpdate;

  


    public void DrawMapInEditior()
    {
        TextureData.ApplyToMaterial(terrainMaterial);

        TextureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        HeightMap heightMap = heightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);
        

        if (drawMode == DrawMode.NoiseMap)
        {
            DrawTexture(textureGenerator.textureFromHeightMap(heightMap));
        }

        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(meshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            DrawTexture(textureGenerator.textureFromHeightMap(new HeightMap(falloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine),0,1)));
        }
    }

    public void DrawTexture(Texture2D texture)
    {
        
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height)/10f;

        textureRender.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshdata)
    {
        meshFilter.sharedMesh = meshdata.createMesh();

        textureRender.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }


    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditior();
        }
    }

    void OnTextureValuesUpdated()
    {
        TextureData.ApplyToMaterial(terrainMaterial);
    }


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
