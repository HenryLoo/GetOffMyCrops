﻿using UnityEngine.UI;

[System.Serializable]
public struct ScoreboardListing
{
    public Text Name;
    public Text Score;
}

public struct ListingValues
{
    public string Name;
    public int Score;

    public ListingValues( string name, int score )
    {
        Name = name;
        Score = score;
    }
}