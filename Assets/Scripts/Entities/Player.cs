using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IEntity
{
    public const string MSG_NOT_PLANTABLE = "You can't plant here!";
    public const string MSG_NOT_MATURE = "This crop isn't mature yet!";
    public const string MSG_SCARE = "GET OFF MY CROPS!!";
    public const string MSG_SCARE_COOLDOWN = "I'm too tired...";

    // Reference to the GameController
    private GameController _gameController;

    // Current position on the TileMap
    private TileCoordinate _tilePos;

    // Reference to the TileMap instance
    private TileMap _tileMap;

    // Reference to the character animator
    private Animator _animator;

    // Don't read key input while player is moving
    private bool _movingLock = false;

    // Lock interaction when doing the scaring animation
    private bool _scaring = false;

    // Duration of the scaring animation
    public const float ScaringAnimationDuration = 0.5f;

    // Log how many frames past since last animation
    private float _scaringAnimationTicks = 0;

    // Hold the number of times the player has jumped during the 
    // scaring animation
    private int _scaringJumpCount = 0;

    // Start position of a movement
    private Vector3 _moveStartPos;

    // Mid position of a movement
    private Vector3 _moveMidPos;

    // End position of a movement
    private Vector3 _moveTargetPos;

    // The time parameter for movement curve
    private float _movementTime = 0;

    // Speed multiplier for movement
    public int MovementSpeed = 6;
    private const float MOVEMENT_HEIGHT = 3f;

    // The amount of money earned for harvesting a crop
    public const int CROP_SELL_PRICE = 20;

    // Cooldown timer for scare attack in seconds
    private GameTimer _scareTimer;
    private const float SCARE_COOLDOWN = 20;

    private delegate void PlayerUpdate();
    private PlayerUpdate _updateEveryFrame;

    // Use this for initialization
    void Start()
    {
        _gameController = GameObject.Find( "GameController" )
            .GetComponent<GameController>();
        _tileMap = GameObject.Find( "TileMap" ).GetComponent<TileMap>();
        _tilePos = _tileMap.GetTileAtPosition( transform.position );

        _updateEveryFrame = UpdateEveryFrame;

        _animator = GetComponent<Animator>();

        _scareTimer = new GameTimer();
    }

    // Update is called once per frame
    void Update()
    {
        // If the game is paused, then pause character animations and movement
        // Keep showing the animation if the game is just counting down
        bool isPaused = _gameController.GetIsPaused();
        bool isLevelStarted = _gameController.GetIsLevelStarted();
        _animator.enabled = !isPaused | !isLevelStarted;

        if( _updateEveryFrame != null && !isPaused ) _updateEveryFrame();
    }

    void UpdateEveryFrame()
    {
        // Progress the scare cooldown timer
        if( _scareTimer.IsStarted() )
        {
            if( _scareTimer.GetTicks() < SCARE_COOLDOWN )
            {
                _scareTimer.Update();
            }
            else
            {
                _scareTimer.StopTimer();
            }
        }

        if( _movingLock )
        {
            KeepMoving();
        }
        else if( _scaring )
        {
            // Perform the scaring animation
            DoScaringAnimation();
        }
    }

    bool LockingInput()
    {
        return _movingLock || _scaring;
    }

    public void MoveH( int dir )
    {
        if( LockingInput() ) return;

        int to = _tilePos.CoordX + dir;
        if( to >= 0 && to < _tileMap.GetSizeX() )
        {
            _tilePos.CoordX = to;
            Move();
        }
    }

    public void MoveV( int dir )
    {
        if( LockingInput() ) return;

        int to = _tilePos.CoordZ + dir;
        if( to >= 0 && to < _tileMap.GetSizeZ() )
        {
            _tilePos.CoordZ = to;
            Move();
        }
    }

    // Detect input for movement and lock input reading once movement 
    // has started
    void Move()
    {
        _moveTargetPos = _tileMap.GetPositionAtTile( _tilePos );
        _moveTargetPos.y = transform.position.y;
        _movingLock = true;

        // Rotate player to face direction of movement
        transform.LookAt( _moveTargetPos );

        // Move along a Bezier curve
        _moveStartPos = transform.position;
        _moveMidPos = _moveStartPos + ( _moveTargetPos - _moveStartPos ) / 2 + Vector3.up * MOVEMENT_HEIGHT;
        //_jumpEffectParticles.Clear();
        SoundController.PlaySound( SoundType.PlayerJump );
    }

    // Movement transition between tiles
    // Call this to update once per frame
    void KeepMoving()
    {
        if( Vector3.Distance( transform.position, _moveTargetPos ) > 0.1f )
        {
            // Gradually move toward the target position
            if( _movementTime < 1.0f )
            {
                _movementTime += 1.0f * MovementSpeed * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp( _moveStartPos, _moveMidPos, _movementTime );
                Vector3 m2 = Vector3.Lerp( _moveMidPos, _moveTargetPos, _movementTime );
                transform.position = Vector3.Lerp( m1, m2, _movementTime );
            }
        }
        else
        {
            // Unlock input reading after reaching target position
            _movingLock = false;

            // Emit player jump particle effect
            ParticleCreator.Emit( "ParticlesPlayerJump", transform.position );

            // Snap to the proper position
            transform.position = _moveTargetPos;

            // Reset the movement time
            _movementTime = 0;
        }
    }

    public void PerformAction()
    {
        if( LockingInput() ) return;

        // If tile is plantable 
        TileData.TileType type = _tileMap.GetTile( _tilePos );
        switch( type )
        {
            case TileData.TileType.Plantable:
                Plant();
                break;
            case TileData.TileType.CropMature:
                Harvest();
                break;
            case TileData.TileType.Ground:
            case TileData.TileType.PlantableCooldown:
                PopupMessageCreator.PopupTip( MSG_NOT_PLANTABLE, transform );
                break;
            case TileData.TileType.CropSeed:
            case TileData.TileType.CropGrowing:
                PopupMessageCreator.PopupTip( MSG_NOT_MATURE, transform );
                break;
        }
    }

    // Plant a crop on the tile that the player is standing on, if that tile’s 
    // type is plantable
    void Plant()
    {
        // Plant the seed
        _tileMap.SetTile( _tilePos, TileData.TileType.CropSeed );
        SoundController.PlaySound( SoundType.PlayerPlant );
    }

    // Remove a mature crop from the tile that the player is standing on.
    // Increment the current money by a fixed amount
    // If an enemy is in the process of eating that crop, then that enemy runs 
    // away (toward the edge of the map)
    void Harvest()
    {
        // Harvest the mature crop and increment money
        _tileMap.RemoveCropFromTile( _tilePos, _gameController, false );
        int combo = _gameController.GetCombo();
        int totalSellPrice = CROP_SELL_PRICE + combo;
        _gameController.AddMoney( totalSellPrice );

        PopupMessageCreator.PopupMoney( "+$" + totalSellPrice,
            transform, new Vector3( 0, 2, 0 ) );
        SoundController.PlaySound( SoundType.PlayerHarvest );

        // Increment combo count
        _gameController.IncrementCombo();
    }

    // Interrupts an enemy if that enemy is in the process of eating a crop
    // That enemy will run away(toward the edge of the map)
    public void Scare()
    {
        if( LockingInput() ) return;

        // Don't scare if it is on cooldown
        if( _scareTimer.IsStarted() && _scareTimer.GetTicks() < SCARE_COOLDOWN )
        {
            PopupMessageCreator.PopupTip( MSG_SCARE_COOLDOWN,
                transform, new Vector3( 0, 2, 0 ) );
            return;
        }

        // Otherwise, proceed with scaring attack
        this._scaring = true;
        this.ScareAllEnemies();
        PopupMessageCreator.PopupMessage( MSG_SCARE, transform, new Vector3( 0, 2, 0 ) );
        SoundController.PlaySound( SoundType.PlayerScare );

        // Emit scaring particle effect
        Vector3 particlePos = new Vector3( transform.position.x, 2, transform.position.z );
        ParticleCreator.Emit( "ParticlesPlayerScare", particlePos );

        // Start the cooldown timer
        _scareTimer.StartTimer();
    }

    // Reset scaring variables after the scare animation has completed
    void ScareFinished()
    {
        this._scaring = false;
        this._scaringAnimationTicks = 0;
        this._scaringJumpCount = 0;
    }

    // Scare all enemies on the level
    void ScareAllEnemies()
    {
        // Get all enemies
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach( Enemy e in enemies )
        {
            if( e.GetCurrentState() != Enemy.EnemyState.Escaping )
            {
                e.RunAway();
            }
        }
    }

    // Perform the scaring animation
    void DoScaringAnimation()
    {
        _scaringAnimationTicks += Time.deltaTime;

        float firstJumpHeight = 1;
        float secondJumpHeight = 0.8f;

        Vector3 ground = new Vector3( transform.position.x, 0, transform.position.z );
        Vector3 firstJumpPos = new Vector3( transform.position.x, firstJumpHeight, transform.position.z );
        Vector3 secondJumpPos = new Vector3( transform.position.x, secondJumpHeight, transform.position.z );

        float y = transform.position.y;
        float moveTime = ScaringAnimationDuration / 4;

        if( _scaringJumpCount == 0 )  // jump up
        {

            transform.position = Vector3.Lerp( ground, firstJumpPos, _scaringAnimationTicks / moveTime );
            if( firstJumpHeight - y < 0.05 )
            {
                _scaringJumpCount = 1;
                _scaringAnimationTicks = 0;
            }
        }
        if( _scaringJumpCount == 1 ) // back
        {
            transform.position = Vector3.Lerp( firstJumpPos, ground, _scaringAnimationTicks / moveTime );
            if( y < 0.05 )
            {
                _scaringJumpCount = 2;
                _scaringAnimationTicks = 0;
            }
        }
        if( _scaringJumpCount == 2 )  // jump up
        {
            transform.position = Vector3.Lerp( ground, secondJumpPos, _scaringAnimationTicks / moveTime );
            if( secondJumpHeight - y < 0.05 )
            {
                _scaringJumpCount = 3;
                _scaringAnimationTicks = 0;
            }
        }
        if( _scaringJumpCount == 3 ) // back
        {
            transform.position = Vector3.Lerp( secondJumpPos, ground, _scaringAnimationTicks / moveTime );
            if( y < 0.05 )
            {
                this.ScareFinished();
            }
        }
    }

    // All the data that is being updated should be cleaned up here
    public void CleanUp()
    {
        _movingLock = true;
        _updateEveryFrame = null;
        _scareTimer.StopTimer();
    }

    // Get the player's position on the tile map
    public TileCoordinate GetTilePosition()
    {
        return _tilePos;
    }

    // Get the remaining cooldown for the scare skill
    public float GetScareCooldown()
    {
        float cooldown = _scareTimer.IsStarted() ?
            SCARE_COOLDOWN - _scareTimer.GetTicks() : 0;
        return cooldown;
    }
}
