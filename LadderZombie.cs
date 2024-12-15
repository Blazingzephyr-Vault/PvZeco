using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class LadderZombie : ZombieBase
{
	public Sprite ladder1;

	public Sprite ladder2;

	public Sprite ladder3;

	public Texture2D arm;

	public Texture2D lostArm;

	public SpriteRenderer Ladder;

	protected override GameObject Prefab => GameManager.Instance.GameConf.LadderZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 50f;

	public override int MaxHP => 500;

	public override void InitZombieHpState()
	{
		if (LVManager.Instance.CurrLVState == LVState.Fighting)
		{
			base.Speed = 1.8f;
		}
		REnderer.material.SetTexture("_ArmTex", arm);
		MaxDoorHp = 500;
		DoorHpState = new List<int> { MaxDoorHp, 300, 150, 0 };
		DoorHpStateSprite = new List<Sprite> { ladder1, ladder2, ladder3, null };
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180)
		{
			REnderer.material.SetTexture("_ArmTex", lostArm);
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

	protected override void DoorHpReduceEvent()
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			if (swfClip.currentFrame == 70)
			{
				Attack();
			}
			break;
		case "eat2":
			if (swfClip.currentFrame == 70)
			{
				Attack();
			}
			break;
		case "dead1":
			_ = swfClip.currentFrame;
			_ = 1;
			if (swfClip.currentFrame == 155)
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
		}
		if (base.State == ZombieState.Walk)
		{
			if (swfClip.currentFrame == 0 || swfClip.currentFrame == 120)
			{
				anCanMove = true;
			}
			if (swfClip.currentFrame == 75 || swfClip.currentFrame == 190)
			{
				anCanMove = false;
			}
		}
	}

	protected override void SpriteChangeEvent(Texture2D texture)
	{
		if ((REnderer.material.GetTexture("_Equip3Tex") == ladder1 || REnderer.material.GetTexture("_Equip3Tex") == ladder2 || REnderer.material.GetTexture("_Equip3Tex") == ladder3) && texture == null)
		{
			CheckState();
			base.Speed = 5f;
		}
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			clipController.clip.sequence = "idel1";
			break;
		case ZombieState.Walk:
			if (base.DoorHp > 0)
			{
				clipController.clip.sequence = "walk1";
			}
			else
			{
				clipController.clip.sequence = "walk2";
			}
			break;
		case ZombieState.Attack:
			if (base.DoorHp > 0)
			{
				clipController.clip.sequence = "eat1";
			}
			else
			{
				clipController.clip.sequence = "eat2";
			}
			break;
		case ZombieState.Dead:
			capsuleCollider2D.enabled = false;
			clipController.clip.sequence = "dead1";
			clipController.loopMode = SwfClipController.LoopModes.Once;
			break;
		}
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		Texture texture = REnderer.material.GetTexture("_Equip3Tex");
		SpriteRenderer result = null;
		if (texture == ladder1 || texture == ladder2 || texture == ladder3)
		{
			result = Ladder;
		}
		if (needClearEquip)
		{
			base.DoorHp = 0;
		}
		return result;
	}
}
