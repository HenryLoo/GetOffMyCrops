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

        //if( currentTime >= maxTime ) // no more Slider process bar for time counter
        //{
        //    TimeValue.value = 1;
        //}
        //if( currentTime <= 0 )
        //{
        //    TimeValue.value = 0;
        //}
        //else
        //{
        //    TimeValue.value = currentTime / maxTime;
        //}

        float timeRemaining = maxTime - currentTime;
        if( timeRemaining < 0 ) timeRemaining = 0;
        string minutes = Mathf.Floor( timeRemaining / 60 ).ToString( "00" );
        string seconds = (Mathf.Ceil(timeRemaining) % 60 ).ToString( "00" ); // ceil: 0.9-0.1 shows 1, only 0.0 shows 0
        TimeText.text = minutes + ":" + seconds;

        if (timeRemaining < 20) // flash number when less then 20 seconds
        {
            float persent = timeRemaining - Mathf.FloorToInt(timeRemaining); // eg 17.6000 - 17 = 0.6 = 60%
            
            TimeText.rectTransform.localScale = new Vector3(1+ persent, 1 + persent, 1);

            //Debug.Log(seconds + "time left: " + timeRemaining + " persent " + TimeText.rectTransform.localScale.x);
        }
    }
}