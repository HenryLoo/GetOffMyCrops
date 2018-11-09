using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateRat : MonoBehaviour
{
    Animator Rat_Animator;
    RatRunAnimator Moving;

    // Use this for initialization
    void Start()
    {
        Rat_Animator = gameObject.GetComponent<Animator>();
        Moving = gameObject.GetComponent<RatRunAnimator>();
    }

    float _moveSpeed = 5;

    // Update is called once per frame
    void Update()
    {
        Moving.SetMoveSpeed(_moveSpeed);
    }

    public void StopAnim()
    {
        //_moveSpeed = 0;
        //Rat_Animator.ResetTrigger("Reset");
        Rat_Animator.ResetTrigger("Idle Reaction");
        //Rat_Animator.ResetTrigger("Jump");
        Rat_Animator.ResetTrigger("Attack");
       // Rat_Animator.ResetTrigger("Die");
    }

    public void ResetAnim()
    {
        Rat_Animator.SetTrigger("Reset");
    }

    public void SetWalkingAnimSpeed(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    public void IdleInspectAnim()
    {
        Rat_Animator.SetTrigger("Idle Reaction");
    }

    public void JumpAnim()
    {
        Rat_Animator.SetTrigger("Jump");
    }

    public void AttackAnim()
    {
        Rat_Animator.SetTrigger("Attack");
    }

    public void DieAnim()
    {
        Rat_Animator.SetTrigger("Die");
    }

}
