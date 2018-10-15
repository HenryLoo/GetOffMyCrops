using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IButtonAction, IEntity
{
    // Reference to the GameController
    private GameController _gameController;

    // Current position on the TileMap
    private TileCoordinate _tilePos;

    // Reference to the TileMap instance
    private TileMap _tileMap;

    // Don't read key input while player is moving
    private bool _movingLock = false;

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
    private const int SEED_BUY_PRICE = 5;
    private const int CROP_SELL_PRICE = 20;

	private delegate void PlayerUpdate();
	private PlayerUpdate _updateEveryFrame;

    // Use this for initialization
    void Start()
    {
        _gameController = GameObject.Find("GameController")
            .GetComponent<GameController>();
        _tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
        _tilePos = _tileMap.GetTileAtPosition(transform.position);

        GameInput.AttachInput(
           actionClick: OnButtonClickAction,
           backClick: OnButtonClickBack,
           leftClick: OnButtonClickLeft,
           rightClick: OnButtonClickRight,
           downClick: OnButtonClickDown,
           upClick: OnButtonClickUp);

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
    }

    public void OnButtonClickLeft()
    {
        if (!_movingLock)
        {
            MoveH(-1);
        }
    }

    public void OnButtonClickRight()
    {
        if (!_movingLock)
        {
            MoveH(1);
        }
    }

    public void OnButtonClickUp()
    {
        if (!_movingLock)
        {
            MoveV(1);
        }
    }

    public void OnButtonClickDown()
    {
        if (!_movingLock)
        {
            MoveV(-1);
        }
    }

    public void OnButtonClickAction()
    {
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
        }
    }

    public void OnButtonClickBack()
    {
        // no functionality
    }

    void MoveH(int dir)
    {
        float moveX = Input.GetAxis("Horizontal");
        int to = _tilePos.CoordX + dir;
        if (to >= 0 && to < _tileMap.GetSizeX())
        {
            _tilePos.CoordX = to;
            Move();
        }
    }

    void MoveV(int dir)
    {
        float moveZ = Input.GetAxis("Vertical");
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

    // Plant a crop on the tile that the player is standing on, if that tile’s 
    // type is plantable
    // Decreases the player’s money by a small amount as an initial investment
    void Plant()
    {
        // If not enough money
        if (_gameController.GetCurrentMoney() < SEED_BUY_PRICE)
        {
            // TODO: show tips
            Debug.Log("Not enough money");
            return;
        }

        // If tile is not plantable 
        TileData.TileType type = _tileMap.GetTile(_tilePos);
        if (type != TileData.TileType.Plantable)
        {
            // TODO: show tips
            Debug.Log("Tile is not Plantable");
            return;
        }

        // Plant the seed and deduct money by investment cost
        _tileMap.SetTile(_tilePos, TileData.TileType.CropSeed);
        _gameController.AddMoney(-SEED_BUY_PRICE);
    }

    // Remove a mature crop from the tile that the player is standing on.
    // Increment the current money by a fixed amount (larger than the initial 
    // investment cost)
    // If an enemy is in the process of eating that crop, then that enemy runs 
    // away (toward the edge of the map)
    void Harvest()
    {
        // If tile is not plantable 
        TileData.TileType type = _tileMap.GetTile(_tilePos);
        if (type != TileData.TileType.CropMature)
        {
            // TODO: show tips
            Debug.Log("Crop is not yet Mature");
            return;
        }

        // Harvest the mature crop and increment money
        _tileMap.SetTile(_tilePos, TileData.TileType.PlantableCooldown);
        _gameController.AddMoney(CROP_SELL_PRICE);
    }

    // Interrupts an enemy if that enemy is in the process of eating a crop
    // That enemy will run away(toward the edge of the map)
    void Scare()
    {
        // TODO: implement this
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
