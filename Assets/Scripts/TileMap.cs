using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour
{
    // The current level's data
    public LevelData Level;

    // The timer for this level's win/lose condition
    private GameTimer _levelTimer;

    // The tileset and the size of each tile on it
    public Texture2D Tileset;
    public int TileClipSize;

    // Map colour from layout image to tile type
    public ColourToTile[] TileMappings;
    public ColourToPrefab[] EntityMappings;

    // Hold the data for each tile on the map
    private TileData _mapData;

    // The tile's size scale factor
    private float _tileSize = 1.0f;

    // Constants
    private const int NUM_VERTICES_PER_TILE = 4;
    private const int NUM_TRIANGLES_PER_TILE = 2;
    private const int NUM_VERTICES_PER_TRIANGLE = 3;

    // Use this for initialization
    void Start()
    {
        // TODO: test level loading, remove this later
        LoadLevel( "level1" );

        _levelTimer = new GameTimer();
        InitTileMap();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _levelTimer.Update();
        _mapData.Update();
    }

    // Call this to recreate the tile map
    public void InitTileMap()
    {
        // Reset the timer
        _levelTimer.StartTimer();

        // Create the tile map
        CreateLevel();
        CreateMesh();
    }

    // Initialize the tile map's data (all the tiles)
    private void CreateLevel()
    {
        _mapData = new TileData( this, Level.TileLayout.width, 
            Level.TileLayout.height );

        // Clear existing entities
        foreach( Transform child in transform )
        {
            GameObject.DestroyImmediate( child.gameObject );
        }

        // Create the map's tiles and entities
        for( int x = 0; x < Level.TileLayout.width; ++x )
        {
            for( int z = 0; z < Level.TileLayout.height; ++z )
            {
                CreateTile( x, z );
                CreateEntity( x, z );
            }
        }
    }

    // Parse the tile map's tile layout image and create the appropriate tile
    private void CreateTile( int x, int z )
    {
        Color pixel = Level.TileLayout.GetPixel( x, z );

        foreach( ColourToTile mapping in TileMappings )
        {
            if( mapping.Colour == pixel )
            {
                _mapData.InitTile( x, z, mapping.TileType );
                return;
            }
        }
    }

    // Parse the tile map's entity layout image and create the appropriate 
    // entity
    private void CreateEntity( int x, int z )
    {
        Color pixel = Level.EntityLayout.GetPixel( x, z );

        foreach( ColourToPrefab mapping in EntityMappings )
        {
            if( mapping.Colour == pixel )
            {
                CreateEntity( x, z, mapping.Prefab );
                return;
            }
        }
    }

    // Instantiate a prefab at a given (x, z) tile position
    public void CreateEntity( int x, int z, GameObject prefab )
    {
        Instantiate( prefab, GetPositionAtTile( x, z ), Quaternion.identity, 
            transform );
    }

    // Initialize the tile map's mesh for graphical purposes
    private void CreateMesh()
    {
        // The number of tiles in row and column for the tileset
        int tilesetWidth = Tileset.width / TileClipSize;
        int tilesetHeight = Tileset.height / TileClipSize;

        // The mesh vertices
        int sizeX = Level.TileLayout.width;
        int sizeZ = Level.TileLayout.height;
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
                int tileType = ( int ) _mapData.GetTile( x, z );

                // Bottom-left vertex
                vertices[ tileIndex ] = new Vector3( x * _tileSize, 0,
                    z * _tileSize );
                normals[ tileIndex ] = Vector3.up;
                uv[ tileIndex ] = new Vector2( 
                    ( float ) tileType / tilesetWidth,
                    ( float ) tileType / tilesetHeight );

                // Top-right vertex
                vertices[ tileIndex + 1 ] = new Vector3( 
                    ( x + 1 ) * _tileSize, 0, ( z + 1 ) * _tileSize );
                normals[ tileIndex + 1 ] = Vector3.up;
                uv[ tileIndex + 1 ] = new Vector2( 
                    ( float ) ( tileType + 1 ) / tilesetWidth,
                    ( float ) ( tileType + 1 ) / tilesetHeight );

                // Bottom-right vertex
                vertices[ tileIndex + 2 ] = new Vector3( 
                    ( x + 1 ) * _tileSize, 0, z * _tileSize );
                normals[ tileIndex + 2 ] = Vector3.up;
                uv[ tileIndex + 2 ] = new Vector2( 
                    ( float ) ( tileType + 1 ) / tilesetWidth,
                    ( float ) tileType / tilesetHeight );

                // Top-left vertex
                vertices[ tileIndex + 3 ] = new Vector3( 
                    x * _tileSize, 0, ( z + 1 ) * _tileSize );
                normals[ tileIndex + 3 ] = Vector3.up;
                uv[ tileIndex + 3 ] = new Vector2( 
                    ( float ) tileType / tilesetWidth,
                    ( float ) ( tileType + 1 ) / tilesetHeight );
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

    public void LoadLevel( string levelName )
    {
        TextAsset levelJson = ( TextAsset ) Resources.Load( "Levels/" 
            + levelName );
        Debug.Log( levelJson.text );
        Level = LevelData.CreateFromJson( levelJson.text );
    }

    // Get the type of the tile at the given (x, z) tile coordinate
    public TileData.TileType GetTile( int x, int z )
    {
        return _mapData.GetTile( x, z );
    }

    // Set the type of the tile at the given (x, z) tile coordinate
    // and recreate the mesh to reflect the changes
    public void SetTile( int x, int z, TileData.TileType type )
    {
        _mapData.SetTile( x, z, type );
        CreateMesh();
    }

    // Get the level's timer
    public GameTimer GetTimer()
    {
        return _levelTimer;
    }

    // Get the size of the map's x-axis in tiles
    public int GetSizeX()
    {
        return Level.TileLayout.width;
    }

    // Get the size of the map's z-axis in tiles
    public int GetSizeZ()
    {
        return Level.TileLayout.height;
    }

    // Return the vector corresponding to the tile coordinate (x, z)
    public Vector3 GetPositionAtTile( int x, int z )
    {
        return new Vector3( x * _tileSize + _tileSize / 2, 0,
            z * _tileSize + _tileSize / 2 );
    }

    public Vector2 GetTileAtPosition(Vector3 position)
    {
        int x = (int)((position.x - (_tileSize / 2)) / _tileSize);
        int z = (int)((position.z - (_tileSize / 2)) / _tileSize);
        return new Vector2(x, z);
    }
}
