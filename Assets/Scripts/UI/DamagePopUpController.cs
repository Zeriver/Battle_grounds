using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DamagePopUpController : MonoBehaviour
{

    private static DamagePopUp popUpText;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("DamagePopUp");
        if (!popUpText)
        {
            popUpText = Resources.Load<DamagePopUp>("Prefabs/DamagePopUpContainer");
        }
    }

    public static void createPopUpText(string text, Transform location)
    {
        DamagePopUp damagePopUpText = Instantiate(popUpText);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(new Vector2(location.position.x, location.position.y));
        damagePopUpText.transform.SetParent(canvas.transform, false);
        damagePopUpText.transform.position = screenPosition;
        damagePopUpText.setText(text);
    }
}
