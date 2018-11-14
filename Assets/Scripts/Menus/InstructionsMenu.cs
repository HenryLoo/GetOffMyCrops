using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsMenu : Menu, IButtonAction
{ 

    public Text m_MyText;


    void Start()
    {
        m_MyText.text
            = "The purpose of this game is to help Farmer Dale restore his broken down farm by raising seasonal crops and earning money. \n"
            + "\n"
            + "\n"
            + "RULES:\n"
            + "- Your overall money will increase as you progress through levels until you have finally met your end goal of raising enough funds to repair the farm.\n"
            + "\n"
            + "- In order to pass a level, you must meet the minimum profits for that season which will be displayed at the top left of the screen. \n"
            + "\n"
            + "- Each level of the game will have plantable soil on which you can plant crops to grow and harvest. \n"
            + "\n"
            + "- planting crops will cost you a set amount of money per seed.\n"
            + "\n"
            + "- when you harvest a crop you will recieve a large sum of money.\n"
            + "\n"
            + "- when a crop has just been harvested, the tile will be on cooldown for a small period of time before it can be planted on again. \n"
            + "\n"
            + "\n"
            + "ENEMIES:\n"
            + "\n"
            + "Enemies will begin to appear as the game progresses and attempt to eat or destroy your crops while they are growing\n"
            + "\n"
            + "The player has the ability to scare away some enemies by either entering the same tile as the enemy, Or by blocking thier path as they are moving \n"
            + "\n"
            + "If the player happens to scare an enemy, they will attempt to flee off the map, generally.\n"
            + "\n"
            + "\n"
            + "CONTROLS:\n"
            + "\n"
            + "1.\n"
            + "2.\n"
            + "3.\n"
            ;
    }


    void IButtonAction.OnButtonClickUp()
    {
        throw new System.NotImplementedException();
    }

    void IButtonAction.OnButtonClickDown()
    {
        throw new System.NotImplementedException();
    }

    void IButtonAction.OnButtonClickLeft()
    {
        throw new System.NotImplementedException();
    }

    void IButtonAction.OnButtonClickRight()
    {
        throw new System.NotImplementedException();
    }

    void IButtonAction.OnButtonClickAction()
    {
        throw new System.NotImplementedException();
    }

    void IButtonAction.OnButtonClickSkill()
    {
        throw new System.NotImplementedException();
    }

    void IButtonAction.OnButtonClickBack()
    {
        ChangeState(GameStateLoader.GAME_STATES.MAIN_MENU);
    }
}
