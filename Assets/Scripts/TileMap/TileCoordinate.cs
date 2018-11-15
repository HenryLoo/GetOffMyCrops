﻿// Wrapper for (x, z) tile coordinates
[System.Serializable]
public struct TileCoordinate
{
    public int CoordX, CoordZ;

    public TileCoordinate( int x, int z )
    {
        CoordX = x;
        CoordZ = z;
    }
}
