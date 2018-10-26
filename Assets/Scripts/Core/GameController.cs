using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, IButtonAction
{
    // Reference to the TileMap instance
    public TileMap TileMap;

    // Reference to the EnemyController instance
    public EnemyController EnemyController;

    // Reference to the UIController instance
    public PauseMenu PauseMenu;

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

	// Delegate to handle cleanup on scene destroy
	private delegate void GameControllerUpdate();
	private GameControllerUpdate _updateEveryFrame;

    void Awake()
    {
        _levelTimer = new GameTimer();

        // TODO: test level loading, remove this later
        LoadLevel( "level1" );
    }

    // Use this for initialization
    void Start()
    {
		Debug.Log( "GameController.cs" );
        _isPaused = false;

        // Reset the timer
        _levelTimer.StartTimer();
		_updateEveryFrame = UpdateEveryFrame;

        GameInput.AttachInput(
           actionClick: OnButtonClickAction,
           skillClick: OnButtonClickSkill,
           backClick: OnButtonClickBack,
           leftClick: OnButtonClickLeft,
           rightClick: OnButtonClickRight,
           downClick: OnButtonClickDown,
           upClick: OnButtonClickUp );
    }

    // Update is called once per frame
    void Update()
    {
		if ( _updateEveryFrame != null ) _updateEveryFrame();
    }

	void UpdateEveryFrame()
	{
		if( !_isPaused )
        {
            _levelTimer.Update();
            TileMap.UpdateEveryFrame();
			_PollEndGame();
        }

        GameInput.UpdateInput();

        // TODO: debug controls, remove this later
        if( Input.GetKeyDown( "a" ) )
        {
            _levelTimer.AddTicks( -10 );
        }
        else if( Input.GetKeyDown( "s" ) )
        {
            _levelTimer.AddTicks( 10 );
        }
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
        EnemyController.Init();
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

	private void _PollEndGame()
	{
		if ( _levelTimer.GetTicks() >= Level.RemainingTime )
		{
			Debug.Log( "GameController.cs: timer expired" );
			CleanUp();

			if ( _currentMoney < Level.MoneyGoal )
			{
				GameStateLoader.SwitchState( GameStateLoader.GAME_STATES.END_GAME_LOSE );
			}
			else
			{
				GameStateLoader.SwitchState( GameStateLoader.GAME_STATES.END_GAME_WIN );
			}
		}
	}

	private void CleanUp()
	{
		_levelTimer.StopTimer();
		_updateEveryFrame = null;
		TileMap.CleanUp();
	}

    public void OnButtonClickUp()
    {
        if( !_isPaused )
        {
            TileMap.GetPlayer().MoveV( 1 );
        }
        else
        {
            PauseMenu.MoveSelectedUp();
        }
    }

    public void OnButtonClickDown()
    {
        if( !_isPaused )
        {
            TileMap.GetPlayer().MoveV( -1 );
        }
        else
        {
            PauseMenu.MoveSelectedDown();
        }
    }

    public void OnButtonClickLeft()
    {
        if( !_isPaused )
        {
            TileMap.GetPlayer().MoveH( -1 );
        }
    }

    public void OnButtonClickRight()
    {
        if( !_isPaused )
        {
            TileMap.GetPlayer().MoveH( 1 );
        }
    }

    public void OnButtonClickAction()
    {
        if( !_isPaused )
        {
            TileMap.GetPlayer().PerformAction();
        }
        else
        {
            PauseMenu.SelectOption();
        }
    }

    public void OnButtonClickBack()
    {
        _isPaused = !_isPaused;
        PauseMenu.TogglePauseMenu( _isPaused );
    }

	public void OnButtonClickSkill()
    {
        TileMap.GetPlayer().Scare();
    }
}
