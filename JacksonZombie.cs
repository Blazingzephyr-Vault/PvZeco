using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class JacksonZombie : ZombieBase
{
	public SpriteRenderer spotlight;

	public Texture2D arm;

	public Texture2D lostArm;

	private int walkGridNum;

	private int dancerNum;

	private int dancerEatNum;

	private int walkNum;

	private int armRaiseNum;

	private Transform anim;

	private List<int> needCreateDancer = new List<int>();

	private List<DancerZombie> dancers = new List<DancerZombie>();

	protected override GameObject Prefab => GameManager.Instance.GameConf.BungiZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 50f;

	public override int MaxHP => 50000;

	public override void InitZombieHpState()
	{
		walkNum = 0;
		armRaiseNum = 0;
		dancerNum = 0;
		spotlight.enabled = false;
		walkGridNum = 0;
		anim = base.transform.Find("Animation");
		needCreateDancer = new List<int> { 1, 2, 3, 4 };
		REnderer.material.SetTexture("_ArmTex", arm);
		if (LVManager.Instance.CurrLVState == LVState.Fighting)
		{
			speed = 1.2f;
			clipController.rateScale = 2f;
			clipController.clip.sequence = "moonwalk";
			ToTurn();
			spotlight.sortingOrder = clipController.clip.sortingOrder + 1;
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat":
			if (swfClip.currentFrame == 25 || swfClip.currentFrame == 80)
			{
				Attack();
			}
			break;
		case "dead1":
			if (swfClip.currentFrame == 90)
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
		case "moonwalk":
			if (walkGridNum > 1)
			{
				StopMoonWalk();
			}
			break;
		case "point":
			if (swfClip.currentFrame == 50)
			{
				base.Speed = DefSpeed;
				clipController.rateScale = 0f;
				CreateDancers();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				anCanMove = true;
				if (base.State == ZombieState.Attack)
				{
					swfClip.sequence = "eat";
				}
				else
				{
					swfClip.sequence = "walk1";
				}
			}
			break;
		case "armraise":
		{
			if (swfClip.currentFrame != swfClip.frameCount - 1)
			{
				break;
			}
			armRaiseNum++;
			ToTurn();
			for (int j = 0; j < dancers.Count; j++)
			{
				dancers[j].ChangeDrict();
			}
			if (armRaiseNum > 3)
			{
				for (int k = 0; k < dancers.Count; k++)
				{
					dancers[k].ChangeSequence("walk1");
				}
				armRaiseNum = 0;
				if (dancerEatNum == 0)
				{
					anCanMove = true;
				}
				swfClip.sequence = "walk1";
			}
			break;
		}
		case "walk1":
			if ((swfClip.currentFrame == 0 || swfClip.currentFrame == 52) && dancerEatNum == 0)
			{
				anCanMove = true;
			}
			if ((swfClip.currentFrame == 20 || swfClip.currentFrame == 80) && dancerEatNum == 0)
			{
				anCanMove = false;
			}
			if (swfClip.currentFrame != swfClip.frameCount - 1 || dancerEatNum != 0)
			{
				break;
			}
			walkNum++;
			if (needCreateDancer.Count > 0 && walkNum > 2)
			{
				walkNum = 0;
				anCanMove = false;
				swfClip.sequence = "point";
			}
			else if (walkNum > 3)
			{
				walkNum = 0;
				for (int i = 0; i < dancers.Count; i++)
				{
					dancers[i].ChangeSequence("armraise");
				}
				anCanMove = false;
				clipController.clip.sequence = "armraise";
			}
			break;
		}
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			clipController.clip.sequence = "moonwalk";
			break;
		case ZombieState.Walk:
			clipController.clip.sequence = "walk1";
			break;
		case ZombieState.Attack:
			if (clipController.clip.sequence == "moonwalk")
			{
				StopMoonWalk();
			}
			else
			{
				clipController.clip.sequence = "eat";
			}
			break;
		case ZombieState.Dead:
			DismissDancers();
			clipController.rateScale = 1f;
			clipController.clip.sequence = "dead1";
			break;
		}
	}

	private void DismissDancers()
	{
		int count = dancers.Count;
		for (int i = 0; i < count; i++)
		{
			dancers[0].LeaveKing();
		}
	}

	private void ToTurn()
	{
		anim.localPosition = new Vector3(2f * Shadow.transform.localPosition.x - anim.localPosition.x, anim.localPosition.y);
		anim.localScale = new Vector3(0f - anim.localScale.x, anim.localScale.y);
	}

	private void StopMoonWalk()
	{
		anCanMove = false;
		spotlight.enabled = true;
		StartCoroutine(StartSpotlight());
		ToTurn();
		clipController.clip.sequence = "point";
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.danceMusic, base.transform.position);
	}

	private void CreateDancers()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position);
		GameObject gameObject = null;
		if (needCreateDancer.Contains(4))
		{
			gameObject = ZombieManager.Instance.CreateOneZombie(GameManager.Instance.GameConf.DancerZombie, base.CurrLine, new Vector2(base.transform.position.x + 1.8f, base.transform.position.y));
			if (gameObject != null)
			{
				DancerZombie component = gameObject.GetComponent<DancerZombie>();
				component.KingInit(this);
				dancers.Add(component);
				if (isHypno)
				{
					component.Hypno();
				}
			}
			needCreateDancer.Remove(4);
		}
		if (needCreateDancer.Contains(3))
		{
			gameObject = ZombieManager.Instance.CreateOneZombie(GameManager.Instance.GameConf.DancerZombie, base.CurrLine, new Vector2(base.transform.position.x - 1.8f, base.transform.position.y));
			if (gameObject != null)
			{
				DancerZombie component2 = gameObject.GetComponent<DancerZombie>();
				component2.KingInit(this);
				dancers.Add(component2);
				if (isHypno)
				{
					component2.Hypno();
				}
			}
			needCreateDancer.Remove(3);
		}
		if (needCreateDancer.Contains(2))
		{
			gameObject = ZombieManager.Instance.CreateOneZombie(GameManager.Instance.GameConf.DancerZombie, base.CurrLine + 1, new Vector2(base.transform.position.x, MapManager.Instance.GetLineY(base.transform.position, base.CurrLine + 1) + 0.1f));
			if (gameObject != null)
			{
				DancerZombie component3 = gameObject.GetComponent<DancerZombie>();
				component3.KingInit(this);
				dancers.Add(component3);
				if (isHypno)
				{
					component3.Hypno();
				}
			}
			needCreateDancer.Remove(2);
		}
		if (!needCreateDancer.Contains(1))
		{
			return;
		}
		gameObject = ZombieManager.Instance.CreateOneZombie(GameManager.Instance.GameConf.DancerZombie, base.CurrLine - 1, new Vector2(base.transform.position.x, MapManager.Instance.GetLineY(base.transform.position, base.CurrLine - 1) + 0.1f));
		if (gameObject != null)
		{
			DancerZombie component4 = gameObject.GetComponent<DancerZombie>();
			component4.KingInit(this);
			dancers.Add(component4);
			if (isHypno)
			{
				component4.Hypno();
			}
		}
		needCreateDancer.Remove(1);
	}

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		walkGridNum++;
	}

	public void MoveUpOver()
	{
		dancerNum++;
		if (dancerNum == dancers.Count)
		{
			ResetAnimationSpeed();
		}
	}

	public void DancerDead(DancerZombie dancer)
	{
		if (dancer.transform.position.x > base.transform.position.x)
		{
			needCreateDancer.Add(4);
		}
		else if (dancer.transform.position.x < base.transform.position.x)
		{
			needCreateDancer.Add(3);
		}
		else if (dancer.transform.position.y > base.transform.position.y)
		{
			needCreateDancer.Add(1);
		}
		else if (dancer.transform.position.y < base.transform.position.y)
		{
			needCreateDancer.Add(2);
		}
		dancers.Remove(dancer);
		dancerNum--;
	}

	public void DancerEat(bool isEat)
	{
		if (isEat)
		{
			dancerEatNum++;
		}
		else
		{
			dancerEatNum--;
		}
		if (dancerEatNum > 0)
		{
			anCanMove = false;
		}
		else if (clipController.clip.sequence != "point")
		{
			anCanMove = true;
		}
		if (dancerEatNum < 0)
		{
			dancerEatNum = 0;
		}
	}

	private IEnumerator StartSpotlight()
	{
		while (spotlight.enabled && spotlight.enabled)
		{
			yield return new WaitForSeconds(1f);
			spotlight.color = new Color32(byte.MaxValue, 127, 127, byte.MaxValue);
			if (spotlight.enabled)
			{
				yield return new WaitForSeconds(1f);
				spotlight.color = new Color32(135, byte.MaxValue, 120, byte.MaxValue);
				if (spotlight.enabled)
				{
					yield return new WaitForSeconds(1f);
					spotlight.color = new Color32(120, 180, byte.MaxValue, byte.MaxValue);
					if (spotlight.enabled)
					{
						yield return new WaitForSeconds(1f);
						spotlight.color = new Color32(byte.MaxValue, byte.MaxValue, 120, byte.MaxValue);
						continue;
					}
					break;
				}
				break;
			}
			break;
		}
	}

	protected override void HypnoEvent()
	{
		DismissDancers();
	}
}
