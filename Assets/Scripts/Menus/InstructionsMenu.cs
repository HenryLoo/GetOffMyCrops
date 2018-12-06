using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsMenu : MonoBehaviour, IButtonAction
{
    public Image[] Tabs;
    public Text[] TabTexts;
    private const float SELECTED_ALPHA = 1;
    private const float DESELECTED_ALPHA = 0.6f;
    private readonly string[] TAB_NAMES = { "Story", "Goal", "Enemies", "Controls" };

    public Text TextArea;
    public Image ImageArea;

    private const string NARRATIVE = "Our protagonist, the farmer, needs to fix his old house.\n\n" +
        "Unfortunately, the repair fees are too high for his budget.\n\n" +
        "Help the farmer to ship out enough crops to pay his bills!\n\n" +
        "But watch out… there are plenty of critters running around who would love to eat your produce!";

    private const string GOAL = "The goal of each level is to reach the minimum amount of money before the timer runs out.\n\n" +
        "<b>You can only plant on dirt tiles!</b>\n" +
        "The planted crop will grow over time.\n\n" +
        "If you harvest a mature crop, you will earn <b>$20</b>.\n" +
        "If you successfully harvest multiple crops in a row, you will receive <b>bonus money</b>!\n\n" +
        "Try to make as much money as possible!";

    private const string ENEMIES = "Wild animals will try to eat your crops.\n\n" +
        "Defend your crops by blocking their path.\n\n" +
        "Enemies will also run away if they lose sight of their target.\n\n" +
        "In a pinch, you can scare away all enemies by shouting!\n\n";
    
    private readonly string[] PAGES = new string[] { NARRATIVE, GOAL, ENEMIES, "" };
    private int _currentPage = 0;

    // Hold page images depending on the platform
    public Sprite[] PageDesktopImages;
    public Sprite[] PagePS4Images;

    // Reference to the help text objects
    public GameObject HelpTextDesktop;
    public GameObject HelpTextPS4;

    void Start()
    {
        GameInput.AttachInput(
            actionClick: OnButtonClickAction,
            skillClick: OnButtonClickSkill,
            backClick: OnButtonClickBack,
            leftClick: OnButtonClickLeft,
            rightClick: OnButtonClickRight,
            downClick: OnButtonClickDown,
            upClick: OnButtonClickUp );

        // Set the initial page
        TextArea.text = PAGES[ 0 ];

        // Update the page's image
        UpdatePageImage();

        // Highlight selected tab
        SelectTab();

        // Show the appropriate help text for the platform
        if( HelperFunctions.IsRunningOnDesktop() ) HelpTextDesktop.SetActive( true );
        else if( HelperFunctions.IsRunningOnPS4() ) HelpTextPS4.SetActive( true );
    }

    void Update()
    {
        GameInput.UpdateInput();
    }

    public void OnButtonClickLeft()
    {
        // Clamp minimum page to first page
        int nextPage = _currentPage - 1 < 0 ? 0 : _currentPage - 1;
        SoundController.PlaySound( SoundType.UIClick, false );
        UpdateTab(nextPage);
    }

    public void OnButtonClickRight()
    {
        // Clamp maximum page to last page
        int nextPage = _currentPage + 1 >= PAGES.Length ? _currentPage : _currentPage + 1;
        SoundController.PlaySound( SoundType.UIClick, false );
        UpdateTab(nextPage);
    }

    public void OnButtonClickUp()
    {
        // No functionality
    }

    public void OnButtonClickDown()
    {
        // No functionality
    }

    public void OnButtonClickAction()
    {
        // No functionality
    }

    public void OnButtonClickSkill()
    {
        // No functionality
    }

    public void OnButtonClickBack()
    {
        GoToMainMenu();
    }

    private void UpdateTab( int index )
    {
        // Deselect current tab
        Color color = Tabs[ _currentPage ].color;
        color.a = DESELECTED_ALPHA;
        Tabs[ _currentPage ].color = color;
        TabTexts[ _currentPage ].text = TAB_NAMES[ _currentPage ];

        // Switch to new page
        _currentPage = index;
        TextArea.text = PAGES[ index ];

        // Update the page's image
        UpdatePageImage();

        // Select the current tab
        SelectTab();
    }

    private void SelectTab()
    {
        Color color = Tabs[ _currentPage ].color;
        color.a = SELECTED_ALPHA;
        Tabs[ _currentPage ].color = color;
        TabTexts[ _currentPage ].text = "> " + TAB_NAMES[ _currentPage ];
    }

    private void GoToMainMenu()
    {
        GameStateLoader.SwitchState( GameStateLoader.GAME_STATES.MAIN_MENU );
    }

    // Show image if there is one for this page
    private void UpdatePageImage()
    {
        // Get the page image depending on platform
        Sprite pageImage = null;
        if( HelperFunctions.IsRunningOnDesktop() )
        {
            pageImage = PageDesktopImages[ _currentPage ];
        }
        else if( HelperFunctions.IsRunningOnPS4() )
        {
            pageImage = PagePS4Images[ _currentPage ];
        }

        // If there is a image, then show it
        if( pageImage != null )
        {
            ImageArea.enabled = true;
            ImageArea.sprite = pageImage;
        }
        else
        {
            ImageArea.enabled = false;
        }
    }
}
