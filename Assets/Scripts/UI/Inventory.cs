using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {

    public Canvas equipment;
	
	void Start () {
        equipment = GetComponent<Canvas>();
        equipment.enabled = false;
    }
	
	void Update () {
	
	}

    public void changeCanvasEnabled()
    {
        equipment.enabled = !equipment.enabled;
    }
}
