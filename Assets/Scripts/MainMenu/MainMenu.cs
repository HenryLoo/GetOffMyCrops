using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/***
 * This script should be attached to the main menu.
 * */
public class MainMenu : MonoBehaviour, iButtonAction
{
	private GameObject currentlySelectedButton;
	private int currentButtonIndex;
	private enum BUTTONS { PLAY_BUTTON = 0, INSTRUCTIONS_BUTTON, EXIT_BUTTON };
	private readonly string[] BUTTON_LIST = { "PlayButton", "InstructionsButton", "ExitButton" };

	// Use this for initialization
	void Start()
	{
		Debug.Log( "MainMenu.cs" );
			
		GameInput.attachInput( actionClick: onButtonClickAction, backClick: onButtonClickBack,
										leftClick: onButtonClickLeft, rightClick: onButtonClickRight,
										downClick: onButtonClickDown, upClick: onButtonClickUp );
			
		selectDefaultButtonInList();
	}
	
	// Update is called once per frame
	void Update()
	{
		GameInput.updateInput();
	}
	
	public void onButtonClickLeft()
	{
		// no functionality
	}
	public void onButtonClickRight()
	{
		// no functionality
	}
	public void onButtonClickAction()
	{
		// select
		if ( currentButtonIndex == (int) BUTTONS.PLAY_BUTTON )
		{
			onPlayButtonSelect();
		}
		else if ( currentButtonIndex == (int) BUTTONS.EXIT_BUTTON )
		{
			onExitButtonSelect();
		}
		else if ( currentButtonIndex == (int) BUTTONS.INSTRUCTIONS_BUTTON )
		{
			// TODO: implement instruction screen and link it.
		}
	}
	public void onButtonClickBack()
	{
		// no functionality
	}
	public void onButtonClickUp()
	{
		// move upwards
		updateButtonIndex( true );
	}
	public void onButtonClickDown()
	{
		// move downwards
		updateButtonIndex( false );
	}

	private void updateButtonIndex( bool isUpwards )
	{
		if ( isUpwards )
		{
			currentButtonIndex--;
		}
		else
		{
			currentButtonIndex++;
		}
		currentButtonIndex = HelperFunctions.mathModulus( currentButtonIndex, BUTTON_LIST.Length );
		Debug.Log( "MainMenu.cs:Button index = " + currentButtonIndex );
		currentlySelectedButton = GameObject.Find( BUTTON_LIST[currentButtonIndex] );
		currentlySelectedButton.GetComponent<Button>().Select();
	}

	private void selectDefaultButtonInList()
	{
		currentButtonIndex = ( int ) BUTTONS.PLAY_BUTTON;
		currentlySelectedButton = GameObject.Find( BUTTON_LIST[currentButtonIndex] );
		currentlySelectedButton.GetComponent<Button>().Select();
	}

	private void onPlayButtonSelect()
	{
		// load the game
		Debug.Log( "play button is selected" );
		changeState( GameStateLoader.GAME_STATES.GAMEPLAY );
	}

	private void onInstructionsButtonSelect()
	{
		// load instructions
		Debug.Log( "instructions button is selected" );
	}

	private void onExitButtonSelect()
	{
		// exit the game
		Debug.Log( "exit button is selected" );

		Application.Quit();
	}

	private void changeState( GameStateLoader.GAME_STATES state )
	{
		GameStateLoader.switchState( state );
	}
}