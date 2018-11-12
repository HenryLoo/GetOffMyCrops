using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPig : Enemy
{
    private Animator _animator;

    // Animation speed constants
    private const int MOVING_ANIMATION_SPEED = 5;
    private const int ESCAPING_ANIMATION_SPEED = 10;

    // Threshold value for determining if this enemy is on a tile while 
    // transitioning to that tile
    private const float ON_TILE_THRESHOLD = 0.1f;

    // Initialize pig specific variables
    protected override void InitEnemy()
    {
        base.InitEnemy();

        _animator = gameObject.GetComponent<Animator>();
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        SoundController.PlaySound( SoundType.PigSpawn );
    }

    protected override void HandleOnTarget()
    {
        // Keep going to the end of the map
        switch( currentDirection )
        {
            case Direction.Down:
                SetTargetTile( new TileCoordinate( currentTilePos.CoordX, -1 ) );
                break;
            case Direction.Up:
                SetTargetTile( new TileCoordinate( currentTilePos.CoordX,
                    gameController.TileMap.GetSizeZ() ) );
                break;
            case Direction.Left:
                SetTargetTile( new TileCoordinate( -1, currentTilePos.CoordZ ) );
                break;
            case Direction.Right:
                SetTargetTile( new TileCoordinate( gameController.TileMap.GetSizeX(),
                    currentTilePos.CoordZ ) );
                break;
        }
    }

    protected override void HandleMoving()
    {
        // Keep moving if not blocked
        if( !isBlocked )
        {
            Move();
        }

        // Immediately destroy a crop if this pig steps on it
        if( currentTilePos.CoordX >= 0 && currentTilePos.CoordX < gameController.TileMap.GetSizeX() &&
            currentTilePos.CoordZ >= 0 && currentTilePos.CoordZ < gameController.TileMap.GetSizeZ() )

        {
            TileData.TileType currentType = gameController.TileMap.GetTile( currentTilePos );
            if( currentType == TileData.TileType.CropSeed ||
                currentType == TileData.TileType.CropGrowing ||
                currentType == TileData.TileType.CropMature )
            {
                gameController.TileMap.SetTile( currentTilePos, TileData.TileType.PlantableCooldown );
            }
        }

        // If this pig has reached the end of the map, destroy it
        if( CheckIsOnDespawnTile() && IsOnTargetTile() )
        {
            currentState = EnemyState.Despawning;
        }
    }

    protected override void HandleEating()
    {
        // Pig has no "eating" state
    }

    protected override void HandleEscaping()
    {
        Move();
    }

    private void Move()
    {
        // Rotate the model to face direction of movement
        OrientInDirection();

        // Translate this pig's world coordinates toward the next tile
        float step = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards( transform.position,
            gameController.TileMap.GetPositionAtTile( targetNextPos ), step );

        // Update current tile position once the pig has fully transitioned 
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

                Debug.Log( "EnemyPig.Move(): Current position: (" +
                    currentTilePos.CoordX + ", " + currentTilePos.CoordZ +
                    "), Next position: (" + targetNextPos.CoordX + ", " +
                    targetNextPos.CoordZ +
                    "), Target position: (" + targetFinalPos.CoordX + ", " +
                    targetFinalPos.CoordZ + ")" );
            }
        }
    }

    protected override void SetAnimationState()
    {
        switch( currentState )
        {
            case EnemyState.Moving:
                _animator.SetFloat( "Speed", MOVING_ANIMATION_SPEED );
                break;

            case EnemyState.Escaping:
                _animator.SetFloat( "Speed", ESCAPING_ANIMATION_SPEED );
                MovementSpeed++;
                break;
        }
    }
}
