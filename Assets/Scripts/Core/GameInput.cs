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
    public delegate void BackButtonClickHandler();
    public delegate void RightButtonClickHandler();
    public delegate void LeftButtonClickHandler();
    public delegate void UpButtonClickHandler();
    public delegate void DownButtonClickHandler();

    private static ActionButtonClickHandler _onActionClick = null;
    private static BackButtonClickHandler _onBackClick = null;
    private static RightButtonClickHandler _onRightClick = null;
    private static LeftButtonClickHandler _onLeftClick = null;
    private static UpButtonClickHandler _onUpClick = null;
    private static DownButtonClickHandler _onDownClick = null;

    private static float _valueAxisH = 0;
    private static float _valueAxisV = 0;

    private static bool _isResetAxisH = true;
    private static bool _isResetAxisV = true;

    // Update should be called once per frame if you are trying to capture input in your scene.
    public static void UpdateInput()
    {
        bool isActionButtonDown = Input.GetButtonDown( "Action" );
        bool isBackButtonDown = Input.GetButtonDown( "Cancel" );
        bool isRightButtonDown = Input.GetAxis( "Horizontal" ) > 0 && Input.GetButtonDown( "Horizontal" );
        bool isLeftButtonDown = Input.GetAxis( "Horizontal" ) < 0 && Input.GetButtonDown( "Horizontal" );
        bool isUpButtonDown = Input.GetAxis( "Vertical" ) > 0 && Input.GetButtonDown( "Vertical" );
        bool isDownButtonDown = Input.GetAxis( "Vertical" ) < 0 && Input.GetButtonDown( "Vertical" );

        if( isActionButtonDown && _onActionClick != null )
        {
            Debug.Log( "Button clicked = Action" );
            _onActionClick();
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
        else if( isBackButtonDown && _onBackClick != null )
        {
            Debug.Log( "Button clicked = Back" );
            _onBackClick();
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
        BackButtonClickHandler backClick = null, 
        RightButtonClickHandler rightClick = null, 
        LeftButtonClickHandler leftClick = null,
        UpButtonClickHandler upClick = null, 
        DownButtonClickHandler downClick = null )
    {
        _onActionClick = actionClick;
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
        _onBackClick = null;
        _onRightClick = null;
        _onLeftClick = null;
        _onUpClick = null;
        _onDownClick = null;
    }
}