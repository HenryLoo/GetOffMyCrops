using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This file is responsible for defining and handling the control scheme for the game.
// Every scene that needs user input should extend IButtonAction and register with this class.
// Registered classes should call UpdateInput function within their update function.
public static class GameInput
{
    // These delegates must be set by objects/scenes expecting user input
    public delegate void ActionButtonClickHandler();
    public delegate void SkillButtonClickHandler();
    public delegate void BackButtonClickHandler();
    public delegate void RightButtonClickHandler();
    public delegate void LeftButtonClickHandler();
    public delegate void UpButtonClickHandler();
    public delegate void DownButtonClickHandler();

    private static ActionButtonClickHandler _onActionClick = null;
    private static SkillButtonClickHandler _onSkillClick = null;
    private static BackButtonClickHandler _onBackClick = null;
    private static RightButtonClickHandler _onRightClick = null;
    private static LeftButtonClickHandler _onLeftClick = null;
    private static UpButtonClickHandler _onUpClick = null;
    private static DownButtonClickHandler _onDownClick = null;

	private static bool isActionButtonDown = false;
    private static bool isSkillButtonDown = false;
    private static bool isBackButtonDown = false;
    private static bool isRightButtonDown = false;
    private static bool isLeftButtonDown = false;
    private static bool isUpButtonDown = false;
    private static bool isDownButtonDown = false;

	private enum JOYSTICK_STATE
    {
        NEUTRAL = 1,
        UP,
		DOWN,
		LEFT,
		RIGHT
    }

	private static JOYSTICK_STATE joystickState = JOYSTICK_STATE.NEUTRAL;

	public static readonly string ACTION_BUTTON = "Action";
	public static readonly string SKILL_BUTTON = "Skill";
	public static readonly string BACK_BUTTON = "Cancel";
	public static readonly string JOY_X = "Horizontal";
	public static readonly string JOY_Y = "Vertical";

    // Update should be called once per frame if you are trying to capture input in your scene.
    public static void UpdateInput()
    {
		computeJoyStickState();

		isActionButtonDown = Input.GetButtonDown( ACTION_BUTTON );
        isSkillButtonDown = Input.GetButtonDown( SKILL_BUTTON );
        isBackButtonDown = Input.GetButtonDown( BACK_BUTTON );

        if( isActionButtonDown && _onActionClick != null )
        {
            Debug.Log( "Button clicked = " + ACTION_BUTTON );
            _onActionClick();
        }
        else if( isSkillButtonDown && _onSkillClick != null )
        {
            Debug.Log( "Button clicked = " + SKILL_BUTTON );
            _onSkillClick();
        }
        else if( isBackButtonDown && _onBackClick != null )
        {
            Debug.Log( "Button clicked = " + BACK_BUTTON );
            _onBackClick();
        }
        else if( isRightButtonDown && _onRightClick != null )
        {
            Debug.Log( "Button clicked = Right" );
            _onRightClick();
        }
        else if( isLeftButtonDown && _onLeftClick != null )
        {
            Debug.Log( "Button clicked = Left" );
            _onLeftClick();
        }
        else if( isUpButtonDown && _onUpClick != null )
        {
            Debug.Log( "Button clicked = Up" );
            _onUpClick();
        }
        else if( isDownButtonDown && _onDownClick != null )
        {
            Debug.Log( "Button clicked = Down" );
            _onDownClick();
        }
    }

    // Binds the input to a scene
    public static void AttachInput( 
        ActionButtonClickHandler actionClick = null, 
        SkillButtonClickHandler skillClick = null, 
        BackButtonClickHandler backClick = null, 
        RightButtonClickHandler rightClick = null, 
        LeftButtonClickHandler leftClick = null,
        UpButtonClickHandler upClick = null, 
        DownButtonClickHandler downClick = null )
    {
        _onActionClick = actionClick;
		_onSkillClick = skillClick;
        _onBackClick = backClick;
        _onRightClick = rightClick;
        _onLeftClick = leftClick;
        _onUpClick = upClick;
        _onDownClick = downClick;
    }

    // Removes the input binding
    public static void DetachInput()
    {
        _onActionClick = null;
		_onSkillClick = null;
        _onBackClick = null;
        _onRightClick = null;
        _onLeftClick = null;
        _onUpClick = null;
        _onDownClick = null;
    }
	
	private static void computeJoyStickState()
	{
		if ( Input.GetAxis( JOY_X ) < 0 && joystickState != JOYSTICK_STATE.LEFT )
		{
			joystickState = JOYSTICK_STATE.LEFT;
			isLeftButtonDown = true;
		}
		else if ( Input.GetAxis( JOY_X ) > 0 && joystickState != JOYSTICK_STATE.RIGHT )
		{
			joystickState = JOYSTICK_STATE.RIGHT;
			isRightButtonDown = true;
		}
		else if ( Input.GetAxis( JOY_Y ) < 0 && joystickState != JOYSTICK_STATE.UP )
		{
			joystickState = JOYSTICK_STATE.UP;
			isUpButtonDown = true;
		}
		else if ( Input.GetAxis( JOY_Y ) > 0 && joystickState != JOYSTICK_STATE.DOWN )
		{
			joystickState = JOYSTICK_STATE.DOWN;
			isDownButtonDown = true;
		}
		else if ( Input.GetAxis( JOY_Y ) == 0 && Input.GetAxis( JOY_X ) == 0 )
		{
			joystickState = JOYSTICK_STATE.NEUTRAL;
			isRightButtonDown = false;
			isLeftButtonDown = false;
			isUpButtonDown = false;
			isDownButtonDown = false;
		}
		else
		{
			//reset buttons
			isRightButtonDown = false;
			isLeftButtonDown = false;
			isUpButtonDown = false;
			isDownButtonDown = false;
		}
	}
}