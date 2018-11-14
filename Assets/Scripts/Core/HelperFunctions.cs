using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Put all the generic helper functions here.
public static class HelperFunctions
{
    // This function ensures the value is within the range.
    // Implements math modulus function unlike c# which doesn't handle negative
    // numbers appropriately
	public static int MathModulus( int value, int modBy )
	{
		return ( value % modBy + modBy ) % modBy;
	}

    // Return the distance from tile a to tile b
    public static TileCoordinate GetTileDistance( TileCoordinate a, TileCoordinate b )
    {
        TileCoordinate dist;
        dist.CoordX = b.CoordX - a.CoordX;
        dist.CoordZ = b.CoordZ - a.CoordZ;
        return dist;
    }
    
    // Check if a value is within a range
    public static bool IsWithinRange( float min, float max, float value )
    {
        return value <= max && value >= min;
    }
}
