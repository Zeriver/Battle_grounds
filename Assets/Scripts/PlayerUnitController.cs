using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerUnitController : MonoBehaviour {

    // mouse events must be moved to BattleGroundController TODO

    private GameObject BattleGroundObject;
    public bool isSelected;
    private bool isActionMode;
    private bool isActionUsed;
    private bool showMoves;
    private int maxMovement, movesLeft;
    private Vector3 coordinates;
    public List<Weapon> weapons;
    public List<HealingItem> healingItems;
    private Item currentItem;

    private List<Tile> validTiles;
    private List<GameObject> highlights;
    private List<GameObject> weaponHighlights;
    private List<GameObject> weaponAreaEffectHighlights;
    private List<Vector3> positionQueue;
    private Vector3 actionMouseHighlight;

    private TileMapBuilder _tileMapBuilder;
    private MouseHighlight _mouseHiglight;


    void Start() {

    }   

    public void createPlayerUnit(int x, int y, int moves)
    {
        BattleGroundObject = GameObject.Find("BattleGrounds");
        _mouseHiglight = BattleGroundObject.GetComponent("MouseHighlight") as MouseHighlight;
        _tileMapBuilder = BattleGroundObject.GetComponent("TileMapBuilder") as TileMapBuilder;

        isSelected = false;
        showMoves = false;
        isActionMode = false;
        isActionUsed = false;
        maxMovement = moves;
        movesLeft = maxMovement;
        coordinates = new Vector3(x, 0.0f, y); //in battle map vertices
        transform.position = new Vector3(coordinates.x + 0.5f, coordinates.y, coordinates.z + 0.5f);
        validTiles = new List<Tile>();
        highlights = new List<GameObject>();
        weapons = new List<Weapon>();
        healingItems = new List<HealingItem>();
        weaponHighlights = new List<GameObject>();
        weaponAreaEffectHighlights = new List<GameObject>();
        positionQueue = new List<Vector3>();
        TileMap.setTileNotWalkable(x, y);
        weapons.Add(new Pistol(5));
        weapons.Add(new FlameThrower(5));
        healingItems.Add(new MediumHealingKit(2));
        currentItem = healingItems[0];
    }
	
	void Update () {
	    if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //ItemCreator.createItems(weapons);
            }
            if (showMoves && positionQueue.Count == 0)
            {
                showAllowedMovements();
            }
            if (positionQueue.Count > 0)
            {
                coordinates += (positionQueue[0] - coordinates).normalized * 15.0f * Time.deltaTime;
                if (Vector3.Distance(positionQueue[0], coordinates) <= 0.1f)
                {
                    coordinates = positionQueue[0];
                    transform.position = new Vector3(positionQueue[0].x + 0.5f, positionQueue[0].y, -positionQueue[0].z + 0.5f);
                    positionQueue.RemoveAt(0);
                }
                if (positionQueue.Count == 0)
                {
                    TileMap.setTileNotWalkable((int)coordinates.x, (int)coordinates.z);
                }
                showMoves = true;
            }
            if (Input.GetMouseButtonDown(0) && positionQueue.Count == 0) //LEFT CLICK
            {
                Tile clickedTile = TileMap.getTile((int)_mouseHiglight.getHighlightSelection().position.x, (int)_mouseHiglight.getHighlightSelection().position.z);
                if (!isActionMode)
                {
                    if (validTiles.Contains(clickedTile))
                    {
                        TileMap.setTileWalkable((int)coordinates.x, (int)coordinates.z);
                        for (int i = 0; i < highlights.Count; i++)
                        {
                            Destroy(highlights[i]);
                        }
                        List<Tile> path = TilePathFinder.FindPath(TileMap.getTile((int)coordinates.x, (int)coordinates.z), clickedTile);
                        for (int i = 0; i < path.Count; i++)
                        {
                            positionQueue.Add(new Vector3(path[i].PosX, 0.0f, path[i].PosY));
                        }
                        movesLeft -= positionQueue.Count;
                    }
                }
                else if (isActionMode)
                {
                    if (validTiles.Contains(clickedTile))
                    {
                        if (currentItem is Weapon)
                        {
                            ((Weapon)currentItem).useWeapon();
                            //TODO
                        }
                        else if (currentItem is HealingItem)
                        {
                            ((HealingItem)currentItem).use();
                            //TODO
                        }
                        isActionUsed = true;
                        switchActionMode();
                    }
                }
            }
            if (Input.GetMouseButtonDown(1) && positionQueue.Count == 0 && !isActionUsed) //RIGHT CLICK
            {
                switchActionMode();
            }
            if (isActionMode)
            {
                Tile hoverTile = TileMap.getTile((int)_mouseHiglight.getHighlightSelection().position.x, (int)_mouseHiglight.getHighlightSelection().position.z);
                if (validTiles.Contains(hoverTile) && currentItem is Weapon) {
                    actionMouseHighlight = new Vector3((int)_mouseHiglight.getHighlightSelection().position.x + 0.5f, 0.0f, (int)_mouseHiglight.getHighlightSelection().position.z + 0.5f);
                    for (int i = 0; i < weaponAreaEffectHighlights.Count; i++)
                        {
                            Destroy(weaponAreaEffectHighlights[i]);
                        }
                    int mouseX = Math.Abs((int)actionMouseHighlight.x);
                    int mouseY = Math.Abs((int)actionMouseHighlight.z - 1);
                    List<Tile> weaponAreaEffect = ((Weapon)currentItem).getAreaEffect(Math.Abs((int) coordinates.x), Math.Abs((int) coordinates.z), mouseX, mouseY);
                    highlightWeaponAreaEffect(weaponAreaEffect);
                } 
                else if (!(validTiles.Contains(hoverTile) && actionMouseHighlight != null)){
                    for (int i = 0; i < weaponAreaEffectHighlights.Count; i++)
                    {
                        Destroy(weaponAreaEffectHighlights[i]);
                    }
                }
            }
        }
	}

    private void showAllowedMovements()
    {
        for (int i = 0; i < highlights.Count; i++)
        {
            Destroy(highlights[i]);
        }
        validTiles = TileHighlight.FindHighlight(TileMap.getTile((int)coordinates.x, (int)coordinates.z), movesLeft, false);
        for (int i = 0; i < validTiles.Count; i++)
        {
            int x = Mathf.FloorToInt(validTiles[i].PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(validTiles[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            highlights.Add(createPlane(x, z, new Color(0.5f, 0.85f, 0.0f, 0.5f)));
        }
        showMoves = false;
    }

    private void highlightWeaponRange()
    {
        if (currentItem is Weapon)
        {
            validTiles = ((Weapon)currentItem).getWeaponHighlights((int)coordinates.x, (int)coordinates.z);
        }
        else if (currentItem is HealingItem)
        {
            validTiles = ((HealingItem)currentItem).getHealingHighlights((int)coordinates.x, (int)coordinates.z);
        }
        for (int i = 0; i < validTiles.Count; i++)
        {
            int x = Mathf.FloorToInt(validTiles[i].PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(validTiles[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            weaponHighlights.Add(createPlane(x, z, new Color(1.0f, 0.0f, 0.05f, 0.5f)));
        }
    }

    private void highlightWeaponAreaEffect(List<Tile> weaponAreaEffect)
    {
        for (int i = 0; i < weaponAreaEffect.Count; i++)
        {
            int x = Mathf.FloorToInt(weaponAreaEffect[i].PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(weaponAreaEffect[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            weaponAreaEffectHighlights.Add(createPlane(x, z, new Color(8.0f, 8.0f, 0.0f, 0.5f)));
        }
    }

    private GameObject createPlane(int x, int z, Color color)   // needs to create mesh from sratch for better performance TODO
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);
        plane.transform.position = new Vector3(x, 0.05f, z) * _tileMapBuilder.tileSize;
        plane.transform.position = new Vector3(plane.transform.position.x + 0.5f, plane.transform.position.y, plane.transform.position.z + 0.5f);
        plane.GetComponent<Renderer>().material.color = color;
        plane.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
        return plane;
    }

    private void setActionMode(bool value)
    {
        isActionMode = !value;
        switchActionMode();
    }

    public void switchActionMode()
    {
        isActionMode = !isActionMode;
        if (isActionMode && !isActionUsed)
        {
            for (int i = 0; i < highlights.Count; i++)
            {
                Destroy(highlights[i]);
            }
            highlightWeaponRange();
        }
        else
        {
            for (int i = 0; i < weaponHighlights.Count; i++)
            {
                Destroy(weaponHighlights[i]);
            }
            for (int i = 0; i < weaponAreaEffectHighlights.Count; i++)
            {
                Destroy(weaponAreaEffectHighlights[i]);
            }
            showMoves = true;
        }
    }


    public void setPlayerUnitActive()
    {
        setActionMode(false);
        isSelected = true;
        showMoves = true;
    }

    public void deactivatePlayerUnit()
    {
        isSelected = false;
        showMoves = false;
        setActionMode(false);
        for (int i = 0; i < highlights.Count; i++)
        {
            Destroy(highlights[i]);
        }
    }

    public void resetAfterTurn()
    {
        for (int i = 0; i < highlights.Count; i++)
        {
            Destroy(highlights[i]);
        }
        movesLeft = maxMovement;
        isActionUsed = false;
        setActionMode(false);
    }


    /*                                      WORKS ONLY IF NOT NEAR EDGE OF THE MAP
    private void getValidMoves(Tile currentTile, int movePoints)
    {
        validMoves.Add(currentTile);
        foreach(Tile tile in TileMap.GetListOfAdjacentTiles(currentTile.getX(), currentTile.getY()))
        {
            if (tile != null)
            {
                int nextMoveCost = movePoints - tile.getMoveCost();
                if (nextMoveCost >= 0 && !validMoves.Contains(tile))
                {
                    getValidMoves(tile, nextMoveCost);
                }
            } else
            {
                return;
            }

        }
    }*/

}
