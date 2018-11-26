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

    private enum KeyType
    {
        Up,
        Down,
        Left,
        Right,
        Action,
        Skill,
        Back,
        NumTypes
    }

    // Flags for if keys are being pressed
    private static bool[] _isKeyPressed = new bool[ ( int ) KeyType.NumTypes ];

    // Flags for if keys were pressed in the previous frame
    private static bool[] _wasKeyPressed = new bool[ ( int ) KeyType.NumTypes ];

    // Names of each input according to the editor's input manager
	public static readonly string ACTION_BUTTON = "Action";
	public static readonly string SKILL_BUTTON = "Skill";
	public static readonly string BACK_BUTTON = "Cancel";
	public static readonly string JOY_X = "Horizontal";
	public static readonly string JOY_Y = "Vertical";

    // Update should be called once per frame if you are trying to capture input in your scene.
    public static void UpdateInput()
    {
        // Update previous key states
        for( int i = 0; i < ( int ) KeyType.NumTypes; ++i )
        {
            _wasKeyPressed[ i ] = _isKeyPressed[ i ];
        }

        // Update key states
        _isKeyPressed[ ( int ) KeyType.Left ] = Input.GetAxis( JOY_X ) < 0;
        _isKeyPressed[ ( int ) KeyType.Right ] = Input.GetAxis( JOY_X ) > 0;
        _isKeyPressed[ ( int ) KeyType.Up ] = Input.GetAxis( JOY_Y ) < 0;
        _isKeyPressed[ ( int ) KeyType.Down ] = Input.GetAxis( JOY_Y ) > 0;
        _isKeyPressed[ ( int ) KeyType.Action ] = Input.GetButtonDown( ACTION_BUTTON );
        _isKeyPressed[ ( int ) KeyType.Skill ] = Input.GetButtonDown( SKILL_BUTTON );
        _isKeyPressed[ ( int ) KeyType.Back ] = Input.GetButtonDown( BACK_BUTTON );

        if( IsKeyPressed( KeyType.Action ) && _onActionClick != null )
        {
            Debug.Log( "Button clicked = " + ACTION_BUTTON );
            _onActionClick();
        }

        if( IsKeyPressed( KeyType.Skill ) && _onSkillClick != null )
        {
            Debug.Log( "Button clicked = " + SKILL_BUTTON );
            _onSkillClick();
        }

        if( IsKeyPressed( KeyType.Back ) && _onBackClick != null )
        {
            Debug.Log( "Button clicked = " + BACK_BUTTON );
            _onBackClick();
        }

        if( IsKeyPressed( KeyType.Right ) && _onRightClick != null )
        {
            Debug.Log( "Button clicked = Right" );
            _onRightClick();
        }
        else if( IsKeyPressed( KeyType.Left ) && _onLeftClick != null )
        {
            Debug.Log( "Button clicked = Left" );
            _onLeftClick();
        }

        if( IsKeyPressed( KeyType.Up ) && _onUpClick != null )
        {
            Debug.Log( "Button clicked = Up" );
            _onUpClick();
        }
        else if( IsKeyPressed( KeyType.Down ) && _onDownClick != null )
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

    // Check if a key is being pressed once (not held down)
    private static bool IsKeyPressed( KeyType key )
    {
        int checkedKey = ( int ) key;
        return (_isKeyPressed[ checkedKey ] && !_wasKeyPressed[ checkedKey ] );
    }
}