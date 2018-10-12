using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Current position on the TileMap
    private TileCoordinate _tilePos;

    // Reference to the TileMap instance;
    private TileMap _tileMap;

    // Don't read key input while player is moving
    private bool _movingLock = false;

    // End position of a movement
    private Vector3 _moveTargetPos;

    public int Speed = 2;

    // Use this for initialization
    void Start()
    {
        _tileMap = GameObject.Find( "TileMap" ).GetComponent<TileMap>();
        _tilePos = _tileMap.GetTileAtPosition( transform.position );
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if( !_movingLock )
        {
            Move();
            if( Input.GetAxis( "Fire1" ) > 0 )
            {
                Plant();
            }
            if( Input.GetAxis( "Fire2" ) > 0 )
            {
                Harvest();
            }
            if( Input.GetAxis( "Fire3" ) > 0 )
            {
                Scare();
            }
        }
        else
        {
            KeepMoving();
        }
    }

    // Detect input for movement and lock input reading once movement 
    // has started
    void Move()
    {
        float moveX = Input.GetAxis( "Horizontal" );
        float moveZ = Input.GetAxis( "Vertical" );

        if( Mathf.Abs( moveX ) > Mathf.Abs( moveZ ) )
        {
            // if reach the edge
            int to = _tilePos.CoordX + ( moveX > 0 ? 1 : -1 );
            if( to >= 0 && to < _tileMap.GetSizeX() )
            {
                _tilePos.CoordX = to;
                this._moveTargetPos = _tileMap.GetPositionAtTile( _tilePos );
                this._moveTargetPos.y = transform.position.y;
                this._movingLock = true;
            }

        }
        else if( Mathf.Abs( moveZ ) > 0 )
        {
            // if reach the edge
            int to = _tilePos.CoordZ + ( moveZ > 0 ? 1 : -1 );
            if( to >= 0 && to < _tileMap.GetSizeZ() )
            {
                _tilePos.CoordZ = to;
                this._moveTargetPos = _tileMap.GetPositionAtTile( _tilePos );
                this._moveTargetPos.y = transform.position.y;
                this._movingLock = true;
            }
        }
    }

    // Movement transition between tiles
    // Call this to update once per frame
    void KeepMoving()
    {
        if( Vector3.Distance( transform.position, _moveTargetPos ) > 0.1f )
        {
            // Gradually move toward the target position
            transform.position = Vector3.MoveTowards( transform.position, 
                _moveTargetPos, Speed * Time.deltaTime );
        }
        else
        {
            // Unlock input reading after reaching target position
            this._movingLock = false;

            // Snap to the proper position
            transform.position = _moveTargetPos;
        }
    }

    // Plant a crop on the tile that the player is standing on, if that tile’s 
    // type is plantable
    // Decreases the player’s money by a small amount as an initial investment
    void Plant()
    {
        // If not enough money
        if( Money.Get() < Money.SEED_BUY_PRICE )
        {
            // TODO: show tips
            Debug.Log( "Not enough money" );
            return;
        }

        // If tile is not plantable 
        TileData.TileType type = _tileMap.GetTile( _tilePos );
        if( type != TileData.TileType.Plantable )
        {
            // TODO: show tips
            Debug.Log( "Tile is not Plantable" );
            return;
        }

        // Plant the seed and deduct money by investment cost
        _tileMap.SetTile( _tilePos, TileData.TileType.CropSeed );
        Money.Add( -Money.SEED_BUY_PRICE );
    }

    // Remove a mature crop from the tile that the player is standing on.
    // Increment the current money by a fixed amount (larger than the initial 
    // investment cost)
    // If an enemy is in the process of eating that crop, then that enemy runs 
    // away (toward the edge of the map)
    void Harvest()
    {
        // If tile is not plantable 
        TileData.TileType type = _tileMap.GetTile( _tilePos );
        if( type != TileData.TileType.CropMature )
        {
            // TODO: show tips
            Debug.Log( "Crop is not yet Mature" );
            return;
        }

        // Harvest the mature crop and increment money
        _tileMap.SetTile( _tilePos, TileData.TileType.PlantableCooldown );
        Money.Add( Money.CROP_SELL_PRICE );
    }

    // Interrupts an enemy if that enemy is in the process of eating a crop
    // That enemy will run away(toward the edge of the map)
    void Scare()
    {
        // TODO: implement this
    }
}
