using UnityEngine;

// This script should be attached to the main menu.
public class MainMenu : Menu, IButtonAction
{
    private readonly string PLAY_BUTTON = "PlayButton";
    private readonly string INSTRUCTIONS_BUTTON = "InstructionsButton";
    private readonly string SCOREBOARD_BUTTON = "ScoreboardButton";
    private readonly string EXIT_BUTTON = "ExitButton";

    // Use this for initialization
    void Start()
    {
        Debug.Log( "MainMenu.cs" );

        GameInput.AttachInput(
            actionClick: OnButtonClickAction,
            skillClick: OnButtonClickSkill,
            backClick: OnButtonClickBack,
            leftClick: OnButtonClickLeft, 
            rightClick: OnButtonClickRight,
            downClick: OnButtonClickDown,
            upClick: OnButtonClickUp );

        // Initialize menu options
        AddMenuOption( PLAY_BUTTON, OnPlayButtonSelect );
        AddMenuOption( INSTRUCTIONS_BUTTON, OnInstructionsButtonSelect );
        AddMenuOption( SCOREBOARD_BUTTON, OnScoreboardButtonSelect );
        AddMenuOption( EXIT_BUTTON, OnExitButtonSelect );

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
        SelectButton();
    }

	public void OnButtonClickSkill()
    {
        // no functionality
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
        ChangeState(GameStateLoader.GAME_STATES.INSTRUCTIONS);
    }

    private void OnScoreboardButtonSelect()
    {
        // Load the scoreboard
        Debug.Log( "Scoreboard button is selected" );
        ChangeState( GameStateLoader.GAME_STATES.SCOREBOARD );
    }

    private void OnExitButtonSelect()
    {
        // Exit the game
        Debug.Log( "Exit button is selected" );

        Application.Quit();
    }
}