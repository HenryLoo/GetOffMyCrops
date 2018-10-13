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
        GAMEPLAY,
        PAUSE_MENU,
        INIT_STATE
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

                case GAME_STATES.GAMEPLAY:
                    Debug.Log( "Game is currently on Gameplay." );
                    break;

                case GAME_STATES.PAUSE_MENU:
                    Debug.Log( "Game is currently on pause menu." );
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

            case GAME_STATES.GAMEPLAY:
                // TODO: load main gameplay levels here
                _sceneLoader.LoadScene( GameSceneSwitcher.SCENE_POSITION.GAMEPLAY );
                break;

            case GAME_STATES.PAUSE_MENU:
                // TODO: load in game pause menu here
                break;

            default:
                Debug.Log( "State cannot be undefined." );
                break;
        }
    }
}
