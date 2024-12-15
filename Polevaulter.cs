using System.Collections;
using UnityEngine;

public class Polevaulter : ZombieBase
{
	private bool isJump;

	private float aniSpeed;

	public Sprite normalArm;

	public Sprite lostArm;

	protected override float DefSpeed => 2.5f;

	protected override float attackValue => 50f;

	public override int MaxHP => 500;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Polevaulter;

	protected override float AnToSpeed => aniSpeed;

	private void FixedUpdate()
	{
		if (!isJump && base.State == ZombieState.Walk && nextGrid != null && nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.ZombieCanEat && ((isHypno && nextGrid.CurrPlantBase.isHypno) || (!isHypno && !nextGrid.CurrPlantBase.isHypno)) && base.transform.position.x - nextGrid.CurrPlantBase.transform.position.x < 1.3f)
		{
			aniSpeed = 5f;
			base.State = ZombieState.Attack;
		}
	}

	public override void InitZombieHpState()
	{
		aniSpeed = 2.5f;
		canButter = true;
		isJump = false;
		animator.Play("run");
		Arm1Renderer.sprite = normalArm;
		Arm2Renderer.enabled = true;
		Arm3Renderer.enabled = true;
		Shadow.transform.localPosition = new Vector3(0f, -0.5f, 0f);
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().InitCharred(1, base.transform.position, new Vector3(-0.72f, 1.43f), Sorting.sortingOrder, base.IsFacingLeft);
		Dead(canDropItem: true, 0f);
	}

	private void CheckHigh()
	{
		if ((base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.Tallnut) || (base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.CarryPlant != null && base.CurrGrid.CurrPlantBase.CarryPlant.GetPlantType() == PlantType.Tallnut) || (nextGrid != null && nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.GetPlantType() == PlantType.Tallnut) || (nextGrid != null && nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.CarryPlant != null && nextGrid.CurrPlantBase.CarryPlant.GetPlantType() == PlantType.Tallnut))
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bonk, base.transform.position);
			capsuleCollider2D.enabled = true;
			base.State = ZombieState.Walk;
			animator.Play("walk1", 0, 0f);
			if (!base.InWater)
			{
				Shadow.enabled = true;
			}
			isJump = true;
			base.Speed = 5f;
		}
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			animator.Play("idel1");
			base.Speed = 5f;
			break;
		case ZombieState.Walk:
			animator.SetInteger("Change", 11);
			break;
		case ZombieState.Attack:
			if (isJump)
			{
				animator.SetInteger("Change", 21);
				break;
			}
			canButter = false;
			canIce = false;
			animator.SetInteger("Change", 41);
			break;
		case ZombieState.Dead:
			Shadow.enabled = false;
			capsuleCollider2D.enabled = false;
			animator.SetInteger("Change", 31);
			break;
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180 && Arm3Renderer.enabled && base.DoorHp <= 0)
		{
			DropArm(new Vector3(0.24f, 0.13f), 0.4f);
			Arm1Renderer.sprite = lostArm;
			Arm2Renderer.enabled = false;
			Arm3Renderer.enabled = false;
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

	protected override void EatCheck()
	{
		if (isJump)
		{
			base.EatCheck();
		}
	}

	public override void SpecialAnimEvent1()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.grassstep, base.transform.position);
		capsuleCollider2D.enabled = false;
		Shadow.enabled = false;
		CheckHigh();
	}

	public override void AnimFailSound()
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

	public override void SpecialAnimEvent2()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Polevault, base.transform.position);
	}

	public override void SpecialAnimEvent3()
	{
		if (!base.InWater)
		{
			Shadow.enabled = true;
		}
		if (base.IsFacingLeft)
		{
			Shadow.transform.position += new Vector3(-2.4f, 0f, 0f);
		}
		else
		{
			Shadow.transform.position += new Vector3(2.4f, 0f, 0f);
		}
	}

	public override void SpecialAnimEvent4()
	{
		animator.Play("walk1");
	}

	public override void SpecialAnimEvent5()
	{
		if (!isJump)
		{
			isJump = true;
			base.State = ZombieState.Walk;
			base.Speed = 5f;
			canButter = true;
			canIce = true;
			capsuleCollider2D.enabled = true;
			if (base.IsFacingLeft)
			{
				Shadow.transform.position += new Vector3(2.4f, 0f, 0f);
				base.transform.position += new Vector3(-2.4f, 0f, 0f);
			}
			else
			{
				Shadow.transform.position += new Vector3(-2.4f, 0f, 0f);
				base.transform.position += new Vector3(2.4f, 0f, 0f);
			}
		}
	}

	public override void SpecialAnimEvent6()
	{
		float num = MapManager.Instance.GetGridByWorldPos(base.transform.position + new Vector3(-2.4f, 0f, 0f), base.CurrLine).Position.y;
		if (base.InWater)
		{
			num -= 0.5f;
		}
		StartCoroutine(MoveDown(num));
	}

	private IEnumerator MoveDown(float y)
	{
		if (base.transform.position.y > y)
		{
			while (base.transform.position.y > y)
			{
				yield return new WaitForFixedUpdate();
				base.transform.Translate(new Vector2(0f, -3f) * Time.deltaTime);
			}
		}
		else
		{
			while (base.transform.position.y < y)
			{
				yield return new WaitForFixedUpdate();
				base.transform.Translate(new Vector2(0f, 3f) * Time.deltaTime);
			}
		}
	}
}
