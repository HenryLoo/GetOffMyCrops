using UnityEngine;

// Hold all level values, to be loaded in from JSON files
[System.Serializable]
public class LevelData
{
    // The name of the level
    public string LevelName;

    // The money goal for winning the level
    public int MoneyGoal;

    // The level's timer in seconds
    public float RemainingTime;

    // Hold the map's tile layout
    public Texture2D TileLayout;
    public string TileLayoutFile;

    // The player's spawn coordinates
    public TileCoordinate PlayerSpawn;

    [System.Serializable]
    public struct EnemySpawnPattern
    {
        public string EnemyType;
        public int Amount;
        public int WhenToSpawn;
    }

    public EnemySpawnPattern[] EnemySpawnPatterns;

    public static LevelData CreateFromJson( string json )
    {
        LevelData level = JsonUtility.FromJson<LevelData>( json );
        level.TileLayout = Resources.Load<Texture2D>( "Levels/" + 
            level.TileLayoutFile );
        return level;
    }
}
