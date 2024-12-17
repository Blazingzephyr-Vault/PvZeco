using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class ZombieBase : MonoBehaviour
{
    #region Major New Logic
	/// <summary>
	/// Whether this zombie can be stalled.
	/// </summary>
    protected bool canStall = true;

    /// <summary>
    /// Stall applied by Scaredy-Shroom.
    /// </summary>
    private float stallLevel;

    /// <summary>
    /// Apply stall and adjust the speed rate.
    /// </summary>
    public float StallLevel
    {
        get => stallLevel;
        private set
        {
            stallLevel = value;
            AdjustSpeedRate();
        }
    }

    /// <summary>
    /// Reset the colors and apply stall and freeze.
    /// </summary>
    private void AdjustSpeedRate()
	{
		SpeedRate = (1f - stallLevel) * (1f - FrozenLevel * 0.05f);
		ResetColor();
    }

    protected void ResetColor()
    {
        float r = 1f;
        float g = 1f;
        float b = 1f;
        float a = sprites[0].color.a;

        if (needHypnoPurple)
        {
            r *= 0.88f;
            g *= 0.39f;
        }

		if (frozenLevel > 0)
		{
			r *= 1f - frozenLevel * 0.055f;
			g *= 1f - frozenLevel * 0.05f;
            b *= 1f - frozenLevel * 0.005f;
        }

        if (stallLevel > 0)
        {
            r *= 1f - stallLevel * 0.7451f;
            g *= 1f - stallLevel * 1.749f;
            b *= 1f - stallLevel * 0.6f;
        }

        SetAllColor(new Color(r, g, b, a));
    }

    /// <summary>
    /// Stall duration coroutine.
    /// </summary>
    private Coroutine stallCoroutine;

    public void ApplyScaredyStall(float stall, float duration, bool isSyn = false)
    {
        if ((isSyn || !GameManager.Instance.isClient) && canStall)
        {
			StallLevel = stall;
			if (stallCoroutine != null) StopCoroutine(stallCoroutine);
			stallCoroutine = StartCoroutine(StallWearOff(stall, duration));

            if (GameManager.Instance.isServer)
            {
                SynItem synItem = new SynItem();
                synItem.OnlineId = OnlineId;
                synItem.Type = 2;
                synItem.SynCode[0] = 0;
                synItem.SynCode[1] = 5;
				synItem.Twofloat = new Vector2(stall, duration);
                SocketServer.Instance.SendSynBag(synItem);
            }
        }
    }

	protected IEnumerator StallWearOff(float stall, float duration)
    {
        float currStall = stall / 100;
        while (stallLevel > 0)
        {
            yield return new WaitForSeconds(duration / 100f);
			StallLevel -= currStall;
        }
    }

    private void RemoveScaredyStall()
	{
		StallLevel = 0;
	}
    #endregion Edits

    public int OnlineId;

	public string PlacePlayer;

	private ZombieState state;

	protected Renderer REnderer;

	protected SwfClipController clipController;

	protected Animator animator;

	protected SortingGroup Sorting;

	protected List<SpriteRenderer> sprites = new List<SpriteRenderer>();

	private Grid currGrid;

	protected Grid nextGrid;

	private bool inWater;

	private bool going;

	protected SpriteRenderer spriteRenderer;

	public CapsuleCollider2D capsuleCollider2D;

	protected Grid AttackGrid;

	protected ZombieBase hypnoAttackTarget;

	public Vector2 MoveTarget;

	private bool isFacingLeft = true;

	private bool returnFirstClick;

	protected MapBase CurrMap;

	protected bool IsOVer;

	private bool isDropHead;

	public Vector2 SpawnPos;

	private Coroutine changeLineCoroutine;

	protected bool needHypnoPurple;

	public bool isHypno;

	private int frozenLevel;

	private bool isFrozen;

	protected bool canFrozen = true;

	private Coroutine frozenCoroutine;

	private bool isIcetrap;

	protected bool canIce = true;

	private SpriteRenderer Icetrap;

	private Coroutine icetrapCoroutine;

	private bool isButter;

	public SpriteRenderer butter;

	protected bool canButter = true;

	private Coroutine butterCoroutine;

	protected SpriteRenderer Shadow;

	public bool invincible;

	public Sprite inWaterSprite;

	public Sprite dropHeadSprite;

	public Sprite dropArmSprite;

	public SpriteRenderer Arm1Renderer;

	public SpriteRenderer Arm2Renderer;

	public SpriteRenderer Arm3Renderer;

	public List<SpriteRenderer> legSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> headSprites = new List<SpriteRenderer>();

	public Sprite normalHead;

	public Sprite yuckHead;

	public SpriteRenderer HeadRenderer;

	public SpriteRenderer JawRenderer;

	public bool anCanMove;

	protected bool dontChangeState;

	protected bool needInWater = true;

	protected bool needChangeLine = true;

	protected bool onlyBoomHurt;

	public float speed;

	private float speedRate = 1f;

	private int hp;

	private int doorHp;

	protected int MaxDoorHp;

	public SpriteRenderer EquipRenderer;

	private int currHpState;

	protected List<int> HpState = new List<int>();

	protected List<Sprite> E1HpStateSprite = new List<Sprite>();

	public SpriteRenderer DoorRenderer;

	private int currDoorHpState;

	protected List<int> DoorHpState = new List<int>();

	protected List<Sprite> DoorHpStateSprite = new List<Sprite>();

	private Coroutine BrightCoroutine;

	private Coroutine DoorBrightCoroutine;

	public virtual int OnlineIdNum { get; } = 1;

	public int SortOrder => Sorting.sortingOrder;

	protected virtual Vector2 LeftAttackDir { get; } = new Vector2(-1f, 0f);

	protected virtual float DropHeadScale { get; } = 0.85f;

	public virtual bool CanEatByChomper { get; } = true;

	protected abstract GameObject Prefab { get; }

	protected abstract float AnToSpeed { get; }

	protected abstract float DefSpeed { get; }

	protected abstract float attackValue { get; }

	public abstract int MaxHP { get; }

	public ZombieState State
	{
		get
		{
			return state;
		}
		set
		{
			if (state != ZombieState.Dead && (!dontChangeState || value == ZombieState.Dead || !LVManager.Instance.GameIsStart) && (!GameManager.Instance.isClient || value != ZombieState.Dead))
			{
				if (GameManager.Instance.isServer && value == ZombieState.Dead)
				{
					SynItem synItem = new SynItem();
					synItem.OnlineId = OnlineId;
					synItem.Type = 2;
					synItem.SynCode[0] = 1;
					synItem.SynCode[1] = 0;
					SocketServer.Instance.SendSynBag(synItem);
				}
				state = value;
				ResetAnimationSpeed();
				CheckState();
			}
		}
	}

	public Grid CurrGrid
	{
		get
		{
			return currGrid;
		}
		set
		{
			if (value != currGrid && value != null)
			{
				Grid lastGrid = currGrid;
				currGrid = value;
				nextGrid = MapManager.Instance.GetNextGrid(currGrid, !IsFacingLeft);
				ResetMoveTarget();
				CurrGridChangeEvent(lastGrid);
			}
		}
	}

	public int Hp
	{
		get
		{
			return hp;
		}
		protected set
		{
			hp = value;
			int num = -1;
			for (int i = 0; i < HpState.Count; i++)
			{
				if (hp <= HpState[i])
				{
					num = i;
				}
			}
			if (num != -1 && currHpState != num)
			{
				SpriteChangeEvent(E1HpStateSprite[num]);
				EquipRenderer.sprite = E1HpStateSprite[num];
				if (E1HpStateSprite[num] == null)
				{
					EquipRenderer.enabled = false;
				}
				currHpState = num;
			}
			if (hp == MaxHP)
			{
				if (E1HpStateSprite.Count != 0)
				{
					EquipRenderer.sprite = E1HpStateSprite[0];
					EquipRenderer.enabled = true;
				}
				else if (EquipRenderer != null)
				{
					EquipRenderer.enabled = false;
				}
			}
			if (hp <= 0 && !GameManager.Instance.isClient)
			{
				if (isButter)
				{
					UnButter();
					RemoveScaredyStall();
				}
				if (isIcetrap)
				{
					UnIce();
				}
				if (DoorHp > 0)
				{
					DoorHp = 0;
				}
				if (State != ZombieState.Dead)
				{
					State = ZombieState.Dead;
				}
			}
		}
	}

	protected int DoorHp
	{
		get
		{
			return doorHp;
		}
		set
		{
			if (ZombieManager.Instance.ZombieInvincible)
			{
				return;
			}
			if (value < 0)
			{
				Hp += value;
			}
			doorHp = value;
			int num = -1;
			for (int i = 0; i < DoorHpState.Count; i++)
			{
				if (doorHp <= DoorHpState[i])
				{
					num = i;
				}
			}
			if (num != -1 && currDoorHpState != num)
			{
				SpriteChangeEvent(DoorHpStateSprite[num]);
				DoorRenderer.sprite = DoorHpStateSprite[num];
				if (DoorHpStateSprite[num] == null)
				{
					DoorRenderer.enabled = false;
				}
				currDoorHpState = num;
			}
			if (doorHp == MaxDoorHp)
			{
				if (DoorHpStateSprite.Count != 0)
				{
					DoorRenderer.sprite = DoorHpStateSprite[0];
					DoorRenderer.enabled = true;
				}
				else if (DoorRenderer != null)
				{
					DoorRenderer.enabled = false;
				}
			}
		}
	}

	public bool InWater
	{
		get
		{
			return inWater;
		}
		protected set
		{
			inWater = value;
			anCanMove = true;
			if (inWater)
			{
				for (int i = 0; i < legSprites.Count; i++)
				{
					legSprites[i].gameObject.SetActive(value: false);
				}
			}
			else
			{
				for (int j = 0; j < legSprites.Count; j++)
				{
					legSprites[j].gameObject.SetActive(value: true);
				}
			}
			InWaterChangeEvent();
		}
	}

	public int CurrLine => CurrGrid.Point.y;

	protected float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			if (value != 0f)
			{
				speed = ((speed >= 0f) ? Mathf.Abs(value) : (0f - Mathf.Abs(value)));
				if (speed == 0f)
				{
					animator.speed = 0f;
				}
				else
				{
					ResetAnimationSpeed();
				}
			}
		}
	}

	protected float SpeedRate
	{
		get
		{
			return speedRate;
		}
		set
		{
			if (value != 0f)
			{
				speedRate = value;
				ResetAnimationSpeed();
			}
		}
	}

	public bool IsFacingLeft
	{
		get
		{
			return isFacingLeft;
		}
		set
		{
			if (isFacingLeft != value)
			{
				isFacingLeft = value;
				int num = 2;
				if (base.transform.localScale.x < 0f)
				{
					num = -2;
				}
				base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
				if (State != ZombieState.Dead)
				{
					State = ZombieState.Walk;
				}
				base.transform.position += new Vector3(Shadow.transform.localPosition.x * (float)num, 0f);
				nextGrid = MapManager.Instance.GetNextGrid(currGrid, !IsFacingLeft);
				ResetMoveTarget();
				ChangeFacingEvent();
			}
		}
	}

    public int FrozenLevel
    {
        get
        {
            return frozenLevel;
        }
        private set
        {
            frozenLevel = value;
            isFrozen = frozenLevel > 0;
            if (frozenLevel > 10)
            {
                frozenLevel = 10;
            }
            else if (frozenLevel < 0)
            {
                frozenLevel = 0;
            }
            if (frozenCoroutine != null)
            {
                StopCoroutine(frozenCoroutine);
            }
            if (FrozenLevel > 0)
            {
                frozenCoroutine = StartCoroutine(DoFuncWait(UnFrozen, 4.5f - (float)FrozenLevel * 0.4f));
            }
            AdjustSpeedRate();
        }
    }

    protected void ResetMoveTarget()
	{
		if (nextGrid == null)
		{
			if (IsFacingLeft)
			{
				MoveTarget = new Vector2(-50f, currGrid.Position.y);
			}
			else
			{
				MoveTarget = new Vector2(50f, currGrid.Position.y);
			}
		}
		else if (currGrid.isSlope)
		{
			if (nextGrid.isSlope)
			{
				MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, (currGrid.Position.y + nextGrid.Position.y) / 2f);
			}
			else
			{
				MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, nextGrid.Position.y);
			}
		}
		else if (nextGrid.isSlope)
		{
			MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, currGrid.Position.y);
		}
		else
		{
			MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, nextGrid.Position.y);
		}
		if (IsFacingLeft)
		{
			MoveTarget -= new Vector2(0.1f, 0f);
		}
		else
		{
			MoveTarget += new Vector2(0.1f, 0f);
		}
	}

	protected void ResetAnimationSpeed()
	{
		if (IsOVer || dontChangeState)
		{
			return;
		}
		if (isIcetrap || isButter)
		{
			animator.speed = 0f;
			return;
		}
		if (State == ZombieState.Walk)
		{
			if (SpeedRate == 0f)
			{
				animator.speed = SpeedRate;
			}
			else if (!isIcetrap && !isButter)
			{
				animator.speed = AnToSpeed / Mathf.Abs(Speed) * SpeedRate;
			}
		}
		else if (!isIcetrap && !isButter)
		{
			animator.speed = SpeedRate;
		}
		if (State == ZombieState.Dead)
		{
			animator.speed = SpeedRate;
		}
	}

	protected void SetAllColor(Color color)
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].color = color;
		}
	}

	protected void SetAllBrightness(float color, SpriteRenderer noThis = null)
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			if (sprites[i] != noThis)
			{
				sprites[i].material.SetFloat("_Brightness", color);
			}
		}
	}

	public void InitForAlmanac(Vector2 pos)
	{
		IsOVer = true;
		Init(0, 100, default(Vector2), null);
		StartIdel();
		base.transform.position = pos;
		Shadow.sortingOrder = 0;
		AlmanacInitZombie();
	}

	public void CreateInit(bool inGrid, Grid grid, bool isRat)
	{
		IsOVer = true;
		Init(0, 100, default(Vector2), null);
		if (grid != null)
		{
			base.transform.position = grid.Position;
		}
		if (inGrid)
		{
			Sorting.sortingOrder = 2011;
			SetAllColor(new Color(1f, 1f, 1f, 0.7f));
		}
		else
		{
			Sorting.sortingOrder = 2012;
			SetAllColor(new Color(1f, 1f, 1f, 1f));
		}
		Shadow.enabled = false;
		capsuleCollider2D.enabled = false;
		StartIdel();
		anCanMove = false;
		if (isRat)
		{
			RatThis(synClient: true);
		}
		animator.speed = 0f;
		CreateInitZombie();
	}

	public void UpdateForCreate(Grid grid)
	{
		base.transform.position = grid.Position;
	}

	public void Init(int lineNum, int orderNum, Vector2 pos, ZombieSpawn spawnInfo)
	{
		if (animator == null)
		{
			animator = base.transform.Find("Animation").GetComponent<Animator>();
			Sorting = animator.GetComponent<SortingGroup>();
			SpriteRenderer[] componentsInChildren = animator.transform.GetComponentsInChildren<SpriteRenderer>();
			sprites.AddRange(componentsInChildren);
			capsuleCollider2D = GetComponent<CapsuleCollider2D>();
			Icetrap = base.transform.Find("icetrap").GetComponent<SpriteRenderer>();
			Shadow = base.transform.Find("Shadow").GetComponent<SpriteRenderer>();
		}
		for (int i = 0; i < headSprites.Count; i++)
		{
			headSprites[i].gameObject.SetActive(value: true);
		}
		for (int j = 0; j < legSprites.Count; j++)
		{
			legSprites[j].gameObject.SetActive(value: true);
		}
		returnFirstClick = false;
		SpawnPos = pos;
		CurrGrid = MapManager.Instance.GetGridByVertical(pos, lineNum);
		IsFacingLeft = true;
		dontChangeState = false;
		onlyBoomHurt = false;
		anCanMove = true;
		dontChangeState = false;
		capsuleCollider2D.enabled = true;
		base.transform.position = pos;
		CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
		InWater = false;
		going = false;
		if (CurrGrid != null)
		{
			animator.GetComponent<SortingGroup>().sortingOrder = CurrGrid.Point.y * 200 + 100 + orderNum;
		}
		FrozenLevel = 0;
		isDropHead = false;
		if (butter != null)
		{
			butter.enabled = false;
		}
		if (normalHead != null)
		{
			HeadRenderer.sprite = normalHead;
			JawRenderer.enabled = true;
		}
		invincible = false;
		isHypno = false;
		hypnoAttackTarget = null;
		needHypnoPurple = false;
		SetAllBrightness(1f);
		SetAllColor(new Color(1f, 1f, 1f));
		Icetrap.sortingOrder = Sorting.sortingOrder + 1;
		Icetrap.enabled = false;
		Shadow.enabled = true;
		Shadow.sortingOrder = 10;
		Shadow.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		if (orderNum != 100)
		{
			IsOVer = false;
		}
		state = ZombieState.Walk;
		ResetAnimationSpeed();
		CheckState();
		InitZombieHpState();
		currHpState = 0;
		Hp = MaxHP;
		DoorHp = MaxDoorHp;
		if (spawnInfo != null)
		{
			if (GameManager.Instance.isServer)
			{
				spawnInfo.SpawnPos = pos;
			}
			if (GameManager.Instance.isClient)
			{
				ClientInit(spawnInfo);
			}
			if (GameManager.Instance.isServer)
			{
				ServerInitInfo(spawnInfo);
			}
		}
		Speed = DefSpeed;
		SpeedRate = 1f;
	}

	public abstract void InitZombieHpState();

	protected virtual void AlmanacInitZombie()
	{
	}

	protected virtual void CreateInitZombie()
	{
	}

	protected virtual void ServerInitInfo(ZombieSpawn spawnInfo)
	{
	}

	protected virtual void ClientInit(ZombieSpawn spawnInfo)
	{
	}

	public virtual void SummonInit()
	{
	}

	public virtual void ZombieOnDead(bool dropItem)
	{
	}

	private void Update()
	{
		if (IsOVer || isIcetrap || isButter)
		{
			return;
		}
		switch (State)
		{
		case ZombieState.Walk:
			Move();
			if (LV.Instance.CurrLVType == LVType.Normal && base.transform.position.x > 8.6f)
			{
				DirectDead(canDropItem: false, 0f);
			}
			else if (MapManager.Instance.GetCurrMap(base.transform.position) == null)
			{
				DirectDead(canDropItem: false, 0f);
			}
			break;
		case ZombieState.Attack:
			if (Hp > 0)
			{
				CurrGrid = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine);
			}
			EatCheck();
			break;
		}
	}

	protected virtual void CheckState()
	{
		switch (State)
		{
		case ZombieState.Idel:
			animator.Play("idel1");
			break;
		case ZombieState.Walk:
			animator.SetInteger("Change", 11);
			break;
		case ZombieState.Attack:
			animator.SetInteger("Change", 21);
			break;
		case ZombieState.Dead:
			Shadow.enabled = false;
			capsuleCollider2D.enabled = false;
			animator.SetInteger("Change", 31);
			break;
		}
	}

	private void Move()
	{
		if (Speed == 0f || SpeedRate == 0f || CurrGrid == null)
		{
			return;
		}
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine);
		if (gridByWorldPos == null)
		{
			return;
		}
		CurrGrid = gridByWorldPos;
		if (!going)
		{
			EatStateCheck();
		}
		if (State != ZombieState.Walk)
		{
			return;
		}
		if (needInWater)
		{
			if (CurrGrid.isWaterGrid && Mathf.Abs(base.transform.position.x - currGrid.Position.x) < 0.5f && !InWater && !going)
			{
				StartCoroutine(MoveInWater());
			}
			if (Mathf.Abs(CurrGrid.Position.x - base.transform.position.x) > 0.65f && (nextGrid == null || !nextGrid.isWaterGrid) && InWater && !going)
			{
				if (IsFacingLeft && CurrGrid.Position.x - base.transform.position.x > 0.65f)
				{
					StartCoroutine(MoveOutWater());
				}
				else if (!IsFacingLeft && base.transform.position.x - CurrGrid.Position.x > 0.65f)
				{
					StartCoroutine(MoveOutWater());
				}
			}
		}
		if (nextGrid != null && nextGrid.needChangeLine && changeLineCoroutine == null && Mathf.Abs(nextGrid.Position.x - base.transform.position.x) < 1.45f && needChangeLine)
		{
			ChangeLine();
		}
		if (IsFacingLeft && base.transform.position.x < CurrMap.EndLine)
		{
			if (!isHypno)
			{
				if (!GameManager.Instance.isClient)
				{
					LVManager.Instance.GameOver(base.transform.position);
				}
			}
			else if (IsFacingLeft)
			{
				GoBack();
			}
		}
		else if (anCanMove)
		{
			float y = 0f;
			if (InWater)
			{
				y = -0.5f;
			}
			base.transform.position = Vector2.MoveTowards(base.transform.position, MoveTarget + new Vector2(0f, y), 1.33f * Time.deltaTime / (Speed / SpeedRate));
		}
	}

	protected virtual void EatStateCheck()
	{
		if (isHypno)
		{
			if (nextGrid != null && nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.ZombieCanEat && nextGrid.CurrPlantBase.isHypno && Mathf.Abs(base.transform.position.x - nextGrid.Position.x) < 0.85f)
			{
				AttackGrid = nextGrid;
				State = ZombieState.Attack;
				return;
			}
			if (CurrGrid.CurrPlantBase != null && CurrGrid.CurrPlantBase.ZombieCanEat && CurrGrid.CurrPlantBase.isHypno && Mathf.Abs(base.transform.position.x - CurrGrid.Position.x) < 0.3f)
			{
				AttackGrid = CurrGrid;
				State = ZombieState.Attack;
				return;
			}
			hypnoAttackTarget = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, IsFacingLeft, !isHypno);
			if (hypnoAttackTarget != null && Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) <= 0.4f)
			{
				State = ZombieState.Attack;
			}
		}
		else if (nextGrid != null && nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.ZombieCanEat && !nextGrid.CurrPlantBase.isHypno && Mathf.Abs(base.transform.position.x - nextGrid.Position.x) < 0.85f)
		{
			AttackGrid = nextGrid;
			State = ZombieState.Attack;
		}
		else if (CurrGrid.CurrPlantBase != null && CurrGrid.CurrPlantBase.ZombieCanEat && !CurrGrid.CurrPlantBase.isHypno && Mathf.Abs(base.transform.position.x - CurrGrid.Position.x) < 0.3f)
		{
			AttackGrid = CurrGrid;
			State = ZombieState.Attack;
		}
		else
		{
			hypnoAttackTarget = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, IsFacingLeft, !isHypno);
			if (hypnoAttackTarget != null && Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) <= 0.4f)
			{
				State = ZombieState.Attack;
			}
		}
	}

	protected virtual void EatCheck()
	{
		if (isHypno)
		{
			if (hypnoAttackTarget != null)
			{
				if (hypnoAttackTarget.Hp <= 0 || !hypnoAttackTarget.capsuleCollider2D.enabled || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f)
				{
					State = ZombieState.Walk;
					hypnoAttackTarget = null;
				}
			}
			else if ((AttackGrid != null && AttackGrid.CurrPlantBase == null) || (AttackGrid.CurrPlantBase != null && !AttackGrid.CurrPlantBase.ZombieCanEat) || (AttackGrid.CurrPlantBase != null && !AttackGrid.CurrPlantBase.isHypno))
			{
				State = ZombieState.Walk;
				AttackGrid = null;
			}
		}
		else if (hypnoAttackTarget != null)
		{
			if (hypnoAttackTarget.Hp <= 0 || !hypnoAttackTarget.capsuleCollider2D.enabled || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f)
			{
				State = ZombieState.Walk;
				hypnoAttackTarget = null;
			}
		}
		else if (AttackGrid != null && (AttackGrid.CurrPlantBase == null || (AttackGrid.CurrPlantBase != null && !AttackGrid.CurrPlantBase.ZombieCanEat) || (AttackGrid.CurrPlantBase != null && AttackGrid.CurrPlantBase.isHypno)))
		{
			State = ZombieState.Walk;
			AttackGrid = null;
		}
		if (State == ZombieState.Attack && AttackGrid != null && ((AttackGrid == nextGrid && Mathf.Abs(base.transform.position.x - nextGrid.Position.x) > 0.85f) || (AttackGrid == CurrGrid && Mathf.Abs(base.transform.position.x - CurrGrid.Position.x) > 0.3f)))
		{
			State = ZombieState.Walk;
			AttackGrid = null;
		}
	}

	private IEnumerator MoveInWater()
	{
		going = true;
		Shadow.enabled = false;
		while (base.transform.position.y > currGrid.Position.y - 0.5f)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(0f, -1f) * Time.deltaTime * 5f);
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.EFObj).GetComponent<EFObj>().CreateInit(base.transform.position + new Vector3(-0.6f, 1f), 3, new Color(1f, 1f, 1f, 1f), CurrGrid.Point.y * 200 + 195);
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
		}
		InWater = true;
		going = false;
	}

	private IEnumerator MoveOutWater()
	{
		going = true;
		while (base.transform.position.y < CurrGrid.Position.y)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(0f, 1f) * Time.deltaTime * 5f);
		}
		Shadow.enabled = true;
		InWater = false;
		going = false;
	}

	protected void GoBack()
	{
		IsFacingLeft = !IsFacingLeft;
	}

	public void AnimAttack(bool isFlat = false)
	{
		Attack(isFlat);
	}

	protected void Attack(bool isFlat = false)
	{
		if (CurrGrid == null)
		{
			return;
		}
		Vector2 dirction = LeftAttackDir;
		if (!IsFacingLeft)
		{
			dirction = new Vector2(0f - LeftAttackDir.x, LeftAttackDir.y);
		}
		if (hypnoAttackTarget != null)
		{
			hypnoAttackTarget.Hurt((int)attackValue, dirction, isHard: true, HitSound: false);
		}
		else if (AttackGrid != null && AttackGrid.CurrPlantBase != null)
		{
			AttackGrid.CurrPlantBase.Hurt(attackValue, this, isFlat);
		}
		if (!isFlat)
		{
			switch (Random.Range(0, 3))
			{
			case 0:
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlant1, base.transform.position);
				break;
			case 1:
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlant2, base.transform.position);
				break;
			case 2:
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlant3, base.transform.position);
				break;
			}
		}
	}

	public virtual void Hurt(int attackValue, Vector2 dirction, bool isHard = true, bool HitSound = true)
	{
		if (invincible)
		{
			return;
		}
		attackValue = HandleHurt(attackValue, dirction);
		if (ZombieManager.Instance.ZombieInvincible)
		{
			attackValue = 0;
		}
		if (DoorHp > 0 && ((dirction.x > 0f && IsFacingLeft) || (dirction.x < 0f && !IsFacingLeft)))
		{
			DoorHp -= attackValue;
			if (attackValue > 0)
			{
				DoorHpReduceEvent();
			}
			if (attackValue >= 0)
			{
				if (DoorBrightCoroutine != null)
				{
					StopCoroutine(DoorBrightCoroutine);
				}
				DoorBrightCoroutine = StartCoroutine(DoorBrightnessEffect(1.5f, null));
			}
			return;
		}
		Hp -= attackValue;
		if (State == ZombieState.Dead)
		{
			return;
		}
		if (attackValue > 0)
		{
			HpReduceEvent(isHard, HitSound);
		}
		if (attackValue >= 0)
		{
			if (BrightCoroutine != null)
			{
				StopCoroutine(BrightCoroutine);
			}
			BrightCoroutine = StartCoroutine(BrightnessEffect(1.5f, null));
		}
	}

	public void BoomHurt(int attackValue, bool HitSound = false)
	{
		if (invincible)
		{
			return;
		}
		if (onlyBoomHurt)
		{
			Hurt(attackValue, Vector2.zero, isHard: true, HitSound: false);
			return;
		}
		int num = Hp;
		if (ZombieManager.Instance.ZombieInvincible)
		{
			attackValue = 0;
		}
		if (attackValue >= num && !ZombieManager.Instance.ZombieInvincible)
		{
			if (GameManager.Instance.isServer && GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
			if (!GameManager.Instance.isClient)
			{
				state = ZombieState.Dead;
				if (InWater)
				{
					Dead(canDropItem: true, 0f);
				}
				else
				{
					PlaceCharred();
				}
			}
		}
		else if (attackValue >= 0)
		{
			Hp -= attackValue;
			HpReduceEvent(isHard: false, HitSound);
			if (BrightCoroutine != null)
			{
				StopCoroutine(BrightCoroutine);
			}
			BrightCoroutine = StartCoroutine(BrightnessEffect(1.5f, null));
		}
	}

	protected virtual void PlaceCharred()
	{
		Dead();
		animator.speed = 0f;
		SetAllColor(new Color(0f, 0f, 0f));
	}

	public void CleanerDead(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			if (GameManager.Instance.isServer && State != ZombieState.Dead && !synClient)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 3;
				SocketServer.Instance.SendSynBag(synItem);
			}
			Object.Instantiate(GameManager.Instance.GameConf.ImitaterParticle).transform.position = base.transform.position;
			Hp = 0;
			Dead(canDropItem: true, 0f);
		}
	}

	public void DirectDead(bool canDropItem = true, float delay = 1f, bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			if (GameManager.Instance.isServer && State != ZombieState.Dead && !synClient)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 2;
				SocketServer.Instance.SendSynBag(synItem);
			}
			Dead(canDropItem, delay);
		}
	}

	protected void Dead(bool canDropItem = true, float delay = 1f)
	{
		hp = 0;
		capsuleCollider2D.enabled = false;
		ZombieOnDead(canDropItem);
		UnIce();
		UnFrozen();
		UnButter();
		RemoveScaredyStall();
		ResetAnimationSpeed();
		if (canDropItem)
		{
			ZombieManager.Instance.DropCoin(base.transform.position);
		}
		StopAllCoroutines();
		currGrid = null;
		ZombieManager.Instance.RemoveZombie(this);
		if (delay == 0f)
		{
			PoolManager.Instance.PushObj(Prefab, base.gameObject);
		}
		else
		{
			StartCoroutine(DeadWait(delay));
		}
	}

	private IEnumerator DeadWait(float delay)
	{
		yield return new WaitForSeconds(delay);
		PoolManager.Instance.PushObj(Prefab, base.gameObject);
	}

	protected IEnumerator BrightnessEffect(float targetBright, UnityAction fun)
	{
		float currBright = sprites[0].material.GetFloat("_Brightness");
		while (currBright < targetBright)
		{
			yield return new WaitForFixedUpdate();
			currBright += 5f * Time.deltaTime;
			SetAllBrightness(currBright, DoorRenderer);
		}
		while (1f < targetBright)
		{
			yield return new WaitForFixedUpdate();
			targetBright -= 5f * Time.deltaTime;
			SetAllBrightness(targetBright, DoorRenderer);
		}
		SetAllBrightness(1f, DoorRenderer);
		fun?.Invoke();
		BrightCoroutine = null;
	}

	protected IEnumerator DoorBrightnessEffect(float targetBright, UnityAction fun)
	{
		float currBright = DoorRenderer.material.GetFloat("_Brightness");
		while (currBright < targetBright)
		{
			yield return new WaitForFixedUpdate();
			currBright += 5f * Time.deltaTime;
			DoorRenderer.material.SetFloat("_Brightness", currBright);
		}
		while (1f < targetBright)
		{
			yield return new WaitForFixedUpdate();
			targetBright -= 5f * Time.deltaTime;
			DoorRenderer.material.SetFloat("_Brightness", targetBright);
		}
		DoorRenderer.material.SetFloat("_Brightness", 1f);
		fun?.Invoke();
		DoorBrightCoroutine = null;
	}

	public virtual void OnlineSynZombie(SynItem syn)
	{
		if (syn.SynCode[0] == 0)
		{
			if (syn.SynCode[1] == 0)
			{
				FrozenLevel = syn.SynCode[2];
			}
			else if (syn.SynCode[1] == 1)
			{
				Ice(isSyn: true);
			}
			else if (syn.SynCode[1] == 2)
			{
				Butter(isSyn: true);
			}
			else if (syn.SynCode[1] == 3)
			{
				if (changeLineCoroutine == null)
				{
					changeLineCoroutine = StartCoroutine(MovetoLine(syn.SynCode[2]));
				}
			}
			else if (syn.SynCode[1] == 4)
			{
				if (syn.SynCode[2] == 1)
				{
					Hypno(synClient: true);
				}
				else
				{
					RatThis(synClient: true);
				}
			}
			else if (syn.SynCode[1] == 5)
            {
				ApplyScaredyStall(syn.Twofloat.x, syn.Twofloat.y, true);
            }
		}
		else if (syn.SynCode[0] == 1)
		{
			if (syn.SynCode[1] == 0)
			{
				if (isButter)
				{
					UnButter();
				}
				if (isIcetrap)
				{
					UnIce();
				}
				Hp = 0;
				state = ZombieState.Dead;
				CheckState();
				ResetAnimationSpeed();
			}
			else if (syn.SynCode[1] == 1)
			{
				if (InWater)
				{
					Dead(canDropItem: true, 0f);
					return;
				}
				Dead();
				PlaceCharred();
			}
			else if (syn.SynCode[1] == 2)
			{
				Dead(canDropItem: true, 0f);
			}
			else if (syn.SynCode[1] == 3)
			{
				CleanerDead(synClient: true);
			}
		}
		else if (syn.SynCode[0] == 3 && syn.SynCode[1] == 0)
		{
			ClickEvent(syn.name);
		}
	}

	public void StartIdel()
	{
		animator.SetInteger("Change", 0);
		State = ZombieState.Idel;
	}

	public void Hypno(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			needHypnoPurple = true;
			ResetColor();
			hypZombie(synClient);
		}
	}

	public void RatThis(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			needHypnoPurple = false;
			ResetColor();
			hypZombie(synClient);
		}
	}

	private void hypZombie(bool synClient)
	{
		GoBack();
		isHypno = !isHypno;
		HypnoEvent();
		ZombieManager.Instance.ZombieHypno(this);
		hypnoAttackTarget = null;
		if (GameManager.Instance.isServer && !synClient)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = 2;
			synItem.SynCode[0] = 0;
			synItem.SynCode[1] = 4;
			if (needHypnoPurple)
			{
				synItem.SynCode[2] = 1;
			}
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	public void Frozen(Vector2 dirct, bool isAudio = true, int frozenLvl = 1)
	{
		if (!GameManager.Instance.isClient && State != ZombieState.Dead && canFrozen && (DoorHp <= 0 || !IsFacingLeft || !(dirct.x > 0f)) && (DoorHp <= 0 || IsFacingLeft || !(dirct.x < 0f)))
		{
			if (!isFrozen && isAudio)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Frozen, base.transform.position);
			}
			FrozenLevel += frozenLvl;
			int num = 9 - frozenLvl;
			if (FrozenLevel > 8 && Random.Range(0, 10) > num)
			{
				Ice();
			}
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 0;
				synItem.SynCode[2] = (byte)FrozenLevel;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void UnFrozen()
	{
		if (!isIcetrap)
		{
			FrozenLevel--;
			if (FrozenLevel == 0)
			{
				isFrozen = false;
			}
		}
	}

	public void Ice(bool isSyn = false)
	{
		if ((isSyn || !GameManager.Instance.isClient) && State != ZombieState.Dead && canIce)
		{
			isIcetrap = true;
			ResetAnimationSpeed();
			if (icetrapCoroutine != null)
			{
				StopCoroutine(icetrapCoroutine);
			}
			icetrapCoroutine = StartCoroutine(DoFuncWait(UnIce, 4f));
			FrozenLevel = 10;
			if (!InWater)
			{
				Icetrap.enabled = true;
			}
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	private void UnIce()
	{
		isIcetrap = false;
		ResetAnimationSpeed();
		ResetColor();
		Icetrap.enabled = false;
		UnFrozen();
	}

	public void Butter(bool isSyn = false)
	{
		if ((isSyn || !GameManager.Instance.isClient) && canButter)
		{
			isButter = true;
			ResetAnimationSpeed();
			if (butterCoroutine != null)
			{
				StopCoroutine(butterCoroutine);
			}
			butterCoroutine = StartCoroutine(DoFuncWait(UnButter, 4f));
			if (butter != null)
			{
				butter.enabled = true;
			}
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 2;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	private void UnButter()
	{
		isButter = false;
		ResetAnimationSpeed();
		if (butter != null)
		{
			butter.enabled = false;
		}
	}

	public void Yuck()
	{
		dontChangeState = true;
		animator.speed = 0f;
		if (yuckHead != null)
		{
			HeadRenderer.sprite = yuckHead;
			JawRenderer.enabled = false;
		}
		if (Random.Range(0, 2) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.yuck1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.yuck2, base.transform.position);
		}
		StartCoroutine(DoFuncWait(delegate
		{
			if (!isButter)
			{
				if (yuckHead != null)
				{
					HeadRenderer.sprite = normalHead;
					JawRenderer.enabled = true;
				}
				if (!isIcetrap)
				{
					if (Hp <= 0)
					{
						State = ZombieState.Dead;
					}
					if (State == ZombieState.Dead)
					{
						ResetAnimationSpeed();
					}
					dontChangeState = false;
					if (State != ZombieState.Dead)
					{
						ResetAnimationSpeed();
						ChangeLine();
					}
				}
			}
		}, 0.4f));
	}

	public void ChangeLine()
	{
		if (!GameManager.Instance.isClient && changeLineCoroutine == null)
		{
			int num = ((MapManager.Instance.GetCurrMap(base.transform.position).MapGridNum.y - 1 == CurrLine) ? (CurrLine - 1) : ((CurrLine == 0) ? (CurrLine + 1) : ((Random.Range(0, 2) != 1) ? (CurrLine + 1) : (CurrLine - 1))));
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 3;
				synItem.SynCode[2] = (byte)num;
				SocketServer.Instance.SendSynBag(synItem);
			}
			changeLineCoroutine = StartCoroutine(MovetoLine(num));
		}
	}

	private IEnumerator MovetoLine(int line)
	{
		Grid grid = null;
		if (State != ZombieState.Dead)
		{
			grid = MapManager.Instance.GetGridByWorldPos(base.transform.position, line);
		}
		if (grid == null)
		{
			yield break;
		}
		State = ZombieState.Walk;
		capsuleCollider2D.enabled = false;
		dontChangeState = true;
		int last2 = Sorting.sortingOrder % 100;
		float high = 0f;
		if (InWater && !grid.isWaterGrid)
		{
			going = true;
			InWater = false;
			while (base.transform.position.y < grid.Position.y)
			{
				yield return new WaitForFixedUpdate();
				base.transform.Translate(new Vector2(0f, 1f) * Time.deltaTime * 5f);
			}
			Shadow.enabled = true;
			going = false;
		}
		if (InWater && grid.isWaterGrid && needInWater)
		{
			high = -0.5f;
		}
		if (line > CurrLine)
		{
			Sorting.sortingOrder = line * 200 + last2 + 100;
			while (base.transform.position.y > grid.Position.y + high)
			{
				yield return new WaitForFixedUpdate();
				base.transform.Translate(new Vector2(0f, -1f) * Time.deltaTime / 0.5f);
			}
		}
		else
		{
			while (base.transform.position.y < grid.Position.y + high)
			{
				yield return new WaitForFixedUpdate();
				base.transform.Translate(new Vector2(0f, 1f) * Time.deltaTime / 0.5f);
			}
			Sorting.sortingOrder = line * 200 + last2 + 100;
		}
		if (InWater && !grid.isWaterGrid)
		{
			StartCoroutine(MoveOutWater());
		}
		dontChangeState = false;
		CurrGrid = grid;
		State = ZombieState.Walk;
		changeLineCoroutine = null;
		capsuleCollider2D.enabled = true;
	}

	protected IEnumerator DoFuncWait(UnityAction fun, float time)
	{
		yield return new WaitForSeconds(time);
		fun?.Invoke();
	}

	protected void DropArm(Vector3 offset, float scale = 0.3f)
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ArmDropParticle).GetComponent<ArmDropParticle>().InitCreate(base.transform.position + offset, Sorting.sortingOrder, dropArmSprite, scale);
	}

	public void DropHead()
	{
		if (!isDropHead)
		{
			isDropHead = true;
			for (int i = 0; i < headSprites.Count; i++)
			{
				headSprites[i].gameObject.SetActive(value: false);
			}
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZombieHeadParticle).GetComponent<ZombieHeadParticle>().InitCreate(HeadRenderer.transform.position, Sorting.sortingOrder, dropHeadSprite, InWater, DropHeadScale);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropHead, base.transform.position);
		}
	}

	public void StopAction()
	{
		animator.speed = 0f;
		anCanMove = false;
	}

	public void GameOverFakeDeath()
	{
		animator.speed = 0f;
		IsOVer = true;
		StopAllCoroutines();
	}

	public virtual void AnimFailSound()
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Zombiefail1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Zombiefail2, base.transform.position);
		}
	}

	private void OnMouseOver()
	{
		if (!MyTool.IsPointerOverGameObject() && Input.GetMouseButtonUp(0))
		{
			if (!returnFirstClick)
			{
				returnFirstClick = true;
			}
			else if (GameManager.Instance.isClient)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.name = GameManager.Instance.LocalPlayer.playerName;
				synItem.SynCode[0] = 3;
				synItem.SynCode[1] = 0;
				SocketClient.Instance.SendSynBag(synItem);
			}
			else
			{
				ClickEvent(GameManager.Instance.LocalPlayer.playerName);
			}
		}
	}

	protected virtual int HandleHurt(int attackValue, Vector2 dirction)
	{
		return attackValue;
	}

	public virtual SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		return null;
	}

	protected virtual void FrameChangeEvent(SwfClip swfClip)
	{
	}

	protected virtual void SpriteChangeEvent(Texture2D nextTexture)
	{
	}

	protected virtual void SpriteChangeEvent(Sprite nextSprite)
	{
	}

	protected virtual void CurrGridChangeEvent(Grid lastGrid)
	{
	}

	protected virtual void InWaterChangeEvent()
	{
	}

	protected virtual void HypnoEvent()
	{
	}

	protected virtual void ClickEvent(string clickPlayer)
	{
	}

	protected virtual void HpReduceEvent(bool isHard, bool HitSound)
	{
	}

	protected virtual void DoorHpReduceEvent()
	{
	}

	protected virtual void ChangeFacingEvent()
	{
	}

	public virtual void SpecialAnimEvent1()
	{
	}

	public virtual void SpecialAnimEvent2()
	{
	}

	public virtual void SpecialAnimEvent3()
	{
	}

	public virtual void SpecialAnimEvent4()
	{
	}

	public virtual void SpecialAnimEvent5()
	{
	}

	public virtual void SpecialAnimEvent6()
	{
	}
}
