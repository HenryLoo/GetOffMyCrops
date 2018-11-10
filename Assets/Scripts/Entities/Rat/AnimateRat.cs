using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateRat : MonoBehaviour
{
    private Animator _animator;
    private RatRunAnimator _animationHandler;
    private float _animationSpeed;

    // Use this for initialization
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _animationHandler = gameObject.GetComponent<RatRunAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        _animationHandler.SetAnimationSpeed( _animationSpeed );
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

    public void SetAnimationSpeed( float moveSpeed )
    {
        _animationSpeed = moveSpeed;
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
