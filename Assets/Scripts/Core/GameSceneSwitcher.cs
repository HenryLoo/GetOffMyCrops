using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneSwitcher
{
    public enum SCENE_POSITION
    {
        MAIN_MENU,
        GAMEPLAY,
        END_GAME_WIN,
        END_GAME_LOSE,
        PAUSE_MENU,
        DUMMY_SCENE
    };

    public static string[] SCENE_LIST = {
        "Assets/Scenes/MenuScene.unity",
        "Assets/Scenes/GameScene.unity",
        "Assets/Scenes/GameplayScreens/EndGame_Win.unity",
        "Assets/Scenes/GameplayScreens/EndGame_Lose.unity"
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
        // Our current design doesn't allow having two scenes loaded at the same time.
        if( _currentScene != SCENE_POSITION.DUMMY_SCENE )
        {
            UnloadScene();
        }

        _currentScene = scene;
        GameInput.DetachInput();

        if( _currentScene != SCENE_POSITION.DUMMY_SCENE )
        {
            Debug.Log( "GameSceneSwitcher.cs loading " + SCENE_LIST[ ( int ) _currentScene ] + " scene" );
        }

        SceneManager.LoadSceneAsync( SCENE_LIST[ ( int ) _currentScene ] );
    }

    public void UnloadScene()
    {
        SceneManager.UnloadSceneAsync( SCENE_LIST[ ( int ) _currentScene ] );
        _currentScene = SCENE_POSITION.DUMMY_SCENE;
    }
}