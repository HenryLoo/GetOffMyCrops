using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// This just adds a "Reload Tile Map" button in the inspector, so that
// you don't have to hit "play" to see tile map changes
[CustomEditor( typeof( TileMap ) )]
public class TileMapInspector : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if( GUILayout.Button( "Reload Tile Map" ) )
        {
            TileMap tileMap = ( TileMap ) target;
            tileMap.InitTileMap();
        }
    }
}
