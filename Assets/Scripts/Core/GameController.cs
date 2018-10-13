using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Reference to the TileMap instance
    public TileMap TileMap;

    // The player's current money
    private int _currentMoney;

    // The timer for the current level's win/lose condition
    private GameTimer _levelTimer;

    // The current level's data
    public LevelData Level;

    // The starting amount of money
    private const int STARTING_MONEY = 20;

    // Flag for if the game is paused
    private bool _isPaused;

    void Awake()
    {
        _levelTimer = new GameTimer();
    }

    // Use this for initialization
    void Start()
    {
        _isPaused = false;

        // Reset the timer
        _levelTimer.StartTimer();

        // TODO: test level loading, remove this later
        LoadLevel( "level1" );
    }

    // Update is called once per frame
    void Update()
    {
        if( !_isPaused )
        {
            _levelTimer.Update();
            TileMap.UpdateTileData();
        }

        GameInput.UpdateInput();
    }

    public void LoadLevel( string levelName )
    {
        // Reset current money
        _currentMoney = STARTING_MONEY;

        // Load the level JSON
        TextAsset levelJson = ( TextAsset ) Resources.Load( "Levels/"
            + levelName );
        Debug.Log( levelJson.text );
        Level = LevelData.CreateFromJson( levelJson.text );
        TileMap.InitTileMap( Level );
    }

    // Add to the current money value (or subtract by using negative values)
    public void AddMoney( int amount )
    {
        _currentMoney += amount;
    }

    // Return the current money value
    public int GetCurrentMoney()
    {
        return _currentMoney;
    }

    // Return the timer for the current level
    public GameTimer GetTimer()
    {
        return _levelTimer;
    }

    // Get the flag for if the game is paused
    public bool GetIsPaused()
    {
        return _isPaused;
    }

    // Set the flag for if the game is paused
    public void SetIsPaused( bool isPaused )
    {
        _isPaused = isPaused;
    }
}
