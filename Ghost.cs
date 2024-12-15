using SocketSave;
using UnityEngine;

public class Ghost : ZombieBase
{
	private bool isLight;

	private float OwnerSpeed;

	private int OwnerHp;

	public override int MaxHP => OwnerHp;

	protected override GameObject Prefab => GameManager.Instance.GameConf.GhostZombie;

	protected override float AnToSpeed => OwnerSpeed;

	protected override float DefSpeed => OwnerSpeed;

	protected override float attackValue => 5f;

	private void FixedUpdate()
	{
		if (base.State != ZombieState.Walk || base.CurrGrid == null || !(Vector2.Distance(base.transform.position, base.CurrGrid.Position) < 0.8f))
		{
			return;
		}
		if (!isLight)
		{
			if (base.CurrGrid.LightNum > 0)
			{
				isLight = true;
				capsuleCollider2D.enabled = true;
				SetAllColor(new Color(1f, 1f, 1f, 1f));
			}
		}
		else if (base.CurrGrid.LightNum <= 0)
		{
			isLight = false;
			capsuleCollider2D.enabled = false;
			SetAllColor(new Color(1f, 1f, 1f, 0.6f));
		}
	}

	public override void InitZombieHpState()
	{
		needInWater = false;
		needChangeLine = false;
		capsuleCollider2D.enabled = false;
		canButter = false;
		canFrozen = false;
		dontChangeState = true;
		isLight = false;
		OwnerSpeed = Random.Range(0.8f, 2.5f);
		OwnerHp = (int)(10f + OwnerSpeed * 20f);
		if (OwnerSpeed > 1.5f)
		{
			OwnerHp += 20;
		}
		if (OwnerSpeed > 2f)
		{
			OwnerHp += 40;
		}
		SetAllColor(new Color(1f, 1f, 1f, 0.6f));
		Shadow.enabled = false;
		animator.speed = 1f;
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp <= 0 && base.State != ZombieState.Dead)
		{
			base.State = ZombieState.Dead;
		}
	}

	protected override void ClientInit(ZombieSpawn spawnInfo)
	{
		OwnerSpeed = spawnInfo.DefSpeed;
		OwnerHp = (int)(10f + OwnerSpeed * 20f);
		if (OwnerSpeed > 1.5f)
		{
			OwnerHp += 40;
		}
		if (OwnerSpeed > 2f)
		{
			OwnerHp += 40;
		}
	}

	protected override void ServerInitInfo(ZombieSpawn spawnInfo)
	{
		spawnInfo.DefSpeed = OwnerSpeed;
	}
}
