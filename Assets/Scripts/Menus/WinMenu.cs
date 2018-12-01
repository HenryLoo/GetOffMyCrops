using UnityEngine;
using UnityEngine.UI;

public class WinMenu : TransitionMenu
{
    private readonly string NEXT_BUTTON = "Button_Next_Level";
    private readonly string QUIT_BUTTON = "Button_Quit";
    
    private readonly string WIN_MESSAGE = "You win!";
    private readonly string ALL_CLEARED_MESSAGE = "All levels cleared!";

    private bool _hasClearedAllLevels = false;
    public const int NUM_LEVELS = 4;

    new void Awake()
    {
        // Initialize menu options
        AddMenuOption( NEXT_BUTTON, OnNextButtonSelect );
        AddMenuOption( QUIT_BUTTON, OnQuitButtonSelect );

        base.Awake();
    }

    // Use this for initialization
    new void Start()
    {
        Debug.Log( "WinMenu.cs" );

        base.Start();

        _hasClearedAllLevels = ( data.CurrentLevel == NUM_LEVELS );
        gameMessage.text = _hasClearedAllLevels ? ALL_CLEARED_MESSAGE : WIN_MESSAGE;

        if( _hasClearedAllLevels )
        {
            // Reset the total money so that it doesn't persist across 
            // play sessions
            dataController.ScoreToSubmit = data.TotalMoney;
            dataController.ResetPlayData();
        }
        else
        {
            // Save next level's number so the player can resume here
            ++data.CurrentLevel;
            dataController.SaveData( data );
            dataController.SaveDataToDisk();
        }

        // Play win music
        SoundController.PlayMusic( MusicType.Win );
    }
	
	// Update is called once per frame
	new void Update()
    {
        base.Update();
    }

    private void OnNextButtonSelect()
    {
        // Move to next level
        Debug.Log( "Next button is selected" );

        // All levels have been cleared
        if( _hasClearedAllLevels )
        {
            // Flag submit request to true to indicate that a score is being
            // submitted
            dataController.IsSubmitting = true;
            ChangeState( GameStateLoader.GAME_STATES.SCOREBOARD );
        }
        // Go to the next level
        else
        {
            ChangeState( GameStateLoader.GAME_STATES.GAMEPLAY );
        }
    }

    private void OnQuitButtonSelect()
    {
        // Quit to main menu
        Debug.Log( "Quit button is selected" );
        ChangeState( GameStateLoader.GAME_STATES.MAIN_MENU );
    }
}
