using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour {

    public Canvas canvas;
    [SerializeField]
    private Text Name = null;
    [SerializeField]
    private Text Text = null;

    void Start () {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }
	
	void Update () {
		
	}

    public void setNewText(string text)
    {
        string[] texts = Regex.Split(text, ":");

        /*
         P1 - Player unit 1
         
         */

        if (texts[0].Equals("P1")) // TODO setting names and images of units by their codes 
        {
            Name.text = texts[0];
        }
        else
        {
            Name.text = texts[0];
        }
        Text.text = texts[1];
    }


    public void changeDialogCanvasEnabled()
    {
        canvas.enabled = !canvas.enabled;
    }

}
