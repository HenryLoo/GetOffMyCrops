using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameShader : MonoBehaviour
{
    // The material with the day shader
    public Material DayCycleMaterial;

    // Use this for initialization
    void Start()
    {
        // Reset the level progress
        DayCycleMaterial.SetFloat( "_LevelProgress", 0 );
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnRenderImage( RenderTexture src, RenderTexture dest )
    {
        // Copy source texture to destination render texture with shader
        // Used for post-processing effects
        Graphics.Blit( src, dest, DayCycleMaterial );
    }
}
