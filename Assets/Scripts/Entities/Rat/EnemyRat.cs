using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRat : Enemy
{
    AnimateRat ratAnimations;
    // Initialize rat specific variables
    protected void InitEnemyRat()
    {
        SpawnDelayDuration = 1;
        spawnDelayTimer = new GameTimer();
        spawnDelayTimer.StartTimer();

        EatingDuration = 5;
        eatingTimer = new GameTimer();

        CanBeBlocked = true;
        MovementSpeed = 1f;
        ratAnimations = gameObject.GetComponent<AnimateRat>();
    }

    private void Start()
    {
        InitEnemy();
        InitEnemyRat();
        PopupMessageCreator.PopupMessage( "Squeek, Squeek", transform );
    }

    // Moves this rat enemy on the tile map from its currentTilePos towards 
    // the targetMovePos
    private void MoveOnTileMap()
    {
        OrientInDirection();
        if ( !currentTilePos.Equals( previousTilePos ) )
        {
            Debug.Log( "EnemyRat.MoveOnTileMap(): Current position: (" + 
                currentTilePos.CoordX + ", " + currentTilePos.CoordZ + 
                "), Target position: (" + targetMovePos.CoordX +  ", " + 
                targetMovePos.CoordZ + ")" );
            previousTilePos = currentTilePos;
        }

        // Move in the direction of the targetMovePos
        float step = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards( transform.position, 
            tileMap.GetPositionAtTile( targetMovePos ), step );

        //Debug.Log( "thisPos: " + transform.position );
        //Debug.Log( "targetPos: " + tileMap.GetPositionAtTile( targetMovePos ) );

        // Update current tile position
        currentTilePos = tileMap.GetTileAtPosition( transform.position );
    }

    private void OrientInDirection()
    {
        switch (currentDirection)
        {
            case Direction.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;

            case Direction.Down:
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;

            case Direction.Left:
                transform.rotation = Quaternion.Euler(0, 260, 0);
                break;

            case Direction.Right:
                transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
        }
    }

    public override void StateAnim()
    {
        switch (currentState)
        {
            case EnemyState.Spawning:
                ratAnimations.IdleInspectAnim();
                break;

            case EnemyState.Moving:
                ratAnimations.StopAnim();
                ratAnimations.SetWalkingAnimSpeed(3);
                break;

            case EnemyState.Eating:
                ratAnimations.AttackAnim();
                break;

            case EnemyState.Escaping:
                ratAnimations.StopAnim();
                ratAnimations.SetWalkingAnimSpeed(10);
                MovementSpeed++;
                break;

            case EnemyState.Despawning:
                ratAnimations.DieAnim();
                break;
      
        }
    }

    public override void Move()
    {
        MoveOnTileMap();
    }

    public override void RunAway()
    {
        MoveOnTileMap();
    }

    public override void CleanUp()
    {
        Destroy( gameObject );
    }
}
