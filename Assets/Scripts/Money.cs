using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money
{

    private static int _currentMoney = 20;

    public const int SEED_BUY_PRICE = 5;

    public const int CROP_SELL_PRICE = 20;

    public static void Set(int value)
    {
        _currentMoney = value;
    }

    // add money, or subtract money by set nagetive value; 
    public static void Add(int value)
    {
        _currentMoney += value;

    }

    // Update is called once per frame
    public static int Get()
    {
        return _currentMoney;
    }
}
