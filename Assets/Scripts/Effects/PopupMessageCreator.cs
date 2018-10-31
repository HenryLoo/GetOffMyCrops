using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMessageCreator : MonoBehaviour
{
    private static PopupText _popupDialog;
    private static PopupText _popupMoney;
    private static PopupText _popupTip;

    private static GameObject _canvas;

    private static readonly Vector3 DEFAULT_OFFSET = new Vector3( 0, 2, 0 );

    // Use this for initialization
    public static void Initialize()
    {
        _canvas = GameObject.Find( "HUD" );

        Debug.Log( "Types : " + Resources.FindObjectsOfTypeAll<PopupText>()[ 0 ].name );

        PopupText[] textObject = Resources.FindObjectsOfTypeAll<PopupText>();

        foreach( PopupText text in textObject )
        {
            if( text.name == "PopupDialog" )
            {
                _popupDialog = text;
            }
            else if( text.name == "PopupMoney" )
            {
                _popupMoney = text;
            }
            else if( text.name == "PopupTip" )
            {
                _popupTip = text;
            }
        }
    }

    // Show large, white popup text
    public static void PopupMessage( string text, Transform transform )
    {
        PopupMessage( text, transform, DEFAULT_OFFSET ); // offset to head
    }

    public static void PopupMessage( string text, Transform transform, Vector3 offset )
    {

        if( _popupDialog == null ) Initialize();
        PopupText msg = Instantiate( _popupDialog );
        ShowPopup( msg, text, transform, offset );
    }

    // Show small, white popup text
    public static void PopupTip( string text, Transform transform )
    {
        PopupTip( text, transform, DEFAULT_OFFSET ); // offset to head
    }

    public static void PopupTip( string text, Transform transform, Vector3 offset )
    {

        if( _popupTip == null ) Initialize();
        PopupText msg = Instantiate( _popupTip );
        ShowPopup( msg, text, transform, offset );
    }

    // Show yellow popup text
    public static void PopupMoney( string text, Transform transform )
    {
        PopupMoney( text, transform, DEFAULT_OFFSET );  // offset to head
    }

    public static void PopupMoney( string text, Transform transform, Vector3 offset )
    {
        if( _popupMoney == null ) Initialize();
        PopupText msg = Instantiate( _popupMoney );
        ShowPopup( msg, text, transform, offset );
    }

    private static void ShowPopup( PopupText textObj, string text, Transform transform, Vector3 offset )
    {
        Transform traget = _canvas.transform;
        Vector2 screenPos = Camera.main.WorldToScreenPoint( transform.position + offset );

        textObj.transform.SetParent( _canvas.transform, false );
        textObj.transform.position = screenPos;
        textObj.SetText( text );
    }
}
