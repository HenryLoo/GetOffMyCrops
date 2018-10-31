
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
    public int MovementSpeed;

    public bool isOnTargetCrop = false;
    // The enemy's movement direction
    protected bool _isMovingUp = false, _isMovingDown = false,
        _isMovingLeft = false, _isMovingRight = false;

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
                    {// Enemy is not at its target yet, keep moving  !=! REDUNDANT AREA !=!
                        //Debug.Log("ENEMY IS NOT BLOCKED");
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
                }
                else
                {
                    RunAway();
                }
                break;
            case EnemyState.Despawning:
                // DESTROY ENEMY ENTITY
                break;
        }
        //Debug.Log("END ENEMY UPDATE");
    }
    
    // Set the crop at this tile to be the enemy's target
    public void SetTargetCrop( TileCoordinate crop )
    {        
        _targetCropPos = crop;
        Debug.Log("enemy SET targetCrop: " + _targetCropPos.CoordX + ", " + _targetCropPos.CoordZ);
    }
    public void SetMapBoundary(TileCoordinate tilemapSize)
    {
        _MaxTileMapSize = tilemapSize;
    }

    // checks if the enemy entity is outside of the tilemap
    public bool IsOnDespawnTile()
    {
        if (_curTilePos.CoordX < 0 || _curTilePos.CoordX > _MaxTileMapSize.CoordX)
        {
            return true;
        }
        if (_curTilePos.CoordZ < 0 || _curTilePos.CoordZ > _MaxTileMapSize.CoordZ)
        {
            return true;
        }
        return false;
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

    //finds the nearest exit to the enemy
    protected TileCoordinate FindNearestExit()
    {

 // TODO: NEEDS TO BE MODIFIED SO THAT IT IS GOING IN A PROPER DIRECTION
        int closestX;
        int closestZ;
        if (_curTilePos.CoordX < _MaxTileMapSize.CoordX - _curTilePos.CoordX)
        {
            closestX = -1;
        }
        else
        {
            closestX = _MaxTileMapSize.CoordX + 1;
        }
        if (_curTilePos.CoordZ < _MaxTileMapSize.CoordZ - _curTilePos.CoordZ)
        {
            closestZ = -1;
        }
        else
        {
            closestZ = _MaxTileMapSize.CoordZ + 1;
        }
        if (closestX < closestZ)
        {
            return new TileCoordinate(closestX, _curTilePos.CoordZ);
        }
        else
        {
            return new TileCoordinate(_curTilePos.CoordZ, closestZ);
        }
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

    // Move the enemy based on its movement behaviour/AI
    public abstract void Move();

    // Escape off the map
    public abstract void RunAway();

    public abstract void CleanUp();

}