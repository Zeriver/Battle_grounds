using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemButton : MonoBehaviour {

    private string description;
    private Item item;
    public Text textDescription = null;
    private GameObject BattleGroundObject;
    private BattleGroundController battleGroundController;

    void Start () {
        textDescription = GameObject.Find("DescriptionText").GetComponent<Text>();
        BattleGroundObject = GameObject.Find("BattleGrounds");
        battleGroundController = BattleGroundObject.GetComponent("BattleGroundController") as BattleGroundController;
    }
	
    public void setItem(Item items)
    {
        item = items;
        description = item.description;
    }

    public void changeItem()
    {
        textDescription.text = description;
        battleGroundController.lastActiveUnit.currentItem = item;
    }



}
