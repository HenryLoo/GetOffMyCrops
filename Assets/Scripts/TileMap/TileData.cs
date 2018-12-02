using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Hold data for all tiles on a tile map
public class TileData
{
    public enum TileType
    {
        Ground,
        Plantable,
        PlantableCooldown,
        CropSeed,
        CropGrowing,
        CropMature,
        Null,
        NumTileTypes
    }

    public enum EntityType
    {
        PlayerSpawn,
        NumEntityTypes
    }
    
    // The duration in seconds for a plantable tile to become usable again
    private const float PLANTABLE_COOLDOWN_DURATION = 20.0f;

    // The duration in seconds for a crop to grow into its next state
    private const float CROP_GROWING_DURATION = 15.0f;

    // Hold a reference to the tile map
    private TileMap _tileMap;

    // The tile map's dimensions, defined by number of tiles
    // (0, 0) is the bottom-leftmost tile
    private int _sizeX, _sizeZ;
    private TileType[] _tiles;

    // The timers for each tile on the map
    private GameTimer[] _timers;

    // TileMap's currently planted crops
    public List<KeyValuePair<TileCoordinate, TileType>> currentPlantedCrops = new List<KeyValuePair<TileCoordinate, TileType>>();

    public TileData( TileMap tileMap, int x, int z )
    {
        _tileMap = tileMap;

        _sizeX = x;
        _sizeZ = z;

        int length = _sizeX * _sizeZ;
        _tiles = new TileType[ length ];
        _timers = new GameTimer[ length ];
    }

    // Get the tile at a given (x, z) tile location
    public TileType GetTile( TileCoordinate tilePos )
    {
        int index = GetTileIndex( tilePos );
        if( index >= _tiles.Length || index < 0 ) return TileType.Null;

        return _tiles[ index ];
    }

    // Set the tile's type at a given (x, z) tile location
    // Also create a timer for the tile if appropriate
    // This should only be called during the tile map's setup
    public void InitTile( TileCoordinate tilePos, TileType type )
    {
        SetTile( tilePos, type );

        if( type == TileType.Plantable || 
            type == TileType.PlantableCooldown ||
            type == TileType.CropSeed ||
            type == TileType.CropGrowing ||
            type == TileType.CropMature )
        {
            _timers[ GetTileIndex( tilePos ) ] = new GameTimer();
        }
    }

    public void SetTile( TileCoordinate tilePos, TileType type )
    {
        _tiles[ GetTileIndex( tilePos ) ] = type;
        _tileMap.UpdateCropArray(tilePos, type);
    }

    // Call this once per frame
    public void Update()
    {
        for( int z = 0; z < _sizeZ; ++z )
        {
            for( int x = 0; x < _sizeX; ++x )
            {
                int index = z * _sizeX + x;
                if( _timers[ index ] == null ) continue;

                // Update timers and their respective tiles according to 
                // their tile type
                _timers[ index ].Update();
                switch( _tiles[ index ] )
                {
                    case TileType.PlantableCooldown:
                        ChangeTileTypeFromTimer( new TileCoordinate( x, z ), 
                            TileType.Plantable, PLANTABLE_COOLDOWN_DURATION );
                        break;
                    case TileType.CropSeed:
                        ChangeTileTypeFromTimer( new TileCoordinate( x, z ), 
                            TileType.CropGrowing, CROP_GROWING_DURATION );
                        break;
                    case TileType.CropGrowing:
                        ChangeTileTypeFromTimer( new TileCoordinate( x, z ), 
                            TileType.CropMature, CROP_GROWING_DURATION );
                        break;
                }
            }
        }
    }

    // Change the type of the tile at (x, z) tile coordinates, if the 
    // that tile's timer has exceeded the given duration
    private void ChangeTileTypeFromTimer( TileCoordinate tilePos, 
        TileType type, float duration )
    {
        int index = GetTileIndex( tilePos );
        if( !_timers[ index ].IsStarted() )
        {
            _timers[ index ].StartTimer();
        }
        else if( _timers[ index ].GetTicks() >= duration )
        {
            _timers[ index ].StopTimer();
            _tileMap.SetTile( tilePos, type );
        }
    }

    // Calculate the tile index from its tile coordinates
    public int GetTileIndex( TileCoordinate tilePos )
    {
        return tilePos.CoordZ * _sizeX + tilePos.CoordX;
    }
}
