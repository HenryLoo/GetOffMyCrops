using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimeMeter : MonoBehaviour
{
    public Slider TimeValue;
    public Text TimeText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateTimeMeter( float currentTime, float maxTime )
    {
        if( currentTime >= maxTime )
        {
            TimeValue.value = 1;
        }
        if( currentTime <= 0 )
        {
            TimeValue.value = 0;
        }
        else
        {
            TimeValue.value = currentTime / maxTime;
        }

        float timeRemaining = maxTime - currentTime;
        string minutes = Mathf.Floor( timeRemaining / 60 ).ToString( "00" );
        string seconds = ( timeRemaining % 60 ).ToString( "00" );
        TimeText.text = minutes + ":" + seconds;
    }
}