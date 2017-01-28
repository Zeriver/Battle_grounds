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
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        damagePopUpText.transform.SetParent(canvas.transform, false);
        damagePopUpText.transform.position = screenPosition + new Vector3(0.0f, 40.0f, 0.0f);
        damagePopUpText.setText(text);
    }
}
