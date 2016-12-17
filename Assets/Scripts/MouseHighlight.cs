using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

[RequireComponent(typeof(TileMapBuilder))]
public class MouseHighlight : MonoBehaviour, IPointerClickHandler
{

    TileMapBuilder _tileMapBuilder;

    Vector3 currentTile;
    public Transform HighlightSelection;

    void Start()
    {
        _tileMapBuilder = GetComponent<TileMapBuilder>();
    }

    public void OnPointerClick(PointerEventData eventData)  //Blocking clicking on world objects when open UI TODO
    {
        
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

            HighlightSelection.transform.position = currentTile * _tileMapBuilder.tileSize;
        }
    }

    public Transform getHighlightSelection()
    {
        return HighlightSelection;
    }

}
