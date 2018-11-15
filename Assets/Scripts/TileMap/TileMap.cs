using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent( typeof( MeshFilter ) )]
[RequireComponent( typeof( MeshRenderer ) )]
[RequireComponent( typeof( MeshCollider ) )]
public class TileMap : MonoBehaviour
{
    // The tileset and the size of each tile on it
    public Texture2D Tileset;
    public int TileClipSize;

    // Map colour from layout image to tile type
    public ColourToTile[] TileMappings;

    // Hold the data for each tile on the map
    private TileData _mapData;

    // The enemies on each tile on the map
    private Queue<Enemy>[] _enemies;

    // Reference to the Player instance
    private Player _player;

    // The current level's data
    private LevelData _levelData;

    // The tile's size scale factor
    private float _tileSize = 1.0f;

    // Constants
    private const int NUM_VERTICES_PER_TILE = 4;
    private const int NUM_TRIANGLES_PER_TILE = 2;
    private const int NUM_VERTICES_PER_TRIANGLE = 3;

    private delegate void TileMapDelegate();
    private TileMapDelegate _tileMapUpdate;

    // TileMap's currently planted crops
    public List<KeyValuePair<TileCoordinate, TileData.TileType>> currentPlantedCrops = new List<KeyValuePair<TileCoordinate, TileData.TileType>>();

    // Z-distance to offset the camera from the player
    private const float CAMERA_Z_OFFSET = -2.31f;

    // Use this for initialization
    void Start()
    {
        _tileMapUpdate = UpdateTileData;

        // Initialize list of enemies for each tile
        int numTiles = Tileset.width * Tileset.height;
        _enemies = new Queue<Enemy>[ numTiles ];
        for( int i = 0; i < numTiles; ++i )
        {
            _enemies[ i ] = new Queue<Enemy>();
        }
    }

    // This is called once per frame, at the end of all updates
    void LateUpdate()
    {
        // Center the camera on the player
        if( _player != null )
        {
            Vector3 pos = _player.transform.position;
            pos.z += CAMERA_Z_OFFSET;
            pos.y = Camera.main.transform.position.y;
            Camera.main.transform.position = pos;
        }
    }

    // Call this once every frame to update the tile data
    public void UpdateEveryFrame()
    {
        if( _tileMapUpdate != null ) _tileMapUpdate();
    }

    public void UpdateTileData()
    {
        _mapData.Update();
    }

    // Call this to recreate the tile map
    public void InitTileMap( LevelData data )
    {
        // Set the level data
        _levelData = data;

        // Create the tile map
        CreateLevel();
        CreateMesh();
    }

    // Initialize the tile map's data (all the tiles)
    private void CreateLevel()
    {
        _mapData = new TileData( this, _levelData.TileLayout.width,
            _levelData.TileLayout.height );

        // Clear existing entities
        foreach( Transform child in transform )
        {
            GameObject.DestroyImmediate( child.gameObject );
        }

        // Create the map's tiles and entities
        for( int x = 0; x < _levelData.TileLayout.width; ++x )
        {
            for( int z = 0; z < _levelData.TileLayout.height; ++z )
            {
                TileCoordinate tilePos = new TileCoordinate( x, z );
                CreateTile( tilePos );
            }
        }

        GameObject player = CreateEntity( _levelData.PlayerSpawn, 
            ( GameObject ) Resources.Load( "PlayerChar" ) );
        _player = player.GetComponent<Player>();
    }

    // Parse the tile map's tile layout image and create the appropriate tile
    private void CreateTile( TileCoordinate tilePos )
    {
        Color pixel = _levelData.TileLayout.GetPixel( tilePos.CoordX,
            tilePos.CoordZ );

        foreach( ColourToTile mapping in TileMappings )
        {
            if( mapping.Colour == pixel )
            {
                _mapData.InitTile( tilePos, mapping.TileType );
                return;
            }
        }
    }

    // Instantiate a prefab at a given (x, z) tile position
    public GameObject CreateEntity( TileCoordinate tilePos, GameObject prefab )
    {
        GameObject entity = Instantiate( prefab, GetPositionAtTile( tilePos ),
            Quaternion.identity, transform );

        return entity;
    }

