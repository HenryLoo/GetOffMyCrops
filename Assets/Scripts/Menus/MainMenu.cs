using UnityEngine;
using UnityEngine.UI;

// This script should be attached to the main menu.
public class MainMenu : Menu, IButtonAction
{
    private readonly string PLAY_BUTTON = "PlayButton";
    private readonly string INSTRUCTIONS_BUTTON = "InstructionsButton";
    private readonly string SCOREBOARD_BUTTON = "ScoreboardButton";
    private readonly string EXIT_BUTTON = "ExitButton";

    public Text PlayText;
    private readonly string CONTINUE_TEXT = "Continue";

    // Reference to the help text objects
    public GameObject HelpTextDesktop;
    public GameObject HelpTextPS4;

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

        // Play title music
        SoundController.PlayMusic( MusicType.Title );

        // Show the appropriate help text for the platform
        if( HelperFunctions.IsRunningOnDesktop() ) HelpTextDesktop.SetActive( true );
        else if( HelperFunctions.IsRunningOnPS4() ) HelpTextPS4.SetActive( true );

        // Set play button to read "Continue" if the player is not on the first level
        GameData data = SaveDataController.GetInstance().LoadData();
        if( data.CurrentLevel > 1 )
        {
            PlayText.text = CONTINUE_TEXT + " (Lv. " + data.CurrentLevel + ")";
        }
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
        UpdateButtonIndex( NavDirection.Up );
    }

    public void OnButtonClickDown()
    {
        // Move selected button downwards
        UpdateButtonIndex( NavDirection.Down );
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
        ChangeState( GameStateLoader.GAME_STATES.INSTRUCTIONS );
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