using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class BobsledZombie : ZombieBase
{
	public BobsledType Type;

	public Texture2D arm;

	public Texture2D lostArm;

	public Sprite bucket1;

	public Sprite bucket2;

	public Sprite bucket3;

	private int onlineIdNum;

	private Sled sled;

	private float DownY;

	private Transform anim;

	private Vector3 animLocalPos;

	private Coroutine SledCoroutine;

	private List<BobsledZombie> bobsleds = new List<BobsledZombie>();

	protected override GameObject Prefab => GameManager.Instance.GameConf.BobsledZombie;

	protected override float AnToSpeed => 6f;

	protected override float DefSpeed => 5f;

	protected override float attackValue => 50f;

	public override int MaxHP => GetTypeHp();

	public override int OnlineIdNum => onlineIdNum;

	private int GetTypeHp()
	{
		int result = 270;
		BobsledType type = Type;
		if (type != 0 && type == BobsledType.Helmet)
		{
			result = 1370;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		onlineIdNum = 1;
		anim = base.transform.Find("Animation");
		animLocalPos = anim.localPosition;
		REnderer.material.SetTexture("_ArmTex", arm);
		switch (Type)
		{
		case BobsledType.Helmet:
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			break;
		case BobsledType.Sled:
			onlineIdNum = 4;
			if (LVManager.Instance.CurrLVState == LVState.Fighting)
			{
				base.transform.position += new Vector3(0f, 0.5f);
				sled = Object.Instantiate(GameManager.Instance.GameConf.Sled).GetComponent<Sled>();
				sled.transform.SetParent(base.transform);
				sled.transform.localPosition = new Vector3(0.6f, -0.6f);
				sled.CreateInit(clipController.clip.sortingOrder);
				clipController.clip.sequence = "push";
				base.Speed = 4f;
				for (int j = 0; j < 3; j++)
				{
					BobsledZombie component2 = ZombieManager.Instance.CreateOneZombie(GameManager.Instance.GameConf.BobsledZombie, base.CurrLine, new Vector2(base.transform.position.x + 0.7f * (float)(j + 1), base.transform.position.y)).GetComponent<BobsledZombie>();
					component2.Type = BobsledType.Normal;
					component2.transform.SetParent(base.transform);
					component2.clipController.clip.sequence = "push";
					component2.anCanMove = false;
					component2.Speed = 4f;
					component2.capsuleCollider2D.enabled = false;
					component2.OnlineId = OnlineId - j - 1;
					bobsleds.Add(component2);
				}
			}
			break;
		case BobsledType.HelmetAndSled:
			onlineIdNum = 4;
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			if (LVManager.Instance.CurrLVState == LVState.Fighting)
			{
				base.transform.position += new Vector3(0f, 0.5f);
				sled = Object.Instantiate(GameManager.Instance.GameConf.Sled).GetComponent<Sled>();
				sled.transform.SetParent(base.transform);
				sled.transform.localPosition = new Vector3(0.6f, -0.6f);
				sled.CreateInit(clipController.clip.sortingOrder);
				clipController.clip.sequence = "push";
				base.Speed = 4f;
				for (int i = 0; i < 3; i++)
				{
					BobsledZombie component = ZombieManager.Instance.CreateOneZombie(GameManager.Instance.GameConf.BobsledZombie, base.CurrLine, new Vector2(base.transform.position.x + 0.7f * (float)(i + 1), base.transform.position.y)).GetComponent<BobsledZombie>();
					component.Type = BobsledType.Helmet;
					component.transform.SetParent(base.transform);
					component.clipController.clip.sequence = "push";
					component.anCanMove = false;
					component.Speed = 4f;
					component.capsuleCollider2D.enabled = false;
					component.OnlineId = OnlineId - i - 1;
					bobsleds.Add(component);
				}
			}
			break;
		case BobsledType.Normal:
			break;
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			if (swfClip.currentFrame == 70 || swfClip.currentFrame == 170)
			{
				Attack();
			}
			break;
		case "dead1":
			_ = swfClip.currentFrame;
			_ = 1;
			if (swfClip.currentFrame == 123)
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
			if (swfClip.currentFrame == 0 || swfClip.currentFrame == 85)
			{
				anCanMove = true;
			}
			if (swfClip.currentFrame == 60 || swfClip.currentFrame == 150)
			{
				anCanMove = false;
			}
			break;
		case "push":
			if (base.transform.position.x < base.CurrGrid.Position.x + 0.3f)
			{
				clipController.rateScale = 2f;
				clipController.clip.sequence = "jump";
				clipController.loopMode = SwfClipController.LoopModes.Once;
				for (int j = 0; j < bobsleds.Count; j++)
				{
					bobsleds[j].clipController.rateScale = 2f;
					bobsleds[j].clipController.clip.sequence = "jump";
					bobsleds[j].clipController.loopMode = SwfClipController.LoopModes.Once;
				}
			}
			break;
		case "jump":
			if (swfClip.currentFrame == 24)
			{
				if ((bool)sled)
				{
					sled.Jump();
				}
				StartCoroutine(MoveDown());
				for (int i = 0; i < bobsleds.Count; i++)
				{
					StartCoroutine(bobsleds[i].MoveDown());
				}
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1 && anCanMove)
			{
				base.Speed = 1.2f;
			}
			break;
		}
	}

	private IEnumerator MoveDown()
	{
		DownY = anim.position.y - 0.25f;
		while (anim.position.y > DownY)
		{
			yield return new WaitForSeconds(0.02f);
			anim.Translate(new Vector2(0f, -1.33f) * (Time.deltaTime / 1f) / 0.1f);
		}
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

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		if ((bool)sled && SledCoroutine == null && base.CurrGrid.IceRoadNum == 0)
		{
			SledCoroutine = StartCoroutine(SledAbrasion());
		}
	}

	private IEnumerator SledAbrasion()
	{
		base.Speed = 2.5f;
		while (sled.Hp > 0)
		{
			yield return new WaitForSeconds(1f);
			if (!sled)
			{
				break;
			}
			sled.Hp -= 100;
		}
		if ((bool)sled && sled.Hp <= 0)
		{
			SledDead();
		}
	}

	public override void Hurt(int attackValue, Vector2 dirction, bool isHard = true, bool HitSound = true)
	{
		if ((bool)sled)
		{
			sled.Hp -= attackValue;
			if (sled.Hp <= 0)
			{
				SledDead();
			}
			if (attackValue > 0)
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
		}
		else
		{
			base.Hurt(attackValue, dirction, isHard, HitSound);
		}
	}

	private void SledDead()
	{
		if ((bool)sled)
		{
			for (int i = 0; i < bobsleds.Count; i++)
			{
				bobsleds[i].anCanMove = true;
				bobsleds[i].Speed = DefSpeed;
				bobsleds[i].capsuleCollider2D.enabled = true;
				bobsleds[i].clipController.clip.sequence = "walk1";
				bobsleds[i].clipController.loopMode = SwfClipController.LoopModes.Loop;
				bobsleds[i].clipController.GotoAndPlay(1);
				bobsleds[i].transform.SetParent(ZombieManager.Instance.transform);
				bobsleds[i].anim.localPosition = bobsleds[i].animLocalPos;
				bobsleds[i].transform.position -= new Vector3(0f, 0.5f);
			}
			bobsleds.Clear();
			base.Speed = DefSpeed;
			clipController.clip.sequence = "walk1";
			clipController.loopMode = SwfClipController.LoopModes.Loop;
			clipController.GotoAndPlay(1);
			base.transform.position -= new Vector3(0f, 0.5f);
			anim.localPosition = animLocalPos;
			Object.Destroy(sled.gameObject);
		}
	}
}
