using UnityEngine;

// Maps a colour to a tile type
// This is used to parse level-specific tile layout images
[System.Serializable]
public class ColourToTile
{
    public Color Colour;
    public TileData.TileType TileType;
}
