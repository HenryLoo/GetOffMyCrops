
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IEntity
{
    ///<summary>( local reference to the tile map instance )</summary>
    protected TileMap _tileMap;
    ///<summary>( the maximum tilemap size )</summary>
    protected TileCoordinate _maxTileMapSize;

    ///<summary>( The enemy's spawn coordinates within the tile map )</summary>
    protected TileCoordinate _spawnTilePos;
    ///<summary>( The enemy's current coordinates within the tile map )</summary>
    protected TileCoordinate _curTilePos;
    ///<summary>( The enemy's previous coordinates within the tile map )</summary>
    protected TileCoordinate _lastTilePos;

    ///<summary>( The enemy's target crop to eat )</summary>
    protected TileCoordinate _targetCropPos;
    ///<summary>( The tile the enemy will move towards )</summary>
    protected TileCoordinate _targetMovePos;

    ///<summary>( The duration of time in seconds it takes for this enemy to begin movement )</summary>
    public float _spawnDelayDuration;
    protected GameTimer _spawnDelayTimer;

    ///<summary>( The duration of time in seconds it takes for this enemy to damage a crop )</summary>
    public float _eatingDuration;
    protected GameTimer _eatingTimer;

    ///<summary>( if the player can block this enemy )</summary>
    public bool _canBeBlocked = false;
    ///<summary>( if the enemy is being blocked )</summary>
    public bool _isBlocked = false;

    ///<summary>( if the enemy has reached the target crop or not )</summary>
    public bool _isOnTargetCrop = false;

    ///<summary>( how quickly the enemy will move between tiles )</summary>
    public float _movementSpeed;

    ///<summary>( The enemy's possible movement directions )</summary>
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
    ///<summary>( The enemy's current movement direction )</summary>
    protected Direction _currentDirection;

    ///<summary>( The enemy's possible states of activity )</summary>
    public enum EnemyState
    {
        Spawning,
        Moving,
        Eating,
        Escaping,
        Despawning
    }
    ///<summary>( The enemy's current state of activity )</summary>
    protected EnemyState _currentState;


    ////////////////////////////////////////////////////////////////

    /// <summary> 
    /// initialise universal enemy variables 
    /// </summary>
    protected void InitEnemy()
    {
        _tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();

        _curTilePos = _tileMap.GetTileAtPosition(transform.position);
        _spawnTilePos = _curTilePos;
        _lastTilePos = _curTilePos;
        _targetMovePos = _targetCropPos;
    }
    void Start()
    {
        InitEnemy();
    }

    /// <summary> 
    /// Update the enemy's behaviour every frame
    /// </summary>
    public void Update()
    {
        switch ( _currentState )
        {
            case EnemyState.Spawning:
                SpawnMoveDelay();

                break;
            case EnemyState.Moving:       
                if (_isOnTargetCrop)
                {
                    _eatingTimer.StartTimer();
                    _currentState = EnemyState.Eating;
                    Debug.Log("ENEMY SWITCHED TO EATING");
                }
                else
                {
                    _isBlocked = false;
                    if ( _canBeBlocked )
                    {
                        CheckIsBlocked();
                    }                    
                    if (!_isBlocked)
                    {
                        Move();
                        CheckIsOnTargetCrop();
                    }
                }

                break;
            case EnemyState.Eating:
                _isBlocked = false;
                if (_canBeBlocked)
                {
                    CheckIsBlocked();
                }
                if (!_isBlocked)
                { 
                    EatCrop();
                }

                break;
            case EnemyState.Escaping:
                _targetMovePos = FindNearestExit();
                if (CheckIsOnDespawnTile())
                {
                    _currentState = EnemyState.Despawning;
                }else
                {
                    RunAway();
                }

                break;
            case EnemyState.Despawning:
                CleanUp();

                break;
        }
    }

    ////////////////////////////////////////////////////////////////
    

    /// <summary>
    ///  Set the crop at this tile to be the enemy's target crop
    /// </summary>
    public void SetTargetCrop( TileCoordinate crop )
    {        
        _targetCropPos = crop;
        //Debug.Log("enemy SET targetCrop: " + _targetCropPos.CoordX + ", " + _targetCropPos.CoordZ);
    }

    /// <summary>
    /// Finds the bounds of the map
    /// </summary>
    public void SetMapBoundary(TileCoordinate tilemapSize)
    {
        _maxTileMapSize = tilemapSize;
    }

    /// <summary>
    /// Delays the movement of the enemy until _spawnDelayTimer matches _spawnDelayDuration
    /// -> Sets _currentState to Moving when this occurs. 
    /// </summary>
    public void SpawnMoveDelay()
    {
        _spawnDelayTimer.Update();
        if (_spawnDelayTimer.GetTicks() >= _spawnDelayDuration)
        {
            _spawnDelayTimer.StopTimer();
            _currentState = EnemyState.Moving;
            Debug.Log("ENEMY SWITCHED TO MOVING");
        }
    }

    /// <summary>
    /// Checks if the enemy is currently on the _targetCropPos
    /// -> Sets _isOnTargetCrop accordingly
    /// </summary>
    protected void CheckIsOnTargetCrop()
    {
        _isOnTargetCrop = (Mathf.Abs(transform.position.x - _tileMap.GetPositionAtTile(_targetCropPos).x) < 0.1f 
            && Mathf.Abs(transform.position.z - _tileMap.GetPositionAtTile(_targetCropPos).z) < 0.1f);
    }

    /// <summary>
    /// Checks if the enemy is blocked by the player, or touched by the player
    /// -> Sets _currentState to Escaping when this occurs and _isBlocked to true
    /// </summary>
    protected void CheckIsBlocked()
    {
        TileCoordinate dist = GetDistanceFromTile(_tileMap.GetPlayer().GetTilePosition());
        GetMovingDirection();
        if ((dist.CoordX == 1 && dist.CoordZ == 0 && _currentDirection.Equals(Direction.Right))
            || (dist.CoordX == -1 && dist.CoordZ == 0 && _currentDirection.Equals(Direction.Left))
            || (dist.CoordX == 0 && dist.CoordZ == 1 && _currentDirection.Equals(Direction.Up))
            || (dist.CoordX == 0 && dist.CoordZ == -1 && _currentDirection.Equals(Direction.Down))
            || (dist.CoordX == 0 && dist.CoordZ == 0)
            )
        {
            _isBlocked = true;

            _currentState = EnemyState.Escaping;
            Debug.Log("ENEMY SWITCHED TO ESCAPING AFTER BLOCKED");
            _targetMovePos = FindNearestExit();         
        }        
    }

    /// <summary>
    /// Delays the destruction of the crop until the _eatingTimer of the enemy matches _eatingDuration
    /// -> Sets _currentState to Escaping when this occurs and the eaten crop changes to a PlantableCooldown state
    /// </summary>
    public void EatCrop()
    {
        _eatingTimer.Update();
        // After crop is eaten, run away
        if (_eatingTimer.GetTicks() >= _eatingDuration)
        {
            _tileMap.SetTile(_targetCropPos, TileData.TileType.PlantableCooldown);
            _eatingTimer.StopTimer();

            _currentState = EnemyState.Escaping;
            Debug.Log("ENEMY SWITCHED TO ESCAPING AFTER EATING");
            _targetMovePos = FindNearestExit();
        }
    }

    /// <summary>
    /// Checks the Movement direction of the enemy based on its _curTilePos and _targetMovePos 
    /// -> Sets _currentDirection accordingly
    /// </summary>
    protected void GetMovingDirection()
    { 
        if (_curTilePos.CoordX == _targetMovePos.CoordX && _curTilePos.CoordZ == _targetMovePos.CoordZ)
        {
            _currentDirection = Direction.None;
        }
        if (_curTilePos.CoordX == _targetMovePos.CoordX)
        {
            if (_curTilePos.CoordZ > _targetMovePos.CoordZ)
            {
                _currentDirection = Direction.Down;
            }
            else
            {
                _currentDirection = Direction.Up;
            }
        }
        if (_curTilePos.CoordZ == _targetMovePos.CoordZ)
        {
            if (_curTilePos.CoordX > _targetMovePos.CoordX)
            {
                _currentDirection = Direction.Left;
            }
            else
            {
                _currentDirection = Direction.Right;
            }
        }
    }

    /// <summary>
    /// returns the relative distance in tiles from the enemy's _curTilePos to a given tile position
    /// </summary>
    public TileCoordinate GetDistanceFromTile( TileCoordinate tile )
    {
        TileCoordinate dist;
        dist.CoordX = tile.CoordX - _curTilePos.CoordX;
        dist.CoordZ = tile.CoordZ - _curTilePos.CoordZ;
        return dist;
    }

    /// <summary>
    /// checks the enemy's _curTilePos and compares it with the tilemaps dimensions to determine the closest exit from the map.
    /// -> returns the TileCoordinate of the closest available exit
    /// </summary>
    protected TileCoordinate FindNearestExit()
    {
        int closestX;
        int closestZ;
        int distX;
        int distZ;
        // compare position on the X axis
        if (_curTilePos.CoordX < _maxTileMapSize.CoordX - 1 - _curTilePos.CoordX)
        {
            closestX = -1;
            distX = _curTilePos.CoordX;
        }
        else
        {
            closestX = _maxTileMapSize.CoordX;
            distX = _maxTileMapSize.CoordX - 1 - _curTilePos.CoordX;
        }
        // compare position on the Z axis
        if (_curTilePos.CoordZ < _maxTileMapSize.CoordZ - 1 - _curTilePos.CoordZ)
        {
            closestZ = -1;
            distZ = _curTilePos.CoordZ;
        }
        else
        {
            closestZ = _maxTileMapSize.CoordZ;
            distZ = _maxTileMapSize.CoordZ - 1 - _curTilePos.CoordZ;
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

    /// <summary>
    /// checks the enemy's _curPosition with the edges of the map to determine if the enemy has reached a despawn zone
    /// -> returns true if this occurs, false otherwise
    /// </summary>
    public bool CheckIsOnDespawnTile()
    {
        if (_curTilePos.CoordX < 0 || _curTilePos.CoordX > _maxTileMapSize.CoordX-1)
        {
            return true;
        }
        if (_curTilePos.CoordZ < 0 || _curTilePos.CoordZ > _maxTileMapSize.CoordZ-1)
        {
            return true;
        }
        return false;
    }


    // Move the enemy based on its movement behaviour/AI
    public abstract void Move();



    public abstract void RunAway();

    public abstract void CleanUp();

}