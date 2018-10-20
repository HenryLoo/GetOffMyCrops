using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Menu
{
    // Reference to the GameController
    public GameController GameController;

    public Image PausePanel;

    private readonly string RESUME_BUTTON = "ResumeButton";
    private readonly string RESTART_BUTTON = "RestartButton";
    private readonly string QUIT_BUTTON = "QuitButton";

    // Use this for initialization
    void Start()
    {
        // Initialize menu options
        AddMenuOption( RESUME_BUTTON, ResumeGame );
        AddMenuOption( RESTART_BUTTON, RestartGame );
        AddMenuOption( QUIT_BUTTON, QuitGame );

        TogglePauseMenu( false );
    }

    public void TogglePauseMenu( bool isPaused )
    {
        PausePanel.gameObject.SetActive( isPaused );
        SelectDefaultButton();
    }

    public void MoveSelectedUp()
    {
        UpdateButtonIndex( BUTTON_INDEX_UP );
    }

    public void MoveSelectedDown()
    {
        UpdateButtonIndex( BUTTON_INDEX_DOWN );
    }

    public void SelectOption()
    {
        SelectButton();
    }

    void ResumeGame()
    {
        TogglePauseMenu( false );
        GameController.SetIsPaused( false );
    }

    void RestartGame()
    {
        GameStateLoader.SwitchState( GameStateLoader.GAME_STATES.GAMEPLAY );
    }

    void QuitGame()
    {
        GameStateLoader.SwitchState( GameStateLoader.GAME_STATES.MAIN_MENU );
    }

}