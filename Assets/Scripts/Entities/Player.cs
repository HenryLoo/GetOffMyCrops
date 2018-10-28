using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IEntity
{
    public const string MSG_NO_ENOUGH_MONEY = "No enought money";

    public const string MSG_NOT_PLANTABLE = "Tile is not Plantable";

    public const string MSG_NOT_MATURE = "Crop is not yet Mature";

    // Reference to the GameController
    private GameController _gameController;

    // Current position on the TileMap
    private TileCoordinate _tilePos;

    // Reference to the TileMap instance
    private TileMap _tileMap;

    // Don't read key input while player is moving
    private bool _movingLock = false;

    // Lock interaction when doing the scaring animation
    private bool _scaring = false;

    // Frame cost of the scaring animation
    public const float animationCostTime = 1;

    // Log how many frames past since last animation
    private float _animPassedTime = 0;

    // Start position of a movement
    private Vector3 _moveStartPos;

    // Mid position of a movement
    private Vector3 _moveMidPos;

    // End position of a movement
    private Vector3 _moveTargetPos;

    // The time parameter for movement curve
    private float _movementTime = 0;

    // Speed multiplier for movement
    public int MovementSpeed = 3;

    // Fixed money investment/return constants
    public const int SEED_BUY_PRICE = 5;
    public const int CROP_SELL_PRICE = 20;

    private delegate void PlayerUpdate();
    private PlayerUpdate _updateEveryFrame;

    // Use this for initialization
    void Start()
    {
        _gameController = GameObject.Find("GameController")
            .GetComponent<GameController>();
        _tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
        _tilePos = _tileMap.GetTileAtPosition(transform.position);

        _updateEveryFrame = UpdateEveryFrame;
    }

    // Update is called once per frame
    void Update()
    {
        if (_updateEveryFrame != null) _updateEveryFrame();
    }

    void UpdateEveryFrame()
    {
        if (_movingLock)
        {
            KeepMoving();
        }
        else if (_scaring)
        {
            // when use animation, lock scaring by timer 
            //_animPassedTime += Time.deltaTime;
            //if (_animPassedTime > animationCostTime) // animation finished, release lock
            //{
            //    _scaring = false;
            //    _animPassedTime = 0;
            //}

            // custom scare action
            DoScaringAnimation();
        }
    }

    bool LockingInput()
    {
        return _movingLock || _scaring;
    }

    public void MoveH(int dir)
    {
        if (LockingInput()) return;

        int to = _tilePos.CoordX + dir;
        if (to >= 0 && to < _tileMap.GetSizeX())
        {
            _tilePos.CoordX = to;
            Move();
        }
    }

    public void MoveV(int dir)
    {
        if (LockingInput()) return;

        int to = _tilePos.CoordZ + dir;
        if (to >= 0 && to < _tileMap.GetSizeZ())
        {
            _tilePos.CoordZ = to;
            Move();
        }
    }

    // Detect input for movement and lock input reading once movement 
    // has started
    void Move()
    {
        _moveTargetPos = _tileMap.GetPositionAtTile(_tilePos);
        _moveTargetPos.y = transform.position.y;
        _movingLock = true;

        // rotation player to moveing direction
        transform.LookAt(_moveTargetPos);

        // Move along a Bezier curve
        _moveStartPos = transform.position;
        _moveMidPos = _moveStartPos + (_moveTargetPos - _moveStartPos) / 2 + Vector3.up * 1.5f;
    }

    // Movement transition between tiles
    // Call this to update once per frame
    void KeepMoving()
    {
        if (Vector3.Distance(transform.position, _moveTargetPos) > 0.1f)
        {
            // Gradually move toward the target position
            if (_movementTime < 1.0f)
            {
                _movementTime += 1.0f * MovementSpeed * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(_moveStartPos, _moveMidPos, _movementTime);
                Vector3 m2 = Vector3.Lerp(_moveMidPos, _moveTargetPos, _movementTime);
                transform.position = Vector3.Lerp(m1, m2, _movementTime);
            }
        }
        else
        {
            // Unlock input reading after reaching target position
            _movingLock = false;

            // Snap to the proper position
            transform.position = _moveTargetPos;

            // Reset the movement time
            _movementTime = 0;
        }
    }

    public void PerformAction()
    {
        if (LockingInput()) return;

        // If tile is plantable 
        TileData.TileType type = _tileMap.GetTile(_tilePos);
        switch (type)
        {
            case TileData.TileType.Plantable:
                Plant();
                break;
            case TileData.TileType.CropMature:
                Harvest();
                break;
            case TileData.TileType.Ground:
            case TileData.TileType.PlantableCooldown:
                PopupMsgCreator.PopupTip(MSG_NOT_PLANTABLE, transform);
                break;
            case TileData.TileType.CropSeed:
            case TileData.TileType.CropGrowing:
                PopupMsgCreator.PopupTip(MSG_NOT_MATURE, transform);
                break;
        }
    }

    // Plant a crop on the tile that the player is standing on, if that tile’s 
    // type is plantable
    // Decreases the player’s money by a small amount as an initial investment
    void Plant()
    {
        // If not enough money
        if (_gameController.GetCurrentMoney() < SEED_BUY_PRICE)
        {
            PopupMsgCreator.PopupTip(MSG_NO_ENOUGH_MONEY, transform);
            return;
        }

        // Plant the seed and deduct money by investment cost
        _tileMap.SetTile(_tilePos, TileData.TileType.CropSeed);
        _gameController.AddMoney(-SEED_BUY_PRICE);
        PopupMsgCreator.PopupMoney("-$" + SEED_BUY_PRICE, transform);
    }

    // Remove a mature crop from the tile that the player is standing on.
    // Increment the current money by a fixed amount (larger than the initial 
    // investment cost)
    // If an enemy is in the process of eating that crop, then that enemy runs 
    // away (toward the edge of the map)
    void Harvest()
    {
        // Harvest the mature crop and increment money
        _tileMap.SetTile(_tilePos, TileData.TileType.PlantableCooldown);
        _gameController.AddMoney(CROP_SELL_PRICE);

        PopupMsgCreator.PopupMoney("+$"+ CROP_SELL_PRICE, transform, new Vector3(0, 2, 0));
    }

    // Interrupts an enemy if that enemy is in the process of eating a crop
    // That enemy will run away(toward the edge of the map)
    public void Scare()
    {
        if (LockingInput()) return;

        this._scaring = true;
        this.DriveEnemyAway();

        // TODO a cooldown of skill

        PopupMsgCreator.PopupMessge("Get Out Of My Crops!!", transform, new Vector3(0, 2, 0));
    }

    // reset data after scare
    void ScareFinished()
    {
        this._scaring = false;
        this._animPassedTime = 0;
        this.jumpCount = 0;
    }

    // call RunAway of enemies near the player
    void DriveEnemyAway()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>(); // get all enemy by script attached, 
        foreach (Enemy e in enemies)
        {
            TileCoordinate dist = e.GetDistanceFromTile(this.GetTilePosition()); // distance
            if (Math.Abs(dist.CoordX) <= 1 && Math.Abs(dist.CoordZ) <= 1)
            {
                e.RunAway();
            }
        }
    }

    private int jumpCount = 0;
    void DoScaringAnimation() // scaring animation. use double jump (provisionally)
    {

        _animPassedTime += Time.deltaTime;

        float firstJumpHeight = 0.5f;
        float secondJumpHeight = 0.3f;

        Vector3 ground = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 firstJumpPos = new Vector3(transform.position.x, firstJumpHeight, transform.position.z);
        Vector3 secondJumpPos = new Vector3(transform.position.x, secondJumpHeight, transform.position.z);

        float y = transform.position.y;
        float moveTime = animationCostTime / 4;

        if (jumpCount == 0)  // jump up
        {

            transform.position = Vector3.Lerp(ground, firstJumpPos, _animPassedTime / moveTime);
            if (firstJumpHeight - y < 0.05)
            {
                jumpCount = 1;
                _animPassedTime = 0;
            }
        }
        if (jumpCount == 1) // back
        {
            transform.position = Vector3.Lerp(firstJumpPos, ground, _animPassedTime / (moveTime));
            if (y < 0.05)
            {
                jumpCount = 2;
                _animPassedTime = 0;
            }
        }
        if (jumpCount == 2)  // jump up
        {
            transform.position = Vector3.Lerp(ground, secondJumpPos, _animPassedTime / moveTime);
            if (secondJumpHeight - y < 0.05)
            {
                jumpCount = 3;
                _animPassedTime = 0;
            }
        }
        if (jumpCount == 3) // back
        {
            transform.position = Vector3.Lerp(secondJumpPos, ground, _animPassedTime / moveTime);
            if (y < 0.05)
            {
                this.ScareFinished();
            }
        }
        //if (jumpCount == 1 && transform.position.y == 0) // jump once and return to ground
        //{
        //    jumpCount = 0;
        //    transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 3, 0), step);
        //}
    }

    // all the data that is being updated should be cleaned up here.
    public void CleanUp()
    {
        _movingLock = true;
        _updateEveryFrame = null;

    }

    // Get the player's position on the tile map
    public TileCoordinate GetTilePosition()
    {
        return _tilePos;
    }
}
