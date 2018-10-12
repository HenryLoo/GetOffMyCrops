using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***
 * This defines the root of the game.
 * Ideally it should be attached to an empty scene which is always there in the background.
 * This should not be controlled via user input.
 * */

public class MainScript : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
		// this script will load everything for the game.
		Debug.Log( "MainScript.cs" );
		// input responder. Should be loaded before scene loader
		GameInput.detachInput();

		// state machine
		requestStateChange( GameStateLoader.GAME_STATES.MAIN_MENU );
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameStateLoader.checkGameState();
	}

	public void requestStateChange( GameStateLoader.GAME_STATES newState )
	{
		GameStateLoader.switchState( newState );
	}
}
