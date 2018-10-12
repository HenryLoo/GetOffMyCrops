using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int _positionX; // current position on Tile, the index of Tile Map
    private int _positionZ;

    private TileMap tileMap; // tileMap instance;

    private bool movingLock = false; // no key response when player is moving

    private Vector3 target; // move target position

    public int speed = 1;

    // Use this for initialization
    void Start()
    {
        tileMap = GameObject.Find("TileMap").GetComponent<TileMap>(); ;
        Vector2 tileIndex = tileMap.GetTileAtPosition(transform.position);
        _positionX = (int)tileIndex.x;
        _positionZ = (int)tileIndex.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!movingLock)
        {
            Move();
            if (Input.GetAxis("Fire1") > 0)
            {
                Plant();
            }
            if (Input.GetAxis("Fire2") > 0)
            {
                Harvest();
            }
            if (Input.GetAxis("Fire3") > 0)
            {
                Scare();
            }
        }
        else
        {
            KeepMoving();
        }
    }

    // Detect move, lock keys once start move
    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (Mathf.Abs(moveX) > Mathf.Abs(moveZ))
        {
            int to = _positionX + (moveX > 0 ? 1 : -1);
            if (to >= 0 && to < tileMap.GetSizeX()) // if reach the edge
            {
                _positionX = to;
                this.target = tileMap.GetPositionAtTile(_positionX, _positionZ);
                this.target.y = transform.position.y;
                this.movingLock = true;
            }

        }
        else if (Mathf.Abs(moveZ) > 0)
        {
            int to = _positionZ + (moveZ > 0 ? 1 : -1);
            if (to >= 0 && to < tileMap.GetSizeZ()) // if reach the edge
            {
                _positionZ = to;
                this.target = tileMap.GetPositionAtTile(_positionX, _positionZ);
                this.target.y = transform.position.y;
                this.movingLock = true;
            }
        }

    }

    // Moving: update per Frame
    // unlock when reach the target position
    void KeepMoving()
    {
        if (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
        else
        {
            this.movingLock = false;
        }
    }

    // Plant a crop on the tile that the player is standing on, if that tile’s type is plantable
    // Decreases the player’s money by a small amount as an initial investment
    void Plant()
    {
        if (Money.Get() < Money.SEED_BUY_PRICE) // if money is not enough 
        {
            //TODO: show tips
            Debug.Log("No enough money");
            return;
        }
        TileData.TileType type = tileMap.GetTile(_positionX, _positionZ); // if tile is not plantable 
        if (type != TileData.TileType.Plantable)
        {
            //TODO: show tips
            Debug.Log("Tile is not Plantable");
            return;
        }

        // Plant
        tileMap.SetTile(this._positionX, this._positionZ, TileData.TileType.CropSeed);

        Money.Add(-Money.SEED_BUY_PRICE);

    }

    // Remove a mature crop from the tile that the player is standing on.
    // Increment the current money by a fixed amount (larger than the initial investment cost)
    // If an enemy is in the process of eating that crop, then that enemy runs away(toward the edge of the map)
    void Harvest()
    {
        TileData.TileType type = tileMap.GetTile(_positionX, _positionZ); // if tile is not plantable 
        if (type != TileData.TileType.CropMature)
        {
            //TODO: show tips
            Debug.Log("Crop is not yet Mature");
            return;
        }

        // Harvest
        tileMap.SetTile(this._positionX, this._positionZ, TileData.TileType.PlantableCooldown);

        Money.Add(Money.CROP_SELL_PRICE);
    }

    // Interrupts an enemy if that enemy is in the process of eating a crop
    // That enemy will run away(toward the edge of the map)
    void Scare()
    {

    }
}
