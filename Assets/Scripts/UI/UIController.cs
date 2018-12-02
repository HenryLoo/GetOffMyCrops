using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private UIMoneyMeter _moneyMeter;
    private UITimeMeter _timeMeter;

    public Animator LevelInfo;
    public Text LevelNumber;
    public Text LevelName;

    // Reference to the GameController
    public GameController GameController;

    // Use this for initialization
    void Start()
    {
        _moneyMeter = GetComponent<UIMoneyMeter>();
        _timeMeter = GetComponent<UITimeMeter>();
        
        // Show the level number and name at the beginning of each level
        UpdateLevelText();
        LevelInfo.Play( "LevelInfo" );
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoneyMeter();
        UpdateTimeMeter();
    }

    // Sets the level's name
    public void UpdateLevelText()
    {
        LevelNumber.text = "Level " + GameController.GetCurrentLevelNumber().ToString();
        LevelName.text = GameController.Level.LevelName;
    }

    // Update the money meter to display the player's current money
    public void UpdateMoneyMeter()
    {
        _moneyMeter.UpdateMoneyMeter( GameController.GetCurrentMoney(),
            GameController.Level.MoneyGoal, GameController.GetCombo() );
    }

    // Update the time meter to display the current level's remaining time
    public void UpdateTimeMeter()
    {
        _timeMeter.UpdateTimeMeter( GameController.GetTimer().GetTicks(),
            GameController.Level.RemainingTime );
    }
}
