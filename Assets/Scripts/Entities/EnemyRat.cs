using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference to the tile map instance
public class EnemyRat : Enemy {

    protected void InitEnemyRat()
    {
        SpawnDelayDuration = 1; // time in seconds it takes for this enemy to begin movement
        EatingDuration = 5;    // time in seconds it takes for this enemy to damage a crop
        CanBeBlocked = true;    // if the player can block this enemy
        MovementSpeed = 1;      // how quickly the enemy will move between tiles
    }

    private void Start()
    {
        InitEnemy();
        InitEnemyRat();
    }
    
    public override void CleanUp()
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        // TODO: fix movement when enemy spawns from the right or top of the map, so that it ends up ontop of the crop instread of next to it
        bool onNewTile = false;
        if (!_curTilePos.Equals(_lastTilePos))
        {
            Debug.Log("ENEMY LAST POS: " + _lastTilePos.CoordX + ", " + _lastTilePos.CoordZ);
            Debug.Log("ENEMY MOVING FROM:" + _curTilePos.CoordX + ", " + _curTilePos.CoordZ + " - TO  CROP:" + _targetCropPos.CoordX + ", " + _targetCropPos.CoordZ);
            _lastTilePos = _curTilePos;
            onNewTile = true;
        }
        // move in the direction of the target crop by the set speed.
        float step = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _tileMap.GetPositionAtTile(_targetCropPos), step);
        // update current tile position of enemy.
        _curTilePos = _tileMap.GetTileAtPosition(transform.position);
        //Debug.Log("--AFTER MOVE POS: " + _curTilePos.CoordX + ", " + _curTilePos.CoordZ);
        if (onNewTile)
        {
            Debug.Log("ON NEW TILE POS: " + _curTilePos.CoordX + ", " + _curTilePos.CoordZ);
            isOnTargetCrop = (_curTilePos.CoordX == _targetCropPos.CoordX &&
                    _curTilePos.CoordZ == _targetCropPos.CoordZ);
            Debug.Log("IS CURRENTLY ON TARGET CROP: " + isOnTargetCrop);
        }
    }

    public override void RunAway()
    {
        // move in the direction of the nearest exit by the set speed.
        float step = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _tileMap.GetPositionAtTile(_targetMovePos), step);
        // update current tile position of enemy.
        _curTilePos = _tileMap.GetTileAtPosition(transform.position);
    }



}
