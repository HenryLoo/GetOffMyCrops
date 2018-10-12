using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface iButtonAction
{
	// triggers once until pressed again
	void onButtonClickLeft();
	void onButtonClickRight();
	void onButtonClickAction();
	void onButtonClickBack();
	void onButtonClickUp();
	void onButtonClickDown();
}