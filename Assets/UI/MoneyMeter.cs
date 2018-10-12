using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyMeter : MonoBehaviour {

    public float CurrentMoney { get; set; }
    public float MaxMoney { get; set; }

    public Slider moneyMeter;
    public Text moneyMeterText;

    // Use this for initialization
    void Start () {

        moneyMeter.value = CalculateMoneyBar();
        moneyMeterText.text = "$"+CurrentMoney+" / $" +MaxMoney;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GainMoney(2000);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoseMoney(1000);
        }


    }

    float CalculateMoneyBar()
    {
        return CurrentMoney / MaxMoney;
    }

    public void GainMoney(float moneyGained)
    {
        CurrentMoney += moneyGained;
        if (CurrentMoney >= MaxMoney)
        {
            moneyMeter.value = 1;
        }
        else
        {
            moneyMeter.value = CalculateMoneyBar();
        }
        moneyMeterText.text = "$" + CurrentMoney + " / $" + MaxMoney;
    }

    public void LoseMoney(float moneyLost)
    {
        CurrentMoney -= moneyLost;
        if (CurrentMoney <= 0)
        {
            moneyMeter.value = 0;
        }
        else
        {
            moneyMeter.value = CalculateMoneyBar();
        }
        moneyMeterText.text = "$" + CurrentMoney + " / $" + MaxMoney;

    }
}
