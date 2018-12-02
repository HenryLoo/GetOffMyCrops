using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class EnemyCrow : Enemy
{
    [HideInInspector]
    public enum LocomotionState
    {
        Idle,
        Eating,
        FlyingUp,
        FlyingDown,
        Flying,
        Hovering,
        RunAwayUp,
        RunAwayOut
    };

    // Crow's movement constants
    private const float TARGET_VECTOR_VARIATION = 0.1f;
    private const float EXIT_VECTOR_VARIATION = 0.5f;
    private const float ROTATION_ANGLE_UPDATE = 3f;
    private const float ROTATION_RADIUS = 0.1f;
    private const float TILE_CENTER_OFFSET = 0.5f;
    private const int HOVER_TIME_IN_SECONDS = 3;
    private const float FLYING_HEIGHT = 3.0f;
    private const float FLYING_Y_SPEED = 5.0f;

    // Animation speed constants
    private const int MOVING_ANIMATION_SPEED = 5;
    private const int ESCAPING_ANIMATION_SPEED = 10;

    private LocomotionState _locomotionState;
    private float _angle = 0;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        
        _locomotionState = LocomotionState.FlyingUp;
        SoundController.PlaySound( SoundType.CrowSpawn );
    }

    protected override void SetAnimationState()
    {
        switch( currentState )
        {
            case EnemyState.Moving:
                animator.SetTrigger( "Moving" );
                animator.ResetTrigger( "Idle" );
                break;

            case EnemyState.Eating:
                animator.SetTrigger( "Idle" );
                animator.ResetTrigger( "Moving" );
                break;
                
            case EnemyState.Escaping:
                animator.SetTrigger( "Moving" );
                animator.ResetTrigger( "Idle" );
                SoundController.PlaySound( SoundType.CrowScared );
                break;
        }
    }

    // Handle how this enemy should behave once it has reached its target
    protected override void HandleOnTarget()
    {
        switch( _locomotionState )
        {
            case LocomotionState.Flying:
                Debug.Log( "EnemyCrow.HandleOnTarget(): Switch to Eating state" );
                actionTimer.StartTimer();
                _locomotionState = LocomotionState.Hovering;
                break;

            case LocomotionState.Hovering:
                Hover();
                break;

            case LocomotionState.FlyingDown:
                FlyDown();
                break;
        }
    }

    // Move the enemy based on its movement behaviour/AI
    protected override void HandleMoving()
    {
        // If the target crop is removed early, just run away
        if( !IsTargetACrop() )
        {
            RunAway();
        }

        switch( _locomotionState )
        {
            case LocomotionState.FlyingUp:
                FlyUp();
                break;

            case LocomotionState.Flying:
                MoveToNextTile();
                break;
        }
    }

    // Attempt to destroy the target crop
    protected override void HandleEating()
    {
        if( _locomotionState != LocomotionState.Eating )
        {
            return;
        }
        
        SetAnimationState();

        // After crop is eaten or removed early, then run away
        if( actionTimer.GetTicks() >= EatingDuration || !IsTargetACrop() )
        {
            // Not eating anymore, so stop the timer
            actionTimer.StopTimer();

            // Set eaten crop's tile to be on cooldown
            gameController.TileMap.RemoveCropFromTile( targetFinalPos, gameController );

            RunAway();
        }
    }

    public void RunAwayFlyUp()
    {
        OrientInDirection();

        if( HelperFunctions.IsWithinRange( FLYING_HEIGHT - ON_TILE_THRESHOLD,
            FLYING_HEIGHT + ON_TILE_THRESHOLD, transform.position.y ) )
        {
            _locomotionState = LocomotionState.RunAwayOut;
        }
        else
        {
            MoveYPositionUp();
        }
    }

    // Attempt to escape the map
    protected override void HandleEscaping()
    {
        switch( _locomotionState )
        {
            case LocomotionState.RunAwayUp:
                RunAwayFlyUp();
                break;

            case LocomotionState.RunAwayOut:
                MoveToNextTile();
                break;

            default:
                // If somehow got here from any other state, prepare to
                // run away
                _locomotionState = LocomotionState.RunAwayUp;
                break;
        }
    }

    // Call necessary methods when cleaning up
    public override void CleanUp()
    {
        base.CleanUp();
    }

    private void Hover()
    {
        _angle += ROTATION_ANGLE_UPDATE * Time.deltaTime;
        if( _angle >= 360 )
        {
            _angle = 0;
        }

        // Rotate around target coordinates
        float xPrime = ( ROTATION_RADIUS * ( float ) Math.Cos( _angle ) ) + 
            ( targetFinalPos.CoordX + TILE_CENTER_OFFSET );
        float zPrime = ( ROTATION_RADIUS * ( float ) Math.Sin( _angle ) ) + 
            ( targetFinalPos.CoordZ + TILE_CENTER_OFFSET );

        transform.position = new Vector3( xPrime, transform.position.y, zPrime );

        if( actionTimer.GetTicks() >= HOVER_TIME_IN_SECONDS )
        {
            actionTimer.StopTimer();
            _locomotionState = LocomotionState.FlyingDown;
        }
    }

    private void FlyUp()
    {
        if( HelperFunctions.IsWithinRange( FLYING_HEIGHT - ON_TILE_THRESHOLD,
            FLYING_HEIGHT + ON_TILE_THRESHOLD, transform.position.y ) )
        {
            _locomotionState = LocomotionState.Flying;
        }
        else
        {
            MoveYPositionUp();
        }
    }

    private void MoveYPositionUp()
    {
        // Get the world position to fly up to
        Vector3 vectorUp = new Vector3( transform.position.x, FLYING_HEIGHT,
            transform.position.z );

        // Fly up
        float displacementPerTime = FLYING_Y_SPEED * Time.deltaTime;
        transform.position = Vector3.MoveTowards( transform.position,
            vectorUp, displacementPerTime );
    }

    private void FlyDown()
    {
        if( transform.position.y <= 0 )
        {
            _locomotionState = LocomotionState.Eating;
            currentState = EnemyState.Eating;
            actionTimer.StartTimer();

            // Add this crow to the list of enemies on the tile
            //gameController.TileMap.AddEnemyToTile( currentTilePos, this );
        }
        else
        {
            Vector3 vectorDown = new Vector3( transform.position.x, 0, 
                transform.position.z );
            
            // Fly down
            float displacementPerTime = FLYING_Y_SPEED * Time.deltaTime;
            transform.position = Vector3.MoveTowards( transform.position, 
                vectorDown, displacementPerTime );
        }
    }

    public LocomotionState GetLocomotionState()
    {
        return _locomotionState;
    }

    // Rotate this enemy to face its direction of movement
    protected override void OrientInDirection()
    {
        switch( currentDirection )
        {
            case Direction.Up:
                transform.rotation = Quaternion.Euler( 0, 180, 0 );
                break;

            case Direction.Down:
                transform.rotation = Quaternion.Euler( 0, 0, 0 );
                break;

            case Direction.Left:
                transform.rotation = Quaternion.Euler( 0, 90, 0 );
                break;

            case Direction.Right:
                transform.rotation = Quaternion.Euler( 0, 270, 0 );
                break;
        }
    }
}
