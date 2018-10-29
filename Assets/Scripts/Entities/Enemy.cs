
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IEntity
{
    // Reference to the tile map instance
    protected TileMap _tileMap;

    // The enemy's spawn position on the tile map
    protected TileCoordinate _spawnTilePos;
    // The enemy's current position on the tile map
    protected TileCoordinate _curTilePos;
    // The enemy's target crop to eat
    protected TileCoordinate _targetCropPos;

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

    // The enemy's movement direction
    protected bool _isMovingUp = false, _isMovingDown = false,
        _isMovingLeft = false, _isMovingRight = false;

    public enum EnemyState
    {
        Spawning,
        Moving,
        Eating,
        Escaping
    }
    // The enemy's current state
    protected EnemyState _currentState;

    protected void InitEnemy()
    {
        _tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
        _eatingTimer = new GameTimer();

        _curTilePos = _tileMap.GetTileAtPosition(transform.position);
        _spawnTilePos = _curTilePos;

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
        // Debug.Log("START UPDATE");
        switch ( _currentState )
        {
            case EnemyState.Spawning:
                StartMoving();
                break;
            case EnemyState.Moving:
                Debug.Log("IM IN A MOVING CASE");
                Move();
// REMOVE
_currentState = EnemyState.Spawning;   
// REMOVE
                // Enemy is on top of the crop, so start eating
                if (_curTilePos.CoordX == _targetCropPos.CoordX &&
                    _curTilePos.CoordZ == _targetCropPos.CoordZ )
                {
                    Debug.Log("ENEMY IS ON A CROP SO START EATING");
                    _eatingTimer.StartTimer();
                    _currentState = EnemyState.Eating;
                    Debug.Log("IM SWITCHED TO EATING");
                }
                else
                {                    
                    // Check if enemy is being blocked
                    if ( CanBeBlocked )
                    {
                        TileCoordinate dist = GetDistanceFromTile( 
                            _tileMap.GetPlayer().GetTilePosition() );
                        if( ( dist.CoordX == 1 && dist.CoordZ == 0 && _isMovingRight ) ||
                            ( dist.CoordX == -1 && dist.CoordZ == 0 && _isMovingLeft ) ||
                            ( dist.CoordX == 0 && dist.CoordZ == 1 && _isMovingUp ) ||
                            ( dist.CoordX == 0 && dist.CoordZ == -1 && _isMovingDown ) )
                        {
                            Debug.Log("ENEMY IS BLOCKED");
                            _currentState = EnemyState.Escaping;
                            Debug.Log("IM SWITCHED TO ESCAPING");
                        }
                    }
                    // Enemy is not at its target yet, keep moving  !=! REDUNDANT AREA !=!
                    else
                    {
                        Debug.Log("ENEMY IS NOT BLOCKED");
                        //_currentState = EnemyState.Moving;
                    }
                }
                break;
            case EnemyState.Eating:
                Debug.Log("IM AN EATING CASE");
                DamageCrop();
                break;
            case EnemyState.Escaping:
                Debug.Log("IM A RUNNING AWAY CASE");
                RunAway();
                break;
        }
        //Debug.Log("END UPDATE");
    }
    
    // Set the crop at this tile to be the enemy's target
    public void SetTargetCrop( TileCoordinate crop )
    {        
        _targetCropPos = crop;
        Debug.Log("enemy SET targetCrop: " + _targetCropPos.CoordX + ", " + _targetCropPos.CoordZ);
    }

    // Get the relative distance in tiles from a given tile position
    public TileCoordinate GetDistanceFromTile( TileCoordinate tile )
    {
        TileCoordinate dist;
        dist.CoordX = tile.CoordX - _curTilePos.CoordX;
        dist.CoordZ = tile.CoordZ - _curTilePos.CoordZ;

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

    public void StopMoving()
    {

    }

    // Move the enemy based on its movement behaviour/AI
    public abstract void Move();

    // Escape off the map
    public abstract void RunAway();

    public abstract void CleanUp();

}