using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatRunAnimator : MonoBehaviour
{
    [SerializeField] Animator anim;
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();

    }
    public void SetMoveSpeed(float speed)
    {
        anim.SetFloat("Speed", speed);
    }
}