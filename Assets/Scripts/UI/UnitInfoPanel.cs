using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitInfoPanel : MonoBehaviour {

    public Canvas canvas;
    public Text unitName;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    void Update () {
	
	}

    public void setNewPosition(Vector3 unitPosition)
    {
        transform.position = new Vector3(unitPosition.x + 2.0f, unitPosition.y + 1.0f, unitPosition.z);
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180, 0);
    }

    public void setInfo(Enemy enemy)
    {
        unitName.text = enemy.getEnemyName();
    }

    public void changeCanvasEnabled(bool value)
    {
        canvas.enabled = value;
    }


}
