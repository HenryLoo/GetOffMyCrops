using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IEntity
{
    // Reference to the tile map instance
    protected TileMap tileMap;

    // The maximum tilemap size
    protected TileCoordinate maxTileMapSize;

    // The enemy's spawn coordinates within the tile map
    protected TileCoordinate spawnTilePos;

    // The enemy's current coordinates within the tile map
    protected TileCoordinate currentTilePos;

    // The enemy's previous coordinates within the tile map
    protected TileCoordinate previousTilePos;

    // The enemy's target crop to eat
    protected TileCoordinate targetCropPos;

    // The tile that this enemy will move towards
    protected TileCoordinate targetMovePos;

    // The duration of time in seconds it takes for this enemy to begin movement
    public float SpawnDelayDuration;
    protected GameTimer spawnDelayTimer;

    // The duration of time in seconds it takes for this enemy to damage a crop
    public float EatingDuration;
    protected GameTimer eatingTimer;

    // Flag for if the player can block this enemy
    public bool CanBeBlocked = false;

    // Flag for if this enemy is being blocked
    public bool IsBlocked = false;

    // Flag for if the enemy has reached the target crop or not
    public bool IsOnTargetCrop = false;

    // How quickly the enemy will move between tiles
    public float MovementSpeed;

    // The enemy's possible movement directions
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    // The enemy's current movement direction
    protected Direction currentDirection;

    // The enemy's possible states of activity
    public enum EnemyState
    {
        Spawning,
        Moving,
        Eating,
        Escaping,
        Despawning
    }

    // The enemy's current state of activity
    protected EnemyState currentState;

    // Initialize universal enemy variables 
    protected void InitEnemy()
    {
        tileMap = GameObject.Find( "TileMap" ).GetComponent<TileMap>();

        currentTilePos = tileMap.GetTileAtPosition( transform.position );
        spawnTilePos = currentTilePos;
        previousTilePos = currentTilePos;
        targetMovePos = targetCropPos;
    }

    void Start()
    {
        InitEnemy();
    }
    
    // Update the enemy's behaviour every frame
    public void Update()
    {
        switch( currentState )
        {
            case EnemyState.Spawning:
                SpawnMoveDelay();
                break;

            case EnemyState.Moving:
                if( IsOnTargetCrop )
                {
                    eatingTimer.StartTimer();
                    currentState = EnemyState.Eating;
                    Debug.Log( "Enemy.Update(): Switched to Eating state" );
                }
                else
                {
                    IsBlocked = false;
                    if( CanBeBlocked )
                    {
                        SetIsBlocked();
                    }
                    if( !IsBlocked )
                    {
                        Move();
                        SetIsOnTargetCrop();
                    }
                }

                break;

            case EnemyState.Eating:
                IsBlocked = false;

                if( CanBeBlocked )
                {
                    SetIsBlocked();
                }

                if( !IsBlocked )
                {
                    EatCrop();
                }

                break;
            case EnemyState.Escaping:
                targetMovePos = FindNearestExit();

                if( CheckIsOnDespawnTile() )
                {
                    currentState = EnemyState.Despawning;
                }
                else
                {
                    RunAway();
                }

                break;

            case EnemyState.Despawning:
                CleanUp();

                break;
        }
    }
    
    // Set the crop at this tile to be the enemy's target crop
    public void SetTargetCrop( TileCoordinate crop )
    {
        targetCropPos = crop;
        //Debug.Log("enemy SET targetCrop: " + _targetCropPos.CoordX + ", " + _targetCropPos.CoordZ);
    }
    
    // Finds the bounds of the map
    public void SetMapBoundary( TileCoordinate tilemapSize )
    {
        maxTileMapSize = tilemapSize;
    }

    // Delays the movement of this enemy until spawnDelayTimer reaches 
    // spawnDelayDuration
    public void SpawnMoveDelay()
    {
        spawnDelayTimer.Update();

        // Set currentState to Moving when spawn delay is over
        if( spawnDelayTimer.GetTicks() >= SpawnDelayDuration )
        {
            spawnDelayTimer.StopTimer();
            currentState = EnemyState.Moving;
            Debug.Log( "Enemy.SpawnMoveDelay(): Switched to Moving state" );
        }
    }
    
    // Updates whether this enemy is currently on the targetCropPos tile
    protected void SetIsOnTargetCrop()
    {
        IsOnTargetCrop = ( Mathf.Abs( transform.position.x - tileMap.GetPositionAtTile( targetCropPos ).x ) < 0.1f
            && Mathf.Abs( transform.position.z - tileMap.GetPositionAtTile( targetCropPos ).z ) < 0.1f );
    }
    
    // Updates whether this enemy is blocked by the player
    protected void SetIsBlocked()
    {
        TileCoordinate dist = GetDistanceFromTile( tileMap.GetPlayer().GetTilePosition() );
        SetMovingDirection();
        if( ( dist.CoordX == 1 && dist.CoordZ == 0 && currentDirection.Equals( Direction.Right ) )
            || ( dist.CoordX == -1 && dist.CoordZ == 0 && currentDirection.Equals( Direction.Left ) )
            || ( dist.CoordX == 0 && dist.CoordZ == 1 && currentDirection.Equals( Direction.Up ) )
            || ( dist.CoordX == 0 && dist.CoordZ == -1 && currentDirection.Equals( Direction.Down ) )
            || ( dist.CoordX == 0 && dist.CoordZ == 0 ) )
        {
            IsBlocked = true;

            currentState = EnemyState.Escaping;
            Debug.Log( "Enemy.CheckIsBlocked(): Switched to Escaping state" );
            targetMovePos = FindNearestExit();
        }
    }

    // Delays the destruction of the crop until the eatingTimer of the enemy 
    // reaches eatingDuration
    public void EatCrop()
    {
        eatingTimer.Update();

        // After crop is eaten, run away
        if( eatingTimer.GetTicks() >= EatingDuration )
        {
            // Not eating anymore, so stop the timer
            eatingTimer.StopTimer();

            // Set eaten crop's tile to be on cooldown
            tileMap.SetTile( targetCropPos, TileData.TileType.PlantableCooldown );

            currentState = EnemyState.Escaping;
            Debug.Log( "Enemy.EatCrop(): Switched to Escaping state" );
            targetMovePos = FindNearestExit();
        }
    }

    // Sets the movement direction of this enemy based on its 
    // currentTilePos and targetMovePos
    protected void SetMovingDirection()
    {
        bool isAtTargetCoordX = ( currentTilePos.CoordX == targetMovePos.CoordX );
        bool isAtTargetCoordZ = ( currentTilePos.CoordZ == targetMovePos.CoordZ );

        // Stop moving if on the target crop's tile
        if( isAtTargetCoordX && isAtTargetCoordZ )
        {
            currentDirection = Direction.None;
        }
        // Only on the same column as the target crop
        else if( isAtTargetCoordX )
        {
            // Above the target crop
            if( currentTilePos.CoordZ > targetMovePos.CoordZ )
            {
                currentDirection = Direction.Down;
            }
            // Below the target crop
            else
            {
                currentDirection = Direction.Up;
            }
        }
        // Only on the same row as the target crop
        else if( isAtTargetCoordZ )
        {
            // To the right of the target crop
            if( currentTilePos.CoordX > targetMovePos.CoordX )
            {
                currentDirection = Direction.Left;
            }
            // To the left of the target crop
            else
            {
                currentDirection = Direction.Right;
            }
        }
    }

    // Returns the relative distance in tiles from the enemy's currentTilePos 
    // to a given tile position
    public TileCoordinate GetDistanceFromTile( TileCoordinate tile )
    {
        TileCoordinate dist;
        dist.CoordX = tile.CoordX - currentTilePos.CoordX;
        dist.CoordZ = tile.CoordZ - currentTilePos.CoordZ;
        return dist;
    }

    // Checks the enemy's currentTilePos and compares it with the tilemap's 
    // dimensions to determine the closest exit from the map
    protected TileCoordinate FindNearestExit()
    {
        // Tile coordinates of the closest exit tile
        int closestX;
        int closestZ;

        // Distance from current position to the closest exit tile in 
        // tile coordinates
        int distX;
        int distZ;

        // Compare position on the X axis
        if( currentTilePos.CoordX < maxTileMapSize.CoordX - 1 - currentTilePos.CoordX )
        {
            closestX = -1;
            distX = currentTilePos.CoordX;
        }
        else
        {
            closestX = maxTileMapSize.CoordX;
            distX = maxTileMapSize.CoordX - 1 - currentTilePos.CoordX;
        }

        // Compare position on the Z axis
        if( currentTilePos.CoordZ < maxTileMapSize.CoordZ - 1 - currentTilePos.CoordZ )
        {
            closestZ = -1;
            distZ = currentTilePos.CoordZ;
        }
        else
        {
            closestZ = maxTileMapSize.CoordZ;
            distZ = maxTileMapSize.CoordZ - 1 - currentTilePos.CoordZ;
        }

        // Compare the closest x and y and return the closer exit
        if( distX < distZ )
        {
            return new TileCoordinate( closestX, currentTilePos.CoordZ );
        }
        else
        {
            return new TileCoordinate( currentTilePos.CoordX, closestZ );
        }
    }

    // Check this enemy's currentPosition with the edges of the map to 
    // determine if the enemy has reached a despawn zone
    public bool CheckIsOnDespawnTile()
    {
        // Reaching x-axis despawn zone
        if( currentTilePos.CoordX < 0 ||
            currentTilePos.CoordX > maxTileMapSize.CoordX - 1 )
        {
            return true;
        }

        // Reaching z-axis despawn zone
        if( currentTilePos.CoordZ < 0 ||
            currentTilePos.CoordZ > maxTileMapSize.CoordZ - 1 )
        {
            return true;
        }

        return false;
    }

    // Move the enemy based on its movement behaviour/AI
    public abstract void Move();

    // Run away to the edge of the map
    public abstract void RunAway();

    // Call necessary methods when cleaning up
    public abstract void CleanUp();
}