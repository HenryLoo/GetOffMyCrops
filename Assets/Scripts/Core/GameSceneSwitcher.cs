using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneSwitcher
{
	public enum SCENE_POSITION { MAIN_MENU = 0, GAMEPLAY, END_GAME, PAUSE_MENU, DUMMY_SCENE };
	public static string[] SCENE_LIST = {	"Assets/Scenes/Other/MainMenu/Menu.unity",
											"Assets/Scenes/Levels/GameScene.unity"
										};

	private SCENE_POSITION currentScene;
		
	public GameSceneSwitcher()
	{
		Debug.Log( "GameSceneSwitcher.cs" );
		currentScene = SCENE_POSITION.DUMMY_SCENE;
	}
	
	// loads a new scene
	public void loadScene( SCENE_POSITION scene )
	{
		// our current design doesn't allow having two scenes loaded at the same time.
		if ( currentScene != SCENE_POSITION.DUMMY_SCENE )
		{
			unloadScene();
		}

		currentScene = scene;
		GameInput.detachInput();

		if ( currentScene != SCENE_POSITION.DUMMY_SCENE )
		{
			Debug.Log( "GameSceneSwitcher.cs loading " + SCENE_LIST[(int) currentScene] + " scene" );
		}

		SceneManager.LoadSceneAsync( SCENE_LIST[(int) currentScene] );
	}

	public void unloadScene()
	{
		SceneManager.UnloadSceneAsync( SCENE_LIST[(int) currentScene] );
		currentScene = SCENE_POSITION.DUMMY_SCENE;
	}
}