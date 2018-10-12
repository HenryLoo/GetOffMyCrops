using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***
 * This file is responsible for defining and handling the control scheme for the game.
 * Every scene that needs user input should extend iButtonAction and register with this class.
 * Registered classes should call updateInput function within their update function.
 * 
 * */
public static class GameInput
{
	// these delegates must be set by objects/scenes expecting user input
	public delegate void ActionButtonClick_Handler();
	public delegate void BackButtonClick_Handler();
	public delegate void RightButtonClick_Handler();
	public delegate void LeftButtonClick_Handler();
	public delegate void UpButtonClick_Handler();
	public delegate void DownButtonClick_Handler();

	private static ActionButtonClick_Handler onActionClick = null;
	private static BackButtonClick_Handler onBackClick = null;
	private static RightButtonClick_Handler onRightClick = null;
	private static LeftButtonClick_Handler onLeftClick = null;
	private static UpButtonClick_Handler onUpClick = null;
	private static DownButtonClick_Handler onDownClick = null;
	
	// Update should be called once per frame if you are trying to capture input in your scene.
	public static void updateInput()
	{
		if ( ( Input.GetButton( "Fire1" ) || Input.GetKeyDown( KeyCode.Space ) ) && onActionClick != null )
		{
			Debug.Log("Button clicked = Action");
			onActionClick();
		}
		if ( ( Input.GetAxis( "Horizontal" ) > 0 || Input.GetKeyDown( KeyCode.RightArrow ) ) && onRightClick != null )
		{
			Debug.Log("Button clicked = Right");
			onRightClick();
		}
		if ( ( Input.GetAxis( "Horizontal" ) < 0 || Input.GetKeyDown( KeyCode.LeftArrow ) ) && onLeftClick != null )
		{
			Debug.Log("Button clicked = Left");
			onLeftClick();
		}
		if ( ( Input.GetButton( "Fire1" ) || Input.GetKeyDown( KeyCode.Escape ) ) && onBackClick != null )
		{
			Debug.Log("Button clicked = Back");
			onBackClick();
		}
		if ( ( Input.GetAxis( "Vertical" ) > 0 || Input.GetKeyDown( KeyCode.UpArrow ) ) && onUpClick != null )
		{
			Debug.Log("Button clicked = Up");
			onUpClick();
		}
		if ( ( Input.GetAxis( "Vertical" ) < 0 || Input.GetKeyDown( KeyCode.DownArrow ) ) && onDownClick != null )
		{
			Debug.Log("Button clicked = Down");
			onDownClick();
		}
	}

	// binds the input to a scene
	public static void attachInput( ActionButtonClick_Handler actionClick = null, BackButtonClick_Handler backClick = null,
									RightButtonClick_Handler rightClick = null, LeftButtonClick_Handler leftClick = null,
									UpButtonClick_Handler upClick = null, DownButtonClick_Handler downClick = null )
	{
		onActionClick = actionClick;
		onBackClick = backClick;
		onRightClick = rightClick;
		onLeftClick = leftClick;
		onUpClick = upClick;
		onDownClick = downClick;
	}

	// removes the input binding
	public static void detachInput()
	{
		onActionClick = null;
		onBackClick = null;
		onRightClick = null;
		onLeftClick = null;
		onUpClick = null;
		onDownClick = null;
	}
}