using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference to the tile map instance
public class EnemyRat : Enemy {

    protected void InitEnemyRat()
    {
        SpawnDelayDuration = 1; // time in seconds it takes for this enemy to begin movement
        EatingDuration = 10;    // time in seconds it takes for this enemy to damage a crop
        CanBeBlocked = true;    // if the player can block this enemy
        MovementSpeed = 2;      // how quickly the enemy will move between tiles
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
        float step = MovementSpeed * Time.deltaTime;
        Debug.Log("ENEMY TARGET CROP: " + _targetCropPos.CoordX + ", " + _targetCropPos.CoordZ + " !!!!! VERY WRONG !!!!!");
        transform.position = Vector3.MoveTowards(transform.position, _tileMap.GetPositionAtTile(_targetCropPos), step);
    }

    public override void RunAway()
    {
        throw new System.NotImplementedException();
    }

}
