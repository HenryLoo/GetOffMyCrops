using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessageCreator : MonoBehaviour
{
    private static GameObject _canvas;

    private static readonly Vector3 DEFAULT_OFFSET = new Vector3( 0, 2, 0 );

    // Message constants
    private static readonly int MESSAGE_SIZE_VERYLARGE = 32;
    private static readonly int MESSAGE_SIZE_LARGE = 18;
    private static readonly int MESSAGE_SIZE_MEDIUM = 14;
    private static readonly Vector3 MESSAGE_COLOUR_MSG = new Vector3( 1, 1, 1 );
    private static readonly Vector3 MESSAGE_OUTLINE_MSG = new Vector3( 0, 0, 0 );
    private static readonly Vector3 MESSAGE_COLOUR_MONEY = new Vector3( 1, 1, 0.2f );
    private static readonly Vector3 MESSAGE_OUTLINE_MONEY = new Vector3( 0.67f, 0.47f, 0.09f );

    // Use this for initialization
    public static void Initialize()
    {
        if( _canvas == null ) _canvas = GameObject.Find( "HUD" );
    }

    private static GameObject InstantiateMessage( int size, Vector3 colour, 
        Vector3 outlineColour, bool isCountdown = false )
    {
        string prefabType = isCountdown ? "CountdownMessage" : "PopupMessage";
        GameObject popupMessage = ( GameObject ) Instantiate( Resources.Load( prefabType ) );
        PopupText popup = popupMessage.GetComponent<PopupText>();
        popup.SetFontSize( size );
        popup.SetFontColour( colour );
        popup.SetOutlineColour( outlineColour );

        return popupMessage;
    }

    // Show large, white popup text
    public static void PopupMessage( string text, Transform transform )
    {
        // Offset from player's head
        PopupMessage( text, transform, DEFAULT_OFFSET );
    }

    public static void PopupMessage( string text, Transform transform, Vector3 offset )
    {
        Initialize();

        GameObject msg = InstantiateMessage( MESSAGE_SIZE_LARGE, MESSAGE_COLOUR_MSG,
            MESSAGE_OUTLINE_MSG );
        msg.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
        ShowPopup( msg, text, transform, offset );
    }

    // Show small, white popup text
    public static void PopupTip( string text, Transform transform )
    {
        // Offset from player's head
        PopupTip( text, transform, DEFAULT_OFFSET );
    }

    public static void PopupTip( string text, Transform transform, Vector3 offset )
    {
        Initialize();
        
        GameObject msg = InstantiateMessage( MESSAGE_SIZE_MEDIUM, 
            MESSAGE_COLOUR_MSG, MESSAGE_OUTLINE_MSG );
        ShowPopup( msg, text, transform, offset );
    }

    // Show yellow popup text
    public static void PopupMoney( string text, Transform transform )
    {
        // Offset from player's head
        PopupMoney( text, transform, DEFAULT_OFFSET );
    }

    public static void PopupMoney( string text, Transform transform, Vector3 offset )
    {
        Initialize();
        
        GameObject msg = InstantiateMessage( MESSAGE_SIZE_MEDIUM, 
            MESSAGE_COLOUR_MONEY, MESSAGE_OUTLINE_MONEY );
        ShowPopup( msg, text, transform, offset );
    }

    // Show countdown text
    public static void PopupCountdown( string text, Transform transform )
    {
        Initialize();

        GameObject msg = InstantiateMessage( MESSAGE_SIZE_VERYLARGE, MESSAGE_COLOUR_MSG,
            MESSAGE_OUTLINE_MSG, true );
        ShowPopup( msg, text, transform, DEFAULT_OFFSET );
    }

    private static void ShowPopup( GameObject textObj, string text, Transform transform, Vector3 offset )
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint( transform.position + offset );

        textObj.transform.SetParent( _canvas.transform, false );
        textObj.transform.position = screenPos;
        textObj.GetComponent<PopupText>().SetText( text );
    }
}
