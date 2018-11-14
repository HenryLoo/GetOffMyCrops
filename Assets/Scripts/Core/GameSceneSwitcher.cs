using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneSwitcher
{
    public enum SCENE_POSITION
    {
        MAIN_MENU,
        INSTRUCTIONS,
        GAMEPLAY,
        WIN_MENU,
        LOSE_MENU,
        SCOREBOARD,
        DUMMY_SCENE
    };

    public static string[] SCENE_LIST = {
        "Assets/Scenes/MenuScene.unity",
        "Assets/Scenes/InstructionsScene.unity",
        "Assets/Scenes/GameScene.unity",
        "Assets/Scenes/WinScene.unity",
        "Assets/Scenes/LoseScene.unity",
        "Assets/Scenes/ScoreboardScene.unity"
    };

    private SCENE_POSITION _currentScene;

    public GameSceneSwitcher()
    {
        Debug.Log( "GameSceneSwitcher.cs" );
        _currentScene = SCENE_POSITION.DUMMY_SCENE;
    }

    // Loads a new scene
    public void LoadScene( SCENE_POSITION scene )
    {
        _currentScene = scene;
        GameInput.DetachInput();

        // In single mode, the current scene is automatically unloaded when a 
        // new one is loaded
        SceneManager.LoadSceneAsync( SCENE_LIST[ ( int ) _currentScene ] );

        if( _currentScene != SCENE_POSITION.DUMMY_SCENE )
        {
            Debug.Log( "GameSceneSwitcher.LoadScene(): " + 
                SCENE_LIST[ ( int ) _currentScene ] );
        }
    }
}