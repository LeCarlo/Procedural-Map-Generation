using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textureGenerator : MonoBehaviour
{
    // Generates the texture for the mesh
    public static Texture2D textureFromColourMap(Color[] colourMap, int width, int height)
    {
        // Sets the texture for the mesh to the width and height
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // Sets the pixels to the colour map and then applies the texture
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    // Generates the texture from the height map
    public static Texture2D textureFromHeightMap(HeightMap heightMap)
    {
        int width = heightMap.values.GetLength(0);
        int height = heightMap.values.GetLength(1);
        
        // Sets the colour map base on the width and height values from the height map
        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]));
            }
        }
        return textureFromColourMap(colourMap, width, height);

    }

}
