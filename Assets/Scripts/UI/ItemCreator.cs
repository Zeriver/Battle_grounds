using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemCreator : MonoBehaviour
{

    public GameObject itemPanel;
    public GameObject parentPanel;
    public bool isWeapon, isHealingItem;

    private List<GameObject> items;

    void Start()
    {
        items = new List<GameObject>();
    }

    void Update()
    {

    }

    public void createItems(List<Item> item)
    {
        for (int i = 0; i < item.Count; i++)
        {
            GameObject panel = Instantiate(itemPanel, itemPanel.transform.position, itemPanel.transform.rotation) as GameObject;
            items.Add(panel);
            ItemPrefab itemPrefab = panel.GetComponent<ItemPrefab>();
            itemPrefab.itemName.text = item[i].name;
            Button button = itemPrefab.itemButton;
            ItemButton itemButton = button.GetComponent<ItemButton>();
            itemButton.setItem(item[i]);
            panel.transform.SetParent(parentPanel.transform);
        }
    }

    public void destroyItems()
    {
        for (int i = 0; i< items.Count; i++)
        {
            Destroy(items[i]);
        }
    }

}
