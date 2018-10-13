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
}
