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

    private const string NARRATIVE = "Our protagonist, the farmer, needs to fix his old house.\n\n" +
        "Unfortunately, the repair fees are too high for his budget.\n\n" +
        "Help the farmer to ship out enough crops to pay his bills!\n\n" +
        "But watch out… there are plenty of critters running around who would love to eat your produce!";

    private const string GOAL = "The goal of each level is to reach the minimum amount of money before the timer runs out.\n\n" +
        "You will start with <b>$20</b> per level.\n\n" +
        "It costs <b>$5</b> to plant a crop.\n" +
        "<b>You can only plant on dirt tiles!</b>\n" +
        "The planted crop will grow over time.\n\n" +
        "If you harvest a mature crop, you will earn <b>$20</b>.\n\n" +
        "Try to make as much money as possible!";

    private const string ENEMIES = "Wild animals will try to eat your crops.\n\n" +
        "Defend your crops by blocking their path!\n\n" + 
        "You can also scare nearby enemies by shouting at them!\n\n" +
        "If an enemy is busy eating a crop, it won't notice you.\n" +
        "You'll need to stand on top of them and hit the <b>ACTION button</b> to remove them off the crop, one at a time!";

    private const string CONTROLS = "<b>Arrow keys (MOVEMENT buttons)</b> - Move\n\n" +
        "<b>X (ACTION button)</b> - Plant crop, harvest mature crop, remove enemy off a crop\n\n" +
        "<b>Z (SKILL button)</b> - Shout loudly to scare away all enemies on the level (20s cooldown)\n\n" +
        "<b>Escape (BACK button)</b> - Go back";
    
    private readonly string[] PAGES = new string[] { NARRATIVE, GOAL, ENEMIES, CONTROLS };
    private int _currentPage = 0;

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

        // Highlight selected tab
        SelectTab();
    }

    void Update()
    {
        GameInput.UpdateInput();
    }

    public void OnButtonClickLeft()
    {
        // Clamp minimum page to first page
        int nextPage = _currentPage - 1 < 0 ? 0 : _currentPage - 1;
        UpdateTab(nextPage);
    }

    public void OnButtonClickRight()
    {
        // Clamp maximum page to last page
        int nextPage = _currentPage + 1 >= PAGES.Length ? _currentPage : _currentPage + 1;
        UpdateTab(nextPage);
    }

    public void OnButtonClickUp()
    {
        // Clamp minimum page to first page
        int nextPage = _currentPage - 1 < 0 ? 0 : _currentPage - 1;
        UpdateTab( nextPage );
    }

    public void OnButtonClickDown()
    {
        // Clamp maximum page to last page
        int nextPage = _currentPage + 1 >= PAGES.Length ? _currentPage : _currentPage + 1;
        UpdateTab( nextPage );
    }

    public void OnButtonClickAction()
    {
        GoToMainMenu();
    }

    public void OnButtonClickSkill()
    {
        GoToMainMenu();
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
}
