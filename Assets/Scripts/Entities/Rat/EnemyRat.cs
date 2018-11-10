﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRat : Enemy
{
    private RatAnimator _animator;

    // Animation speed constants
    private const int MOVING_ANIMATION_SPEED = 3;
    private const int ESCAPING_ANIMATION_SPEED = 10;

    // Threshold value for determining if this enemy is on a tile while 
    // transitioning to that tile
    private const float ON_TILE_THRESHOLD = 0.1f;

    // The text for the pop-up message to show upon spawning
    private const string SPAWN_TEXT = "Squeak, squeak!";

    // Initialize rat specific variables
    protected override void InitEnemy()
    {
        base.InitEnemy();

        _animator = gameObject.GetComponent<RatAnimator>();
    }

    public override void Start()
    {
        base.Start();
        PopupMessageCreator.PopupMessage( SPAWN_TEXT, transform );
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
                MovementSpeed++;
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
            Move();
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
        Move();
    }

    private void Move()
    {
        // Rotate the model to face direction of movement
        OrientInDirection();

        // Translate this rat's world coordinates toward the next tile
        float step = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards( transform.position,
            gameController.TileMap.GetPositionAtTile( targetNextPos ), step );

        // Update current tile position once the rat has fully transitioned 
        // onto the next tile
        Vector3 targetPos = gameController.TileMap.GetPositionAtTile( targetNextPos );
        if( Math.Abs( transform.position.x - targetPos.x ) < ON_TILE_THRESHOLD &&
            Math.Abs( transform.position.z - targetPos.z ) < ON_TILE_THRESHOLD )
        {
            currentTilePos = targetNextPos;

            // Set the next tile to move to if not at target yet
            if( !currentTilePos.Equals( targetFinalPos ) )
            {
                MoveInDirection( currentDirection );

                Debug.Log( "EnemyRat.Move(): Current position: (" +
                    currentTilePos.CoordX + ", " + currentTilePos.CoordZ +
                    "), Next position: (" + targetNextPos.CoordX + ", " +
                    targetNextPos.CoordZ +
                    "), Target position: (" + targetFinalPos.CoordX + ", " +
                    targetFinalPos.CoordZ + ")" );
            }
        }
    }
}
