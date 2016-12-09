using UnityEngine;
using System.Collections.Generic;

public class PlayerUnitController : MonoBehaviour {

    // mouse events must be moved to BattleGroundController TODO

    private GameObject BattleGroundObject;
    public bool isSelected, showMoves;
    private bool isActionMode;
    private bool isActionUsed;
    private int maxMovement, movesLeft;
    private Vector3 coordinates;
    private Weapon currentWeapon;

    private List<Tile> validTiles;
    private List<GameObject> highlights;
    private List<GameObject> weaponHighlights;
    private List<Vector3> positionQueue;

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
        weaponHighlights = new List<GameObject>();
        positionQueue = new List<Vector3>();
        TileMap.setTileNotWalkable(x, y);
        //currentWeapon = new Pistol(5);
        currentWeapon = new FlameThrower(5);
    }
	
	void Update () {
	    if (isSelected)
        {
            if (showMoves && positionQueue.Count == 0)
            {
                showAllowedMovements();
            }
            if (Input.GetMouseButtonDown(0) && positionQueue.Count == 0)
            {
                Transform mousePositin = _mouseHiglight.getHighlightSelection();
                Tile clickedTile = TileMap.getTile((int)mousePositin.position.x, (int)mousePositin.position.z);
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
                        currentWeapon.useWeapon();
                        isActionUsed = true;
                        switchActionMode();
                        //TODO
                    }
                }
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
            if (Input.GetMouseButtonDown(1) && positionQueue.Count == 0 && !isActionUsed)
            {
                switchActionMode();
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
        highlightAvailableMoves();
        showMoves = false;
    }

    private void highlightAvailableMoves()
    {
        for (int i = 0; i < validTiles.Count; i++)
        {
            int x = Mathf.FloorToInt(validTiles[i].PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(validTiles[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);
            plane.transform.position = new Vector3(x, 0.05f, z) * _tileMapBuilder.tileSize;
            plane.transform.position = new Vector3(plane.transform.position.x + 0.5f, plane.transform.position.y, plane.transform.position.z + 0.5f);
            plane.GetComponent<Renderer>().material.color = new Color(0.5f, 0.85f, 0.0f, 0.5f);
            plane.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            highlights.Add(plane);
        }
    }

    private void highlightWeaponRange()
    {
        validTiles = currentWeapon.getWeaponHighlights((int)coordinates.x, (int)coordinates.z);
        for (int i = 0; i < validTiles.Count; i++)
        {
            int x = Mathf.FloorToInt(validTiles[i].PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(validTiles[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);
            plane.transform.position = new Vector3(x, 0.05f, z) * _tileMapBuilder.tileSize;
            plane.transform.position = new Vector3(plane.transform.position.x + 0.5f, plane.transform.position.y, plane.transform.position.z + 0.5f);
            plane.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.05f, 0.5f);
            plane.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            weaponHighlights.Add(plane);
        }
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
