using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame_Win : MonoBehaviour, IButtonAction
{
	public static readonly string WIN_MESSAGE_TEXTBOX = "TB_WinMessage";
	public static readonly string LEVEL_MONEY_TEXTBOX = "TB_LevelMoney";
	public static readonly string TOTAL_MONEY_TEXTBOX = "TB_TotalMoney";
	public static readonly string LEVEL_NUMBER_TEXTBOX = "TB_CurrentLevel";

	private SaveDataController _dataController;
	SaveDataController.DataStruct data;

	private Text _gameWinMessage;
	private Text _levelNumber;
	private Text _currentLevelMoney;
	private Text _totalMoney;

	// Use this for initialization
	void Start ()
	{
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

	}

	public void OnButtonClickBack()
	{
		GameStateLoader.SwitchState( GameStateLoader.GAME_STATES.MAIN_MENU );
	}

	public void OnButtonClickUp()
	{

	}

	public void OnButtonClickDown()
	{

	}

	public void OnButtonClickSkill()
    {
        // no functionality
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
		_gameWinMessage.text = "You Win!";	// TODO: not ideal, try to get it from the DB
		_levelNumber.text = data.currentLevel.ToString();
		_currentLevelMoney.text = "$" + data.levelMoney.ToString();
		_totalMoney.text = "$" + data.totalMoney.ToString();
	}

	private void GetDataFromDB()
	{
		_dataController = SaveDataController.GetInstance();
		data = _dataController.GetDataSnapshot();
	}
}
