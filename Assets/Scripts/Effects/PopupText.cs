using System.Collections;
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

    public void SetFontSize( int size )
    {
        Animator.GetComponent<Text>().fontSize = size;
    }

    public void SetFontColour( Vector3 rgb )
    {
        Animator.GetComponent<Text>().color = new Color( rgb.x, rgb.y, rgb.z );
    }

    public void SetOutlineColour( Vector3 rgb )
    {
        Animator.GetComponent<Outline>().effectColor = new Color( rgb.x, rgb.y, rgb.z );
    }
}
