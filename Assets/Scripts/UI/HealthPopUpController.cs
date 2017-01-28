using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthPopUpController : MonoBehaviour
{

    private static HealthPopUp popUpText;
    private static GameObject canvas;
    private static Color red = new Color(1.0f, 0.2f, 0.2f);
    private static Color green = new Color(0.4f, 1.0f, 0.4f);

    public static void Initialize()
    {
        canvas = GameObject.Find("HealthPopUp");
        if (!popUpText)
        {
            popUpText = Resources.Load<HealthPopUp>("Prefabs/HealthPopUpContainer");
        }
    }

    public static void createPopUpText(string text, Transform location, bool isDamage)
    {
        HealthPopUp healthPopUpText = Instantiate(popUpText);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        healthPopUpText.transform.SetParent(canvas.transform, false);
        healthPopUpText.transform.position = screenPosition + new Vector3(0.0f, 40.0f, 0.0f);
        if (isDamage)
        {
            healthPopUpText.healthText.color = red;
        }
        else
        {
            healthPopUpText.healthText.color = green;
        }
        healthPopUpText.setText(text);
    }
}
