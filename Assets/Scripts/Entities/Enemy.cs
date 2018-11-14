using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IEntity
{
    // Reference to the GameController instance
    protected GameController gameController;

    // Reference to the enemy animator
    protected Animator animator;

    // The enemy's current coordinates within the tile map
    protected TileCoordinate currentTilePos;

    // The tile that this enemy will move towards
    protected TileCoordinate targetFinalPos;

    // The next tile to move to, on the path towards targetFinalPos
    protected TileCoordinate targetNextPos;

    // Threshold value for determining if this enemy is on a tile while 
    // transitioning to that tile
    protected const float ON_TILE_THRESHOLD = 0.1f;

    // Handle timing for this enemy's current action
    // Example: delays, eating
    protected GameTimer actionTimer;

    // The duration of time in seconds it takes for this enemy to begin movement
    public float SpawnDelayDuration;

    // The duration of time in seconds it takes for this enemy to damage a crop
    public float EatingDuration;

    // Flag for if the player can block this enemy
    public bool CanBeBlocked;

    // Flag for if this enemy is being blocked
    protected bool isBlocked = false;

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
    protected virtual void InitEnemy()
    {
        gameController = GameObject.Find( "GameController" ).GetComponent<GameController>();

        currentTilePos = gameController.TileMap.GetTileAtPosition( transform.position );
        UpdateMovingDirection();
        OrientInDirection();
        MoveInDirection( currentDirection );

        // Start timer for spawn delay
        actionTimer = new GameTimer();
        actionTimer.StartTimer();

        animator = GetComponent<Animator>();
    }

    public virtual void Start()
    {
        InitEnemy();
    }

    // Update the enemy's behaviour every frame
    public void Update()
    {
        // If the game is paused, then pause the enemy's animations and
        // don't update movement
        bool isPaused = gameController.GetIsPaused();
        animator.enabled = !isPaused;
        if( isPaused ) return;

        actionTimer.Update();

        switch( currentState )
        {
            case EnemyState.Spawning:
                DelaySpawnMovement();
                break;

            case EnemyState.Moving:
                if( IsOnTargetTile() )
                {
                    HandleOnTarget();
                }
                else
                {
                    // Update whether this enemy has been blocked
                    if( CanBeBlocked )
                    {
                        UpdateIsBlocked();
                    }

                    HandleMoving();
                }

                break;

            case EnemyState.Eating:
                // Update whether this enemy has been blocked
                if( CanBeBlocked )
                {
                    UpdateIsBlocked();
                }

                // Keep eating if not blocked
                if( !isBlocked )
                {
                    HandleEating();
                }

                break;

            case EnemyState.Escaping:
                // Remove this enemy once it has escaped
                if( CheckIsOnDespawnTile() )
                {
                    currentState = EnemyState.Despawning;
                }
                // Otherwise, keep trying to escape
                else
                {
                    HandleEscaping();
                }

                break;

            case EnemyState.Despawning:
                CleanUp();
                break;
        }
    }

    // Delays the movement of this enemy until spawnDelayTimer reaches 
    // spawnDelayDuration
    public void DelaySpawnMovement()
    {
        // Set currentState to Moving when spawn delay is over
        if( actionTimer.GetTicks() >= SpawnDelayDuration )
        {
            actionTimer.StopTimer();
            currentState = EnemyState.Moving;
            Debug.Log( "Enemy.DelaySpawnMovement(): Switched to Moving state" );
            SetAnimationState();
        }
    }

    // Check whether this enemy is currently on the targetFinalPos tile
    protected bool IsOnTargetTile()
    {
        TileCoordinate dist = HelperFunctions.GetTileDistance( currentTilePos, targetFinalPos );
        return dist.CoordX == 0 && dist.CoordZ == 0;
    }

    // Updates whether this enemy is blocked by the player
    protected void UpdateIsBlocked()
    {
        // Get distance to player
        TileCoordinate dist = HelperFunctions.GetTileDistance( currentTilePos,
            gameController.TileMap.GetPlayer().GetTilePosition() );

        // If this enemy is being blocked by the player, then run away
        // If the enemy is eating, then it can only be blocked when the 
        // player is directly on top of it
        if( ( currentState != EnemyState.Eating && 
            ( ( dist.CoordX == 1 && dist.CoordZ == 0 && currentDirection == Direction.Right )
            || ( dist.CoordX == -1 && dist.CoordZ == 0 && currentDirection == Direction.Left )
            || ( dist.CoordX == 0 && dist.CoordZ == 1 && currentDirection == Direction.Up )
            || ( dist.CoordX == 0 && dist.CoordZ == -1 && currentDirection == Direction.Down ) ) ) ||
            ( currentState == EnemyState.Eating && dist.CoordX == 0 && dist.CoordZ == 0 ) )
        {
            isBlocked = true;
            RunAway();
        }
    }

    // Sets the movement direction of this enemy based on its 
    // currentTilePos and targetFinalPos
    protected void UpdateMovingDirection()
    {
        bool isAtTargetCoordX = ( currentTilePos.CoordX == targetFinalPos.CoordX );
        bool isAtTargetCoordZ = ( currentTilePos.CoordZ == targetFinalPos.CoordZ );

        // Stop moving if on the target crop's tile
        if( isAtTargetCoordX && isAtTargetCoordZ )
        {
            currentDirection = Direction.None;
        }
        // Only on the same column as the target crop
        else if( isAtTargetCoordX )
        {
            // Above the target crop
            if( currentTilePos.CoordZ > targetFinalPos.CoordZ )
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
            if( currentTilePos.CoordX > targetFinalPos.CoordX )
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

    // Checks the enemy's currentTilePos and compares it with the tilemap's 
    // dimensions to determine the closest exit from the map
    protected TileCoordinate FindNearestExit()
    {
        TileCoordinate mapSize = gameController.TileMap.GetSize();

        // Tile coordinates of the closest exit tile
        int closestX;
        int closestZ;

        // Distance from current position to the closest exit tile in 
        // tile coordinates
        int distX;
        int distZ;

        // Compare position on the X axis
        if( currentTilePos.CoordX < mapSize.CoordX - 1 - currentTilePos.CoordX )
        {
            closestX = -2;
            distX = currentTilePos.CoordX;
        }
        else
        {
            closestX = mapSize.CoordX + 1;
            distX = mapSize.CoordX - 1 - currentTilePos.CoordX;
        }

        // Compare position on the Z axis
        if( currentTilePos.CoordZ < mapSize.CoordZ - 1 - currentTilePos.CoordZ )
        {
            closestZ = -2;
            distZ = currentTilePos.CoordZ;
        }
        else
        {
            closestZ = mapSize.CoordZ + 1;
            distZ = mapSize.CoordZ - 1 - currentTilePos.CoordZ;
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
    protected bool CheckIsOnDespawnTile()
    {
        TileCoordinate mapSize = gameController.TileMap.GetSize();

        // Reaching x-axis despawn zone
        if( currentTilePos.CoordX < -1 ||
            currentTilePos.CoordX > mapSize.CoordX )
        {
            return true;
        }

        // Reaching z-axis despawn zone
        if( currentTilePos.CoordZ < -1 ||
            currentTilePos.CoordZ > mapSize.CoordZ )
        {
            return true;
        }

        return false;
    }

    // Set the targetFinalPos that this enemy should move toward
    public void SetTargetTile( TileCoordinate tile )
    {
        targetFinalPos = tile;
        UpdateMovingDirection();
    }

    // Set the next tile to the adjacent tile in the given direction
    // This will prepare the movement for update
    protected void MoveInDirection( Direction direction )
    {
        targetNextPos = currentTilePos;
        currentDirection = direction;

        switch( direction )
        {
            case Direction.Down:
                --targetNextPos.CoordZ;
                break;
            case Direction.Up:
                ++targetNextPos.CoordZ;
                break;
            case Direction.Left:
                --targetNextPos.CoordX;
                break;
            case Direction.Right:
                ++targetNextPos.CoordX;
                break;
        }
    }

    // Check if the target is a crop
    protected bool IsTargetACrop()
    {
        TileData.TileType targetType = gameController.TileMap.GetTile( targetFinalPos );
        return targetType == TileData.TileType.CropSeed ||
            targetType == TileData.TileType.CropGrowing ||
            targetType == TileData.TileType.CropMature;
    }

    // Get the enemy's position on the tile map
    public TileCoordinate GetTilePosition()
    {
        return currentTilePos;
    }

    // Rotate this enemy to face its direction of movement
    protected void OrientInDirection()
    {
        switch( currentDirection )
        {
            case Direction.Up:
                transform.rotation = Quaternion.Euler( 0, 0, 0 );
                break;

            case Direction.Down:
                transform.rotation = Quaternion.Euler( 0, 180, 0 );
                break;

            case Direction.Left:
                transform.rotation = Quaternion.Euler( 0, 270, 0 );
                break;

            case Direction.Right:
                transform.rotation = Quaternion.Euler( 0, 90, 0 );
                break;
        }
    }

    // Run away to the edge of the map
    public void RunAway()
    {
        Debug.Log( "Enemy.RunAway(): Switched to Escaping state" );
        currentState = EnemyState.Escaping;
        MovementSpeed++;

        // Interrupt this enemy's current path
        targetNextPos = currentTilePos;

        // Run towards nearest exit
        SetTargetTile( FindNearestExit() );
        SetAnimationState();
    }

    // Animate the enemy based on its current state
    protected abstract void SetAnimationState();

    // Handle how this enemy should behave once it has reached its target
    protected abstract void HandleOnTarget();

    // Move the enemy based on its movement behaviour/AI
    protected abstract void HandleMoving();

    // Move the enemy to its next tile along the path to the target tile
    protected void MoveToNextTile()
    {
        // Rotate the model to face direction of movement
        OrientInDirection();

        // Translate this enemy's world coordinates toward the next tile,
        // while retaining this enemy's height (y-position)
        float step = MovementSpeed * Time.deltaTime;
        Vector3 nextPos = gameController.TileMap.GetPositionAtTile( targetNextPos );
        nextPos.y = transform.position.y;
        transform.position = Vector3.MoveTowards( transform.position,
           nextPos, step );

        // Update current tile position once the rat has fully transitioned 
        // onto the next tile, while retaining this enemy's height (y-position)
        Vector3 targetPos = gameController.TileMap.GetPositionAtTile( targetNextPos );
        targetPos.y = transform.position.y;
        if( Math.Abs( transform.position.x - targetPos.x ) < ON_TILE_THRESHOLD &&
            Math.Abs( transform.position.z - targetPos.z ) < ON_TILE_THRESHOLD )
        {
            currentTilePos = targetNextPos;

            // Set the next tile to move to if not at target yet
            if( !currentTilePos.Equals( targetFinalPos ) )
            {
                MoveInDirection( currentDirection );

                Debug.Log( "Enemy.MoveToNextTile(): Current position: (" +
                    currentTilePos.CoordX + ", " + currentTilePos.CoordZ +
                    "), Next position: (" + targetNextPos.CoordX + ", " +
                    targetNextPos.CoordZ +
                    "), Target position: (" + targetFinalPos.CoordX + ", " +
                    targetFinalPos.CoordZ + ")" );
            }
        }
    }

    // Attempt to destroy the target crop
    protected abstract void HandleEating();

    // Attempt to escape the map
    protected abstract void HandleEscaping();

    // Call necessary methods when cleaning up
    public virtual void CleanUp()
    {
        actionTimer.StopTimer();
        Destroy( gameObject );
    }
}