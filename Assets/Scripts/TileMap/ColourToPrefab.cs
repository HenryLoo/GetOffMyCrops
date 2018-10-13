using UnityEngine;

// Maps a colour to a prefab
// This is used to parse level-specific entity layout images
[System.Serializable]
public class ColourToPrefab
{
    public Color Colour;
    public GameObject Prefab;
}
