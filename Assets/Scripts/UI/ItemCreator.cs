using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemCreator : MonoBehaviour
{

    public GameObject itemPanel;
    public GameObject parentPanel;

    private List<GameObject> items;

    void Start()
    {
        items = new List<GameObject>();
    }

    void Update()
    {

    }

    public void createItems(List<Weapon> weapons)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            GameObject panel = Instantiate(itemPanel, itemPanel.transform.position, itemPanel.transform.rotation) as GameObject;
            items.Add(panel);
            ItemPrefab itemPrefab = itemPanel.GetComponent<ItemPrefab>();
            itemPrefab.itemName.text = weapons[i].getName();
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
