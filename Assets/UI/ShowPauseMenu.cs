using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPauseMenu : MonoBehaviour {

    public Button pauseButton;
    public bool GamePaused { get; set; }

    public Image pausePanel;

    public Button resumeButton;
    public Button restartButton;
    public bool RestartStatus { get; set; }
    public Button quitButton;
    public bool QuitStatus { get; set; }



    // Use this for initialization
    void Start () {
        
        Button pause = pauseButton.GetComponent<Button>();
        pause.onClick.AddListener(OpenPauseMenu);

        Button resume = resumeButton.GetComponent<Button>();
        resume.onClick.AddListener(ResumeGame);

        Button restart = restartButton.GetComponent<Button>();
        restart.onClick.AddListener(RestartGame);

        Button quit = quitButton.GetComponent<Button>();
        quit.onClick.AddListener(QuitGame);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPauseMenu();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            RestartGame();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            QuitGame();
        }

    }

    void OpenPauseMenu()
    {
        GamePaused = !GamePaused;
        pausePanel.gameObject.SetActive(GamePaused);
    }

    void ResumeGame()
    {
        GamePaused = false;
        pausePanel.gameObject.SetActive(false);
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
