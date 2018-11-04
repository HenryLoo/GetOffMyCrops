using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference to the tile map instance
public class EnemyRat : Enemy {

    /// <summary> 
    /// initialise rat specific variables 
    /// </summary>
    protected void InitEnemyRat()
    {
        _spawnDelayDuration = 1;
        _spawnDelayTimer = new GameTimer();
        _spawnDelayTimer.StartTimer();

        _eatingDuration = 5;
        _eatingTimer = new GameTimer();

        _canBeBlocked = true;
        _movementSpeed = 1f;
    }
    private void Start()
    {
        InitEnemy();
        InitEnemyRat();
        PopupMessageCreator.PopupMessage("Squeek, Squeek", transform);
    }

    /// <summary>
    /// Moves the rat enemy on the tile map from its _curtilePos towards the _targetMovePos
    /// -> Sets the _lastTilePos before moving and the _curTilePos after moving
    /// </summary>
    private void MoveOnTileMap()
    {
        if (!_curTilePos.Equals(_lastTilePos))
        {
            Debug.Log(" - ENEMY MOVING FROM:" + _curTilePos.CoordX + ", " + _curTilePos.CoordZ + " - TO  TILE:" + _targetMovePos.CoordX + ", " + _targetMovePos.CoordZ);
            _lastTilePos = _curTilePos;
        }

        // move in the direction of the nearest exit by the set speed.
        float step = _movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _tileMap.GetPositionAtTile(_targetMovePos), step);

        // update current tile position of enemy.
        _curTilePos = _tileMap.GetTileAtPosition(transform.position);
    }

    public override void Move()
    {
        MoveOnTileMap();
    }

    public override void RunAway()
    {
        MoveOnTileMap();
    }

    public override void CleanUp()
    {
        Destroy(gameObject);
    }

}
