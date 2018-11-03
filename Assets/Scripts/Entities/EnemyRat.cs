using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference to the tile map instance
public class EnemyRat : Enemy {

    // initialise rat variables
    protected void InitEnemyRat()
    {
        SpawnDelayDuration = 1; // time in seconds it takes for this enemy to begin movement
        EatingDuration = 5;     // time in seconds it takes for this enemy to damage a crop
        CanBeBlocked = true;    // if the player can block this enemy
        MovementSpeed = 1f;      // how quickly the enemy will move between tiles
    }

    private void Start()
    {
        InitEnemy();
        InitEnemyRat();
        PopupMessageCreator.PopupMessage("Squeek, Squeek", transform);
    }
    
    public override void CleanUp()
    {
        Destroy(gameObject);
    }

    public override void Move()
    {
        // TODO: fix movement when enemy spawns from the right or top of the map, so that it ends up ontop of the crop instread of next to it
        // TODO: ensure that targetMovePos is used more regularly throughout the movement situations. 
        if (!_curTilePos.Equals(_lastTilePos))
        {
            Debug.Log("ENEMY LAST POS: " + _lastTilePos.CoordX + ", " + _lastTilePos.CoordZ);
            Debug.Log(" - ENEMY MOVING FROM:" + _curTilePos.CoordX + ", " + _curTilePos.CoordZ + " - TO  CROP:" + _targetCropPos.CoordX + ", " + _targetCropPos.CoordZ);
            _lastTilePos = _curTilePos;
        }
        // move in the direction of the target crop by the set speed.
        float step = MovementSpeed * Time.deltaTime;
        Vector3 targetVectorPosition = _tileMap.GetPositionAtTile(_targetCropPos);
        transform.position = Vector3.MoveTowards(transform.position, targetVectorPosition, step);
        // update current tile position of enemy.
        _curTilePos = _tileMap.GetTileAtPosition(transform.position);
        // check if the target crop has been reached
        isOnTargetCrop = (Mathf.Abs(transform.position.x - targetVectorPosition.x) < 0.1f && Mathf.Abs(transform.position.z - targetVectorPosition.z) < 0.1f);
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
