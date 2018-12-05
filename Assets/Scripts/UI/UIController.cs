using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private UIMoneyMeter _moneyMeter;
    private UITimeMeter _timeMeter;

    public Animator LevelInfo;
    public Text LevelNumber;
    public Text LevelName;

    public Image SkillIcon;
    public Text SkillCooldown;
    public Image ActionIcon;
    public Sprite PlantSprite;
    public Sprite HarvestSprite;
    private readonly Color FADED_COLOUR = new Color( 1, 1, 1, 0.5f );

    // Delegate to handle cleanup on scene destroy
    private delegate void UIControllerUpdate();
    private UIControllerUpdate _updateEveryFrame;

    // Reference to the GameController
    public GameController GameController;

    // Use this for initialization
    void Start()
    {
        _moneyMeter = GetComponent<UIMoneyMeter>();
        _timeMeter = GetComponent<UITimeMeter>();

        // Show the level number and name at the beginning of each level
        UpdateLevelText();
        LevelInfo.Play( "LevelInfo" );

        _updateEveryFrame = UpdateWrapper;
    }

    // Update is called once per frame
    void Update()
    {
        if( _updateEveryFrame != null ) _updateEveryFrame();
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    private void UpdateWrapper()
    {
        UpdateMoneyMeter();
        UpdateTimeMeter();
        UpdateSkillIcon();
        UpdateActionIcon();
    }

    // Sets the level's name
    private void UpdateLevelText()
    {
        LevelNumber.text = "Level " + GameController.GetCurrentLevelNumber().ToString();
        LevelName.text = GameController.Level.LevelName;
    }

    // Update the money meter to display the player's current money
    private void UpdateMoneyMeter()
    {
        _moneyMeter.UpdateMoneyMeter( GameController.GetCurrentMoney(),
            GameController.Level.MoneyGoal, GameController.GetCombo() );
    }

    // Update the time meter to display the current level's remaining time
    private void UpdateTimeMeter()
    {
        _timeMeter.UpdateTimeMeter( GameController.GetTimer().GetTicks(),
            GameController.Level.RemainingTime );
    }

    // Show the skill cooldown if there is any
    private void UpdateSkillIcon()
    {
        float cooldown = GameController.TileMap.GetPlayer().GetScareCooldown();
        bool isOnCooldown = cooldown > 0;
        SkillCooldown.enabled = isOnCooldown;
        SkillIcon.color = isOnCooldown ? FADED_COLOUR : Color.white;
        if( isOnCooldown )
        {
            SkillCooldown.text = cooldown.ToString( "0.0" );
        }
    }

    // Change the action icon based on the tile that the player is standing on
    private void UpdateActionIcon()
    {
        TileCoordinate playerPos = GameController.TileMap.GetPlayer().GetTilePosition();
        TileData.TileType tileType = GameController.TileMap.GetTile( playerPos );
        switch( tileType )
        {
            case TileData.TileType.Plantable:
                ActionIcon.sprite = PlantSprite;
                ActionIcon.color = Color.white;
                break;
                
            case TileData.TileType.Ground:
            case TileData.TileType.PlantableCooldown:
                ActionIcon.sprite = PlantSprite;
                ActionIcon.color = FADED_COLOUR;
                break;

            case TileData.TileType.CropSeed:
            case TileData.TileType.CropGrowing:
                ActionIcon.sprite = HarvestSprite;
                ActionIcon.color = FADED_COLOUR;
                break;

            case TileData.TileType.CropMature:
                ActionIcon.sprite = HarvestSprite;
                ActionIcon.color = Color.white;
                break;
        }
    }

    private void CleanUp()
    {
        _updateEveryFrame = null;
    }
}
