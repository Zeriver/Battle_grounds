using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

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
    public GameObject _dialogUI;
    public GameObject _itemCreator;
    public GameObject _inventory;
    public GameObject _menu;
    public GameObject _unitInfo;
    public Text playerUIHealth;

    public AudioSource _AudioSource;

    public AudioClip playerMusic;
    public AudioClip enemyMusic;

    //enemies
    public GameObject _impaler;
    public GameObject _devoured;
    public GameObject _salamand;
    public GameObject _tuber;

    private PlayerUI playerUI;
    private ItemCreator itemCreator;
    private Inventory inventory;
    private UnitInfoPanel unitInfo;
    public DialogController dialogController;
    public MenuController menuController;
    public List<PlayerUnitController> playerUnits = new List<PlayerUnitController>();
    public List<Enemy> enemyUnits = new List<Enemy>();
    public PlayerUnitController lastActiveUnit;

    private bool playerTurn, enemyTurn, allyTurn, endTurn, enemyHealthUpdate, playerHealthUpdate, initializeNewEnemy, newEnemiesInitialized, dialog;
    private int turnNumber;

    private string[][] dialogs;
    private string dialogCache = "empty";

    void Start()
    {
        _tileMapBuilder = GetComponent<TileMapBuilder>();
        _mouseHighlight = GetComponent<MouseHighlight>();
        _cameraController = GetComponent<CameraController>();
        playerUI = _playerUI.GetComponent("PlayerUI") as PlayerUI;
        dialogController = _dialogUI.GetComponent("DialogController") as DialogController;
        itemCreator = _itemCreator.GetComponent("ItemCreator") as ItemCreator;
        inventory = _inventory.GetComponent("Inventory") as Inventory;
        menuController = _menu.GetComponent("MenuController") as MenuController;
        unitInfo = _unitInfo.GetComponent("UnitInfoPanel") as UnitInfoPanel;
        createBattleGround(25, 25);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuWindowService();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dialog = true;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            getDialogForCode("T1");
        }
        if (dialog)
        {
            if (!checkDialogs())
            {
                dialog = false;
                lastActiveUnit.setPlayerUnitActive();
                _cameraController.setCameraToActiveUnit(lastActiveUnit.transform.position);
            }
        }
        if (!menuController.menu.enabled && !dialogController.canvas.enabled)
        {
            if (playerHealthUpdate)
            {
                for (int i = 0; i < playerUnits.Count; i++)
                {
                    playerUnits[i].updateHealthModifiers();
                }
                playerHealthUpdate = false;
                playerTurn = true;
                _AudioSource.clip = playerMusic;
                _AudioSource.Play();
            }
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
                            lastActiveUnit.destroyMovementHiglights();
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
                                lastActiveUnit.showAllowedMovements();
                            }
                        }
                    }
                }
            }
            if (enemyHealthUpdate)
            {
                for (int i = 0; i < enemyUnits.Count; i++)
                {
                    enemyUnits[i].updateHealthModifiers();
                }
                enemyHealthUpdate = false;
                enemyTurn = true;
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
                initializeNewEnemy = true;
            }
            if (checkWinCondition())
            {
                // end game TODO
            }
            if (initializeNewEnemy)
            {
                if (enterNewEnemies())
                {
                    initializeNewEnemy = false;
                    endTurn = true;
                }
            }
            if (endTurn)
            {
                nextTurn();
            }
        }
    }

    public void createBattleGround(int wdith, int height)
    {
        turnNumber = 1;
        _tileMapBuilder.BuildMesh(wdith, height);
        string fileLevel = "MP2";
        dialogs = FileReader.readMapDialogs(Application.dataPath + "/Maps/" + fileLevel + "/dialog.txt");

        //Create units (player units and enemies)

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
        for (int i = 0; i < 6; i++)
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
            else if (i == 2)
            {
                enemyUnits.Add(Instantiate(_impaler).GetComponent<Impaler>());
                enemyUnits[i].createImpaler(20, 6, "down");
            }
            else if (i == 3)
            {
                enemyUnits.Add(Instantiate(_devoured).GetComponent<Devoured>());
                enemyUnits[i].createDevoured(15, 13, "down");
            } 
            else if (i == 4)
            {
                enemyUnits.Add(Instantiate(_salamand).GetComponent<Salamand>());
                enemyUnits[i].createSalamand(20, 18, "left");
            }
            else if (i == 5)
            {
                enemyUnits.Add(Instantiate(_tuber).GetComponent<Tuber>());
                enemyUnits[i].createTuber(10, 18, "left");
            }
        }

        lastActiveUnit.setPlayerUnitActive();
        playerUIHealth.text = lastActiveUnit.health.ToString() + " HP";
        _cameraController.setCameraToActiveUnit(lastActiveUnit.transform.position);
        HealthPopUpController.Initialize();
        playerHealthUpdate = true;
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

    private bool checkDialogs()
    {
        if (dialogCache != null && !dialogCache.Equals("empty"))
        {
            int charLocation = dialogCache.IndexOf(";", StringComparison.Ordinal);
            if (charLocation > 0)
            {
                dialogController.setNewText(dialogCache.Substring(0, charLocation));
                dialogCache = dialogCache.Remove(0, dialogCache.Substring(0, charLocation).Length + 1);
                return false;
            }
            else
            {
                dialogCache = "empty";
                dialogController.changeDialogCanvasEnabled();
                playerUI.IsOpen = true;
                return false;
            }
        }
        else
        {
            for (int i = 0; i < dialogs.GetLength(0); i++)
            {
                if (dialogs[i][0].Equals(turnNumber.ToString()))
                {
                    dialogs[i][0] += "- READ";
                    dialogCache = dialogs[i][1];
                    dialogController.changeDialogCanvasEnabled();
                    playerUI.IsOpen = false;
                    lastActiveUnit.deactivatePlayerUnit();
                    checkDialogs();
                }
            }
        }
        return false;
    }

    private void getDialogForCode(string code)
    {
        for (int i = 0; i < dialogs.GetLength(0); i++)
        {
            if (dialogs[i][0].Equals(code))
            {
                dialogs[i][0] += "- READ";
                dialogCache = dialogs[i][1];
                dialogController.changeDialogCanvasEnabled();
                playerUI.IsOpen = false;
                checkDialogs();
            }
        }
        Debug.Log("Warning: dialog code not found!");
    }

    private bool enterNewEnemies()
    {
        if (!newEnemiesInitialized)
        {
            if (turnNumber == 2)
            {
                newEnemiesInitialized = true;
                enemyUnits.Add(Instantiate(_impaler).GetComponent<Impaler>());
                enemyUnits[enemyUnits.Count - 1].createImpaler(2, 2, "down");
                enemyUnits[enemyUnits.Count - 1].enterBattleground(4, 13);
            }
            if (turnNumber == 3)
            {
                newEnemiesInitialized = true;
                enemyUnits.Add(Instantiate(_impaler).GetComponent<Impaler>());
                enemyUnits[enemyUnits.Count - 1].createImpaler(2, 2, "down");
                enemyUnits[enemyUnits.Count - 1].enterBattleground(4, 13);
                //enemyUnits.Add(Instantiate(_impaler).GetComponent<Impaler>());
                //enemyUnits[enemyUnits.Count - 1].createImpaler(4, 4, "down");
                //enemyUnits[enemyUnits.Count - 1].enterBattleground(4, 25);
            }
        }

        bool newEnemiesReady = true;
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (!enemyUnits[i].turnDone)
            {
                newEnemiesReady = false;
            }
        }
        return newEnemiesReady;
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
        newEnemiesInitialized = false;
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
        _cameraController.setCameraToActiveUnit(lastActiveUnit.transform.position);
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].resetAfterTurn();
        }
        playerHealthUpdate = true;
        dialog = true;
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
            enemyHealthUpdate = true;
            playerUI.IsOpen = false;
            _AudioSource.clip = enemyMusic;
            _AudioSource.Play();
        }
    }


    public void inventoryWindowService()
    {
        if (!inventory.equipment.enabled)
        {
            if (lastActiveUnit.isActionMode)
            {
                lastActiveUnit.switchActionMode();
            }
            itemCreator.createItems(lastActiveUnit.weapons.ConvertAll(x => (Item)x));
        }
        else
        {
            itemCreator.destroyItems();
            lastActiveUnit.showAllowedMovements();
        }
        inventory.changeCanvasEnabled();
    }

    public void menuWindowService()
    {
        menuController.changeCanvasEnabled();
    }

    public void restartService()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void saveGameService()
    {

    }

    public void loadGameService()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                    _cameraController.setCameraToActiveUnit(enemyUnits[i].transform.position);
                    enemyUnits[i].performTurn();
                }
                else if (i == 0)
                {
                    _cameraController.setCameraToActiveUnit(enemyUnits[i].transform.position);
                    enemyUnits[i].performTurn();
                }
            }
        }
    }
}
