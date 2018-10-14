using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This defines the root of the game.
// Ideally it should be attached to an empty scene which is always there in the background.
// This should not be controlled via user input.
public class MainScript : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        // This script will load everything for the game
        Debug.Log( "MainScript.cs" );

        // Disable mouse controls
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Input responder, should be loaded before scene loader
        GameInput.DetachInput();

        // State machine
        RequestStateChange( GameStateLoader.GAME_STATES.MAIN_MENU );
    }

    // Update is called once per frame
    void Update()
    {
        GameStateLoader.CheckGameState();
    }

    public void RequestStateChange( GameStateLoader.GAME_STATES newState )
    {
        GameStateLoader.SwitchState( newState );
    }
}
