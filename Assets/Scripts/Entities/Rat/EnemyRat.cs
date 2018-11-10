using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRat : Enemy
{
    private AnimateRat _ratAnimations;

    // Initialize rat specific variables
    protected override void InitEnemy()
    {
        base.InitEnemy();

        _ratAnimations = gameObject.GetComponent<AnimateRat>();
    }

    public override void Start()
    {
        base.Start();
        PopupMessageCreator.PopupMessage( "Squeak, squeak!", transform );
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
                _ratAnimations.SetAnimationSpeed( 3 );
                break;

            case EnemyState.Eating:
                _ratAnimations.SetAttackAnimation();
                break;

            case EnemyState.Escaping:
                _ratAnimations.StopAnimation();
                _ratAnimations.SetAnimationSpeed( 10 );
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
        if( Math.Abs( transform.position.x - gameController.TileMap.GetPositionAtTile( targetNextPos ).x ) < 0.1f &&
            Math.Abs( transform.position.z - gameController.TileMap.GetPositionAtTile( targetNextPos ).z ) < 0.1f )
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
