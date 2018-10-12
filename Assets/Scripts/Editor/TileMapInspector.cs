using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// This just adds a "Reload Tile Map" button in the inspector, so that
// you don't have to hit "play" to see tile map changes
[CustomEditor( typeof( GameController ) )]
public class TileMapInspector : Editor
{
    string levelToLoad;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        levelToLoad = EditorGUILayout.TextField( "Level To Load", levelToLoad );
        if( GUILayout.Button( "Reload Tile Map" ) )
        {
            GameController game = ( GameController ) target;
            game.LoadLevel( levelToLoad );
        }
    }
}
