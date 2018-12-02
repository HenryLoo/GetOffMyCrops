using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Reference to the GameController
    public GameController GameController;

    // Hold mappings between enemy type strings and their prefabs
    public StringToEnemy[] EnemyMappings;

    // Hold the enemy spawn patterns
    // These should already be defined in chronological order in the level 
    // JSON file
    private Queue<LevelData.EnemySpawnPattern> _spawnPatterns;

    // Hold the random number generator
    private System.Random _randomNum;

    // List of tile positions where enemies are already spawning at
    // Prevents overlapping spawns
    private Dictionary<TileCoordinate, OccupiedSpawns> _occupiedSpawns;

    private struct OccupiedSpawns
    {
        public bool IsLeftOccupied;
        public bool IsRightOccupied;
        public bool IsUpOccupied;
        public bool IsDownOccupied;
    }

    // Use this for initialization
    void Awake()
    {
        _spawnPatterns = new Queue<LevelData.EnemySpawnPattern>();
        _randomNum = new System.Random();
        _occupiedSpawns = new Dictionary<TileCoordinate, OccupiedSpawns>();
    }

    public void Init()
    {
        foreach( LevelData.EnemySpawnPattern pattern in GameController.Level.EnemySpawnPatterns )
        {
            _spawnPatterns.Enqueue( pattern );
        }
    }

    // Update is called once per frame
    void Update()
    {
        if( _spawnPatterns.Count == 0 ) return;

        LevelData.EnemySpawnPattern next = _spawnPatterns.Peek();
        if( GameController.GetTimer().GetTicks() >= next.WhenToSpawn )
        {
            // Find the appropriate prefab for this enemy
            foreach( StringToEnemy mapping in EnemyMappings )
            {
                if( next.EnemyType == mapping.EnemyType )
                {
                    // Spawn the enemy
                    int amount = next.Amount;
                    SpawnEnemy( mapping.Prefab, amount );
                }
            }

            // Done spawning the enemy, remove this pattern from the queue
            _spawnPatterns.Dequeue();
        }
    }

    // Return false if there is no available spawn location, otherwise return true
    private bool GetRandomSpawnLocation( TileCoordinate targetCrop, 
        out TileCoordinate spawnTile )
    {
        TileCoordinate mapSize = GameController.TileMap.GetSize();

        // Hold the directions corresponding to the unoccupied spawn tiles
        // Ex: Direction.Left is the (unoccupied) tile that is left of the target tile
        List<Enemy.Direction> unoccupied = new List<Enemy.Direction>()
        {
            Enemy.Direction.Left,
            Enemy.Direction.Right,
            Enemy.Direction.Up,
            Enemy.Direction.Down
        };

        // Get which spawn locations are still available
        OccupiedSpawns occupied;
        if( _occupiedSpawns.TryGetValue( targetCrop, out occupied ) )
        {
            // Left tile is occupied
            if( occupied.IsLeftOccupied )
            {
                unoccupied.Remove( Enemy.Direction.Left );
            }

            // Right tile is occupied
            if( occupied.IsRightOccupied )
            {
                unoccupied.Remove( Enemy.Direction.Right );
            }

            // Up tile is occupied
            if( occupied.IsUpOccupied )
            {
                unoccupied.Remove( Enemy.Direction.Up );
            }

            // Down tile is occupied
            if( occupied.IsDownOccupied )
            {
                unoccupied.Remove( Enemy.Direction.Down );
            }
        }

        // Randomly pick the spawn tile from remaining spawns
        spawnTile = new TileCoordinate();
        if( unoccupied.Count == 0 ) return false;
        int randomPicked = _randomNum.Next( unoccupied.Count );
        switch( unoccupied[ randomPicked ] )
        {
            case Enemy.Direction.Left:
                spawnTile = new TileCoordinate( -1, targetCrop.CoordZ );
                occupied.IsLeftOccupied = true;
                break;

            case Enemy.Direction.Right:
                spawnTile = new TileCoordinate( mapSize.CoordX, targetCrop.CoordZ );
                occupied.IsRightOccupied = true;
                break;

            case Enemy.Direction.Up:
                spawnTile = new TileCoordinate( targetCrop.CoordX, mapSize.CoordZ );
                occupied.IsUpOccupied = true;
                break;

            case Enemy.Direction.Down:
                spawnTile = new TileCoordinate( targetCrop.CoordX, -1 );
                occupied.IsDownOccupied = true;
                break;
        }

        // Update occupied tiles
        _occupiedSpawns.Remove( targetCrop );
        _occupiedSpawns.Add( targetCrop, occupied );

        Debug.Log( "Spawn Location [x:" + spawnTile.CoordX + " z:" + spawnTile.CoordZ +
                "] - for Target Crop: [x:" + targetCrop.CoordX + " z:" + targetCrop.CoordZ + "]" );

        return true;
    }

    private void SpawnEnemy( GameObject prefab, int amount )
    {
        // Only spawn the enemy if there are crops available to target
        List<KeyValuePair<TileCoordinate, TileData.TileType>> currentPlantedCrops = GameController.TileMap.GetCurrentPlantedCrops();
        if( currentPlantedCrops.Count == 0 ) return;

        for( int i = 0; i < amount; ++i )
        {
            // Choose a random crop for the enemy to target
            int randomTilePicked = _randomNum.Next( currentPlantedCrops.Count );
            TileCoordinate targetCrop = currentPlantedCrops[ randomTilePicked ].Key;

            // Instantiate the enemy and set its target crop
            TileCoordinate spawnPos = new TileCoordinate();
            if( GetRandomSpawnLocation( targetCrop, out spawnPos ) )
            {
                GameObject enemy = GameController.TileMap.CreateEntity( spawnPos, prefab );
                enemy.GetComponent<Enemy>().SetTargetTile( targetCrop );
            }
        }

        // Clear occupied spawn tiles after spawning
        _occupiedSpawns.Clear();
    }
}
