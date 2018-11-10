using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatRunAnimator : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    public void SetAnimationSpeed( float speed )
    {
        _animator.SetFloat( "Speed", speed );
    }
}