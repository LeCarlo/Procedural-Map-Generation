using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshdata, Texture2D texture)
    {
        meshFilter.sharedMesh = meshdata.createMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
