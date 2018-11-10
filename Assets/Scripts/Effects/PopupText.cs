﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour
{
    public Animator Animator;

    private void Start()
    {
        AnimatorClipInfo[] clips = Animator.GetCurrentAnimatorClipInfo( 0 );

        // Destroy itself after the animation has finished
        Destroy( gameObject, clips[ 0 ].clip.length );
    }

    public void SetText( string text )
    {
        Animator.GetComponent<Text>().text = text;
    }
}