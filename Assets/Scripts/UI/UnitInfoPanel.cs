using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitInfoPanel : MonoBehaviour {

    public Canvas canvas;
    public GameObject panel;
    public Slider healthBar;
    public Text unitName;
    public Text unitHealth;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    void Update () {
	
	}

    public void setNewPosition(Vector3 unitPosition)
    {
        panel.transform.position = new Vector3(Input.mousePosition.x + 70, Input.mousePosition.y, Input.mousePosition.z);
    }

    public void setInfo(Enemy enemy)
    {
        unitName.text = enemy.getEnemyName();
        unitHealth.text = enemy.health.ToString() + " / " + enemy.maxHealth.ToString();
        healthBar.value = (float)enemy.health / enemy.maxHealth;
    }

    public void changeCanvasEnabled(bool value)
    {
        canvas.enabled = value;
    }


}
