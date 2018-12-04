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

    // Navigation direction on the menu, either moving up or down
    public enum NavDirection
    {
        Up,
        Down
    }

    private GameObject _currentlySelectedButton;
    protected int currentButtonIndex;
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
        _options[ currentButtonIndex ].Function();
        SoundController.PlaySound( SoundType.UIAction, false );
    }

    protected void SelectDefaultButton()
    {
        // Manually clear out the selected object from the event system
        // so that it highlights properly if the menu is toggled between
        // active/inactive
        EventSystem.current.SetSelectedGameObject( null );

        currentButtonIndex = 0;
        _options[ currentButtonIndex ].Button.Select();
    }

    protected void UpdateButtonIndex( NavDirection dir )
    {
        switch( dir )
        {
            case NavDirection.Up:
                // Clamp minimum index to first option
                currentButtonIndex = currentButtonIndex - 1 < 0 ? 
                    0 : currentButtonIndex - 1;
                SoundController.PlaySound( SoundType.UIClick, false );
                break;
            case NavDirection.Down:
                // Clamp maximum index to last option
                currentButtonIndex = currentButtonIndex + 1 >= _options.Count ? 
                    currentButtonIndex : currentButtonIndex + 1;
                SoundController.PlaySound( SoundType.UIClick, false );
                break;
        }
    
        Debug.Log( "Menu.cs: Button index = " + currentButtonIndex );
        _options[ currentButtonIndex ].Button.Select();
    }
    
    protected void ChangeState( GameStateLoader.GAME_STATES state )
    {
        GameStateLoader.SwitchState( state );
    }
}
