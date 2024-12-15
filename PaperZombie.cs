using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class PaperZombie : ZombieBase
{
	public Sprite paper1;

	public Sprite paper2;

	public Sprite paper3;

	public Sprite normalArm;

	public Sprite lostArm;

	public Sprite whiteEye;

	public Sprite redEye;

	public SpriteRenderer pupils;

	public SpriteRenderer hands;

	public SpriteRenderer hands2;

	public List<Texture2D> DecoratesTex = new List<Texture2D>();

	private float AnTospeed = 4f;

	private float defSpeed = 5f;

	protected override GameObject Prefab => GameManager.Instance.GameConf.PaperZombie;

	protected override float AnToSpeed => AnTospeed;

	protected override float DefSpeed => defSpeed;

	protected override float attackValue => 150f;

	public override int MaxHP => 400;

	public override void InitZombieHpState()
	{
		AnTospeed = 4f;
		defSpeed = 5f;
		normalHead = whiteEye;
		HeadRenderer.sprite = whiteEye;
		pupils.enabled = true;
		hands.enabled = true;
		hands2.enabled = false;
		Arm3Renderer.enabled = false;
		Arm2Renderer.enabled = true;
		Arm1Renderer.sprite = normalArm;
		DoorHpState = new List<int> { 150, 100, 50, 0 };
		DoorHpStateSprite = new List<Sprite> { paper1, paper2, paper3, null };
		MaxDoorHp = 150;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			if (swfClip.currentFrame == 43)
			{
				Attack();
			}
			break;
		case "eat2":
			if (swfClip.currentFrame == 34)
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
		case "walk1":
			if (swfClip.currentFrame == 10 || swfClip.currentFrame == 120)
			{
				anCanMove = true;
			}
			if (swfClip.currentFrame == 75 || swfClip.currentFrame == 190)
			{
				anCanMove = false;
			}
			break;
		case "walk2":
			if (swfClip.currentFrame == 0 || swfClip.currentFrame == 60)
			{
				anCanMove = true;
			}
			if (swfClip.currentFrame == 38 || swfClip.currentFrame == 96)
			{
				anCanMove = false;
			}
			break;
		case "gasp":
			if (swfClip.currentFrame == 0)
			{
				anCanMove = false;
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				base.Speed = 1.8f;
				defSpeed = 1.8f;
				AnTospeed = 3f;
				anCanMove = true;
				base.State = ZombieState.Walk;
				clipController.clip.sequence = "walk2";
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.paper_rarrgh1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.paper_rarrgh2, base.transform.position);
				}
			}
			break;
		}
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		Sprite sprite = DoorRenderer.sprite;
		if (base.DoorHp <= 0 && (sprite == paper1 || sprite == paper2 || sprite == paper3) && nextSprite == null)
		{
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZombieEquipParticle).GetComponent<EquipDropParticle>().InitCreate(base.transform.position + new Vector3(-0.3f, 0.25f), Sorting.sortingOrder, paper3);
			hands.enabled = false;
			hands2.enabled = true;
			Arm3Renderer.enabled = true;
			if (base.Hp < 180)
			{
				DropArm(new Vector3(0.1f, 0.3f, 0f));
				Arm1Renderer.sprite = lostArm;
				Arm2Renderer.enabled = false;
				Arm3Renderer.enabled = false;
			}
		}
	}

	private void SetDecorate()
	{
		if (DecoratesTex.Count == 0)
		{
			return;
		}
		if (DecoratesTex.Count <= 2)
		{
			REnderer.material.SetTexture("_Decorate1Tex", DecoratesTex[0]);
			if (DecoratesTex.Count == 2)
			{
				REnderer.material.SetTexture("_Decorate2Tex", DecoratesTex[1]);
			}
			return;
		}
		int num = Random.Range(0, DecoratesTex.Count);
		int num2 = Random.Range(0, DecoratesTex.Count);
		while (num == num2)
		{
			num2 = Random.Range(0, DecoratesTex.Count);
		}
		REnderer.material.SetTexture("_Decorate1Tex", DecoratesTex[num]);
		REnderer.material.SetTexture("_Decorate2Tex", DecoratesTex[num2]);
	}

	protected override void DoorHpReduceEvent()
	{
		if (base.DoorHp <= 0)
		{
			anCanMove = false;
			animator.SetInteger("Change", 41);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.paper_rip, base.transform.position);
		}
		else if (Random.Range(0, 3) == 0)
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

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180 && Arm3Renderer.enabled && base.DoorHp <= 0)
		{
			DropArm(new Vector3(0.3f, 0.32f), 0.7f);
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
			if (base.DoorHp > 0)
			{
				animator.SetInteger("Change", 11);
			}
			else
			{
				animator.SetInteger("Change", 12);
			}
			break;
		case ZombieState.Attack:
			if (base.DoorHp > 0)
			{
				animator.SetInteger("Change", 21);
			}
			else
			{
				animator.SetInteger("Change", 22);
			}
			break;
		case ZombieState.Dead:
			base.DoorHp = 0;
			Shadow.enabled = false;
			capsuleCollider2D.enabled = false;
			animator.SetInteger("Change", 31);
			break;
		}
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().InitCharred(1, base.transform.position, new Vector3(-0.74f, 1.45f), Sorting.sortingOrder, base.IsFacingLeft);
		Dead(canDropItem: true, 0f);
	}

	public override void SpecialAnimEvent1()
	{
		base.Speed = 1.8f;
		defSpeed = 1.8f;
		AnTospeed = 3f;
		anCanMove = true;
		base.State = ZombieState.Walk;
		animator.SetInteger("Change", 12);
		normalHead = redEye;
		HeadRenderer.sprite = redEye;
		pupils.enabled = false;
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.paper_rarrgh1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.paper_rarrgh2, base.transform.position);
		}
	}
}
