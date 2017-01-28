using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    protected GameObject BattleGroundObject;
    protected Canvas InventoryCanvas;
    protected bool showMoves;
    protected bool turningToTarget;
    protected bool attack;

    public Vector3 coordinates;
    public bool moving;
    public bool isActionUsed;
    public List<Weapon> weapons;
    public List<HealingItem> healingItems;
    public Item currentItem;
    public string facingDirection;
    public List<Vector3> positions;
    public int type;
    public List<Vector3> additionalPositions;

    protected List<GameObject> movementHighlights;
    protected List<GameObject> weaponHighlights;
    protected List<GameObject> weaponAreaEffectHighlights;
    protected List<Vector3> positionQueue;
    protected Quaternion targetRotation;
    protected Vector3 targetPosition;
    protected TileMapBuilder _tileMapBuilder;
    protected BattleGroundController _battleGroundController;
    protected List<Unit> targets;
    protected List<Obstacle> obstaclesToAttack;
    protected string currentEffect;
    protected List<int> healthEffects;
    protected List<WeaponSkill> weaponSkills;


    // units parameters

    public int maxHealth;
    public int health;

    protected int fireResistance = 0;
    protected int freezeResistance = 0;
    protected int poisonResistance = 0;
    protected int defendBonus = 0;

    protected float moveSpeed;
    protected int maxMovement, movesLeft;
    protected bool defending;

    void Start () {
	
	}
	
	void Update () {
	
	}

    public Tile getUnitTile()
    {
        return TileMap.getTile((int)coordinates.x, (int)coordinates.z);
    }

    public void getAttacked(Weapon weapon, Unit attacker, int bonusDamage)
    {
        float finalDamage = weapon.damage + bonusDamage + calculateFlankDamage(attacker);
        if (weapon.damageType.Equals("fire") )
        {
            if (fireResistance == 100)
            {
                return; //Should tell player that unit is resistant to fire
            }
            finalDamage -= fireResistance * 0.01f;
        }
        if (weapon.damageType.Equals("poison"))
        {
            if (poisonResistance == 100)
            {
                return;
            }
            finalDamage -= poisonResistance * 0.01f;
        }
        if (weapon.damageType.Equals("freeze"))
        {
            if (freezeResistance == 100)
            {
                return;
            }
            finalDamage -= freezeResistance * 0.01f;
        }
        if (defending)
        {
            finalDamage -= finalDamage * 0.35f;
        }
        finalDamage -= defendBonus * 0.01f;
        health = health - (int)finalDamage;
        _battleGroundController.playerUIHealth.text = _battleGroundController.lastActiveUnit.health.ToString() + " HP";
        HealthPopUpController.createPopUpText(((int)finalDamage).ToString(), transform, true);
        if (health <= 0)
        {
            _battleGroundController.playerUIHealth.text = 0 + " HP";
            killUnit();
        }

        if (weapon.damageType.Equals("fire"))
        {
            if (currentEffect.Contains("freeze"))
            {
                currentEffect = "none";
            }
            else
            {
                healthEffects.Clear();
                for (int i = 0; i < weapon.nextTurnsDamage.Count; i++)
                {
                    healthEffects.Add(weapon.nextTurnsDamage[i] - (int)(fireResistance * 0.01f));
                }
            }
        }
        else if (weapon.damageType.Equals("freeze"))
        {

        }
        else if (weapon.damageType.Equals("poison"))
        {

        }
    }

    private int calculateFlankDamage(Unit attacker)
    {
        int damage = 0;
        if (facingDirection.Equals("up") || facingDirection.Equals("down"))
        {
            if ((attacker.coordinates.x < coordinates.x && facingDirection.Equals("up")) || (attacker.coordinates.x > coordinates.x && facingDirection.Equals("down")))
            {
                damage = 8;
            }
            else if (attacker.coordinates.x == coordinates.x)
            {
                damage = 4;
            }
        }
        else if (facingDirection.Equals("right") || facingDirection.Equals("left"))
        {
            if ((attacker.coordinates.z > coordinates.z && facingDirection.Equals("right")) || (attacker.coordinates.z < coordinates.z && facingDirection.Equals("left")))
            {
                damage = 8;
            }
            else if (attacker.coordinates.z == coordinates.z)
            {
                damage = 4;
            }
        }
        return damage;
    }

    protected int getBonusDamageFromWeaponSkill()
    {
        int bonusDamage = -1;
        for (int i = 0; i < weaponSkills.Count; i++)
        {
            if (weaponSkills[i].name.Equals(currentItem.name))
            {
                bonusDamage = weaponSkills[i].level * 2;
            }
        }
        if (bonusDamage == -1)
        {
            bonusDamage = 0;
            weaponSkills.Add(new WeaponSkill(currentItem.name));
        }
        return bonusDamage;
    }

    public void getHealed(HealingItem healingItem)
    {
        int healing = 0;
        if (health + healingItem.healingPoints > maxHealth)
        {
            healing = maxHealth - health;
        }
        else
        {
            healing = healingItem.healingPoints;
        }
        health += healing;
        _battleGroundController.playerUIHealth.text = _battleGroundController.lastActiveUnit.health.ToString() + " HP";
        HealthPopUpController.createPopUpText(healing.ToString(), transform, false);
    }

    protected void weaponSkillUpgrade()
    {
        for (int i = 0; i < weaponSkills.Count; i++)
        {
            if (weaponSkills[i].name.Equals(currentItem.name))
            {
                weaponSkills[i].experience++;
                if (weaponSkills[i].experience >= weaponSkills[i].levelRequirements[weaponSkills[i].level])
                {
                    weaponSkills[i].level++;
                }
            }
        }
    }

    public void killUnit()
    {
        TileMap.setTileWalkable((int)coordinates.x, (int)coordinates.z);
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 90.0f));
        if (this is Enemy)
        {
            _battleGroundController.enemyUnits.Remove((Enemy)this);
        }
        else if (this is PlayerUnitController)
        {
            _battleGroundController.playerUnits.Remove((PlayerUnitController)this);
        }
    }

    protected GameObject createPlane(int x, int z, Color color)   // needs to create mesh from sratch for better performance TODO
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);
        plane.transform.position = new Vector3(x, 0.05f, z) * _tileMapBuilder.tileSize;
        plane.transform.position = new Vector3(plane.transform.position.x + 0.5f, plane.transform.position.y, plane.transform.position.z + 0.5f);
        plane.GetComponent<Renderer>().material.color = color;
        plane.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
        return plane;
    }

    protected void moveToNextStep(float offset)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * 2.2f * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(targetPosition, transform.position) <= 0.01f)
        {
            coordinates = positionQueue[0];
            transform.position = targetPosition;
            setPositions();
            positionQueue.RemoveAt(0);
            moving = false;
        }
        if (positionQueue.Count == 0)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                TileMap.setTileNotWalkable((int)positions[i].x, (int)positions[i].z - 1);
            }
            if (this is PlayerUnitController)
            {
                for (int i = 0; i < _battleGroundController.playerUnits.Count; i++)
                {
                    _battleGroundController.playerUnits[i].calculateDefendBonus(); // should check performance TODO
                }
            }
        }
    }


    protected void setNextStep(Vector3 [] rotations)
    {
        if (positionQueue[0].x > coordinates.x)
        {
            turnCorrectWay(facingDirection = "up", rotations);
            targetPosition = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
            if (type == 3)
            {
                additionalPositions[0] = transform.position;
            }
        }
        else if (positionQueue[0].x < coordinates.x)
        {
            turnCorrectWay(facingDirection = "down", rotations);
            targetPosition = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z);
            if (type == 3)
            {
                additionalPositions[0] = transform.position;
            }
        }
        else if (positionQueue[0].z < coordinates.z)
        {
            turnCorrectWay(facingDirection = "right", rotations);
            targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f);
            if (type == 3)
            {
                additionalPositions[0] = transform.position;
            }
        }
        else if (positionQueue[0].z > coordinates.z)
        {
            turnCorrectWay(facingDirection = "left", rotations);
            targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);
            if (type == 3)
            {
                additionalPositions[0] = transform.position;
            }
        }
        moving = true;
    }

    protected void turnCorrectWay(string direction, Vector3[] rotations)
    {
        switch (direction)
        {
            case "up":
                targetRotation = Quaternion.Euler(rotations[0]);
                break;
            case "right":
                targetRotation = Quaternion.Euler(rotations[2]);
                break;
            case "down":
                targetRotation = Quaternion.Euler(rotations[1]);
                break;
            case "left":
                targetRotation = Quaternion.Euler(rotations[3]);
                break;
        }
    }

    protected void turnToEnemy()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, moveSpeed * 80f * Time.deltaTime);
        if (Quaternion.Angle(transform.rotation, targetRotation) < 5.0f)
        {
            transform.rotation = targetRotation;
            turningToTarget = false;
            attack = true;
        }
    }

    protected void setPositions()
    {
        positions.Clear();
        positions.Add(transform.position);
        if (additionalPositions != null)
        {
            for (int i = 0; i < additionalPositions.Count; i++)
            {
                positions.Add(additionalPositions[i]);
            }
        }
    }

    protected void calculateDefendBonus()
    {
        defendBonus = 0;
        List<Tile> nearbyTiles = TileMap.GetListOfAdjacentTiles((int)coordinates.x, (int)coordinates.z);
        for (int i = 0; i < nearbyTiles.Count; i++)
        {
            for (int j = 0; j < _battleGroundController.playerUnits.Count; j++)
            {
                if (nearbyTiles[i].Equals(_battleGroundController.playerUnits[j].getUnitTile()))
                {
                    defendBonus += 13;
                }
            }
        }
    }

    protected void standardReset()
    {
        movesLeft = maxMovement;
    }

    public void updateHealthModifiers()
    {
        if (healthEffects.Count > 0)
        {
            health -= healthEffects[0];
            HealthPopUpController.createPopUpText(healthEffects[0].ToString(), transform, true);
            if (health <= 0)
            {
                killUnit();
            }
            healthEffects.RemoveAt(0);
        }
    }

}
