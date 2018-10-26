using UnityEngine;

// Maps a string to an enemy prefab
// This is used by the EnemyController to determine which enemy to spawn
[System.Serializable]
public class StringToEnemy
{
    public string EnemyType;
    public GameObject Prefab;
}
