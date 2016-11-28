using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMapBuilder))]
public class BattleGroundController : MonoBehaviour {

    TileMapBuilder _tileMapBuilder;
    public GameObject _playerUnit;

    void Start () {
        _tileMapBuilder = GetComponent<TileMapBuilder>();
        createBattleGround(30, 30);
	}
	
	void Update () {
	
	}

    public void createBattleGround(int wdith, int height)
    {
        _tileMapBuilder.BuildMesh(wdith, height);

        //Create units (player units and enemies)
        GameObject playerUnit = Instantiate(_playerUnit) as GameObject;
        PlayerUnitController playerUnitController = playerUnit.GetComponent<PlayerUnitController>();

        playerUnitController.createPlayerUnit(5, -5, 5);

        //playerUnitController.createPlayerUnit(10, -10, 3);


        //Create interface


        //Set Camera
    }


}
