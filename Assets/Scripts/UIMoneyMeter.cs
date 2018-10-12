using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoneyMeter : MonoBehaviour
{
    public Slider MoneyValue;
    public Text MoneyText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateMoneyMeter( float currentMoney, float moneyGoal )
    {
        if( currentMoney >= moneyGoal )
        {
            // If current money exceeds the goal, cap the meter at full
            MoneyValue.value = 1;
        }
        else if( currentMoney <= 0 )
        {
            // If current money is negative, cap the meter at empty
            MoneyValue.value = 0;
        }
        else
        {
            MoneyValue.value = ( float ) currentMoney / moneyGoal;
        }

        MoneyText.text = "$" + currentMoney + " / $" + moneyGoal;
    }
}