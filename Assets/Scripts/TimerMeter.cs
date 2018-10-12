
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerMeter : MonoBehaviour {

    public float CurrentTime { get; set; }
    public float MaxTime { get; set; }

    public Slider TimeMeter;
    public Text TimeMeterText;

    // Use this for initialization
    void Start()
    {
        CurrentTime = 0f;

        TimeMeter.value = CalculateTimeMeter();
        DisplayTimeLeft();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            UpdateTimeMeter(--CurrentTime);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateTimeMeter(++CurrentTime);
        }
    }

    void DisplayTimeLeft()
    {
        if(CurrentTime > MaxTime) {CurrentTime = MaxTime; }
        if(CurrentTime < 0) { CurrentTime = 0; }

        float timeRemaining = MaxTime - CurrentTime;
        string minutes = Mathf.Floor(timeRemaining / 60).ToString("00");
        string seconds = (timeRemaining % 60).ToString("00");
        TimeMeterText.text = minutes + ":" +seconds;
    }

    float CalculateTimeMeter()
    {
        return CurrentTime / MaxTime;
    }

    public void UpdateTimeMeter(float newCurTime)
    {
        CurrentTime = newCurTime;
        if (CurrentTime >= MaxTime)
        {
            TimeMeter.value = 1;
        }
        if (CurrentTime <= 0)
        {
            TimeMeter.value = 0;
        }
        else
        {
            TimeMeter.value = CalculateTimeMeter();
        }
        DisplayTimeLeft();
    }
}
