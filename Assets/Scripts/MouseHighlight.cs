using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMapBuilder))]
public class MouseHighlight : MonoBehaviour
{

    TileMapBuilder _tileMapBuilder;

    Vector3 currentTile;
    public Transform HighlightSelection;

    void Start()
    {
        _tileMapBuilder = GetComponent<TileMapBuilder>();
    }


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (GetComponent<Collider>().Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            int x = Mathf.FloorToInt(hitInfo.point.x / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(hitInfo.point.z / _tileMapBuilder.tileSize);
            //Debug.Log(gameObject.transform.InverseTransformPoint(hitInfo.point));

            currentTile.z = z;
            currentTile.x = x;

            HighlightSelection.transform.position = currentTile * _tileMapBuilder.tileSize;
        }
    }

    public Transform getHighlightSelection()
    {
        return HighlightSelection;
    }
}
