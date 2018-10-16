using UnityEngine;

public class GameTimer
{
    // Flag for if the timer has started
    private bool _isStarted;

    // Flag for if the timer is paused
    private bool _isPaused;

    // The elapsed time in seconds since the timer has started
    private float _ticks;

	// Use this for initialization
	public GameTimer()
    {
        StopTimer();
	}
	
	// Update is called once per frame
	public void Update()
    {
        if( _isStarted && !_isPaused )
        {
            _ticks += Time.deltaTime;
        }
	}

    // Reset the timer and start it
    public void StartTimer()
    {
        _isStarted = true;
        _isPaused = false;
        _ticks = 0;
    }

    // Pause the timer and keep the number of ticks
    public void PauseTimer()
    {
        if( _isStarted && !_isPaused )
        {
            _isPaused = true;
        }
    }

    // Unpause the timer and resume tick count
    public void UnpauseTimer()
    {
        if( _isStarted && _isPaused )
        {
            _isPaused = false;
        }
    }

    // Stop the timer and reset ticks
    public void StopTimer()
    {
        _isStarted = false;
        _isPaused = false;
        _ticks = 0;
    }

    // Check if the timer has started
    public bool IsStarted()
    {
        return _isStarted;
    }

    // Return the recorded duration of time
    public float GetTicks()
    {
        return _ticks;
    }

    // Manually add a duration of time to the recorded amount
    // Use a negative value to subtract time instead
    public void AddTicks( float duration )
    {
        _ticks = _ticks + duration < 0 ? 0 : _ticks + duration;
    }
}
