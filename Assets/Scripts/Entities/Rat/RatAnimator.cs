using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAnimator : MonoBehaviour
{
    private Animator _animator;
    private float _animationSpeed;

    // Use this for initialization
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    public void StopAnimation()
    {
        _animator.ResetTrigger( "Idle Reaction" );
        _animator.ResetTrigger( "Attack" );
    }

    public void ResetAnimation()
    {
        _animator.SetTrigger( "Reset" );
    }

    public void SetAnimationSpeed( float speed )
    {
        _animator.SetFloat( "Speed", speed );
    }

    public void SetIdleAnimation()
    {
        _animator.SetTrigger( "Idle Reaction" );
    }

    public void SetJumpAnimation()
    {
        _animator.SetTrigger( "Jump" );
    }

    public void SetAttackAnimation()
    {
        _animator.SetTrigger( "Attack" );
    }

    public void SetDieAnimation()
    {
        _animator.SetTrigger( "Die" );
    }

}
