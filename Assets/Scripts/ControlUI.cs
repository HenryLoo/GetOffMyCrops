using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour {

    private MoneyMeter moneyBar;
    private ShowPauseMenu pauseMenu;
    private TimerMeter timeMeter;
    public Text LevelText;

	// Use this for initialization
	void Start () {

        moneyBar = GetComponent<MoneyMeter>();
        pauseMenu = GetComponent<ShowPauseMenu>();
        timeMeter = GetComponent<TimerMeter>();


        SetLevelText("Level 1");
        SetMoneyMeter(1000f, 20000f);
        AddMoney(0f);
        SetTimeMeter(30f);
        UpdateCurrentTime(0f);
    }

    // Update is called once per frame
    void Update()
    {
        PauseGameCheck();
        RestartGameCheck();
        QuitGameCheck();
    }

    // Sets the Level number display
    public void SetLevelText(string levelText)
    {
        LevelText.text = levelText;
    }

    // Sets the initial money and money goal for the level
    public void SetMoneyMeter(float startingMoney, float moneyGoal)
    {
        moneyBar.CurrentMoney = startingMoney;
        moneyBar.MaxMoney = moneyGoal;
    }
    public void AddMoney(float moneyGained)
    {
        moneyBar.GainMoney(moneyGained);
    }
    public void ReduceMoney(float moneySpent)
    {
        moneyBar.LoseMoney(moneySpent);
    }


    // sets the initial time limit for the level
    public void SetTimeMeter(float maxTimeInSeconds)
    {
        timeMeter.MaxTime = maxTimeInSeconds;
    }
    private void UpdateCurrentTime(float timePassedInSeconds)
    {
        timeMeter.UpdateTimeMeter(timePassedInSeconds);
    }


    // Pause Menu Game Status functions
    public bool PauseGameCheck()
    {
        if (pauseMenu.GamePaused)
        {
            //perform action or method call
            //Debug.Log("PAUSED GAME");
            return true;
        }
        return false;
    }
    public bool RestartGameCheck()
    {
        if (pauseMenu.RestartStatus)
        {
            //perform action or method call
            Debug.Log("RESTART GAME");
            return true;
        }
        return false;
    }
    public bool QuitGameCheck()
    {
        if (pauseMenu.QuitStatus)
        {
            //perform action or method call
            Debug.Log("QUIT GAME");
            return true;
        }
        return false;
    }





}
