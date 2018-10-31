using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Menu : MonoBehaviour
{
    public struct MenuOption
    {
        public Button Button;
        public Action Function;
    }

    protected static readonly int BUTTON_INDEX_UP = -1;
    protected static readonly int BUTTON_INDEX_DOWN = 1;

    private GameObject _currentlySelectedButton;
    private int _currentButtonIndex;
    private List<MenuOption> _options;

    protected Menu()
    {
        _options = new List<MenuOption>();
    }

    protected void AddMenuOption( string buttonName, Action function )
    {
        Menu.MenuOption option;
        option.Button = GameObject.Find( buttonName ).GetComponent<Button>();
        option.Function = function;
        _options.Add( option );
    }

    protected void SelectButton()
    {
        _options[ _currentButtonIndex ].Function();
    }

    protected void SelectDefaultButton()
    {
        // Manually clear out the selected object from the event system
        // so that it highlights properly if the menu is toggled between
        // active/inactive
        EventSystem.current.SetSelectedGameObject( null );

        _currentButtonIndex = 0;
        _options[ _currentButtonIndex ].Button.Select();
    }

    protected void UpdateButtonIndex( int indexChange )
    {
        _currentButtonIndex += indexChange;

        _currentButtonIndex = HelperFunctions.MathModulus( _currentButtonIndex, _options.Count );
        Debug.Log( "Menu.cs: Button index = " + _currentButtonIndex );
        _options[ _currentButtonIndex ].Button.Select();
    }
    
    protected void ChangeState( GameStateLoader.GAME_STATES state )
    {
        GameStateLoader.SwitchState( state );
    }
}
