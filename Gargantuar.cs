using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class Gargantuar : ZombieBase
{
	public GargantuarType Type;

	public Sprite BungeeHead;

	public Sprite NormalRedHead;

	public Sprite BungeeRedHead;

	public Sprite NormalArm;

	public Sprite BungeeArm;

	public Sprite helmet1;

	public Sprite helmet2;

	public Sprite helmet3;

	public Texture2D CharredImp;

	public SpriteRenderer WeaponRenderer;

	public List<Sprite> DecoratesTex = new List<Sprite>();

	public List<SpriteRenderer> ImpSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> ImpBagSprites = new List<SpriteRenderer>();

	private bool isNoImp;

	protected override float DefSpeed => 4.2f;

	protected override float attackValue => 2000f;

	public override int MaxHP => GetTypeHp();

	protected override GameObject Prefab => GameManager.Instance.GameConf.Gargantuar;

	protected override float AnToSpeed => 2.8f;

	public override int OnlineIdNum => 2;

	protected override Vector2 LeftAttackDir => new Vector2(0f, -1f);

	public override bool CanEatByChomper => false;

	private int GetTypeHp()
	{
		int result = 3000;
		switch (Type)
		{
		case GargantuarType.Normal:
			result = 3000;
			break;
		case GargantuarType.Injured:
			result = 1500;
			break;
		case GargantuarType.Helmet:
			result = 6000;
			break;
		case GargantuarType.Redeye:
			result = 6000;
			break;
		case GargantuarType.RedeyeAndHelmet:
			result = 9000;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		canButter = true;
		canIce = true;
		canFrozen = true;
		isNoImp = false;
		HpState.Clear();
		E1HpStateSprite.Clear();
		GiveWeapon();
		for (int i = 0; i < ImpSprites.Count; i++)
		{
			ImpSprites[i].enabled = true;
		}
		for (int j = 0; j < ImpBagSprites.Count; j++)
		{
			ImpBagSprites[j].enabled = true;
		}
		HeadRenderer.sprite = normalHead;
		Arm1Renderer.sprite = NormalArm;
		switch (Type)
		{
		case GargantuarType.Injured:
		{
			isNoImp = true;
			Arm1Renderer.sprite = BungeeArm;
			for (int k = 0; k < ImpSprites.Count; k++)
			{
				ImpSprites[k].enabled = false;
			}
			for (int l = 0; l < ImpBagSprites.Count; l++)
			{
				ImpBagSprites[l].enabled = false;
			}
			break;
		}
		case GargantuarType.Helmet:
			HpState = new List<int> { 6000, 5000, 4000, 3000 };
			E1HpStateSprite = new List<Sprite> { helmet1, helmet2, helmet3, null };
			canButter = false;
			canIce = false;
			canFrozen = false;
			break;
		case GargantuarType.Redeye:
			HeadRenderer.sprite = NormalRedHead;
			break;
		case GargantuarType.RedeyeAndHelmet:
			HeadRenderer.sprite = NormalRedHead;
			HpState = new List<int> { 9000, 8000, 7000, 6000 };
			E1HpStateSprite = new List<Sprite> { helmet1, helmet2, helmet3, null };
			canButter = false;
			canIce = false;
			canFrozen = false;
			break;
		}
	}

	private void GiveWeapon()
	{
		WeaponRenderer.sprite = DecoratesTex[Random.Range(0, DecoratesTex.Count)];
	}

	private void ownerAttack()
	{
		if (hypnoAttackTarget != null && hypnoAttackTarget.Hp <= 0)
		{
			hypnoAttackTarget = null;
		}
		if (isHypno)
		{
			if (hypnoAttackTarget != null)
			{
				hypnoAttackTarget.Hurt((int)attackValue, Vector2.down);
			}
			else if (base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.isHypno)
			{
				base.CurrGrid.CurrPlantBase.Hurt(attackValue, this, isFlat: true);
			}
			else if (nextGrid != null && nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.isHypno)
			{
				nextGrid.CurrPlantBase.Hurt(attackValue, this, isFlat: true);
			}
		}
		else if (hypnoAttackTarget != null)
		{
			hypnoAttackTarget.Hurt((int)attackValue, Vector2.down);
		}
		else if (base.CurrGrid.CurrPlantBase != null && !base.CurrGrid.CurrPlantBase.isHypno)
		{
			base.CurrGrid.CurrPlantBase.Hurt(attackValue, this, isFlat: true);
		}
		else if (nextGrid != null && nextGrid.CurrPlantBase != null && !nextGrid.CurrPlantBase.isHypno)
		{
			nextGrid.CurrPlantBase.Hurt(attackValue, this, isFlat: true);
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		ThrowImp();
		if (base.Hp <= MaxHP / 4)
		{
			Arm1Renderer.sprite = BungeeArm;
		}
		else if (base.Hp <= MaxHP / 2)
		{
			if (Type == GargantuarType.Normal || Type == GargantuarType.Helmet || Type == GargantuarType.Injured)
			{
				HeadRenderer.sprite = BungeeHead;
			}
			else
			{
				HeadRenderer.sprite = BungeeRedHead;
			}
		}
		if (HitSound)
		{
			if (Random.Range(0, 3) == 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat1, base.transform.position);
			}
			else if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat2, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat3, base.transform.position);
			}
		}
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		if ((Type == GargantuarType.Helmet || Type == GargantuarType.RedeyeAndHelmet) && nextSprite == null)
		{
			canButter = true;
			canIce = true;
			canFrozen = true;
		}
	}

	private void ThrowImp(bool synClient = false)
	{
		if (GameManager.Instance.isClient && !synClient)
		{
			return;
		}
		if (synClient)
		{
			base.State = ZombieState.Attack;
			animator.SetInteger("Change", 41);
			anCanMove = false;
			isNoImp = true;
		}
		else
		{
			if (base.Hp > MaxHP / 2 || isNoImp)
			{
				return;
			}
			Grid farestGrid = MapManager.Instance.GetFarestGrid(base.transform.position, base.IsFacingLeft, base.CurrLine);
			if (farestGrid != null && Mathf.Abs(farestGrid.Position.x - base.transform.position.x) > 6f)
			{
				base.State = ZombieState.Attack;
				animator.SetInteger("Change", 41);
				anCanMove = false;
				isNoImp = true;
				if (GameManager.Instance.isServer)
				{
					SynItem synItem = new SynItem();
					synItem.OnlineId = OnlineId;
					synItem.Type = 2;
					synItem.SynCode[0] = 2;
					synItem.SynCode[1] = 0;
					SocketServer.Instance.SendSynBag(synItem);
				}
			}
		}
	}

	protected override void EatCheck()
	{
		if (isHypno)
		{
			if (!(hypnoAttackTarget == null) && (hypnoAttackTarget.Hp <= 0 || !hypnoAttackTarget.capsuleCollider2D.enabled || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f))
			{
				hypnoAttackTarget = null;
			}
		}
		else if (hypnoAttackTarget != null && (hypnoAttackTarget.Hp <= 0 || !hypnoAttackTarget.capsuleCollider2D.enabled || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f))
		{
			hypnoAttackTarget = null;
		}
	}

	private void OwnerEatCheck()
	{
		if (isHypno)
		{
			if (hypnoAttackTarget == null)
			{
				base.State = ZombieState.Walk;
			}
			else if (hypnoAttackTarget.Hp <= 0 || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f)
			{
				base.State = ZombieState.Walk;
				hypnoAttackTarget = null;
			}
			else if (base.CurrGrid.CurrPlantBase == null)
			{
				base.State = ZombieState.Walk;
			}
			else if (!base.CurrGrid.CurrPlantBase.isHypno)
			{
				base.State = ZombieState.Walk;
			}
		}
		else if (hypnoAttackTarget != null)
		{
			if (hypnoAttackTarget.Hp <= 0 || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f)
			{
				base.State = ZombieState.Walk;
				hypnoAttackTarget = null;
			}
		}
		else if (base.CurrGrid.CurrPlantBase == null)
		{
			base.State = ZombieState.Walk;
		}
		else if (base.CurrGrid.CurrPlantBase.isHypno)
		{
			base.State = ZombieState.Walk;
		}
		if (base.State == ZombieState.Walk && GameManager.Instance.isServer)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = 2;
			synItem.SynCode[0] = 2;
			synItem.SynCode[1] = 1;
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	protected override void EatStateCheck()
	{
		hypnoAttackTarget = ZombieManager.Instance.GetZombieByLineMinDistance(base.CurrLine, base.transform.position, base.IsFacingLeft, !isHypno);
		if (hypnoAttackTarget != null && Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) <= 0.4f)
		{
			base.State = ZombieState.Attack;
		}
		else if (isHypno)
		{
			if (base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.Position.x < base.transform.position.x && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.65f)
			{
				base.State = ZombieState.Attack;
			}
			else if (nextGrid != null && nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.isHypno && base.transform.position.x - nextGrid.Position.x < 1.5f)
			{
				base.State = ZombieState.Attack;
			}
		}
		else if (base.CurrGrid.CurrPlantBase != null && !base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.Position.x < base.transform.position.x && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.65f)
		{
			base.State = ZombieState.Attack;
		}
		else if (nextGrid != null && nextGrid.CurrPlantBase != null && !nextGrid.CurrPlantBase.isHypno && base.transform.position.x - nextGrid.Position.x < 1.5f)
		{
			base.State = ZombieState.Attack;
		}
	}

	protected override void PlaceCharred()
	{
		CharredZombie component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>();
		component.InitCharred(4, base.transform.position, new Vector3(-0.8f, 1.65f), Sorting.sortingOrder, base.IsFacingLeft);
		if (!isNoImp)
		{
			component.transform.GetComponent<Renderer>().material.SetTexture("_EyeTex", CharredImp);
		}
		Dead(canDropItem: true, 0f);
	}

	public override void OnlineSynZombie(SynItem syn)
	{
		base.OnlineSynZombie(syn);
		if (syn.SynCode[0] == 2)
		{
			if (syn.SynCode[1] == 0)
			{
				ThrowImp(synClient: true);
			}
			else if (syn.SynCode[1] == 1)
			{
				base.State = ZombieState.Walk;
			}
		}
	}

	public override void SpecialAnimEvent1()
	{
		if (base.State == ZombieState.Attack)
		{
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.lowgroan1, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.lowgroan2, base.transform.position);
			}
		}
		else if (base.State == ZombieState.Dead)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gargantuardeath, base.transform.position);
		}
	}

	public override void SpecialAnimEvent2()
	{
		ownerAttack();
		CameraControl.Instance.ShakeCamera(base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gargantuar_thump, base.transform.position);
	}

	public override void SpecialAnimEvent3()
	{
		OwnerEatCheck();
	}

	public override void SpecialAnimEvent4()
	{
		int num = 1;
		if (!base.IsFacingLeft)
		{
			num = -1;
		}
		ImpZombie component = ZombieManager.Instance.CreateOneZombie(GameManager.Instance.GameConf.ImpZombie, base.CurrLine, base.transform.position + new Vector3(-3.5f * (float)num, 1.3f)).GetComponent<ImpZombie>();
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(base.transform.position + new Vector3(-5.5f * (float)num, 0f), base.CurrLine);
		Vector2 goal = new Vector2(base.transform.position.x - 5.5f * (float)num, gridByWorldPos.Position.y);
		if (isHypno)
		{
			if (needHypnoPurple)
			{
				component.Hypno();
			}
			else
			{
				component.RatThis();
			}
		}
		component.ThrowInit(goal, base.InWater);
		component.OnlineId = OnlineId - 1;
		for (int i = 0; i < ImpSprites.Count; i++)
		{
			ImpSprites[i].enabled = false;
		}
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.imp1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.imp2, base.transform.position);
		}
		dontChangeState = false;
	}

	public override void SpecialAnimEvent5()
	{
		base.State = ZombieState.Walk;
		anCanMove = true;
	}

	public override void AnimFailSound()
	{
		CameraControl.Instance.ShakeCamera(base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gargantuar_thump, base.transform.position);
	}
}
