using System.Collections.Generic;
using UnityEngine;

public class CatapultZombie : ZombieBase
{
	public CatapultZombieType Type;

	public SpriteRenderer ball1;

	public SpriteRenderer ball2;

	public SpriteRenderer ball3;

	public SpriteRenderer ball4;

	public SpriteRenderer sidingRenderer;

	public SpriteRenderer poleRenderer;

	public Sprite bucket1;

	public Sprite bucket2;

	public Sprite bucket3;

	public Sprite Siding1;

	public Sprite Siding2;

	public Sprite pole1;

	public Sprite pole1noBall;

	public Sprite pole2;

	public Sprite pole2noBall;

	private Grid BasketGrid;

	private int BasketNum = 20;

	private bool isHalfHp;

	protected override GameObject Prefab => GameManager.Instance.GameConf.CatapultZombie;

	protected override float AnToSpeed => 5f;

	protected override float DefSpeed => 5f;

	protected override float attackValue => 800f;

	public override int MaxHP => GetTypeHp();

	private int GetTypeHp()
	{
		int result = 850;
		switch (Type)
		{
		case CatapultZombieType.Normal:
			result = 850;
			break;
		case CatapultZombieType.Bucket:
			result = 1950;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		BasketNum = 20;
		isHalfHp = false;
		ball1.enabled = true;
		ball2.enabled = true;
		ball3.enabled = true;
		ball4.enabled = true;
		sidingRenderer.sprite = Siding1;
		poleRenderer.sprite = pole1;
		if (Type == CatapultZombieType.Bucket)
		{
			HpState = new List<int> { 1950, 1600, 1240, 850 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
		}
	}

	private void FixedUpdate()
	{
		if (!IsOVer && (base.State == ZombieState.Walk || base.State == ZombieState.Attack))
		{
			if (nextGrid != null && nextGrid.CurrPlantBase != null && Vector2.Distance(nextGrid.Position, base.transform.position) < 1f)
			{
				HurtPlant(nextGrid);
			}
			if (base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && Vector2.Distance(base.CurrGrid.Position, base.transform.position) < 0.7f)
			{
				HurtPlant(base.CurrGrid);
			}
		}
	}

	private void HurtPlant(Grid grid)
	{
		if (grid.CurrPlantBase.GetPlantType() == PlantType.Spike || grid.CurrPlantBase.GetPlantType() == PlantType.SpikeRock)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.balloon_pop, base.transform.position);
			if (grid.CurrPlantBase.GetPlantType() == PlantType.Spike)
			{
				grid.CurrPlantBase.Hurt(attackValue, this);
			}
			else
			{
				grid.CurrPlantBase.Hurt(attackValue, this, isFlat: true);
			}
			base.State = ZombieState.Dead;
		}
		else if (isHypno && grid.CurrPlantBase.isHypno)
		{
			grid.CurrPlantBase.Hurt(attackValue, this, isFlat: true);
		}
		else if (!isHypno && !grid.CurrPlantBase.isHypno)
		{
			grid.CurrPlantBase.Hurt(attackValue, this, isFlat: true);
		}
	}

	protected override void EatCheck()
	{
		if (BasketNum <= 0)
		{
			base.EatCheck();
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 300 && !isHalfHp)
		{
			isHalfHp = true;
			if (BasketNum > 0)
			{
				poleRenderer.sprite = pole2;
			}
			else
			{
				poleRenderer.sprite = pole2noBall;
			}
			sidingRenderer.sprite = Siding2;
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

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().InitCharred(2, base.transform.position, new Vector2(-1.14f, 1.52f), Sorting.sortingOrder, base.IsFacingLeft);
		Dead(canDropItem: true, 0f);
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			animator.Play("idel1");
			break;
		case ZombieState.Walk:
			animator.SetInteger("Change", 11);
			break;
		case ZombieState.Attack:
			if (BasketNum > 0)
			{
				animator.SetInteger("Change", 21);
			}
			break;
		case ZombieState.Dead:
			Shadow.enabled = false;
			capsuleCollider2D.enabled = false;
			if (base.Hp <= 0)
			{
				SpecialAnimEvent4();
			}
			else
			{
				animator.SetInteger("Change", 31);
			}
			break;
		}
	}

	public override void SpecialAnimEvent1()
	{
		if (BasketGrid != null || hypnoAttackTarget != null)
		{
			int num = 1;
			if (!base.IsFacingLeft)
			{
				num = -1;
			}
			Basketball component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Basketball).GetComponent<Basketball>();
			if (BasketGrid != null)
			{
				component.Init(base.transform.position + new Vector3(1.4f * (float)num, 1.3f), BasketGrid, 75);
			}
			else
			{
				component.Init(base.transform.position + new Vector3(1.4f * (float)num, 1.3f), hypnoAttackTarget, 75);
			}
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.basketball, base.transform.position);
			if (isHalfHp)
			{
				poleRenderer.sprite = pole2noBall;
			}
			else
			{
				poleRenderer.sprite = pole1noBall;
			}
			BasketNum--;
		}
	}

	public override void SpecialAnimEvent2()
	{
		anCanMove = true;
		if (BasketNum > 0 && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) > 0.9f)
		{
			return;
		}
		BasketGrid = MapManager.Instance.GetLastPlantGrid(base.transform.position, base.CurrLine, base.IsFacingLeft, isHypno);
		if (BasketGrid != null && BasketNum > 0)
		{
			base.State = ZombieState.Attack;
		}
		else if (BasketNum > 0)
		{
			hypnoAttackTarget = ZombieManager.Instance.GetLastZombieByLine(base.CurrLine, base.transform.position, !base.IsFacingLeft, !isHypno);
			if (hypnoAttackTarget != null)
			{
				base.State = ZombieState.Attack;
			}
		}
		if (base.State == ZombieState.Attack && hypnoAttackTarget != null && BasketNum <= 0)
		{
			Vector2 dirction = LeftAttackDir;
			if (!base.IsFacingLeft)
			{
				dirction = new Vector2(0f - LeftAttackDir.x, LeftAttackDir.y);
			}
			hypnoAttackTarget.Hurt(25, dirction);
		}
	}

	public override void SpecialAnimEvent3()
	{
		if (BasketNum == 4)
		{
			ball1.enabled = false;
		}
		if (BasketNum == 3)
		{
			ball2.enabled = false;
		}
		if (BasketNum == 2)
		{
			ball3.enabled = false;
		}
		if (BasketNum == 1)
		{
			ball4.enabled = false;
		}
		if (BasketNum != 0)
		{
			if (isHalfHp)
			{
				poleRenderer.sprite = pole2;
			}
			else
			{
				poleRenderer.sprite = pole1;
			}
		}
		BasketGrid = MapManager.Instance.GetLastPlantGrid(base.transform.position, base.CurrLine, base.IsFacingLeft, isHypno);
		if (BasketGrid == null || BasketNum == 0)
		{
			base.State = ZombieState.Walk;
			return;
		}
		hypnoAttackTarget = ZombieManager.Instance.GetLastZombieByLine(base.CurrLine, base.transform.position, !base.IsFacingLeft);
		if (hypnoAttackTarget == null)
		{
			base.State = ZombieState.Walk;
		}
	}

	public override void SpecialAnimEvent4()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.explosion, base.transform.position);
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CloudParticle).transform.position = base.transform.position;
		Dead(canDropItem: true, 0f);
	}
}
