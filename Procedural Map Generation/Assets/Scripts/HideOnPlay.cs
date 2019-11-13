using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    // Hides the preview meshes and planes when the game starts
    void Start()
    {
        gameObject.SetActive(false);
    }

    
    void Update()
    {
        
    }
}
