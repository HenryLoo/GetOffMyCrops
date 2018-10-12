using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoneyMeter : MonoBehaviour
{
    public int CurrentMoney;
    public int MaxMoney { get; set; }

    public Slider MoneyValue;
    public Text MoneyText;

    // Use this for initialization
    void Start()
    {
        MoneyValue.value = CalculateMoneyBar();
        UpdateMoneyText();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: remove this debug code
        if( Input.GetKeyDown( KeyCode.Alpha4 ) )
        {
            AddMoney( 10 );
        }
        if( Input.GetKeyDown( KeyCode.Alpha3 ) )
        {
            AddMoney( -10 );
        }
    }

    // Return the percentage filled for the money meter
    float CalculateMoneyBar()
    {
        return ( float ) CurrentMoney / MaxMoney;
    }

    public void AddMoney( int amount )
    {
        CurrentMoney += amount;
        if( CurrentMoney >= MaxMoney )
        {
            // If current money exceeds the goal, cap the meter at full
            MoneyValue.value = 1;
        }
        else if( CurrentMoney <= 0 )
        {
            // If current money is negative, cap the meter at empty
            MoneyValue.value = 0;
        }
        else
        {
            MoneyValue.value = CalculateMoneyBar();
        }

        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        MoneyText.text = "$" + CurrentMoney + " / $" + MaxMoney;
    }
}