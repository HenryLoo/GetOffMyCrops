using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMsgCreator : MonoBehaviour {

    private static PopupText popupDialog;
    private static PopupText popupMoney;
    private static PopupText popupTip;

    private static GameObject canvas;

    public static Vector3 defaultOffset = new Vector3(0, 2, 0);

    // Use this for initialization
    public static void Initialize()
    {
        canvas = GameObject.Find("HUD");

        Debug.Log("Types : " + Resources.FindObjectsOfTypeAll<PopupText>()[0].name);

        PopupText[] textObject = Resources.FindObjectsOfTypeAll<PopupText>();

        foreach(PopupText text in textObject)
        {
            if(text.name == "PopupDialog")
            {
                popupDialog = text;
            }
            else if (text.name == "PopupMoney")
            {
                popupMoney = text;
            }
            else if (text.name == "PopupTip")
            {
                popupTip = text;
            }
        }
    }

    // show text in a big white TEXT
    public static void PopupMessge(string text, Transform transform)
    {
        PopupMessge(text, transform, defaultOffset); // offset to head
    }

    public static void PopupMessge (string text, Transform transform, Vector3 offset) {

        if (popupDialog == null) Initialize();
        PopupText msg = Instantiate(popupDialog);
        ShowPopup(msg, text, transform, offset);
    }

    // show text in a small white TEXT
    public static void PopupTip(string text, Transform transform)
    {
        PopupTip(text, transform, defaultOffset); // offset to head
    }

    public static void PopupTip(string text, Transform transform, Vector3 offset)
    {

        if (popupTip == null) Initialize();
        PopupText msg = Instantiate(popupTip);
        ShowPopup(msg, text, transform, offset);
    }

    // show text in a yellow TEXT
    public static void PopupMoney(string text, Transform transform)
    {
        PopupMoney(text, transform, defaultOffset);  // offset to head
    }

    public static void PopupMoney(string text, Transform transform, Vector3 offset)
    {
        if (popupMoney == null) Initialize();
        PopupText msg = Instantiate(popupMoney);
        ShowPopup(msg, text, transform, offset);
    }

    private static void ShowPopup(PopupText textObj, string text, Transform transform, Vector3 offset)
    {
        Transform traget = canvas.transform;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + offset);

        textObj.transform.SetParent(canvas.transform, false);
        textObj.transform.position = screenPos;
        textObj.SetText(text);
    }

}
