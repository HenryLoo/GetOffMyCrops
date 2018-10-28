using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour {

    public Animator animator;
    
    private void Start()
    {
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);

        // destory itself after animation finished
        Destroy(gameObject, clips[0].clip.length);
    }

    public void SetText(string text)
    {
        animator.GetComponent<Text>().text = text;
    }

}
