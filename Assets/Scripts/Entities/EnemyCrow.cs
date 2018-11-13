using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class EnemyCrow : Enemy
{
	[HideInInspector]
	public enum LocomotionState { Idle, Eating, FlyingUp, FlyingDown, Flying, Hovering };
	private const float MAX_HEIGHT = 5.0f;
	private const float MIN_HEIGHT = 3.0f;
	private const float TARGET_VECTOR_VARIATION = 0.1f;
	private const float ROTATION_ANGLE_UPDATE = 3f;
	private const float ROTATION_RADIUS = 0.1f;
	private const int HOVER_TIME_IN_SECONDS = 3;

	// Threshold value for determining if this enemy is on a tile while 
    // transitioning to that tile
    private const float ON_TILE_THRESHOLD = 0.1f;

    private System.Random _randomGenerator;

	private LocomotionState _locomotionState;
	private bool _isUpdatedOnce;
	private Vector3 _updateOnceVector;
	private float _angle = 0;
	private int _hoverStartTime;

	private static Timer _hoverTimer;
	
	// Use this for initialization
    public override void Start()
    {
        base.Start();

		_randomGenerator = new System.Random();
		_locomotionState = LocomotionState.Idle;
		currentDirection = Direction.None;
		_updateOnceVector = new Vector3(0, 0, 0);
		_locomotionState = LocomotionState.FlyingUp;
    }

	protected override void SetAnimationState()
	{

	}

    // Handle how this enemy should behave once it has reached its target
    protected override void HandleOnTarget()
	{
		Debug.Log( "EnemyCrow.HandleOnTarget(): Switch to Eating state" );
	}

    // Move the enemy based on its movement behaviour/AI
    protected override void HandleMoving()
	{
		Debug.Log( "EnemyCrow.HandleMoving()" + _locomotionState );
		FlyUp();
		Fly();
		Hover();
		FlyDown();
	}

    // Attempt to destroy the target crop
    protected override void HandleEating()
	{
		Debug.Log( "EnemyCrow.HandleEating()" );
	}

    // Attempt to escape the map
    protected override void HandleEscaping()
	{
		Debug.Log( "EnemyCrow.HandleEscaping()" );
	}

    // Call necessary methods when cleaning up
    public override void CleanUp()
	{
		base.CleanUp();
	}

	private void Hover()
	{
		if ( _locomotionState != LocomotionState.Hovering )
		{
			return;
		}
		_angle += ROTATION_ANGLE_UPDATE * Time.deltaTime;
		if ( _angle >= 360 )
		{
			_angle = 0;
		}

		// rotate around target coordinates
		float xPrime = ( ROTATION_RADIUS * ( float ) Math.Cos( _angle ) ) + targetFinalPos.CoordX;
		float zPrime = ( ROTATION_RADIUS * ( float ) Math.Sin( _angle ) ) + targetFinalPos.CoordZ;

		transform.position = new Vector3( xPrime, transform.position.y, zPrime );
		
		if ( ( ( int ) Time.time - _hoverStartTime ) >= HOVER_TIME_IN_SECONDS )
		{
			_locomotionState = LocomotionState.FlyingDown;
		}
	}

	private void FlyUp()
	{
		if ( _locomotionState != LocomotionState.FlyingUp )
		{
			return;
		}

		Vector3 origin = UpdateOnceVector( transform.position, false );

		if ( IsWithinRange( origin.y + MIN_HEIGHT, origin.y + MAX_HEIGHT, transform.position.y ) )
		{
			_locomotionState = LocomotionState.Flying;
			UpdateOnceVector( transform.position, true );
		}
		else
		{
			// get a random point in space at a given height and som edistance apart.
			float vectorUp_y = (float) _randomGenerator.NextDouble() * ( MAX_HEIGHT - MIN_HEIGHT ) + MIN_HEIGHT;
			Vector3 vectorUp = new Vector3( transform.position.x, vectorUp_y, transform.position.z );
			// fly up
			float displacementPerTime = MovementSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards( transform.position, vectorUp, displacementPerTime );
		}
	}

	private void FlyDown()
	{
		if ( _locomotionState != LocomotionState.FlyingDown )
		{
			return;
		}

		Vector3 origin = UpdateOnceVector( transform.position, false );

		if ( transform.position.y <= 0 )
		{
			_locomotionState = LocomotionState.Eating;
			UpdateOnceVector( transform.position, true );
		}
		else
		{
			Vector3 vectorDown = new Vector3( transform.position.x, 0, transform.position.z );
			// fly down
			float displacementPerTime = MovementSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards( transform.position, vectorDown, displacementPerTime );
		}
	}

	private void Fly()
	{
		if ( _locomotionState != LocomotionState.Flying )
		{
			return;
		}

		if ( IsWithinRange( targetFinalPos.CoordX - TARGET_VECTOR_VARIATION, targetFinalPos.CoordX + TARGET_VECTOR_VARIATION, transform.position.x ) &&
			IsWithinRange( targetFinalPos.CoordZ - TARGET_VECTOR_VARIATION, targetFinalPos.CoordZ + TARGET_VECTOR_VARIATION, transform.position.z ) )
		{
			_hoverStartTime = ( int ) Time.time;
			_locomotionState = LocomotionState.Hovering;
		}
		else
		{
			// get a random point near the target tile
			Vector3 vectorMove = new Vector3( targetFinalPos.CoordX, transform.position.y, targetFinalPos.CoordZ );
			// fly to target
			float displacementPerTime = MovementSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards( transform.position, vectorMove, displacementPerTime );
		}
	}

	public LocomotionState GetLocomotionState()
	{
		return _locomotionState;
	}

	private bool IsWithinRange( float min, float max, float value )
	{
		return value <= max && value >= min;
	}

	private Vector3 UpdateOnceVector( Vector3 value, bool reset )
	{
		if ( reset )
		{
			_isUpdatedOnce = false;
			return _updateOnceVector;
		}
		if ( !_isUpdatedOnce )
		{
			_updateOnceVector = value;
			_isUpdatedOnce = true;
		}

		return _updateOnceVector;
	}
}
