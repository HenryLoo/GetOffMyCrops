using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script should be attached to the main menu.
public class MainMenu : MonoBehaviour, IButtonAction
{
    private static readonly int BUTTON_INDEX_UP = -1;
    private static readonly int BUTTON_INDEX_DOWN = 1;

    private GameObject _currentlySelectedButton;
    private int _currentButtonIndex;
    private enum BUTTONS
    {
        PLAY_BUTTON,
        INSTRUCTIONS_BUTTON,
        EXIT_BUTTON
    };
    private readonly string[] BUTTON_LIST = { "PlayButton", "InstructionsButton", "ExitButton" };

    // Use this for initialization
    void Start()
    {
        Debug.Log( "MainMenu.cs" );

        GameInput.AttachInput(
            actionClick: OnButtonClickAction,
            backClick: OnButtonClickBack,
            leftClick: OnButtonClickLeft, 
            rightClick: OnButtonClickRight,
            downClick: OnButtonClickDown,
            upClick: OnButtonClickUp );

        SelectDefaultButton();
    }

    // Update is called once per frame
    void Update()
    {
        GameInput.UpdateInput();
    }

    public void OnButtonClickLeft()
    {
        // no functionality
    }

    public void OnButtonClickRight()
    {
        // no functionality
    }

    public void OnButtonClickAction()
    {
        // Select the button
        if( _currentButtonIndex == ( int ) BUTTONS.PLAY_BUTTON )
        {
            OnPlayButtonSelect();
        }
        else if( _currentButtonIndex == ( int ) BUTTONS.EXIT_BUTTON )
        {
            OnExitButtonSelect();
        }
        else if( _currentButtonIndex == ( int ) BUTTONS.INSTRUCTIONS_BUTTON )
        {
            OnInstructionsButtonSelect();
        }
    }

    public void OnButtonClickBack()
    {
        // no functionality
    }

    public void OnButtonClickUp()
    {
        // Move selected button upwards
        UpdateButtonIndex( BUTTON_INDEX_UP );
    }

    public void OnButtonClickDown()
    {
        // Move selected button downwards
        UpdateButtonIndex( BUTTON_INDEX_DOWN );
    }

    private void UpdateButtonIndex( int indexChange )
    {
        _currentButtonIndex += indexChange;

        _currentButtonIndex = HelperFunctions.MathModulus( _currentButtonIndex, BUTTON_LIST.Length );
        Debug.Log( "MainMenu.cs: Button index = " + _currentButtonIndex );
        _currentlySelectedButton = GameObject.Find( BUTTON_LIST[ _currentButtonIndex ] );
        _currentlySelectedButton.GetComponent<Button>().Select();
    }

    private void SelectDefaultButton()
    {
        _currentButtonIndex = ( int ) BUTTONS.PLAY_BUTTON;
        _currentlySelectedButton = GameObject.Find( BUTTON_LIST[ _currentButtonIndex ] );
        _currentlySelectedButton.GetComponent<Button>().Select();
    }

    private void OnPlayButtonSelect()
    {
        // Load the game
        Debug.Log( "Play button is selected" );
        ChangeState( GameStateLoader.GAME_STATES.GAMEPLAY );
    }

    private void OnInstructionsButtonSelect()
    {
        // Load instructions
        Debug.Log( "Instructions button is selected" );

        // TODO: implement instruction screen and link it.
    }

    private void OnExitButtonSelect()
    {
        // Exit the game
        Debug.Log( "Exit button is selected" );

        Application.Quit();
    }

    private void ChangeState( GameStateLoader.GAME_STATES state )
    {
        GameStateLoader.SwitchState( state );
    }
}