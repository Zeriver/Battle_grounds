using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemButton : MonoBehaviour {

    public string description;
    private Item item;
    public Text textDescription = null;
    private GameObject BattleGroundObject;
    private BattleGroundController battleGroundController;

    void Start () {
        textDescription = GameObject.Find("DescriptionText").GetComponent<Text>();
        BattleGroundObject = GameObject.Find("BattleGrounds");
        battleGroundController = BattleGroundObject.GetComponent("BattleGroundController") as BattleGroundController;
        setItemByDescription();
    }
	

    public void changeWeapon()
    {
        textDescription.text = description;
        battleGroundController.lastActiveUnit.currentItem = item;
    }

    private void setItemByDescription() //Setting item in ItemCreator does not work for some reason TODO
    {
        for (int i = 0; i < battleGroundController.lastActiveUnit.weapons.Count; i++)
        {
            if (description.Equals(battleGroundController.lastActiveUnit.weapons[i].getDescription()))
            {
                item = battleGroundController.lastActiveUnit.weapons[i];
            }
        }
    }


}
