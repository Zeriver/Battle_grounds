using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    private GameObject battleGroundObject;
    private Canvas inventoryCanvas;
    private bool showMoves;
    private bool turningToTarget;
    private bool attack;

    private Vector3 coordinates;
    private bool moving;
    private bool isActionUsed;
    private List<Weapon> weapons;
    private List<HealingItem> healingItems;
    private Item currentItem;
    private string facingDirection;
    private List<Vector3> positions;
    private int type;
    private List<Vector3> additionalPositions;

    private List<GameObject> movementHighlights;
    private List<GameObject> weaponHighlights;
    private List<GameObject> weaponAreaEffectHighlights;
    private List<Vector3> positionQueue;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private TileMapBuilder tileMapBuilder;
    private BattleGroundController battleGroundController;
    private List<UnitController> targets;
    private List<Obstacle> obstaclesToAttack;
    private string currentEffect;
    private List<int> healthEffects;
    private List<WeaponSkill> weaponSkills;


    // units parameters

    private int maxHealth;
    private int health;

    private int fireResistance = 0;
    private int freezeResistance = 0;
    private int poisonResistance = 0;
    private int defendBonus = 0;

    private float moveSpeed;
    private int movesLeft;
    private bool defending;
    private int maxMovement;












    protected GameObject BattleGroundObject
    {
        get
        {
            return battleGroundObject;
        }

        set
        {
            battleGroundObject = value;
        }
    }

    protected Canvas InventoryCanvas
    {
        get
        {
            return inventoryCanvas;
        }

        set
        {
            inventoryCanvas = value;
        }
    }

    protected bool ShowMoves
    {
        get
        {
            return showMoves;
        }

        set
        {
            showMoves = value;
        }
    }

    protected bool TurningToTarget
    {
        get
        {
            return turningToTarget;
        }

        set
        {
            turningToTarget = value;
        }
    }

    protected bool Attack
    {
        get
        {
            return attack;
        }

        set
        {
            attack = value;
        }
    }

    public Vector3 Coordinates
    {
        get
        {
            return coordinates;
        }

        set
        {
            coordinates = value;
        }
    }

    public bool Moving
    {
        get
        {
            return moving;
        }

        set
        {
            moving = value;
        }
    }

    public bool IsActionUsed
    {
        get
        {
            return isActionUsed;
        }

        set
        {
            isActionUsed = value;
        }
    }

    public List<Weapon> Weapons
    {
        get
        {
            return weapons;
        }

        set
        {
            weapons = value;
        }
    }

    public List<HealingItem> HealingItems
    {
        get
        {
            return healingItems;
        }

        set
        {
            healingItems = value;
        }
    }

    public Item CurrentItem
    {
        get
        {
            return currentItem;
        }

        set
        {
            currentItem = value;
        }
    }

    public string FacingDirection
    {
        get
        {
            return facingDirection;
        }

        set
        {
            facingDirection = value;
        }
    }

    public List<Vector3> Positions
    {
        get
        {
            return positions;
        }

        set
        {
            positions = value;
        }
    }

    public int Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public List<Vector3> AdditionalPositions
    {
        get
        {
            return additionalPositions;
        }

        set
        {
            additionalPositions = value;
        }
    }

    protected List<GameObject> MovementHighlights
    {
        get
        {
            return movementHighlights;
        }

        set
        {
            movementHighlights = value;
        }
    }

    protected List<GameObject> WeaponHighlights
    {
        get
        {
            return weaponHighlights;
        }

        set
        {
            weaponHighlights = value;
        }
    }

    protected List<GameObject> WeaponAreaEffectHighlights
    {
        get
        {
            return weaponAreaEffectHighlights;
        }

        set
        {
            weaponAreaEffectHighlights = value;
        }
    }

    protected List<Vector3> PositionQueue
    {
        get
        {
            return positionQueue;
        }

        set
        {
            positionQueue = value;
        }
    }

    protected Quaternion TargetRotation
    {
        get
        {
            return targetRotation;
        }

        set
        {
            targetRotation = value;
        }
    }

    protected Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }

        set
        {
            targetPosition = value;
        }
    }

    protected TileMapBuilder TileMapBuilder
    {
        get
        {
            return tileMapBuilder;
        }

        set
        {
            tileMapBuilder = value;
        }
    }

    protected BattleGroundController BattleGroundController
    {
        get
        {
            return battleGroundController;
        }

        set
        {
            battleGroundController = value;
        }
    }

    protected List<UnitController> Targets
    {
        get
        {
            return targets;
        }

        set
        {
            targets = value;
        }
    }

    protected List<Obstacle> ObstaclesToAttack
    {
        get
        {
            return obstaclesToAttack;
        }

        set
        {
            obstaclesToAttack = value;
        }
    }

    protected string CurrentEffect
    {
        get
        {
            return currentEffect;
        }

        set
        {
            currentEffect = value;
        }
    }

    protected List<int> HealthEffects
    {
        get
        {
            return healthEffects;
        }

        set
        {
            healthEffects = value;
        }
    }

    protected List<WeaponSkill> WeaponSkills
    {
        get
        {
            return weaponSkills;
        }

        set
        {
            weaponSkills = value;
        }
    }

    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }

        set
        {
            maxHealth = value;
        }
    }

    public int Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;
        }
    }

    protected int FireResistance
    {
        get
        {
            return fireResistance;
        }

        set
        {
            fireResistance = value;
        }
    }

    protected int FreezeResistance
    {
        get
        {
            return freezeResistance;
        }

        set
        {
            freezeResistance = value;
        }
    }

    protected int PoisonResistance
    {
        get
        {
            return poisonResistance;
        }

        set
        {
            poisonResistance = value;
        }
    }

    protected int DefendBonus
    {
        get
        {
            return defendBonus;
        }

        set
        {
            defendBonus = value;
        }
    }

    protected float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }

        set
        {
            moveSpeed = value;
        }
    }

    protected int MaxMovement
    {
        get
        {
            return maxMovement;
        }

        set
        {
            maxMovement = value;
        }
    }

    protected int MovesLeft
    {
        get
        {
            return movesLeft;
        }

        set
        {
            movesLeft = value;
        }
    }

    protected bool Defending
    {
        get
        {
            return defending;
        }

        set
        {
            defending = value;
        }
    }
}
