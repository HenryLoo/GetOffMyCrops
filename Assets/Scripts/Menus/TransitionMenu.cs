using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionMenu : Menu, IButtonAction
{

    // Values for the incrementing money animation
    protected float totalMoneyValue;
    protected float moneyIncrement;
    protected float currentTotalMoney;

    protected SaveDataController dataController;
    protected GameData data;

    // GameObject names
    private static readonly string GAME_MESSAGE_TEXTBOX = "TB_GameMessage";
    private static readonly string LEVEL_MONEY_TEXTBOX = "TB_LevelMoney";
    private static readonly string TOTAL_MONEY_TEXTBOX = "TB_TotalMoney";
    private static readonly string LEVEL_NUMBER_TEXTBOX = "TB_CurrentLevel";
    private static readonly string MESSAGES_AREA = "Messages";
    private static readonly string BUTTONS_AREA = "Buttons";

    private static readonly string ANIMATION_NAME = "WinLoseMessages";


    // Text box messages
    private readonly string LEVEL_MESSAGE = "Level ";
    private readonly string EARNED1_MESSAGE = "You earned $";
    private readonly string EARNED2_MESSAGE = " in this level";
    private readonly string TOTAL_MONEY_MESSAGE = "Total: $";

    // Text areas
    private Animator _messageAnimator;
    protected Text gameMessage;
    private Text _levelNumber;
    private Text _currentLevelMoney;
    private Text _totalMoney;

    // Buttons area
    private GameObject _buttons;

    // Flag for if the animation is playing
    private bool _isAnimating;
    private bool _isIncrementing;

    protected void Awake()
    {
        _messageAnimator = GameObject.Find( MESSAGES_AREA ).GetComponent<Animator>();

        // Set up references to text boxes
        gameMessage = GameObject.Find( GAME_MESSAGE_TEXTBOX ).GetComponent<Text>();
        _levelNumber = GameObject.Find( LEVEL_NUMBER_TEXTBOX ).GetComponent<Text>();
        _currentLevelMoney = GameObject.Find( LEVEL_MONEY_TEXTBOX ).GetComponent<Text>();
        _totalMoney = GameObject.Find( TOTAL_MONEY_TEXTBOX ).GetComponent<Text>();

        // Hide the buttons while animation is playing
        _buttons = GameObject.Find( BUTTONS_AREA );
        _buttons.SetActive( false );
    }

    // Use this for initialization
    protected void Start()
    {
        GetDataFromController();
        SetTextBoxData();

        StartCoroutine( StartAnimation() );

        moneyIncrement = dataController.LevelMoney / 2;
        totalMoneyValue = data.TotalMoney;
        currentTotalMoney = data.TotalMoney - dataController.LevelMoney;
        _totalMoney.text = TOTAL_MONEY_MESSAGE + Mathf.RoundToInt( currentTotalMoney ).ToString();

        GameInput.AttachInput(
            actionClick: OnButtonClickAction,
            backClick: OnButtonClickBack,
            leftClick: OnButtonClickLeft,
            rightClick: OnButtonClickRight,
            downClick: OnButtonClickDown,
            upClick: OnButtonClickUp );
    }

    private IEnumerator StartAnimation()
    {
        _isAnimating = true;
        _isIncrementing = false;
        _messageAnimator.Play( ANIMATION_NAME );
        yield return new WaitForSeconds( 3.5f );
        _isIncrementing = true;
    }

    public void OnButtonClickLeft()
    {
        // no functionality
    }

    public void OnButtonClickRight()
    {
        // no functionality
    }

    public void OnButtonClickAction()
    {
        SelectButton();
    }

    public void OnButtonClickBack()
    {
        // no functionality
    }

    public void OnButtonClickUp()
    {
        // Move selected button upwards
        UpdateButtonIndex( NavDirection.Up );
    }

    public void OnButtonClickDown()
    {
        // Move selected button downwards
        UpdateButtonIndex( NavDirection.Down );
    }

    public void OnButtonClickSkill()
    {
        // no functionality
    }

    // Update is called once per frame
    protected void Update()
    {
        // Don't allow input while animating
        if( !_isAnimating )
        {
            GameInput.UpdateInput();
        }
        else if( _isIncrementing )
        {
            UpdateTotalMoney();
        }
    }

    private void SetTextBoxData()
    {
        _levelNumber.text = LEVEL_MESSAGE + data.CurrentLevel.ToString();
        _currentLevelMoney.text = EARNED1_MESSAGE + dataController.LevelMoney.ToString() + EARNED2_MESSAGE;
    }

    private void UpdateTotalMoney()
    {
        // If money value hasn't been fully incremented yet, keep incrementing
        if( currentTotalMoney < totalMoneyValue )
        {
            float amount = Mathf.Max( 1, moneyIncrement * Time.deltaTime );
            currentTotalMoney += amount;
        }
        // Otherwise, stop the animation
        else
        {
            currentTotalMoney = totalMoneyValue;
            StopAnimation();
        }

        _totalMoney.text = TOTAL_MONEY_MESSAGE + Mathf.RoundToInt( currentTotalMoney ).ToString();
    }

    private void StopAnimation()
    {
        _isAnimating = false;
        _isIncrementing = false;
        SoundController.PlaySound( SoundType.Coin, false );

        // Reveal the buttons
        _buttons.SetActive( true );
        SelectDefaultButton();
    }

    private void GetDataFromController()
    {
        dataController = SaveDataController.GetInstance();
        data = dataController.LoadData();
    }

    private void CleanUp()
    {
        // put all the clean up code here
    }
}
