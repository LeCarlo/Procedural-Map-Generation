using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class textureData : updatableData
{
    // MAXIMUM TEXTURE SIZE AND FORMATE
    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;

    // The colour and texture "regions"/"layers" for the mesh
    public Layer[] layers;

    // Saves the min and max height of the HeightMaps
    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material p_meshMaterial)
    {
        // sets all the settings to the mesh material uses the lambda expression to delegate the variables from the layer struct
        // to the x variable of the material and then converts them to an array for the floats.
        p_meshMaterial.SetInt("layerCount", layers.Length);
        p_meshMaterial.SetColorArray("baseColours", layers.Select(x => x.tint).ToArray());
        p_meshMaterial.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
        p_meshMaterial.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
        p_meshMaterial.SetFloatArray("baseColourStrength", layers.Select(x => x.tintStrength).ToArray());
        p_meshMaterial.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());

        // calls the generate texture array function and applies the generated texture to the material;
        Texture2DArray textureArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
        p_meshMaterial.SetTexture("baseTextures", textureArray);

        // updates the mesh heights
        UpdateMeshHeights(p_meshMaterial, savedMinHeight, savedMaxHeight);
    }

    
    // Updates and sets the min and max heights of the meshes
    public void UpdateMeshHeights(Material p_meshMaterial, float p_minHeight,float p_maxHeight)
    {
        savedMinHeight = p_minHeight;
        savedMaxHeight = p_maxHeight;

        p_meshMaterial.SetFloat("minHeight", p_minHeight);
        p_meshMaterial.SetFloat("maxHeight", p_maxHeight);
    }

    // Generates the texture array and applies it to the matrial
    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        // creates a new texture2D array using the const texture size of 512 for the height and width, the length of the textures for the depth
        // and using the const texture format of RGB565 for a 16 bit format. MipChain is set to true for the a smooth texture mip mapping effect

        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            // Sets the pixels for the texture array
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }

        // applies and returns the texture arrray
        textureArray.Apply();
        return textureArray;
    }


    // Public class for the textures to be added to the different layers of the mesh
    [System.Serializable]
    public class Layer
    {
        // User puts in the texture file here
        public Texture2D texture;

        // Tint is a solid colour to be added over the texture
        public Color tint;

        // The strength of the tint ranging from 0 to 1 and inbetween, if 0 only the texture will be seen, if 1, vice versa
        [Range(0,1)]
        public float tintStrength;

        // The starting height on the mesh for the layer of the texture and colour to be drawn at
        [Range(0, 1)]
        public float startHeight;

        // Blending of the colours and textures between the layers
        [Range(0, 1)]
        public float blendStrength;

        // scale of the textures;
        public float textureScale;
    }
}
