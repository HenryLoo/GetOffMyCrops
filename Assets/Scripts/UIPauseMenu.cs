using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    public Button PauseButton;
    public bool GamePaused { get; set; }
    public Image PausePanel;

    public Button ResumeButton;

    public Button RestartButton;
    public bool RestartStatus { get; set; }

    public Button QuitButton;
    public bool QuitStatus { get; set; }

    // Use this for initialization
    void Start()
    {
        Button pause = PauseButton.GetComponent<Button>();
        pause.onClick.AddListener( OpenPauseMenu );

        Button resume = ResumeButton.GetComponent<Button>();
        resume.onClick.AddListener( ResumeGame );

        Button restart = RestartButton.GetComponent<Button>();
        restart.onClick.AddListener( RestartGame );

        Button quit = QuitButton.GetComponent<Button>();
        quit.onClick.AddListener( QuitGame );
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Escape ) )
        {
            OpenPauseMenu();
        }
        if( Input.GetKeyDown( KeyCode.Alpha9 ) )
        {
            RestartGame();
        }
        if( Input.GetKeyDown( KeyCode.Alpha0 ) )
        {
            QuitGame();
        }
    }

    void OpenPauseMenu()
    {
        GamePaused = !GamePaused;
        PausePanel.gameObject.SetActive( GamePaused );
    }

    void ResumeGame()
    {
        GamePaused = false;
        PausePanel.gameObject.SetActive( false );
    }

    void RestartGame()
    {
        RestartStatus = true;
    }

    void QuitGame()
    {
        QuitStatus = true;
    }

}