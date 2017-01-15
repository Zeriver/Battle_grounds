using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileMapBuilder))]
[RequireComponent(typeof(MouseHighlight))]
[RequireComponent(typeof(CameraController))]
public class BattleGroundController : MonoBehaviour
{

    TileMapBuilder _tileMapBuilder;
    MouseHighlight _mouseHighlight;
    CameraController _cameraController;

    public GameObject _playerUnit;
    public GameObject _playerUI;
    public GameObject _itemCreator;
    public GameObject _inventory;
    public GameObject _unitInfo;
    public Text playerUIHealth;

    //enemies
    public GameObject _impaler;
    public GameObject _devoured;
    public GameObject _salamand;

    private PlayerUI playerUI;
    private ItemCreator itemCreator;
    private Inventory inventory;
    private UnitInfoPanel unitInfo;
    public List<PlayerUnitController> playerUnits = new List<PlayerUnitController>();
    public List<Enemy> enemyUnits = new List<Enemy>();
    public PlayerUnitController lastActiveUnit;

    private bool playerTurn, enemyTurn, allyTurn, endTurn;
    private int turnNumber;

    void Start()
    {
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

    void Update()
    {
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
                Transform mousePosition = _mouseHighlight.getHighlightSelection();
                if (Input.GetMouseButtonDown(0))     /// LEFT CLICK
                {
                    if (lastActiveUnit.giveHighlights.Count == 0 && !lastActiveUnit.isActionMode)
                    {
                        PlayerUnitController clickedUnit = getClickedUnit((int)mousePosition.position.x, (int)mousePosition.position.z);
                        if (clickedUnit != null)
                        {
                            deactivatePlayerUnits();
                            clickedUnit.setPlayerUnitActive();
                            lastActiveUnit = clickedUnit;
                            playerUIHealth.text = lastActiveUnit.health.ToString() + " HP";
                            _cameraController.setCameraToActiveUnit(clickedUnit.transform.position);
                        }
                        Tile clickedTile = TileMap.getTile((int)mousePosition.position.x, (int)mousePosition.position.z);
                    }
                }
                if (!lastActiveUnit.isActionMode)
                {
                    Enemy enemy = getHoveredEnemy((int)mousePosition.position.x, (int)mousePosition.position.z);
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
            //Debug.Log("Enemy turn in progress...");
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
        if (checkLoseCondition())
        {
            // reset / end game TODO
        }
        if (allyTurn)
        {
            allyTurn = false;
            endTurn = true;
        }
        if (checkWinCondition())
        {
            // end game TODO
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


        for (int i = 0; i < 3; i++)  //Need to load number of players from scenario/something
        {
            playerUnits.Add(Instantiate(_playerUnit).GetComponent<PlayerUnitController>());
            if (i == 0)                             // make better loading position for scenarios TODO
            {
                playerUnits[i].createPlayerUnit(5, 5, 5, "up");
                lastActiveUnit = playerUnits[i];
            }
            else if (i == 1)
            {
                playerUnits[i].createPlayerUnit(16, 9, 5, "down");
            }
            else if (i == 2)
            {
                playerUnits[i].createPlayerUnit(8, 15, 5, "right");
            }

        }

        GameObject impaler = _impaler as GameObject;
        Impaler impalerController = impaler.GetComponent<Impaler>();

        GameObject devoured = _devoured as GameObject;
        Devoured devouredController = devoured.GetComponent<Devoured>();

        GameObject salamand = _salamand as GameObject;
        Salamand salamandController = salamand.GetComponent<Salamand>();

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                enemyUnits.Add(Instantiate(_salamand).GetComponent<Salamand>());
                enemyUnits[i].createSalamand(18, 3, "up");
            }
            else if (i == 1)
            {
                enemyUnits.Add(Instantiate(_impaler).GetComponent<Impaler>());
                enemyUnits[i].createImpaler(9, 6, "down");
            }
            else if(i == 2)
            {
                enemyUnits.Add(Instantiate(_impaler).GetComponent<Impaler>());
                enemyUnits[i].createImpaler(20, 6, "down");
            }
            else if(i == 3)
            {
                enemyUnits.Add(Instantiate(_devoured).GetComponent<Devoured>());
                enemyUnits[i].createDevoured(15, 13, "down");
            }
        }

        lastActiveUnit.setPlayerUnitActive();
        playerUIHealth.text = lastActiveUnit.health.ToString() + " HP";
        _cameraController.setCameraToActiveUnit(lastActiveUnit.transform.position);
        playerTurn = true;
    }

    private bool checkWinCondition() // add more conditions based on mission objectives TODO
    {
        if (enemyUnits.Count == 0)
        {
            return true;
        }
        return false;
    }

    private bool checkLoseCondition() // add more conditions based on mission objectives TODO
    {
        if (playerUnits.Count == 0)
        {
            return true;
        }
        return false;
    }

    private void setNextUnitActive()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (playerUnits[i].isSelected)
            {
                playerUnits[i].deactivatePlayerUnit();
                if (i + 1 != playerUnits.Count)
                {
                    playerUnits[i + 1].setPlayerUnitActive();
                    lastActiveUnit = playerUnits[i + 1];
                    _cameraController.setCameraToActiveUnit(playerUnits[i + 1].transform.position);
                    playerUIHealth.text = lastActiveUnit.health.ToString() + " HP";
                    break;
                }
                else
                {
                    playerUnits[0].setPlayerUnitActive();
                    lastActiveUnit = playerUnits[0];
                    _cameraController.setCameraToActiveUnit(playerUnits[0].transform.position);
                    playerUIHealth.text = lastActiveUnit.health.ToString() + " HP";
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

    public void changePushMode()
    {
        if (!lastActiveUnit.moving && !lastActiveUnit.isActionUsed)
        {
            lastActiveUnit.pushMode();
        }
    }

    public void defendPosition()
    {
        if (!lastActiveUnit.moving && !lastActiveUnit.isActionUsed)
        {
            lastActiveUnit.defendingPosition();
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
            if ((int)playerUnits[i].transform.position.x == x && (int)playerUnits[i].transform.position.z == z + 1) //+1 hack beacuse float is rounded to bigger value. Must improve TODO
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
            for (int j = 0; j < enemyUnits[i].positions.Count; j++)
            {
                if ((int)enemyUnits[i].positions[j].x == x && (int)enemyUnits[i].positions[j].z == z + 1) //+1 hack beacuse float is rounded to bigger value. Must improve TODO
                {
                    unit = enemyUnits[i];
                    break;
                }
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
        if (lastActiveUnit.health <= 0)
        {
            if (playerUnits.Count != 0)
            {
                lastActiveUnit = playerUnits[0];
            }
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
        playerUIHealth.text = lastActiveUnit.health.ToString() + " HP";
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
                if (i > 0 && enemyUnits[i - 1].turnDone)
                {
                    enemyUnits[i].performTurn();
                }
                else if (i == 0)
                {
                    enemyUnits[i].performTurn();
                }

            }

        }
    }
}
