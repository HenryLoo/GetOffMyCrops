public interface IButtonAction
{
    // triggers once until pressed again
    void OnButtonClickUp();
    void OnButtonClickDown();
    void OnButtonClickLeft();
	void OnButtonClickRight();
	void OnButtonClickAction();
	void OnButtonClickBack();
}