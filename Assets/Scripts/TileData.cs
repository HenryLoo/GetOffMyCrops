// Hold data for all tiles on a tile map
public class TileData
{
    public enum TileType
    {
        Ground,
        Plantable,
        PlantableCooldown,
        NumTileTypes
    }

    public enum EntityType
    {
        PlayerSpawn,
        NumEntityTypes
    }
    
    // The duration in seconds for a plantable tile to become usable again
    private const float PLANTABLE_COOLDOWN_DURATION = 3.0f;

    // Hold a reference to the tile map
    private TileMap _tileMap;

    // The tile map's dimensions, defined by number of tiles
    // (0, 0) is the bottom-leftmost tile
    private int _sizeX, _sizeZ;
    private TileType[] _tiles;
    private GameTimer[] _timers;

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
    public TileType GetTile( int x, int z )
    {
        return _tiles[ z * _sizeX + x ];
    }

    // Set the tile's type at a given (x, z) tile location
    // Also create a timer for the tile if appropriate
    // This should only be called during the tile map's setup
    public void InitTile( int x, int z, TileType type )
    {
        SetTile( x, z, type );

        if( type == TileType.Plantable || 
            type == TileType.PlantableCooldown )
        {
            _timers[ z * _sizeX + x ] = new GameTimer();
        }
    }

    public void SetTile( int x, int z, TileType type )
    {
        _tiles[ z * _sizeX + x ] = type;
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
                        if( !_timers[ index ].IsStarted() )
                        {
                            _timers[ index ].StartTimer();
                        }
                        else if( _timers[ index ].GetTicks() >= 
                            PLANTABLE_COOLDOWN_DURATION )
                        {
                            _timers[ index ].StopTimer();
                            _tileMap.SetTile( x, z, TileType.Plantable );
                        }

                        break;
                }
            }
        }
    }
}
