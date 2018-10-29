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

    // Use this for initialization
    void Awake()
    {
        _spawnPatterns = new Queue<LevelData.EnemySpawnPattern>();
        _randomNum = new System.Random();
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
                    for( int i = 0; i < next.Amount; ++i )
                    {
                        SpawnEnemy( mapping.Prefab );
                    }
                    break;
                }
            }

            // Done spawning the enemy, remove this pattern from the queue
            _spawnPatterns.Dequeue();
        }
    }

    public TileCoordinate GetRandomSpawnLocation( TileCoordinate targetCrop )
    {
        int randomPicked = _randomNum.Next( 4 );
        TileCoordinate spawnTile = new TileCoordinate();
        switch( randomPicked )
        {
            case 3:
                spawnTile = new TileCoordinate( targetCrop.CoordX, GameController.TileMap.GetSizeZ() );
                break;
            case 2:
                spawnTile = new TileCoordinate( targetCrop.CoordX, -1 );
                break;
            case 1:
                spawnTile = new TileCoordinate( GameController.TileMap.GetSizeX(), targetCrop.CoordZ );
                break;
            default:
                spawnTile = new TileCoordinate( -1, targetCrop.CoordZ );
                break;
        }
        Debug.Log( "Spawn Location [x:" + spawnTile.CoordX + " z:" + spawnTile.CoordZ +
                "] - for Target Crop: [x:" + targetCrop.CoordX + " z:" + targetCrop.CoordZ + "]" );

        return spawnTile;
    }

    private void SpawnEnemy( GameObject prefab )
    {
        // Only spawn the enemy if there are crops available to target
        if( GameController.TileMap.currentPlantedCrops.Count == 0 ) return;

        // Choose a random crop for the enemy to target
        int randomTilePicked = _randomNum.Next( GameController.TileMap.currentPlantedCrops.Count );
        TileCoordinate targetCrop = GameController.TileMap.currentPlantedCrops[ randomTilePicked ].Key;

        // Instantiate the enemy and set its target crop
        GameObject enemy = GameController.TileMap.CreateEntity( GetRandomSpawnLocation( targetCrop ), prefab);
        enemy.GetComponent<Enemy>().SetTargetCrop( targetCrop );
    }
}