    // Initialize the tile map's mesh for graphical purposes
    private void CreateMesh()
    {
        // The number of tiles in row and column for the tileset
        int tilesetWidth = Tileset.width / TileClipSize;
        int tilesetHeight = Tileset.height / TileClipSize;

        // The mesh vertices
        int sizeX = _levelData.TileLayout.width;
        int sizeZ = _levelData.TileLayout.height;
        int numTiles = sizeX * sizeZ;
        int numVertices = numTiles * NUM_VERTICES_PER_TILE;
        Vector3[] vertices = new Vector3[ numVertices ];

        // One normal for each vertex
        Vector3[] normals = new Vector3[ numVertices ];

        // u,v coordinates for texture mapping
        Vector2[] uv = new Vector2[ numVertices ];

        // Create the mesh vertices
        int z, x;
        for( z = 0; z < sizeZ; ++z )
        {
            for( x = 0; x < sizeX; ++x )
            {
                int tileIndex = z * sizeX * NUM_VERTICES_PER_TILE + x *
                    NUM_VERTICES_PER_TILE;
                int tileType = ( int ) _mapData.GetTile(
                    new TileCoordinate( x, z ) );

                // The position of the tile type on the tileset
                int tileTypeX = tileType % tilesetWidth;
                int tileTypeY = ( tileType - tileTypeX ) / tilesetHeight;

                // Bottom-left vertex
                vertices[ tileIndex ] = new Vector3( x * _tileSize, 0,
                    z * _tileSize );
                normals[ tileIndex ] = Vector3.up;
                uv[ tileIndex ] = new Vector2(
                    ( float ) tileTypeX / tilesetWidth,
                    ( float ) tileTypeY / tilesetHeight );

                // Top-right vertex
                vertices[ tileIndex + 1 ] = new Vector3(
                    ( x + 1 ) * _tileSize, 0, ( z + 1 ) * _tileSize );
                normals[ tileIndex + 1 ] = Vector3.up;
                uv[ tileIndex + 1 ] = new Vector2(
                    ( float ) ( tileTypeX + 1 ) / tilesetWidth,
                    ( float ) ( tileTypeY + 1 ) / tilesetHeight );

                // Bottom-right vertex
                vertices[ tileIndex + 2 ] = new Vector3(
                    ( x + 1 ) * _tileSize, 0, z * _tileSize );
                normals[ tileIndex + 2 ] = Vector3.up;
                uv[ tileIndex + 2 ] = new Vector2(
                    ( float ) ( tileTypeX + 1 ) / tilesetWidth,
                    ( float ) tileTypeY / tilesetHeight );

                // Top-left vertex
                vertices[ tileIndex + 3 ] = new Vector3(
                    x * _tileSize, 0, ( z + 1 ) * _tileSize );
                normals[ tileIndex + 3 ] = Vector3.up;
                uv[ tileIndex + 3 ] = new Vector2(
                    ( float ) tileTypeX / tilesetWidth,
                    ( float ) ( tileTypeY + 1 ) / tilesetHeight );
            }
        }

        // A triangle is defined by 3 vertices, clockwise direction
        int numTriangles = numTiles * NUM_TRIANGLES_PER_TILE;
        int[] triangles = new int[ numTriangles * NUM_VERTICES_PER_TRIANGLE ];

        // Create the triangles
        for( z = 0; z < sizeZ; ++z )
        {
            for( x = 0; x < sizeX; ++x )
            {
                int tileIndex = z * sizeX + x;
                int indexOffset = tileIndex * 6;
                int vertexOffset = z * sizeX * NUM_VERTICES_PER_TILE + x *
                    NUM_VERTICES_PER_TILE;

                // 2 triangles for each quad
                triangles[ indexOffset ] = vertexOffset;
                triangles[ indexOffset + 1 ] = vertexOffset + 1;
                triangles[ indexOffset + 2 ] = vertexOffset + 2;

                triangles[ indexOffset + 3 ] = vertexOffset;
                triangles[ indexOffset + 4 ] = vertexOffset + 3;
                triangles[ indexOffset + 5 ] = vertexOffset + 1;
            }
        }

        // Create the mesh with its data
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        // Assign the mesh to the components
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.sharedMaterials[ 0 ].mainTexture = Tileset;
    }

    // Get the type of the tile at the given (x, z) tile coordinate
    public TileData.TileType GetTile( TileCoordinate tilePos )
    {
        return _mapData.GetTile( tilePos );
    }

    // Set the type of the tile at the given (x, z) tile coordinate
    // and recreate the mesh to reflect the changes
    public void SetTile( TileCoordinate tilePos, TileData.TileType type )
    {
        _mapData.SetTile( tilePos, type );
        CreateMesh();
    }

