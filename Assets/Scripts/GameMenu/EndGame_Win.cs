using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame_Win : MonoBehaviour, IButtonAction
{
	// Use this for initialization
	void Start ()
	{
		GameInput.AttachInput(
            actionClick: OnButtonClickAction,
            backClick: OnButtonClickBack,
            leftClick: OnButtonClickLeft, 
            rightClick: OnButtonClickRight,
            downClick: OnButtonClickDown,
            upClick: OnButtonClickUp );
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameInput.UpdateInput();
	}

	public void OnButtonClickLeft()
	{

	}

	public void OnButtonClickRight()
	{

	}

	public void OnButtonClickAction()
	{

	}

	public void OnButtonClickBack()
	{
		GameStateLoader.SwitchState( GameStateLoader.GAME_STATES.MAIN_MENU );
	}

	public void OnButtonClickUp()
	{

	}

	public void OnButtonClickDown()
	{

	}

	private void CleanUp()
	{
		// put all the clean up code here
	}
}
