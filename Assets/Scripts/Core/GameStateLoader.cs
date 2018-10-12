using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/***
 * This class defines the game flow. It will be responsible for loading unloading all the scene within the game
 * 
 * */

public static class GameStateLoader
{
	public enum GAME_STATES { MAIN_MENU = 1, GAMEPLAY, PAUSE_MENU, INIT_STATE }

	private static GAME_STATES currentGameState = GAME_STATES.INIT_STATE;
	private static GameSceneSwitcher sceneLoader = new GameSceneSwitcher();
	
	// Update is called once per frame
	public static void Update ()
	{
		checkGameState();
	}

	public static GAME_STATES checkGameState( bool printState = false )
	{
		if ( printState )
		{
			switch (currentGameState)
			{
				case GAME_STATES.MAIN_MENU:
						Debug.Log("Game is currently on main menu");
					break;

				case GAME_STATES.GAMEPLAY:
						Debug.Log("Game is currently on Gameplay");
					break;

				case GAME_STATES.PAUSE_MENU:
						Debug.Log("Game is currently on pause menu");
					break;

				default:
					Debug.Log("State cannot be undefined.");
					break;
			}
            
		}
		return currentGameState;
	}

	public static void switchState( GAME_STATES newState )
	{
		if (currentGameState == newState)
		{
			Debug.Log( "cannot switch the state to " + newState + " because it is already in this state." );
			return;
		}
		Debug.Log( "GameStateLoader.cs Attempting to load new scene" );
		switch (newState)
		{
			case GAME_STATES.MAIN_MENU:
				sceneLoader.loadScene( GameSceneSwitcher.SCENE_POSITION.MAIN_MENU );
			break;

			case GAME_STATES.GAMEPLAY:
				// TODO: load main gameplay levels here
			break;

			case GAME_STATES.PAUSE_MENU:
				// TODO: load in game pause menu here
			break;

			default:
				Debug.Log("State cannot be undefined.");
			break;
		}
	}
}
