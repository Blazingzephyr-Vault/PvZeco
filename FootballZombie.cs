using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class FootballZombie : ZombieBase
{
	public FootballZombieType Type;

	public Sprite helmet1;

	public Sprite helmet2;

	public Sprite helmet3;

	public Sprite arm;

	public Sprite lostArm;

	private bool needDropHat;

	private GameObject prefab;

	protected override float DefSpeed => 2.5f;

	protected override float attackValue => 50f;

	public override int MaxHP => GetTypeHp();

	protected override GameObject Prefab => prefab;

	protected override float AnToSpeed => 5f;

	protected override float DropHeadScale => 0.76f;

	private int GetTypeHp()
	{
		int result = 270;
		switch (Type)
		{
		case FootballZombieType.Normal:
			result = 1670;
			break;
		case FootballZombieType.Black:
			result = 3070;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		needDropHat = true;
		Arm1Renderer.sprite = arm;
		Arm2Renderer.enabled = true;
		Arm3Renderer.enabled = true;
		switch (Type)
		{
		case FootballZombieType.Normal:
			prefab = GameManager.Instance.GameConf.Zombie_Football;
			HpState = new List<int> { 1670, 1200, 740, 270 };
			break;
		case FootballZombieType.Black:
			prefab = GameManager.Instance.GameConf.Zombie_BlackFootball;
			HpState = new List<int> { 3070, 1660, 1200, 270 };
			break;
		}
		E1HpStateSprite = new List<Sprite> { helmet1, helmet2, helmet3, null };
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			EatCheck();
			if (swfClip.currentFrame == 30 || swfClip.currentFrame == 76)
			{
				Attack();
			}
			break;
		case "dead1":
			_ = swfClip.currentFrame;
			if (swfClip.currentFrame == 60)
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
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				Dead();
			}
			break;
		case "waterdead1":
			_ = swfClip.currentFrame;
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				Dead();
			}
			break;
		}
	}

	protected override void InWaterChangeEvent()
	{
		if (base.State != ZombieState.Dead)
		{
			if (base.InWater)
			{
				animator.SetInteger("Change", 12);
			}
			else
			{
				animator.SetInteger("Change", 11);
			}
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180 && Arm3Renderer.enabled)
		{
			DropArm(new Vector3(-0.51f, 0.41f), 0.76f);
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

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			animator.Play("idel1");
			break;
		case ZombieState.Walk:
			if (base.InWater)
			{
				animator.SetInteger("Change", 12);
			}
			else
			{
				animator.SetInteger("Change", 11);
			}
			break;
		case ZombieState.Attack:
			animator.SetInteger("Change", 21);
			break;
		case ZombieState.Dead:
			Shadow.enabled = false;
			capsuleCollider2D.enabled = false;
			if (base.InWater)
			{
				animator.SetInteger("Change", 32);
			}
			else
			{
				animator.SetInteger("Change", 31);
			}
			break;
		}
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		SpriteRenderer result = null;
		if (base.Hp > 270 && Type == FootballZombieType.Normal)
		{
			result = EquipRenderer;
			if (needClearEquip)
			{
				needDropHat = false;
				base.Hp = 270;
			}
		}
		return result;
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		if (nextSprite == null && needDropHat)
		{
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZombieEquipParticle).GetComponent<EquipDropParticle>().InitCreate(base.transform.position + new Vector3(-0.1f, 0.8f), Sorting.sortingOrder, EquipRenderer.sprite);
		}
	}
}
