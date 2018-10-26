
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IEntity
{
    public enum EnemyState
    {
        Moving,
        Eating,
        Escaping
    }
    
    // This defines how quickly the enemy will move between tiles
    public int MovementSpeed;

    // Flag for if the player can block this enemy
    public bool CanBeBlocked;

    // The duration of time in seconds it takes for this enemy to damage a crop
    public float EatingDuration;
    private GameTimer _eatingTimer;

    // Reference to the player instance
    public Player Player;

    // Reference to the tile map instance
    public TileMap TileMap;


    static System.Random randomNo = new System.Random();

    // The enemy's current position on the tile map
    private TileCoordinate _tilePos;

    // The enemy will attempt to eat the crop at this tile
    private TileCoordinate _targetCrop;

    // The enemy's movement direction
    private bool _isMovingUp = false, _isMovingDown = false, 
        _isMovingLeft = false, _isMovingRight = false;

    // The enemy's current state
    private EnemyState _currentState;

    public Enemy()
    {
        _eatingTimer = new GameTimer();
        getRandomTargetCrop();
        _tilePos = SetRandomSpawnLocation();
    }

    // Update the enemy's behaviour
    // This should be called every frame
    public void Update()
    {
        switch( _currentState )
        {
            case EnemyState.Moving:
                // Enemy is on top of the crop, so start eating
                if( _tilePos.CoordX == _targetCrop.CoordX &&
                    _tilePos.CoordZ == _targetCrop.CoordZ )
                {
                    _eatingTimer.StartTimer();
                    _currentState = EnemyState.Eating;
                }
                else
                {
                    // Check if enemy is being blocked
                    if( CanBeBlocked )
                    {
                        TileCoordinate dist = GetDistanceFromTile( 
                            Player.GetTilePosition() );
                        if( ( dist.CoordX == 1 && dist.CoordZ == 0 && _isMovingRight ) ||
                            ( dist.CoordX == -1 && dist.CoordZ == 0 && _isMovingLeft ) ||
                            ( dist.CoordX == 0 && dist.CoordZ == 1 && _isMovingUp ) ||
                            ( dist.CoordX == 0 && dist.CoordZ == -1 && _isMovingDown ) )
                        {
                            _currentState = EnemyState.Escaping;
                        }
                    }
                    // Enemy is not at its target yet, keep moving
                    else
                    {
                        _currentState = EnemyState.Moving;
                    }
                }
                break;
            case EnemyState.Eating:
                DamageCrop();
                break;
            case EnemyState.Escaping:
                RunAway();
                break;
        }
    }

    public void getRandomTargetCrop()
    {        
        if (TileMap.currentPlantedCrops.Count != 0)
        {
            int randomTilePicked = randomNo.Next(TileMap.currentPlantedCrops.Count);
            SetTargetCrop(TileMap.currentPlantedCrops[randomTilePicked].Key);
        }
    }
    // Set the crop at this tile to be the enemy's target
    public void SetTargetCrop( TileCoordinate crop )
    {
        _targetCrop = crop;
    }

    public TileCoordinate SetRandomSpawnLocation()
    {
        int randomPicked = randomNo.Next(4);
        TileCoordinate spawnTile = new TileCoordinate();
        switch (randomPicked)
        {
            case 3:
                spawnTile = new TileCoordinate(_targetCrop.CoordX, TileMap.GetSizeZ());
                break;
            case 2:
                spawnTile = new TileCoordinate(_targetCrop.CoordX, 0);
                break;
            case 1:
                spawnTile = new TileCoordinate(TileMap.GetSizeX(), _targetCrop.CoordZ);
                break;
            default:
                spawnTile = new TileCoordinate(0, _targetCrop.CoordZ);
                break;
        }
        Debug.Log("Spawn Location [x:" + _targetCrop.CoordX + " z:" + _targetCrop.CoordZ + 
                "] - for Target Crop: [x:" + spawnTile.CoordX + " z:" + spawnTile.CoordZ + "]");

        return spawnTile;
    }

    // Get the relative distance in tiles from a given tile position
    public TileCoordinate GetDistanceFromTile( TileCoordinate tile )
    {
        TileCoordinate dist;
        dist.CoordX = tile.CoordX - _tilePos.CoordX;
        dist.CoordZ = tile.CoordZ - _tilePos.CoordZ;

        return dist;
    }

    // Attempt to eat the targeted crop
    public void DamageCrop()
    {
        _eatingTimer.Update();

        // After crop is eaten, run away
        if( _eatingTimer.GetTicks() >= EatingDuration )
        {
            TileMap.SetTile( _targetCrop, TileData.TileType.PlantableCooldown );
            _eatingTimer.StopTimer();
            _currentState = EnemyState.Escaping;
        }
    }

    // Move the enemy based on its movement behaviour/AI
    public abstract void Move();

    // Escape off the map
    public abstract void RunAway();

    public abstract void CleanUp();

}