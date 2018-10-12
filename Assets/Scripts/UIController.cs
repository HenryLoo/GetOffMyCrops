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

    // Reference to the TileMap instance;
    private TileMap _tileMap;

    // Use this for initialization
    void Start()
    {
        _tileMap = GameObject.Find( "TileMap" ).GetComponent<TileMap>();

        _moneyMeter = GetComponent<UIMoneyMeter>();
        _pauseMenu = GetComponent<UIPauseMenu>();
        _timeMeter = GetComponent<UITimeMeter>();
        
        SetLevelText( _tileMap.Level.LevelName );
        InitMoneyMeter( 20, _tileMap.Level.MoneyGoal );
        AddMoney( 0 );
        InitTimeMeter( 30f );
        UpdateCurrentTime( 0f );
    }

    // Update is called once per frame
    void Update()
    {
        PauseGameCheck();
        RestartGameCheck();
        QuitGameCheck();
    }

    // Sets the Level number display
    public void SetLevelText( string levelText )
    {
        LevelText.text = levelText;
    }

    // Sets the initial money and money goal for the level
    public void InitMoneyMeter( int startingMoney, int moneyGoal )
    {
        _moneyMeter.CurrentMoney = startingMoney;
        _moneyMeter.MaxMoney = moneyGoal;
    }

    // Increment the money meter by a given amount
    // Use a negative value to decrement it
    public void AddMoney( int amount )
    {
        _moneyMeter.AddMoney( amount );
    }

    // Sets the initial time limit for the level
    public void InitTimeMeter( float maxTimeInSeconds )
    {
        _timeMeter.MaxTime = maxTimeInSeconds;
    }

    private void UpdateCurrentTime( float timePassedInSeconds )
    {
        _timeMeter.UpdateTimeMeter( timePassedInSeconds );
    }

    // Pause Menu Game Status functions
    public bool PauseGameCheck()
    {
        if( _pauseMenu.GamePaused )
        {
            //perform action or method call
            Debug.Log( "PAUSED GAME" );
            return true;
        }
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
