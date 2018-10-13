using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButtonAction
{
	// triggers once until pressed again
	void OnButtonClickLeft();
	void OnButtonClickRight();
	void OnButtonClickAction();
	void OnButtonClickBack();
	void OnButtonClickUp();
	void OnButtonClickDown();
}