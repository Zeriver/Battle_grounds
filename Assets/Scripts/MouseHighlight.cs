using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

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

            currentTile.z = z;
            currentTile.x = x;
            if (TileMap.getTile(x, z) != null && TileMap.getTile(x, z).Id != 49)
            {
                HighlightSelection.transform.position = currentTile * _tileMapBuilder.tileSize;
            } else
            {
                HighlightSelection.transform.position = new Vector3(0.0f, -999.0f, 0.0f);
            }
        }
    }

    public Transform getHighlightSelection()
    {
        return HighlightSelection;
    }

}
