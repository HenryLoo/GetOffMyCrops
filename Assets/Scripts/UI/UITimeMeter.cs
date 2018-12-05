using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimeMeter : MonoBehaviour
{
    public Slider TimeValue;
    public Text TimeText;

    private int _previousTime;

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
        float timeRemaining = maxTime - currentTime;
        if( timeRemaining < 0 ) timeRemaining = 0;
        string minutes = Mathf.Floor( ( timeRemaining + 1 ) / 60 ).ToString( "00" );

        // Ceil: 0.9-0.1 shows 1, only 0.0 shows 0
        string seconds = ( Mathf.Ceil( timeRemaining ) % 60 ).ToString( "00" );
        TimeText.text = minutes + ":" + seconds;

        // Flash the timer when less then 20 seconds
        if( timeRemaining < 20 )
        {
            // Example: 17.6000 - 17 = 0.6 = 60%
            float percent = timeRemaining - Mathf.FloorToInt( timeRemaining );
            
            // Play the countdown sound
            int currentSeconds = Mathf.RoundToInt( timeRemaining );
            if( _previousTime > currentSeconds )
            {
                SoundController.PlaySound( SoundType.CountdownBlip, false );
            }
            _previousTime = currentSeconds;

            TimeText.rectTransform.localScale = new Vector3( 1 + percent, 1 + percent, 1 );

            //Debug.Log(seconds + "time left: " + timeRemaining + " persent " + TimeText.rectTransform.localScale.x);
        }
    }
}