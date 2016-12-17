using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemButton : MonoBehaviour {

    public string description;
    public Text textDescription = null;

    void Start () {
        textDescription = GameObject.Find("DescriptionText").GetComponent<Text>();
    }
	

	void Update () {
	
	}

    public void updateDescription()
    {
        textDescription.text = description;
    }

}
