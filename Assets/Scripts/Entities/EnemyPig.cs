using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPig : Enemy
{
    // Animation speed constants
    private const int MOVING_ANIMATION_SPEED = 5;
    private const int ESCAPING_ANIMATION_SPEED = 10;

    // Initialize pig specific variables
    protected override void InitEnemy()
    {
        base.InitEnemy();
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
            MoveToNextTile();
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
        MoveToNextTile();
    }

    protected override void SetAnimationState()
    {
        switch( currentState )
        {
            case EnemyState.Moving:
                animator.SetFloat( "Speed", MOVING_ANIMATION_SPEED );
                break;

            case EnemyState.Escaping:
                animator.SetFloat( "Speed", ESCAPING_ANIMATION_SPEED );
                SoundController.PlaySound( SoundType.PigScared );
                break;
        }
    }
}
