using UnityEngine;
using System.Collections.Generic;

public class PlayerUnitController : MonoBehaviour {


    GameObject Mouse; // Must be moved to BattleGroundController TODO

    int counter = 0;

    class Coordinates
    {
        int posX, posY;

        public Coordinates(int x, int y)
        {
            posX = x;
            posY = y;
        }

        public int getX()
        {
            return posX;
        }
        public void setX(int x)
        {
            posX = x;
        }

        public int getY()
        {
            return posY;
        }
        public void setY(int y)
        {
            posY = y;
        }
    }


    private bool isSelected, showMoves;
    private int maxMovement, movesLeft;
    private Coordinates coordinates;

    private List<Tile> validMoves;
    private List<GameObject> highlights;

    TileMapBuilder _tileMapBuilder;


    void Start () {

        _tileMapBuilder = GetComponent<TileMapBuilder>();
        isSelected = true;
        showMoves = true;
        maxMovement = 10;
        movesLeft = maxMovement;
        coordinates = new Coordinates(5, -5); //in battle map vertices
        validMoves = new List<Tile>();
        highlights = new List<GameObject>();
    }
	
	void Update () {
	    if (isSelected)
        {
            if (showMoves)
            {
                showAllowedMovements();
            }
            if (Input.GetMouseButtonDown(0))
            {
                foreach(GameObject plane in highlights)
                {
                    Destroy(plane);
                }
                highlights.Clear();
                Mouse = GameObject.Find("BattleGrounds");
                MouseHighlight ass = Mouse.GetComponent("MouseHighlight") as MouseHighlight;
                Transform mousePositin = ass.getHighlightSelection();
                coordinates.setX((int)mousePositin.position.x);
                coordinates.setY((int)mousePositin.position.z);
                Debug.Log(coordinates.getX() + "    " + coordinates.getY());
                showMoves = true;
            }
        }
	}

    private void showAllowedMovements()
    {
        validMoves.Clear();
        validMoves = TileHighlight.FindHighlight(TileMap.getTile(coordinates.getX(), coordinates.getY()), movesLeft);
        //getValidMoves(TileMap.getTile(coordinates.getX(), coordinates.getY()), movesLeft);
        highlightAvailableMoves();
        showMoves = false;
    }

    private void getValidMoves(Tile currentTile, int movePoints)
    {
        counter++;
        Debug.Log(counter + "        " + currentTile.getX() + " : " + currentTile.getY());
        validMoves.Add(currentTile);
        foreach(Tile tile in TileMap.GetListOfAdjacentTiles(currentTile.getX(), currentTile.getY()))            // MUST IMPROVE TODO
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
                Debug.Log("NOT GUT");
                return;
            }
            
        }
    }

    private void highlightAvailableMoves()
    {
        foreach(Tile tile in validMoves)
        {
            int x = Mathf.FloorToInt(tile.getX() / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(tile.getY()*(-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);
            plane.transform.position = new Vector3(x, 0.05f, z) * _tileMapBuilder.tileSize;
            plane.transform.position = new Vector3(plane.transform.position.x + 0.5f, plane.transform.position.y, plane.transform.position.z + 0.5f);
            plane.GetComponent<Renderer>().material.color = new Color(0.5f, 0.85f, 0.0f, 0.5f);
            plane.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            highlights.Add(plane);
        }
    }
}
