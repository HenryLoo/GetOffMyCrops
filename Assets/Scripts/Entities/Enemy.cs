
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IEntity
{
    // Reference to the tile map instance
    protected TileMap _tileMap;

    protected TileCoordinate _MaxTileMapSize;

    protected GameController GameController;

    // The enemy's spawn position on the tile map
    protected TileCoordinate _spawnTilePos;
    // The enemy's current position on the tile map
    protected TileCoordinate _curTilePos;
    // The enemy's previous position on the tile map
    protected TileCoordinate _lastTilePos;
    // The enemy's target crop to eat
    protected TileCoordinate _targetCropPos;
    // The enemy's target tile to move towards
    protected TileCoordinate _targetMovePos;

    // The duration of time in seconds it takes for this enemy to begin movement
    public float SpawnDelayDuration;
    protected GameTimer _spawnDelayTimer;
    protected bool freshSpawn = true;

    // The duration of time in seconds it takes for this enemy to damage a crop
    public float EatingDuration;
    protected GameTimer _eatingTimer;

    // Flag for if the player can block this enemy
    public bool CanBeBlocked;

    // This defines how quickly the enemy will move between tiles
    public float MovementSpeed;

    //whether the enemy has reached the target crop or not
    public bool isOnTargetCrop = false;

    // The enemy's possible movement directions
    protected bool _isMovingUp = false, _isMovingDown = false,
        _isMovingLeft = false, _isMovingRight = false;
    public enum Direction
    {
        none,
        up,
        down,
        left,
        right
    }
    // The enemy's current movement direction
    protected Direction _currentDirection;

    // the enemy's possible states of activity
    public enum EnemyState
    {
        Spawning,
        Moving,
        Eating,
        Escaping,
        Despawning
    }
    // The enemy's current state
    protected EnemyState _currentState;

    ////////////////////////////////////////////////////////////////

    // initialise enemy variables
    protected void InitEnemy()
    {
        _tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
        _eatingTimer = new GameTimer();

        _curTilePos = _tileMap.GetTileAtPosition(transform.position);
        _spawnTilePos = _curTilePos;
        _lastTilePos = _spawnTilePos;

        _targetMovePos = _targetCropPos;

        _spawnDelayTimer = new GameTimer();
        _spawnDelayTimer.StartTimer();
    }

    void Start()
    {
        InitEnemy();
    }
       
    // Update the enemy's behaviour
    // This should be called every frame
    public void Update()
    {
        // Debug.Log("START ENEMY UPDATE");
        switch ( _currentState )
        {
            case EnemyState.Spawning:
                StartMoving();
                break;
            case EnemyState.Moving:               
                               
                if (isOnTargetCrop)
                { // Enemy is on top of the crop, so start eating
                    Debug.Log("ENEMY IS ON A CROP SO START EATING");
                    _eatingTimer.StartTimer();
                    _currentState = EnemyState.Eating;
                    Debug.Log("IM SWITCHED TO EATING");
                }
                else
                {
                    bool blocked = false;
                    if ( CanBeBlocked )
                    {// Check if enemy is being blocked
                        TileCoordinate dist = GetDistanceFromTile( 
                            _tileMap.GetPlayer().GetTilePosition() );

                        // TODO blocked by player behaviour

                        //if( ( dist.CoordX == 1 && dist.CoordZ == 0 && _isMovingRight ) ||
                        //    ( dist.CoordX == -1 && dist.CoordZ == 0 && _isMovingLeft ) ||
                        //    ( dist.CoordX == 0 && dist.CoordZ == 1 && _isMovingUp ) ||
                        //    ( dist.CoordX == 0 && dist.CoordZ == -1 && _isMovingDown ) )
                        //{
                        //    blocked = true;
                        //    _currentState = EnemyState.Escaping;
                        //    Debug.Log("IM SWITCHED TO ESCAPING");
                        //}
                    }                    
                    if (!blocked)
                    {// Enemy is not at its target yet, keep moving
                        Move();
                        _currentState = EnemyState.Moving;
                    }
                }
                break;
            case EnemyState.Eating:
                //Debug.Log("IM AN EATING CASE");
                DamageCrop();
                break;
            case EnemyState.Escaping:
                //Debug.Log("IM AN ESCAPING CASE");                
                if (IsOnDespawnTile())
                {
                    _currentState = EnemyState.Despawning;
                }else
                {
                    RunAway();
                }
                break;
            case EnemyState.Despawning:
                // DESTROY ENEMY ENTITY
                CleanUp();
                break;
        }
        //Debug.Log("END ENEMY UPDATE");
    }

    ////////////////////////////////////////////////////////////////

    // Set the crop at this tile to be the enemy's target
    public void SetTargetCrop( TileCoordinate crop )
    {        
        _targetCropPos = crop;
        //Debug.Log("enemy SET targetCrop: " + _targetCropPos.CoordX + ", " + _targetCropPos.CoordZ);
    }

    // finds the bounds of the map
    public void SetMapBoundary(TileCoordinate tilemapSize)
    {
        _MaxTileMapSize = tilemapSize;
    }

    // Movement delaying after the enemy has spawned
    public void StartMoving()
    {
        //Debug.Log("delay Timer: " + _spawnDelayTimer.GetTicks());
        _spawnDelayTimer.Update();

        if (_spawnDelayTimer.GetTicks() >= SpawnDelayDuration)
        {
            _spawnDelayTimer.StopTimer();
            _currentState = EnemyState.Moving;

            Debug.Log("IM SWITCHED TO MOVING");
        }
    }

    private void CheckIsOnTargetCrop()
    {
        //TODO modify isontargetcrop so that top and right spawning enemies will be declared true afterwards

    }

    private void CheckMovingDirection()
    {
        // TODO IMPLEMENT DIRECTION IF STATEMENTS
        _currentDirection = Direction.up;
        _currentDirection = Direction.down;
        _currentDirection = Direction.left;
        _currentDirection = Direction.right;
        _currentDirection = Direction.none;

    }

    // Get the relative distance in tiles from a given tile position
    public TileCoordinate GetDistanceFromTile( TileCoordinate tile )
    {
        TileCoordinate dist;
        dist.CoordX = tile.CoordX - _curTilePos.CoordX;
        dist.CoordZ = tile.CoordZ - _curTilePos.CoordZ;

        //Debug.Log("DISTANCE Between PlayerAndEnemy: " + dist.CoordX + ", " + dist.CoordZ);

        return dist;
    }

    // Attempt to eat the targeted crop
    public void DamageCrop()
    {
        _eatingTimer.Update();

        // After crop is eaten, run away
        if( _eatingTimer.GetTicks() >= EatingDuration )
        {
            _tileMap.SetTile( _targetCropPos, TileData.TileType.PlantableCooldown );
            _eatingTimer.StopTimer();
            _currentState = EnemyState.Escaping;
            Debug.Log("IM SWITCHED TO ESCAPING AFTER EATING");
            _targetMovePos = FindNearestExit();
        }
    }

    //finds the nearest exit to the enemy
    protected TileCoordinate FindNearestExit()
    {
        int closestX;
        int closestZ;
        int distX;
        int distZ;
        // compare position on the X axis
        if (_curTilePos.CoordX < _MaxTileMapSize.CoordX - 1 - _curTilePos.CoordX)
        {
            closestX = -1;
            distX = _curTilePos.CoordX;
        }
        else
        {
            closestX = _MaxTileMapSize.CoordX;
            distX = _MaxTileMapSize.CoordX - 1 - _curTilePos.CoordX;
        }
        // compare position on the Z axis
        if (_curTilePos.CoordZ < _MaxTileMapSize.CoordZ - 1 - _curTilePos.CoordZ)
        {
            closestZ = -1;
            distZ = _curTilePos.CoordZ;
        }
        else
        {
            closestZ = _MaxTileMapSize.CoordZ;
            distZ = _MaxTileMapSize.CoordZ - 1 - _curTilePos.CoordZ;
        }
        // compare the closest x and y for closer exit
        if (distX < distZ)
        {
            return new TileCoordinate(closestX, _curTilePos.CoordZ);
        }
        else
        {
            return new TileCoordinate(_curTilePos.CoordX, closestZ);
        }
    }

    // checks if the enemy entity is outside of the tilemap
    public bool IsOnDespawnTile()
    {
        if (_curTilePos.CoordX < 0 || _curTilePos.CoordX > _MaxTileMapSize.CoordX-1)
        {
            return true;
        }
        if (_curTilePos.CoordZ < 0 || _curTilePos.CoordZ > _MaxTileMapSize.CoordZ-1)
        {
            return true;
        }
        return false;
    }


    // Move the enemy based on its movement behaviour/AI
    public abstract void Move();
    // Escape off the map
    public abstract void RunAway();

    public abstract void CleanUp();

}