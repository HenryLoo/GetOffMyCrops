﻿using UnityEngine;
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
        UpdateButtonIndex( NavDirection.Up );
    }

    public void MoveSelectedDown()
    {
        UpdateButtonIndex( NavDirection.Down );
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
        ChangeState( GameStateLoader.GAME_STATES.GAMEPLAY );
    }

    void QuitGame()
    {
        ChangeState( GameStateLoader.GAME_STATES.MAIN_MENU );
    }

}