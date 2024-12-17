using System.Collections;
using FTRuntime;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public abstract class PlantBase : MonoBehaviour
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
	/// 
	/// </summary>
    private float stallDuration;

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
        float a = REnderer.material.GetColor("_Color").a;

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

        SetColor(new Color(r, g, b, a));
    }

    /// <summary>
    /// Stall duration coroutine.
    /// </summary>
    private Coroutine stallCoroutine;

    public void ApplyScaredyStall(float stall, float duration, bool isSyn = false)
    {
        if ((isSyn || !GameManager.Instance.isClient) && canStall)
        {
            StallLevel += stall;

            //if (stallCoroutine != null) StopCoroutine(stallCoroutine);
            //stallCoroutine = StartCoroutine(StallWearOff(stall, duration));
            StartCoroutine(StallWearOff(stall, duration));

            if (GameManager.Instance.isServer)
            {
                SynItem synItem = new SynItem();
                synItem.OnlineId = OnlineId;
                synItem.Type = 2;
                synItem.SynCode[0] = 1;
                synItem.SynCode[1] = 6;
                SocketServer.Instance.SendSynBag(synItem);
            }
        }
    }

    protected IEnumerator StallWearOff(float stall, float duration)
    {
        float currStall = stall / 100f;
		float currDur = duration / 100f;

        while (stallLevel > 0)
        {
            yield return new WaitForSeconds(duration / 100f);
            StallLevel -= currStall;
			stallDuration -= currDur;
        }
    }
    #endregion Edits

    public int OnlineId;

	public string PlacePlayer;

	protected SwfClipController clipController;

	protected SpriteRenderer Shadow;

	protected Transform ZZZ;

	protected Grid currGrid;

	protected Renderer REnderer;

	public Texture2D eye1;

	public Texture2D eye2;

	public Renderer ExtraREnderer;

	protected int closeEyeFrame;

	protected int idelFrameCount;

	protected bool isClosingEye;

	private float speedRate = 1f;

	public bool isSleeping;

	private float hp;

	private bool isFlash;

	public PlantBase CarryPlant;

	public PlantBase ProtectPlant;

	protected bool needFlatDead;

	private Coroutine FlashCoroutine;

	private Coroutine BrightCoroutine;

	private bool isFacingLeft;

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

	protected virtual int attackValue { get; }

	protected virtual bool isShroom { get; }

	public virtual bool isHaveSpecialCheck { get; }

	public virtual bool isProtectPlant { get; }

	public virtual PlantType BasePlant { get; }

	public virtual int BasePlantSunNum { get; } = -1;

	protected abstract PlantType plantType { get; }

	protected virtual Vector2 offSet { get; } = Vector2.zero;

	public virtual bool ZombieCanEat { get; } = true;

	public virtual bool CanPlaceOnWaterCarry { get; } = true;

	public virtual bool CanPlaceOnGrass { get; } = true;

	public virtual bool CanPlaceOnWater { get; }

	public virtual bool CanPlaceOnHardGround { get; }

	public virtual bool CanCarryed { get; } = true;

	public virtual bool CanCarryOtherPlant { get; }

	public virtual bool CanProtect { get; } = true;

	public virtual Vector2 CarryOffset { get; } = Vector2.zero;

	public virtual bool IsZombiePlant { get; }

	public int SortingOrder => clipController.clip.sortingOrder;

	public abstract float MaxHp { get; }

	protected virtual bool HaveShadow { get; } = true;

	public float Hp
	{
		get
		{
			return hp;
		}
		protected set
		{
			if (value <= hp)
			{
				if (BrightCoroutine != null)
				{
					StopCoroutine(BrightCoroutine);
				}
				BrightCoroutine = StartCoroutine(BrightnessEffect(1.5f, null));
			}
			hp = value;
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
			speedRate = value;
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
				base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
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

	protected void InitForAll()
	{
		needFlatDead = true;
		SpeedRate = 1f;
		isFlash = false;
		closeEyeFrame = -1;
		REnderer = base.transform.Find("Animation").GetComponent<Renderer>();
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		if (clipController == null)
		{
			clipController = base.transform.Find("Animation").GetComponent<SwfClipController>();
			clipController.clip.OnChangeCurrentFrameEvent += FrameChangeEvent;
		}
		clipController.clip.ToBeginFrame();
		REnderer.material.SetFloat("_Brightness", 1f);
		clipController.clip.sequence = "idel";
		clipController.clip.currentFrame = 0;
		if (ExtraREnderer != null)
		{
			ExtraREnderer.material.SetFloat("_Brightness", 1f);
			ExtraREnderer.transform.GetComponent<SwfClipController>().clip.currentFrame = 0;
		}
		OpenBlackAndWhite(isOpen: false);
		Shadow = base.transform.Find("Shadow").GetComponent<SpriteRenderer>();
		if (HaveShadow)
		{
			Shadow.enabled = false;
		}
		ZZZ = base.transform.Find("ZZZ");
		ZZZ.gameObject.SetActive(value: false);
		isSleeping = false;
		isFacingLeft = false;
		base.transform.localScale = new Vector3(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y);
		isHypno = false;
		needHypnoPurple = false;
		FrozenLevel = 0;
		OnInitForAll();
	}

	public void UpdateForCreate(Grid grid)
	{
		if (grid.CurrPlantBase != null)
		{
			base.transform.position = grid.Position + offSet + grid.CurrPlantBase.CarryOffset;
		}
		else
		{
			base.transform.position = grid.Position + offSet;
		}
	}

	public void InitForAlmanac(Vector2 pos)
	{
		InitForAll();
		if (HaveShadow)
		{
			Shadow.enabled = true;
		}
		base.transform.position = pos;
		currGrid = null;
		clipController.rateScale = SpeedRate;
		REnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
		if (ExtraREnderer != null)
		{
			ExtraREnderer.transform.GetComponent<SwfClipController>().rateScale = SpeedRate;
			ExtraREnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
		}
		OnInitForAlmanac();
	}

	public void InitForCreate(bool inGrid, Grid grid, bool isBlcWhi)
	{
		InitForAll();
		if (grid != null)
		{
			if (grid.CurrPlantBase != null)
			{
				base.transform.position = grid.Position + offSet + grid.CurrPlantBase.CarryOffset;
			}
			else
			{
				base.transform.position = grid.Position + offSet;
			}
		}
		clipController.rateScale = 0f;
		if (ExtraREnderer != null)
		{
			ExtraREnderer.transform.GetComponent<SwfClipController>().rateScale = 0f;
		}
		if (inGrid)
		{
			clipController.clip.sortingOrder = 2011;
			REnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.7f));
			if (ExtraREnderer != null)
			{
				ExtraREnderer.transform.GetComponent<SwfClipController>().clip.sortingOrder = 2010;
				ExtraREnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.7f));
			}
		}
		else
		{
			clipController.clip.sortingOrder = 2013;
			REnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
			if (ExtraREnderer != null)
			{
				ExtraREnderer.transform.GetComponent<SwfClipController>().clip.sortingOrder = 2012;
				ExtraREnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
			}
		}
		OpenBlackAndWhite(isBlcWhi);
		OnInitForCreate();
	}

	public void InitForPlace(Grid grid, int orderNum)
	{
		InitForAll();
		Hp = MaxHp;
		currGrid = grid;
		if (grid.CurrPlantBase != null)
		{
			base.transform.position = grid.Position + offSet + grid.CurrPlantBase.CarryOffset;
		}
		else
		{
			base.transform.position = grid.Position + offSet;
		}
		clipController.rateScale = SpeedRate;
		if (CanCarryOtherPlant)
		{
			CheckOrder(11);
		}
		else
		{
			CheckOrder(orderNum);
		}
		ProtectPlant = null;
		REnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
		if (ExtraREnderer != null)
		{
			ExtraREnderer.transform.GetComponent<SwfClipController>().rateScale = SpeedRate;
			ExtraREnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
		}
		if (grid.isWaterGrid)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlantWater, base.transform.position);
		}
		else if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Plant1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Plant2, base.transform.position);
		}
		if (HaveShadow)
		{
			Shadow.enabled = true;
			Shadow.sortingOrder = clipController.clip.sortingOrder / 200 * 200 + 12;
			Shadow.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
			if (grid.isWaterGrid)
			{
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SmallWaterParticle).transform.position = grid.Position + new Vector2(0f, -0.3f);
			}
			else
			{
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SmallDirtParticle).transform.position = grid.Position + new Vector2(0f, -0.3f);
			}
		}
		else
		{
			Shadow.enabled = false;
		}
		clipController.clip.sequence = "idel";
		idelFrameCount = clipController.clip.frameCount;
		closeEyeFrame = Random.Range(0, idelFrameCount);
		if (GameManager.Instance.isClient)
		{
			OnInitForCreate();
		}
		OnInitForPlace();
		if (isShroom)
		{
			if (SkyManager.Instance.GetIsDay())
			{
				GoSleep();
			}
			else if (isSleeping)
			{
				GoAwake();
			}
		}
		else if (!SkyManager.Instance.GetIsDay() && Random.Range(0, 8) > 6)
		{
			GoSleep();
		}
		else if (isSleeping)
		{
			GoAwake();
		}
	}

	private void SetColor(Color color)
	{
		REnderer.material.SetColor("_Color", color);
		if (ExtraREnderer != null)
		{
			ExtraREnderer.material.SetColor("_Color", color);
		}
		OwnerSetColor(color);
	}

	protected void ResetAnimationSpeed()
	{
		if (isIcetrap)
		{
			clipController.rateScale = 0f;
		}
		else
		{
			clipController.rateScale = SpeedRate;
		}
	}

	public virtual void OnlineSynPlant(SynItem syn)
	{
		if (syn.SynCode[0] != 1)
		{
			return;
		}
		if (syn.SynCode[1] == 0)
		{
			Dead(syn.SynCode[2] == 1, syn.Twofloat.x, synClient: true, syn.SynCode[2] != 2);
		}
		else if (syn.SynCode[1] == 1)
		{
			GoSleep(synClient: true);
		}
		else if (syn.SynCode[1] == 2)
		{
			GoAwake(synClient: true);
		}
		else if (syn.SynCode[1] == 3)
		{
			FrozenLevel = syn.SynCode[2];
		}
		else if (syn.SynCode[1] == 4)
		{
			Ice(isSyn: true);
		}
		else if (syn.SynCode[1] == 5)
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
		else if (syn.SynCode[1] == 6)
		{
			ApplyScaredyStall(syn.Twofloat.x, syn.Twofloat.y, true);
		}
	}

	public void Hurt(float hurtValue, ZombieBase zombie, bool isFlat = false)
	{
		hurtValue = HandleHurt(hurtValue, isFlat);
		if (PlantManager.Instance.PlantInvincible)
		{
			hurtValue = 0f;
		}
		if (!isFlat)
		{
			if (ProtectPlant != null)
			{
				ProtectPlant.Hurt(hurtValue, zombie, isFlat);
				return;
			}
			if (CarryPlant != null)
			{
				CarryPlant.Hurt(hurtValue, zombie, isFlat);
				return;
			}
			Hp -= hurtValue;
			if (Hp <= 0f)
			{
				HpUpdateEvents(zombie, isFlat);
				Dead();
			}
			else
			{
				HpUpdateEvents(zombie, isFlat);
			}
		}
		else if (hurtValue > 0f)
		{
			if (ProtectPlant != null)
			{
				ProtectPlant.Hurt(hurtValue, zombie, isFlat);
			}
			if (CarryPlant != null)
			{
				CarryPlant.Hurt(hurtValue, zombie, isFlat);
			}
			HpUpdateEvents(zombie, isFlat);
			Dead(isFlat: true);
		}
	}

	protected void GoBack()
	{
		IsFacingLeft = !IsFacingLeft;
	}

	public void Hypno(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			needHypnoPurple = true;
			ResetColor();
			hypZombie();
		}
	}

	public void RatThis(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			needHypnoPurple = false;
			ResetColor();
			hypZombie();
		}
	}

	private void hypZombie()
	{
		GoBack();
		isHypno = true;
		if (GameManager.Instance.isServer)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = 2;
			synItem.SynCode[0] = 1;
			synItem.SynCode[1] = 5;
			if (needHypnoPurple)
			{
				synItem.SynCode[2] = 1;
			}
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	public void Frozen(bool isAudio = true, int frozenLvl = 1)
	{
		if (!GameManager.Instance.isClient && canFrozen)
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
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 3;
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
		if ((isSyn || !GameManager.Instance.isClient) && canIce)
		{
			isIcetrap = true;
			ResetAnimationSpeed();
			if (icetrapCoroutine != null)
			{
				StopCoroutine(icetrapCoroutine);
			}
			icetrapCoroutine = StartCoroutine(DoFuncWait(UnIce, 4f));
			FrozenLevel = 10;
			Icetrap.enabled = true;
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 4;
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

	protected IEnumerator DoFuncWait(UnityAction fun, float time)
	{
		yield return new WaitForSeconds(time);
		fun?.Invoke();
	}

	public void Dead(bool isFlat = false, float waitTime = 0f, bool synClient = false, bool deadRattle = true)
	{
		if ((!synClient && GameManager.Instance.isClient) || !base.gameObject.activeSelf)
		{
			return;
		}
		if (currGrid != null)
		{
			if (currGrid.CurrPlantBase != null && (currGrid.CurrPlantBase.CarryPlant != null || currGrid.CurrPlantBase.ProtectPlant != null))
			{
				base.transform.SetParent(currGrid.CurrPlantBase.transform.parent);
				if (currGrid.CurrPlantBase.CarryPlant == this)
				{
					currGrid.CurrPlantBase.CarryPlant = null;
				}
				else if (currGrid.CurrPlantBase.ProtectPlant == this)
				{
					currGrid.CurrPlantBase.ProtectPlant = null;
				}
			}
			if (CarryPlant != null)
			{
				CarryPlant.transform.SetParent(base.transform.parent);
				if (isFlat)
				{
					CarryPlant.Dead(isFlat: true);
				}
				else
				{
					CarryPlant.Dead();
				}
				CarryPlant = null;
			}
			if (ProtectPlant != null)
			{
				ProtectPlant.transform.SetParent(base.transform.parent);
				if (isFlat)
				{
					ProtectPlant.Dead(isFlat: true);
				}
				else
				{
					currGrid.CurrPlantBase = ProtectPlant;
					ProtectPlant = null;
				}
			}
			if (currGrid.CurrPlantBase == this)
			{
				currGrid.CurrPlantBase = null;
			}
			DeadEvent();
			if (deadRattle)
			{
				DeadrattleEvent();
			}
			hp = 0f;
		}
		Shadow.enabled = false;
		REnderer.material.SetTexture("_EyeTex", null);
		if (GameManager.Instance.isServer && !synClient)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = 1;
			synItem.SynCode[0] = 1;
			synItem.SynCode[1] = 0;
			if (isFlat && needFlatDead)
			{
				synItem.SynCode[2] = 1;
			}
			if (!deadRattle)
			{
				synItem.SynCode[2] = 2;
			}
			synItem.Twofloat.x = waitTime;
			SocketServer.Instance.SendSynBag(synItem);
		}
		if (isFlat && needFlatDead)
		{
			CreateFlatDead();
		}
		else
		{
			StartCoroutine(WaitDead(waitTime));
		}
	}

	private void CreateFlatDead()
	{
		ZZZ.gameObject.SetActive(value: false);
		Vector3 position = base.transform.position;
		base.transform.position = new Vector3(position.x, position.y - 0.2f);
		base.transform.localScale = new Vector3(base.transform.localScale.x, 0.4f, base.transform.localScale.z);
		clipController.rateScale = 0f;
		Shadow.enabled = false;
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
		StartCoroutine(WaitDead(1f));
	}

	private IEnumerator WaitDead(float waitTime)
	{
		if (waitTime != 0f)
		{
			yield return new WaitForSeconds(waitTime);
		}
		currGrid = null;
		clipController.rateScale = 0f;
		StopAllCoroutines();
		CancelInvoke();
		PlantManager.Instance.PlantDeadRemove(this);
	}

	protected IEnumerator ColorEffect(float wantTime, Color targetColor, float delayTime, UnityAction fun)
	{
		if (isFlash)
		{
			yield break;
		}
		float currTime = 0f;
		while (currTime < wantTime)
		{
			yield return new WaitForSeconds(delayTime);
			float t = currTime / wantTime;
			currTime += 0.05f;
			REnderer.material.SetColor("_Color", Color.Lerp(Color.white, targetColor, t));
			if (ExtraREnderer != null)
			{
				ExtraREnderer.material.SetColor("_Color", Color.Lerp(Color.white, targetColor, t));
			}
		}
		REnderer.material.SetColor("_Color", Color.white);
		if (ExtraREnderer != null)
		{
			ExtraREnderer.material.SetColor("_Color", Color.white);
		}
		fun?.Invoke();
	}

	protected IEnumerator BrightnessEffect(float targetBright, UnityAction fun)
	{
		float currBright = REnderer.material.GetFloat("_Brightness");
		while (currBright < targetBright)
		{
			yield return new WaitForFixedUpdate();
			currBright += 5f * Time.deltaTime;
			REnderer.material.SetFloat("_Brightness", currBright);
			if (ExtraREnderer != null)
			{
				ExtraREnderer.material.SetFloat("_Brightness", currBright);
			}
		}
		while (1f < targetBright)
		{
			yield return new WaitForFixedUpdate();
			targetBright -= 5f * Time.deltaTime;
			REnderer.material.SetFloat("_Brightness", targetBright);
			if (ExtraREnderer != null)
			{
				ExtraREnderer.material.SetFloat("_Brightness", targetBright);
			}
		}
		REnderer.material.SetFloat("_Brightness", 1f);
		if (ExtraREnderer != null)
		{
			ExtraREnderer.material.SetFloat("_Brightness", 1f);
		}
		fun?.Invoke();
		BrightCoroutine = null;
	}

	protected IEnumerator BrightnessEffect2(float wantBright, UnityAction fun)
	{
		float currBright = 1f;
		while (currBright < wantBright)
		{
			yield return new WaitForSeconds(0.05f);
			currBright += 0.05f;
			REnderer.material.SetFloat("_Brightness", currBright);
			if (ExtraREnderer != null)
			{
				ExtraREnderer.material.SetFloat("_Brightness", currBright);
			}
		}
		REnderer.material.SetFloat("_Brightness", 1f);
		if (ExtraREnderer != null)
		{
			ExtraREnderer.material.SetFloat("_Brightness", 1f);
		}
		fun?.Invoke();
	}

	private void CheckOrder(int orderNum)
	{
		clipController.clip.sortingOrder = currGrid.Point.y * 200 + orderNum;
		if (ExtraREnderer != null)
		{
			ExtraREnderer.transform.GetComponent<SwfClipController>().clip.sortingOrder = currGrid.Point.y * 200 + orderNum - 1;
		}
	}

	protected int GetBulletSortOrder(int line = 0)
	{
		return (clipController.clip.sortingOrder / 200 + line) * 200 + 199;
	}

	public PlantType GetPlantType()
	{
		return plantType;
	}

	public void StartFlash()
	{
		if (!(CarryPlant != null))
		{
			isFlash = true;
			FlashCoroutine = StartCoroutine(flash());
		}
	}

	public void StopFlash()
	{
		if (FlashCoroutine != null)
		{
			StopCoroutine(FlashCoroutine);
		}
		REnderer.material.SetColor("_Color", new Color(1f, 1f, 1f));
		isFlash = false;
	}

	private IEnumerator flash()
	{
		float a = 1f;
		while (isFlash)
		{
			if (a >= 0.5f)
			{
				while (a > 0.5f)
				{
					yield return new WaitForSeconds(0.04f);
					a -= 0.05f;
					REnderer.material.SetColor("_Color", new Color(a, a, a));
					if (ExtraREnderer != null)
					{
						ExtraREnderer.material.SetColor("_Color", new Color(a, a, a));
					}
				}
			}
			else
			{
				if (!(a <= 0.5f))
				{
					continue;
				}
				while (a < 0.95f)
				{
					yield return new WaitForSeconds(0.04f);
					a += 0.05f;
					REnderer.material.SetColor("_Color", new Color(a, a, a));
					if (ExtraREnderer != null)
					{
						ExtraREnderer.material.SetColor("_Color", new Color(a, a, a));
					}
				}
			}
		}
		REnderer.material.SetColor("_Color", new Color(1f, 1f, 1f));
		if (ExtraREnderer != null)
		{
			ExtraREnderer.material.SetColor("_Color", new Color(1f, 1f, 1f));
		}
	}

	protected void StartCloseEyes()
	{
		if (!isSleeping && Random.Range(0, idelFrameCount) <= idelFrameCount / 4)
		{
			closeEyeFrame = Random.Range(0, idelFrameCount);
			if (!isClosingEye)
			{
				StartCoroutine(CloseEyes());
			}
		}
	}

	private IEnumerator CloseEyes()
	{
		isClosingEye = true;
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_EyeTex", eye1);
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_EyeTex", eye2);
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_EyeTex", eye1);
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_EyeTex", null);
		isClosingEye = false;
	}

	public void GoAwake(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			isSleeping = false;
			ZZZ.gameObject.SetActive(value: false);
			clipController.clip.sequence = "idel";
			REnderer.material.SetTexture("_EyeTex", null);
			GoAwakeSpecial();
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 1;
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 2;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void GoSleep(bool synClient = false)
	{
		if (!PlantManager.Instance.PlantDontSleep && (synClient || !GameManager.Instance.isClient))
		{
			isSleeping = true;
			ZZZ.gameObject.SetActive(value: true);
			clipController.clip.sequence = "idel";
			REnderer.material.SetTexture("_EyeTex", eye2);
			GoSleepSpecial();
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 1;
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void GameOverFakeDeath()
	{
		if (ExtraREnderer != null)
		{
			ExtraREnderer.transform.GetComponent<SwfClipController>().rateScale = 0f;
		}
		GameOverSpecial();
		clipController.rateScale = 0f;
		SpeedRate = 0f;
		CancelInvoke();
		StopAllCoroutines();
	}

	public virtual void OpenBlackAndWhite(bool isOpen)
	{
		if (isOpen)
		{
			REnderer.material.SetInt("_OpenGray", 1);
			if (ExtraREnderer != null)
			{
				ExtraREnderer.material.SetInt("_OpenGray", 1);
			}
		}
		else
		{
			REnderer.material.SetInt("_OpenGray", 0);
			if (ExtraREnderer != null)
			{
				ExtraREnderer.material.SetInt("_OpenGray", 0);
			}
		}
	}

	protected virtual void FrameChangeEvent(SwfClip swfClip)
	{
	}

	protected virtual void OnInitForPlace()
	{
	}

	protected virtual void OnInitForCreate()
	{
	}

	protected virtual void OnInitForAlmanac()
	{
	}

	protected virtual void OnInitForAll()
	{
	}

	protected virtual void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
	}

	protected virtual void DeadEvent()
	{
	}

	protected virtual void DeadrattleEvent()
	{
	}

	protected virtual void GoAwakeSpecial()
	{
	}

	protected virtual void GoSleepSpecial()
	{
	}

	protected virtual void GameOverSpecial()
	{
	}

	protected virtual float HandleHurt(float hurt, bool isFlat)
	{
		if (isSleeping)
		{
			hurt *= 2f;
		}
		return hurt;
	}

	public virtual bool SpecialPlantCheck(Grid grid, int NeedSun, string Player)
	{
		return true;
	}

	protected virtual void OwnerSetColor(Color color)
	{
	}

	protected virtual void ChangeFacingEvent()
	{
	}
}
