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

    public Image star;
    public float goalPosition = 0.7f;

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
            float fullOver = goalPosition + (1f - goalPosition)*((currentMoney - moneyGoal)/ currentMoney);

            MoneyValue.value = fullOver;

            if(star.color.a <1)
            { 
                Color tempColor = star.color;
                tempColor.a = 1f;
                star.color = tempColor;
            }
        }
        else if( currentMoney <= 0 )
        {
            // If current money is negative, cap the meter at empty
            MoneyValue.value = 0;
        }
        else
        {
            MoneyValue.value = ( float ) currentMoney / moneyGoal * goalPosition;
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