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
    public GameObject _enemyUnit;
    public GameObject _playerUI;
    public GameObject _itemCreator;
    public GameObject _inventory;
    public GameObject _unitInfo;

    private PlayerUI playerUI;
    private ItemCreator itemCreator;
    private Inventory inventory;
    private UnitInfoPanel unitInfo;
    public List<PlayerUnitController> playerUnits = new List<PlayerUnitController>();
    public List<Enemy> enemyUnits = new List<Enemy>();
    public PlayerUnitController lastActiveUnit;

    private bool playerTurn, enemyTurn, allyTurn, endTurn;
    private int turnNumber;

    void Start () {
        _tileMapBuilder = GetComponent<TileMapBuilder>();
        _mouseHighlight = GetComponent<MouseHighlight>();
        _cameraController = GetComponent<CameraController>();
        playerUI = _playerUI.GetComponent("PlayerUI") as PlayerUI;
        itemCreator = _itemCreator.GetComponent("ItemCreator") as ItemCreator;
        inventory = _inventory.GetComponent("Inventory") as Inventory;
        unitInfo = _unitInfo.GetComponent("UnitInfoPanel") as UnitInfoPanel;
        turnNumber = 1;
        createBattleGround(30, 30);
	}
	
	void Update () {
        if (playerTurn && !lastActiveUnit.moving)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                inventoryWindowService();
            }
            if (!inventory.equipment.enabled)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    setNextUnitActive();
                }
                Transform mousePositin = _mouseHighlight.getHighlightSelection();
                if (Input.GetMouseButtonDown(0))     /// LEFT CLICK
                {
                    if (lastActiveUnit.giveHighlights.Count == 0)
                    {
                        PlayerUnitController clickedUnit = getClickedUnit((int)mousePositin.position.x, (int)mousePositin.position.z);
                        if (clickedUnit != null)
                        {
                            deactivatePlayerUnits();
                            clickedUnit.setPlayerUnitActive();
                            _cameraController.setCameraToActiveUnit(clickedUnit.transform.position);
                        }
                        Tile clickedTile = TileMap.getTile((int)mousePositin.position.x, (int)mousePositin.position.z);
                    }
                }
                if (!lastActiveUnit.isActionMode)
                {
                    Enemy enemy = getHoveredEnemy((int)mousePositin.position.x, (int)mousePositin.position.z);
                    if (enemy != null)
                    {
                        enemy.showPossibleMovement();
                        if (!unitInfo.canvas.enabled)
                        {
                            unitInfo.setNewPosition(enemy.transform.position);
                            unitInfo.setInfo(enemy);
                            unitInfo.changeCanvasEnabled(true);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < enemyUnits.Count; i++)
                        {
                            enemyUnits[i].destroyMovementHighlights();
                        }
                        if (unitInfo.canvas.enabled)
                        {
                            unitInfo.changeCanvasEnabled(false);
                        }
                    }
                }
            }
        }
        if (enemyTurn)
        {
            performEnemyTurn();
            bool allEnemiesDone = true;
            Debug.Log("Enemy turn in progress...");
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                if (!enemyUnits[i].turnDone)
                {
                    allEnemiesDone = false;
                }
            }
            if (allEnemiesDone)
            {
                enemyTurn = false;
                allyTurn = true;
            }            
        }
        if (allyTurn)
        {
            allyTurn = false;
            endTurn = true;
        }
        if (endTurn)
        {
            nextTurn();
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
                playerUnits[i].createPlayerUnit(5, 5, 5, "up");
                lastActiveUnit = playerUnits[i];
            } else if (i == 1)
            {
                playerUnits[i].createPlayerUnit(16, 9, 5, "down");
            }
            else if (i == 2)
            {
                playerUnits[i].createPlayerUnit(8, 15, 5, "right");
            }

        }


        GameObject enemyUnit = _enemyUnit as GameObject;
        Enemy enemyUnitController = enemyUnit.GetComponent<Enemy>();

        for (int i = 0; i < 3; i++)
        {
            enemyUnits.Add(Instantiate(_enemyUnit).GetComponent<Enemy>());
            if (i == 0)
            {
                enemyUnits[i].createEnemy(12, 12, 1, "down");
            }
            else if (i == 1)
            {
                enemyUnits[i].createEnemy(12, 5, 1, "down");
            }
            else if (i == 2)
            {
                enemyUnits[i].createEnemy(8, 8, 1, "down");
            }
        }

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
                    lastActiveUnit = playerUnits[i + 1];
                    _cameraController.setCameraToActiveUnit(playerUnits[i + 1].transform.position);
                    break;
                } else
                {
                    playerUnits[0].setPlayerUnitActive();
                    lastActiveUnit = playerUnits[0];
                    _cameraController.setCameraToActiveUnit(playerUnits[0].transform.position);
                    break;
                }
            }
        }
    }

    public void changeGiveMode()
    {
        if (!lastActiveUnit.moving)
        {
            lastActiveUnit.giveMode();
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

    public void switchPlayerUnitMode()
    {
        if (!lastActiveUnit.moving)
        {
            lastActiveUnit.switchActionMode();
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
                lastActiveUnit = unit;
            }
        }
        return unit;
    }

    private Enemy getHoveredEnemy(int x, int z)
    {
        Enemy unit = null;
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if ((int)enemyUnits[i].transform.position.x == x && (int)enemyUnits[i].transform.position.z == z + 1) //+1 hack beacuse float is rounded to bigger value. Must improve TODO
            {
                unit = enemyUnits[i];
            }
        }
        return unit;
    }

    public void nextTurn()
    {
        endTurn = false;
        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].resetAfterTurn();
        }
        lastActiveUnit.setPlayerUnitActive();
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].resetAfterTurn();
        }
        playerTurn = true;
        turnNumber++;
        playerUI.updateTurn(turnNumber);
        playerUI.IsOpen = true;

    }

    public void endPlayerTurn()
    {
        if (!lastActiveUnit.moving)
        {
            deactivatePlayerUnits();
            playerTurn = false;
            enemyTurn = true;
            playerUI.IsOpen = false;
        }
    }


    public void inventoryWindowService()
    {
        if (!inventory.equipment.enabled)
        {
            lastActiveUnit.DestroyWeaponHiglights();
            itemCreator.createItems(lastActiveUnit.weapons.ConvertAll(x => (Item)x));
        }
        else
        {
            itemCreator.destroyItems();
            lastActiveUnit.showAllowedMovements();
        }
        inventory.changeCanvasEnabled();
    }

    public void showWeapons()
    {
        if (!itemCreator.isWeapon)
        {
            itemCreator.destroyItems();
            itemCreator.createItems(lastActiveUnit.weapons.ConvertAll(x => (Item)x));
        }
    }

    public void showHealingItems()
    {
        if (!itemCreator.isHealingItem)
        {
            itemCreator.destroyItems();
            itemCreator.createItems(lastActiveUnit.healingItems.ConvertAll(x => (Item)x));
        }
    }


    private void performEnemyTurn()
    {
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (!enemyUnits[i].turnInProgress)
            {
                if ( i > 0 && enemyUnits[i-1].turnDone)
                {
                    if (enemyUnits[i].type == 1)
                    {
                        enemyUnits[i].performTurn();
                    }
                }
                else if (i == 0)
                {
                    enemyUnits[i].performTurn();
                }
                
            }
            
        }
    }
}
