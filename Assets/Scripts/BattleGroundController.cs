using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMapBuilder))]
[RequireComponent(typeof(PlayerUnitController))]
public class BattleGroundController : MonoBehaviour {

    TileMapBuilder _tileMapBuilder;
    PlayerUnitController _playerUnitController;

    void Start () {
        _tileMapBuilder = GetComponent<TileMapBuilder>();
        _playerUnitController = GetComponent<PlayerUnitController>();
        createBattleGround(30, 30);
	}
	
	void Update () {
	
	}

    public void createBattleGround(int wdith, int height)
    {
        _tileMapBuilder.BuildMesh(wdith, height);

        //Create units (player units and enemies)
        _playerUnitController.createPlayerUnit(5, -5, 5);

        
        //Create interface


        //Set Camera
    }


}
