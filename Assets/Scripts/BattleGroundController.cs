using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileMapBuilder))]
public class BattleGroundController : MonoBehaviour {

    TileMapBuilder _tileMapBuilder;
    public GameObject _playerUnit;

    private List<PlayerUnitController> playerUnits = new List<PlayerUnitController>();

    void Start () {
        _tileMapBuilder = GetComponent<TileMapBuilder>();
        createBattleGround(30, 30);
	}
	
	void Update () {
        foreach (PlayerUnitController unit in playerUnits)
        {

        }
    }

    public void createBattleGround(int wdith, int height)
    {
        _tileMapBuilder.BuildMesh(wdith, height);

        //Create units (player units and enemies)
        GameObject playerUnit = Instantiate(_playerUnit) as GameObject;
        PlayerUnitController playerUnitController = playerUnit.GetComponent<PlayerUnitController>();


        for (int i = 0; i <3; i++)  //Need to load number of players from scenario/something
        {
            playerUnits.Add(Instantiate(_playerUnit).GetComponent<PlayerUnitController>());
            if (i == 0)                             // make better loading position for scenarios TODO
            {
                playerUnits[i].createPlayerUnit(5, -5, 5);
                playerUnits[i].setPlayerUnitActive();
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


        //Set Camera
    }


}
