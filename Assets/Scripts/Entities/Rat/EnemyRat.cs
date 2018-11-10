using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRat : Enemy
{
    private AnimateRat _ratAnimations;

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

        _ratAnimations = gameObject.GetComponent<AnimateRat>();
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
                _ratAnimations.SetIdleAnimation();
                break;

            case EnemyState.Moving:
                _ratAnimations.StopAnimation();
                _ratAnimations.SetAnimationSpeed( MOVING_ANIMATION_SPEED );
                break;

            case EnemyState.Eating:
                _ratAnimations.SetAttackAnimation();
                break;

            case EnemyState.Escaping:
                _ratAnimations.StopAnimation();
                _ratAnimations.SetAnimationSpeed( ESCAPING_ANIMATION_SPEED );
                MovementSpeed++;
                break;

            case EnemyState.Despawning:
                _ratAnimations.SetDieAnimation();
                break;

        }
    }

    protected override void Move()
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

    public override void CleanUp()
    {
        Destroy( gameObject );
    }
}
