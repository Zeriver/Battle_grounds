using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileMapBuilder))]
[RequireComponent(typeof(MouseHighlight))]
[RequireComponent(typeof(CameraController))]
public class BattleGroundController : MonoBehaviour {

    TileMapBuilder _tileMapBuilder;
    MouseHighlight _mouseHighlight;
    CameraController _cameraController;
    public GameObject _playerUnit;

    private List<PlayerUnitController> playerUnits = new List<PlayerUnitController>();
    private PlayerUnitController lastActiveUnit;

    private bool playerTurn;

    void Start () {
        _tileMapBuilder = GetComponent<TileMapBuilder>();
        _mouseHighlight = GetComponent<MouseHighlight>();
        _cameraController = GetComponent<CameraController>();
        createBattleGround(30, 30);
	}
	
	void Update () {
        if (playerTurn)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                setNextUnitActive();
            }
            if (Input.GetMouseButtonDown(0))
            {
                Transform mousePositin = _mouseHighlight.getHighlightSelection();
                PlayerUnitController clickedUnit = getClickedUnit((int)mousePositin.position.x, (int)mousePositin.position.z);
                if (clickedUnit != null)
                {
                    deactivatePlayerUnits();
                    clickedUnit.setPlayerUnitActive();
                    _cameraController.setCameraToActiveUnit(clickedUnit.transform.position);
                }
                Tile clickedTile = TileMap.getTile((int)mousePositin.position.x, (int)mousePositin.position.z);
                //Checking click on future events TODO
            }
        }
    }

    public void createBattleGround(int wdith, int height)
    {
        _tileMapBuilder.BuildMesh(wdith, height);

        //Create units (player units and enemies)
        GameObject playerUnit = _playerUnit as GameObject;
        PlayerUnitController playerUnitController = playerUnit.GetComponent<PlayerUnitController>();


        for (int i = 0; i <3; i++)  //Need to load number of players from scenario/something
        {
            playerUnits.Add(Instantiate(_playerUnit).GetComponent<PlayerUnitController>());
            if (i == 0)                             // make better loading position for scenarios TODO
            {
                playerUnits[i].createPlayerUnit(5, -5, 5);
                lastActiveUnit = playerUnits[i];
            } else if (i == 1)
            {
                playerUnits[i].createPlayerUnit(16, -9, 5);
            }
            else if (i == 2)
            {
                playerUnits[i].createPlayerUnit(8, -15, 5);
            }

        }


        //Create interface


        //Start turns
        lastActiveUnit.setPlayerUnitActive();
        _cameraController.setCameraToActiveUnit(lastActiveUnit.transform.position);
        playerTurn = true;
    }


    private void setNextUnitActive()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (playerUnits[i].isSelected)
            {
                playerUnits[i].deactivatePlayerUnit();
                if (i+1 != playerUnits.Count)
                {
                    playerUnits[i+1].setPlayerUnitActive();
                    _cameraController.setCameraToActiveUnit(playerUnits[i + 1].transform.position);
                    break;
                } else
                {
                    playerUnits[0].setPlayerUnitActive();
                    _cameraController.setCameraToActiveUnit(playerUnits[0].transform.position);
                    break;
                }
            }
        }
    }

    private void deactivatePlayerUnits()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (playerUnits[i].isSelected)
            {
                playerUnits[i].deactivatePlayerUnit();
            }
        }
    }

    private PlayerUnitController getClickedUnit(int x, int z)
    {
        PlayerUnitController unit = null;
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if ((int)playerUnits[i].transform.position.x == x && (int)playerUnits[i].transform.position.z == z+1) //+1 hack beacuse float is rounded to bigger value. Must improve TODO
            {
                unit = playerUnits[i];
            }
        }
        return unit;
    }

}
