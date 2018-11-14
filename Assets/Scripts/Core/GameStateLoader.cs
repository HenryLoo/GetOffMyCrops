using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// This class defines the game flow.
// It will be responsible for loading/unloading all scenes within the game.
public static class GameStateLoader
{
    public enum GAME_STATES
    {
        MAIN_MENU = 1,
        INSTRUCTIONS,
        GAMEPLAY,
        INIT_STATE,
		WIN_MENU,
		LOSE_MENU,
        SCOREBOARD
    }

    private static GAME_STATES _currentGameState = GAME_STATES.INIT_STATE;
    private static GameSceneSwitcher _sceneLoader = new GameSceneSwitcher();

    // Update is called once per frame
    public static void Update()
    {
        CheckGameState();
    }

    public static GAME_STATES CheckGameState( bool printState = false )
    {
        if( printState )
        {
            switch( _currentGameState )
            {
                case GAME_STATES.MAIN_MENU:
                    Debug.Log( "Game is currently on main menu." );
                    break;

                case GAME_STATES.INSTRUCTIONS:
                    Debug.Log("Game is currently on INSTRUCTIONS.");
                    break;

                case GAME_STATES.GAMEPLAY:
                    Debug.Log( "Game is currently on Gameplay." );
                    break;

				case GAME_STATES.WIN_MENU:
                    Debug.Log( "Game is currently on game win state." );
                    break;

				case GAME_STATES.LOSE_MENU:
                    Debug.Log( "Game is currently on game lose state." );
                    break;

                case GAME_STATES.SCOREBOARD:
                    Debug.Log( "Game is currently on scoreboard state." );
                    break;

                case GAME_STATES.INIT_STATE:
                    Debug.Log( "Game is currently on init state." );
                    break;
				
                default:
                    Debug.Log( "State is undefined." );
                    break;
            }

        }
        return _currentGameState;
    }

    public static void SwitchState( GAME_STATES newState )
    {
        if( _currentGameState == newState )
        {
            Debug.Log( "Cannot switch the state to " + newState + " because it is already in this state." );
            return;
        }

        Debug.Log( "GameStateLoader.cs Attempting to load new scene." );
        switch( newState )
        {
            case GAME_STATES.MAIN_MENU:
                _sceneLoader.LoadScene( GameSceneSwitcher.SCENE_POSITION.MAIN_MENU );
                break;

            case GAME_STATES.INSTRUCTIONS:
                _sceneLoader.LoadScene(GameSceneSwitcher.SCENE_POSITION.INSTRUCTIONS);
                break;

            case GAME_STATES.GAMEPLAY:
                _sceneLoader.LoadScene( GameSceneSwitcher.SCENE_POSITION.GAMEPLAY );
                break;
                
			case GAME_STATES.WIN_MENU:
                _sceneLoader.LoadScene( GameSceneSwitcher.SCENE_POSITION.WIN_MENU );
                break;
			
			case GAME_STATES.LOSE_MENU:
                _sceneLoader.LoadScene( GameSceneSwitcher.SCENE_POSITION.LOSE_MENU );
                break;

            case GAME_STATES.SCOREBOARD:
                _sceneLoader.LoadScene( GameSceneSwitcher.SCENE_POSITION.SCOREBOARD );
                break;

            default:
                Debug.Log( "State cannot be undefined." );
                break;
        }
    }
}
