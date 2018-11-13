using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseMenu : Menu, IButtonAction
{
	public static readonly string LOSE_MESSAGE_TEXTBOX = "TB_LoseMessage";
	public static readonly string LEVEL_MONEY_TEXTBOX = "TB_LevelMoney";
	public static readonly string TOTAL_MONEY_TEXTBOX = "TB_TotalMoney";
	public static readonly string LEVEL_NUMBER_TEXTBOX = "TB_CurrentLevel";

    private readonly string SUBMIT_BUTTON = "Button_Submit";
    private readonly string QUIT_BUTTON = "Button_Quit";

    private readonly string LEVEL_MESSAGE = "Level ";
    private readonly string LOSE_MESSAGE = "You lose!";
    private readonly string EARNED1_MESSAGE = "You earned $";
    private readonly string EARNED2_MESSAGE = " in this level";
    private readonly string TOTAL_MONEY_MESSAGE = "Total: $";

    private SaveDataController _dataController;
    private GameData _data;

    private Text _gameLoseMessage;
	private Text _levelNumber;
	private Text _currentLevelMoney;
	private Text _totalMoney;

	// Use this for initialization
	void Start ()
    {
        Debug.Log( "LoseMenu.cs" );

        GetDataFromController();
		InitTextBoxes();
		SetTextBoxData();

        // Reset the total money so that it doesn't persist across 
        // play sessions
        _dataController.ScoreToSubmit = _data.TotalMoney;
        _dataController.ResetPlayData();

        GameInput.AttachInput(
            actionClick: OnButtonClickAction,
            backClick: OnButtonClickBack,
            leftClick: OnButtonClickLeft, 
            rightClick: OnButtonClickRight,
            downClick: OnButtonClickDown,
            upClick: OnButtonClickUp );

        // Initialize menu options
        AddMenuOption( SUBMIT_BUTTON, OnSubmitButtonSelect );
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

    private void OnSubmitButtonSelect()
    {
        // Submit score
        Debug.Log( "Submit button is selected" );

        // Flag submit request to true to indicate that a score is being
        // submitted
        SaveDataController.GetInstance().IsSubmitting = true;
        ChangeState( GameStateLoader.GAME_STATES.SCOREBOARD );
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
		_gameLoseMessage = GameObject.Find( LOSE_MESSAGE_TEXTBOX ).GetComponent<Text>();
		_levelNumber = GameObject.Find( LEVEL_NUMBER_TEXTBOX ).GetComponent<Text>();
		_currentLevelMoney = GameObject.Find( LEVEL_MONEY_TEXTBOX ).GetComponent<Text>();
		_totalMoney = GameObject.Find( TOTAL_MONEY_TEXTBOX ).GetComponent<Text>();
	}

	private void SetTextBoxData()
	{
        _gameLoseMessage.text = LOSE_MESSAGE;
        _levelNumber.text = LEVEL_MESSAGE + _data.CurrentLevel.ToString();
        _currentLevelMoney.text = EARNED1_MESSAGE + _dataController.LevelMoney.ToString() + EARNED2_MESSAGE;
        _totalMoney.text = TOTAL_MONEY_MESSAGE + _data.TotalMoney.ToString();
    }

	private void GetDataFromController()
	{
		_dataController = SaveDataController.GetInstance();
        _data = _dataController.LoadData();
	}
}
