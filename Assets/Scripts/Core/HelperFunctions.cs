using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***
 * Put all the generic helper functions here.
 * 
 * */

public static class HelperFunctions
{
	// this function ensures the value is withing the range.
	// Implements math modulus function unlike c# which doesn't hanle negative numbers appropriately
	public static int mathModulus( int value, int modBy )
	{
		return ( value % modBy + modBy ) % modBy;
	}
}
