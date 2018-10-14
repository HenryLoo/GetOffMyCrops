using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private UIMoneyMeter _moneyMeter;
    private UIPauseMenu _pauseMenu;
    private UITimeMeter _timeMeter;
    public Text LevelText;

    // Reference to the GameController
    private GameController _gameController;

    // Use this for initialization
    void Start()
    {
        _gameController = GameObject.Find( "GameController" )
            .GetComponent<GameController>();

        _moneyMeter = GetComponent<UIMoneyMeter>();
        _pauseMenu = GetComponent<UIPauseMenu>();
        _timeMeter = GetComponent<UITimeMeter>();
        
        SetLevelText( _gameController.Level.LevelName );
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Menu is disabled for alpha, since it isn't functional
        //PauseGameCheck();
        //RestartGameCheck();
        //QuitGameCheck();

        UpdateMoneyMeter();
        UpdateTimeMeter();
    }

    // Sets the level's name
    public void SetLevelText( string levelText )
    {
        LevelText.text = levelText;
    }

    // Update the money meter to display the player's current money
    public void UpdateMoneyMeter()
    {
        _moneyMeter.UpdateMoneyMeter( _gameController.GetCurrentMoney(),
            _gameController.Level.MoneyGoal );
    }

    // Update the time meter to display the current level's remaining time
    public void UpdateTimeMeter()
    {
        _timeMeter.UpdateTimeMeter( _gameController.GetTimer().GetTicks(),
            _gameController.Level.RemainingTime );
    }

    // Pause Menu Game Status functions
    public bool PauseGameCheck()
    {
        if( _pauseMenu.GamePaused )
        {
            Debug.Log( "PAUSED GAME" );
            _gameController.SetIsPaused( true );
            return true;
        }

        _gameController.SetIsPaused( false );
        return false;
    }

    public bool RestartGameCheck()
    {
        if( _pauseMenu.RestartStatus )
        {
            //perform action or method call
            Debug.Log( "RESTART GAME" );
            return true;
        }
        return false;
    }

    public bool QuitGameCheck()
    {
        if( _pauseMenu.QuitStatus )
        {
            //perform action or method call
            Debug.Log( "QUIT GAME" );
            return true;
        }
        return false;
    }
    
}
