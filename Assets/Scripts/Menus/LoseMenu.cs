using UnityEngine;

public class LoseMenu : TransitionMenu
{
    private readonly string SUBMIT_BUTTON = "Button_Submit";
    private readonly string QUIT_BUTTON = "Button_Quit";

    private readonly string LOSE_MESSAGE = "You lose!";

    new void Awake()
    {
        // Initialize menu options
        AddMenuOption( SUBMIT_BUTTON, OnSubmitButtonSelect );
        AddMenuOption( QUIT_BUTTON, OnQuitButtonSelect );

        base.Awake();
    }

    // Use this for initialization
    new void Start()
    {
        Debug.Log( "LoseMenu.cs" );

        base.Start();
        gameMessage.text = LOSE_MESSAGE;

        // Reset the total money so that it doesn't persist across 
        // play sessions
        dataController.ScoreToSubmit = data.TotalMoney;
        dataController.ResetPlayData();

        // Play lose music
        SoundController.PlayMusic( MusicType.Lose );
    }
	
	// Update is called once per frame
	new void Update()
	{
        base.Update();
    }

    private void OnSubmitButtonSelect()
    {
        // Submit score
        Debug.Log( "Submit button is selected" );

        // Flag submit request to true to indicate that a score is being
        // submitted
        SaveDataController.GetInstance().IsSubmitting = true;
        ChangeState( GameStateLoader.GAME_STATES.SCOREBOARD );
    }

    private void OnQuitButtonSelect()
    {
        // Quit to main menu
        Debug.Log( "Quit button is selected" );
        ChangeState( GameStateLoader.GAME_STATES.MAIN_MENU );
    }
}
