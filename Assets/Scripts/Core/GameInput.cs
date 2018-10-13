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

	private static float axisH_Value = 0;
	private static float axisV_Value = 0;
	
	private static bool axisH_reset = true;
	private static bool axisV_reset = true;
	
	// Update should be called once per frame if you are trying to capture input in your scene.
	public static void updateInput()
	{
		axisH_Value = Input.GetAxis( "Horizontal" );
		axisV_Value = Input.GetAxis( "Vertical" );

		axisH_reset = axisH_Value == 0 ? true : false;
		axisV_reset = axisV_Value == 0 ? true : false;

		bool actionButtonDown = Input.GetButtonDown( "Fire1" ) || Input.GetKeyDown( KeyCode.Space );
		bool rightButtonDown = ( axisH_Value > 0 && axisH_reset ) || Input.GetKeyDown( KeyCode.RightArrow );
		bool leftButtonDown = ( axisH_Value < 0 && axisH_reset ) || Input.GetKeyDown( KeyCode.LeftArrow );
		bool backButtonDown = Input.GetButtonDown( "Fire2" ) || Input.GetKeyDown( KeyCode.Escape );
		bool upButtonDown = ( axisV_Value > 0 && axisV_reset ) || Input.GetKeyDown( KeyCode.UpArrow );
		bool downButtonDown = ( axisV_Value < 0 && axisV_reset ) || Input.GetKeyDown( KeyCode.DownArrow );
		
		if ( actionButtonDown && onActionClick != null )
		{
			Debug.Log("Button clicked = Action");
			onActionClick();
		}
		if ( rightButtonDown && onRightClick != null )
		{
			Debug.Log("Button clicked = Right");

			onRightClick();
		}
		if ( leftButtonDown && onLeftClick != null )
		{
			Debug.Log("Button clicked = Left");
			onLeftClick();
		}
		if ( backButtonDown && onBackClick != null )
		{
			Debug.Log("Button clicked = Back");
			onBackClick();
		}
		if ( upButtonDown && onUpClick != null )
		{
			Debug.Log("Button clicked = Up");
			onUpClick();
		}
		if ( downButtonDown && onDownClick != null )
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