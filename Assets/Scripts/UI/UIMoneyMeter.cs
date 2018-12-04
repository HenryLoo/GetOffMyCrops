using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoneyMeter : MonoBehaviour
{
    public Slider MoneyValue;
    public Text MoneyText;

    public Text ComboBonus;
    private readonly string COMBO_TEXT = "Combo Bonus: +$";

    public Image Star;
    private const float GOAL_POSITION = 0.7f;

    // Use this for initialization
    void Start()
    {
        ComboBonus.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateMoneyMeter( float currentMoney, float moneyGoal, int combo )
    {

        if( currentMoney >= moneyGoal )
        {
            // If current money exceeds the goal, cap the meter at full
            float fullOver = GOAL_POSITION + ( 1f - GOAL_POSITION ) * ( ( currentMoney - moneyGoal ) / currentMoney );

            MoneyValue.value = fullOver;

            if( Star.color.a < 1 )
            {
                Color tempColour = Star.color;
                tempColour.a = 1f;
                Star.color = tempColour;
            }
        }
        else if( currentMoney <= 0 )
        {
            // If current money is negative, cap the meter at empty
            MoneyValue.value = 0;
        }
        else
        {
            MoneyValue.value = ( float ) currentMoney / moneyGoal * GOAL_POSITION;
        }

        MoneyText.text = "$" + currentMoney + " / $" + moneyGoal;

        // Update the combo bonus
        if( combo > 0 )
        {
            ComboBonus.enabled = true;
            ComboBonus.text = COMBO_TEXT + combo;
        }
        else
        {
            ComboBonus.enabled = false;
        }
    }
}