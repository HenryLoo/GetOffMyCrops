using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimeMeter : MonoBehaviour
{
    // The level timer in seconds
    public float CurrentTime;
    public float MaxTime { get; set; }

    public Slider TimeValue;
    public Text TimeText;

    // Use this for initialization
    void Start()
    {
        CurrentTime = 0f;

        TimeValue.value = CalculateTimeMeter();
        DisplayRemainingTime();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: remove this debug code
        if( Input.GetKeyDown( KeyCode.Alpha1 ) )
        {

            UpdateTimeMeter( --CurrentTime );
        }
        if( Input.GetKeyDown( KeyCode.Alpha2 ) )
        {
            UpdateTimeMeter( ++CurrentTime );
        }
    }

    void DisplayRemainingTime()
    {
        if( CurrentTime > MaxTime ) { CurrentTime = MaxTime; }
        if( CurrentTime < 0 ) { CurrentTime = 0; }

        float timeRemaining = MaxTime - CurrentTime;
        string minutes = Mathf.Floor( timeRemaining / 60 ).ToString( "00" );
        string seconds = ( timeRemaining % 60 ).ToString( "00" );
        TimeText.text = minutes + ":" + seconds;
    }

    float CalculateTimeMeter()
    {
        return CurrentTime / MaxTime;
    }

    public void UpdateTimeMeter( float newCurTime )
    {
        CurrentTime = newCurTime;
        if( CurrentTime >= MaxTime )
        {
            TimeValue.value = 1;
        }
        if( CurrentTime <= 0 )
        {
            TimeValue.value = 0;
        }
        else
        {
            TimeValue.value = CalculateTimeMeter();
        }

        DisplayRemainingTime();
    }
}