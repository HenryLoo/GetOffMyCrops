using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMenu : Menu, IButtonAction {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void IButtonAction.OnButtonClickAction()
    {
        ChangeState(GameStateLoader.GAME_STATES.MAIN_MENU);
    }

    void IButtonAction.OnButtonClickBack()
    {
        ChangeState(GameStateLoader.GAME_STATES.MAIN_MENU);
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

    void IButtonAction.OnButtonClickSkill()
    {
        ChangeState(GameStateLoader.GAME_STATES.MAIN_MENU);
    }

    void IButtonAction.OnButtonClickUp()
    {
        throw new System.NotImplementedException();
    }


}
