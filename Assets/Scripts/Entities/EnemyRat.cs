using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRat : Enemy
{
    private RatAnimator _animator;

    // Animation speed constants
    private const int MOVING_ANIMATION_SPEED = 3;
    private const int ESCAPING_ANIMATION_SPEED = 10;
    
    // Initialize rat specific variables
    protected override void InitEnemy()
    {
        base.InitEnemy();

        _animator = gameObject.GetComponent<RatAnimator>();
    }

    public override void Start()
    {
        base.Start();

        SoundController.PlaySound( SoundType.RatSpawn );
    }

    protected override void SetAnimationState()
    {
        switch( currentState )
        {
            case EnemyState.Spawning:
                _animator.SetIdleAnimation();
                break;

            case EnemyState.Moving:
                _animator.StopAnimation();
                _animator.SetAnimationSpeed( MOVING_ANIMATION_SPEED );
                break;

            case EnemyState.Eating:
                _animator.SetAttackAnimation();
                break;

            case EnemyState.Escaping:
                _animator.StopAnimation();
                _animator.SetAnimationSpeed( ESCAPING_ANIMATION_SPEED );
                SoundController.PlaySound( SoundType.RatScared );
                break;

            case EnemyState.Despawning:
                _animator.SetDieAnimation();
                break;
        }
    }

    protected override void HandleOnTarget()
    {
        // Start eating the target crop if on top of it
        actionTimer.StartTimer();
        currentState = EnemyState.Eating;
        Debug.Log( "EnemyRat.HandleOnTarget(): Switched to Eating state" );
    }

    protected override void HandleMoving()
    {
        // If the target crop is removed early, just run away
        if( !IsTargetACrop() )
        {
            RunAway();
        }

        // Keep moving if not blocked
        if( !isBlocked )
        {
            MoveToNextTile();
        }
    }


    // Delays the destruction of the crop until the eatingTimer of the enemy 
    // reaches eatingDuration
    protected override void HandleEating()
    {
        SetAnimationState();

        // After crop is eaten or removed early, then run away
        if( actionTimer.GetTicks() >= EatingDuration || !IsTargetACrop() )
        {
            // Not eating anymore, so stop the timer
            actionTimer.StopTimer();

            // Set eaten crop's tile to be on cooldown
            gameController.TileMap.SetTile( targetFinalPos, TileData.TileType.PlantableCooldown );

            RunAway();
        }
    }

    protected override void HandleEscaping()
    {
        MoveToNextTile();
    }
}
