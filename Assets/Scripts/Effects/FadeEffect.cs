using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    public Texture2D FadeTexture;
    private float _fadeSpeed = 0.6f;
    private int _drawDepth = -1000;

    private float _alpha = 0;
    private float _fadeDir = -1;

    private bool _isFadingIn = false;
    private bool _isFadingOut = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if( _isFadingIn || _isFadingOut )
        {
            float fadeSpeed = _isFadingIn ? -_fadeSpeed : _fadeSpeed;
            _alpha -= _fadeDir * fadeSpeed * Time.deltaTime;
            _alpha = Mathf.Clamp01( _alpha );

            GUI.color = new Color( GUI.color.r, GUI.color.g, GUI.color.b, _alpha );
            GUI.depth = _drawDepth;
            GUI.DrawTexture( new Rect( 0, 0, Screen.width, Screen.height ), FadeTexture );
        }

        if( _isFadingIn && GUI.color.a >= 1 )
        {
            _isFadingIn = false;
        }
        else if( _isFadingOut && GUI.color.a <= 0 )
        {
            _isFadingOut = false;
        }
    }

    public void StartFadingIn()
    {
        _alpha = 1;
        _isFadingOut = false;
        _isFadingIn = true;
    }

    public void StartFadingOut()
    {
        _alpha = 0;
        _isFadingOut = true;
        _isFadingIn = false;
    }
}
