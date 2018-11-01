using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMenu : Menu, IButtonAction
{
	public static readonly string WIN_MESSAGE_TEXTBOX = "TB_WinMessage";
	public static readonly string LEVEL_MONEY_TEXTBOX = "TB_LevelMoney";
	public static readonly string TOTAL_MONEY_TEXTBOX = "TB_TotalMoney";
	public static readonly string LEVEL_NUMBER_TEXTBOX = "TB_CurrentLevel";

    private readonly string NEXT_BUTTON = "Button_Next_Level";
    private readonly string QUIT_BUTTON = "Button_Quit";
    
    private readonly string LEVEL_MESSAGE = "Level ";
    private readonly string WIN_MESSAGE = "You win!";
    private readonly string EARNED1_MESSAGE = "You earned $";
    private readonly string EARNED2_MESSAGE = " in this level";
    private readonly string TOTAL_MONEY_MESSAGE = "Total: $";

    private SaveDataController _dataController;
	private SaveDataController.DataStruct _data;

	private Text _gameWinMessage;
	private Text _levelNumber;
	private Text _currentLevelMoney;
	private Text _totalMoney;

	// Use this for initialization
	void Start ()
    {
        Debug.Log( "WinMenu.cs" );

        GetDataFromDB();
		InitTextBoxes();
		SetTextBoxData();

		GameInput.AttachInput(
            actionClick: OnButtonClickAction,
            backClick: OnButtonClickBack,
            leftClick: OnButtonClickLeft, 
            rightClick: OnButtonClickRight,
            downClick: OnButtonClickDown,
            upClick: OnButtonClickUp );

        // Initialize menu options
        AddMenuOption( NEXT_BUTTON, OnNextButtonSelect );
        AddMenuOption( QUIT_BUTTON, OnQuitButtonSelect );

        SelectDefaultButton();
    }
	
	// Update is called once per frame
	void Update ()
	{
		GameInput.UpdateInput();
	}

	public void OnButtonClickLeft()
	{

	}

	public void OnButtonClickRight()
	{

	}

    public void OnButtonClickAction()
    {
        SelectButton();
    }

    public void OnButtonClickBack()
    {
        // no functionality
    }

    public void OnButtonClickUp()
    {
        // Move selected button upwards
        UpdateButtonIndex( BUTTON_INDEX_UP );
    }

    public void OnButtonClickDown()
    {
        // Move selected button downwards
        UpdateButtonIndex( BUTTON_INDEX_DOWN );
    }

    public void OnButtonClickSkill()
    {
        // no functionality
    }

    private void OnNextButtonSelect()
    {
        // Move to next level
        Debug.Log( "Next button is selected" );
        int nextLevel = _data.CurrentLevel + 1;

        // All levels have been cleared
        if( nextLevel > SaveDataController.NUM_LEVELS )
        {
            // Reset the total money so that it doesn't persist across 
            // play sessions
            _dataController.ScoreToSubmit = _dataController.TotalMoney;
            _dataController.TotalMoney = 0;
            _dataController.CurrentLevel = 1;
            _dataController.SaveDataToDisk();

            // Flag submit request to true to indicate that a score is being
            // submitted
            SaveDataController.GetInstance().IsSubmitting = true;
            ChangeState( GameStateLoader.GAME_STATES.SCOREBOARD );
        }
        // Go to the next level
        else
        {
            _dataController.CurrentLevel = nextLevel;
            ChangeState( GameStateLoader.GAME_STATES.GAMEPLAY );
        }
    }

    private void OnQuitButtonSelect()
    {
        // Quit to main menu
        Debug.Log( "Quit button is selected" );
        ChangeState( GameStateLoader.GAME_STATES.MAIN_MENU );
    }

    private void CleanUp()
	{
		// put all the clean up code here
	}

	private void InitTextBoxes()
	{
		_gameWinMessage = GameObject.Find( WIN_MESSAGE_TEXTBOX ).GetComponent<Text>();
		_levelNumber = GameObject.Find( LEVEL_NUMBER_TEXTBOX ).GetComponent<Text>();
		_currentLevelMoney = GameObject.Find( LEVEL_MONEY_TEXTBOX ).GetComponent<Text>();
		_totalMoney = GameObject.Find( TOTAL_MONEY_TEXTBOX ).GetComponent<Text>();
	}

	private void SetTextBoxData()
	{
		_gameWinMessage.text = WIN_MESSAGE;
		_levelNumber.text = LEVEL_MESSAGE + _data.CurrentLevel.ToString();
		_currentLevelMoney.text = EARNED1_MESSAGE + _data.LevelMoney.ToString() + EARNED2_MESSAGE;
		_totalMoney.text = TOTAL_MONEY_MESSAGE + _data.TotalMoney.ToString();
	}

	private void GetDataFromDB()
	{
		_dataController = SaveDataController.GetInstance();
		_data = _dataController.GetDataSnapshot();
	}
}