    // Place a tile on cooldown and remove all enemies from its list
    public void RemoveCropFromTile( TileCoordinate tilePos )
    {
        TileData.TileType type = _mapData.GetTile( tilePos );
        if( type == TileData.TileType.CropSeed ||
            type == TileData.TileType.CropGrowing ||
            type == TileData.TileType.CropMature )
        {
            SetTile( tilePos, TileData.TileType.PlantableCooldown );
            ClearEnemiesFromTile( tilePos );
        }
    }

    // Get the size of the map's x-axis in tiles
    public int GetSizeX()
    {
        return _levelData.TileLayout.width;
    }

    // Get the size of the map's z-axis in tiles
    public int GetSizeZ()
    {
        return _levelData.TileLayout.height;
    }

    // Get the size of the tile map in tiles
    public TileCoordinate GetSize()
    {
        return new TileCoordinate( GetSizeX(), GetSizeZ() );
    }

    // Return the vector corresponding to the tile coordinate (x, z)
    public Vector3 GetPositionAtTile( TileCoordinate tilePos )
    {
        return new Vector3( tilePos.CoordX * _tileSize + _tileSize / 2, 0,
            tilePos.CoordZ * _tileSize + _tileSize / 2 );
    }

    // Return the tile coordinate (x, z) corresponding to a vector position
    public TileCoordinate GetTileAtPosition( Vector3 position )
    {
        TileCoordinate tilePos;
        tilePos.CoordX = ( int ) ( ( position.x - _tileSize / 2 ) / _tileSize );
        tilePos.CoordZ = ( int ) ( ( position.z - _tileSize / 2 ) / _tileSize );
        return tilePos;
    }

    // adds o9r removes crop tiles in an array list of all currently growing tiles
    public void UpdateCropArray( TileCoordinate tilePos, TileData.TileType type )
    {
        if( type == TileData.TileType.CropSeed )
        {
            currentPlantedCrops.Add( new KeyValuePair<TileCoordinate, TileData.TileType>( tilePos, type ) );
            //Debug.Log("ADDED TILE TO CROP ARRAY x:" + tilePos.CoordX + " z:" + tilePos.CoordZ + " Type: " + type);
        }
        else if( type == TileData.TileType.PlantableCooldown )
        {
            int curIndex = -1;
            int removeIndex = -1;
            bool removeCrop = false;
            foreach( var crop in currentPlantedCrops )
            {
                curIndex++;
                if( crop.Key.Equals( tilePos ) )
                {
                    removeCrop = true;
                    removeIndex = curIndex;
                }
            }
            if( removeCrop )
            {
                currentPlantedCrops.RemoveAt( removeIndex );
                //Debug.Log("REMOVED TILE FROM CROP ARRAY x:" + tilePos.CoordX + " z:" + tilePos.CoordZ + " Type: " + type);
            }
        }
        foreach( var crop in currentPlantedCrops )
        {
            //Debug.Log("CROPS IN ARRAY: x:" + crop.Key.CoordX + " z:" + crop.Key.CoordZ + " Type: " + crop.Value);
        }
    }

    public void CleanUp()
    {
        _tileMapUpdate = null;
        foreach( Transform child in transform )
        {
            //GameObject.DestroyImmediate( child.gameObject );
            GameObject.Destroy( child.gameObject );
        }
    }

    public Player GetPlayer()
    {
        return _player;
    }


    // Add an enemy to the list of enemies for a tile
    public void AddEnemyToTile( TileCoordinate tilePos, Enemy enemy )
    {
        int index = _mapData.GetTileIndex( tilePos );
        _enemies[ index ].Enqueue( enemy );
    }

    // Remove the oldest enemy from the list of enemies for a tile
    public Enemy RemoveEnemyFromTile( TileCoordinate tilePos )
    {
        int index = _mapData.GetTileIndex( tilePos );
        return _enemies[ index ].Dequeue();
    }

    // Remove all enemies from the list of enemies for a tile
    public void ClearEnemiesFromTile( TileCoordinate tilePos )
    {
        int index = _mapData.GetTileIndex( tilePos );
        _enemies[ index ].Clear();
    }

    // Check if the tile has enemies on it
    public bool DoesTileHasEnemies( TileCoordinate tilePos )
    {
        int index = _mapData.GetTileIndex( tilePos );
        return _enemies[ index ].Count > 0;
    }
}
