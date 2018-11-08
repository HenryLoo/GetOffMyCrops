using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRat : Enemy
{
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
        if( !currentTilePos.Equals( previousTilePos ) )
        {
            Debug.Log( "EnemyRat.MoveOnTileMap(): Current position: (" + 
                currentTilePos.CoordX + ", " + currentTilePos.CoordZ + 
                "), Target position: (" + targetMovePos.CoordX +  ", " + 
                targetMovePos.CoordZ + ")" );
            previousTilePos = currentTilePos;
        }

        // Move in the direction of the nearest exit
        float step = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards( transform.position, 
            tileMap.GetPositionAtTile( targetMovePos ), step );

        //Debug.Log( "thisPos: " + transform.position );
        //Debug.Log( "targetPos: " + tileMap.GetPositionAtTile( targetMovePos ) );

        // Update current tile position
        currentTilePos = tileMap.GetTileAtPosition( transform.position );
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
