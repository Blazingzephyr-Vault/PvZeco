using System.Collections;
using FTRuntime;
using UnityEngine;

public class DancerZombie : ZombieBase
{
	public Texture2D arm;

	public Texture2D lostArm;

	private Transform anim;

	private float animY;

	private int walkGridNum;

	private JacksonZombie theKing;

	protected override GameObject Prefab => GameManager.Instance.GameConf.DancerZombie;

	protected override float AnToSpeed => 8f;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 50f;

	public override int MaxHP => 270;

	public override void InitZombieHpState()
	{
		theKing = null;
		anim = base.transform.Find("Animation");
		animY = anim.transform.localPosition.y;
		walkGridNum = 0;
		clipController.rateScale = 0f;
		anCanMove = false;
		REnderer.material.SetTexture("_ArmTex", arm);
	}

	public void KingInit(JacksonZombie jackson)
	{
		theKing = jackson;
		base.transform.SetParent(jackson.transform);
		anim.position += new Vector3(0f, -2.7f);
		clipController.clip.sequence = "walk1";
		StartCoroutine(MoveOut());
	}

	public void ChangeSequence(string name)
	{
		clipController.clip.sequence = name;
	}

	public void ChangeDrict()
	{
		anim.localPosition = new Vector3(2f * Shadow.transform.localPosition.x - anim.localPosition.x, anim.localPosition.y);
		anim.localScale = new Vector3(0f - anim.localScale.x, anim.localScale.y);
	}

	public void LeaveKing()
	{
		if (theKing.Hp > 0)
		{
			theKing.DancerDead(this);
		}
		theKing = null;
		base.transform.SetParent(ZombieManager.Instance.transform);
		anCanMove = true;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			if (swfClip.currentFrame == 30 || swfClip.currentFrame == 80)
			{
				Attack();
			}
			break;
		case "walk1":
			if (theKing == null)
			{
				if (swfClip.currentFrame == 30 || swfClip.currentFrame == 80)
				{
					anCanMove = false;
				}
				if (swfClip.currentFrame == 0 || swfClip.currentFrame == 52)
				{
					anCanMove = true;
				}
			}
			break;
		case "dead1":
			if (swfClip.currentFrame == 136)
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
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Walk:
			if (theKing != null)
			{
				theKing.DancerEat(isEat: false);
			}
			clipController.clip.sequence = "walk1";
			break;
		case ZombieState.Attack:
			if (theKing != null)
			{
				theKing.DancerEat(isEat: true);
			}
			clipController.clip.sequence = "eat1";
			break;
		case ZombieState.Dead:
			if (theKing != null)
			{
				if (clipController.clip.sequence == "eat1")
				{
					theKing.DancerEat(isEat: false);
				}
				theKing.DancerDead(this);
			}
			base.transform.SetParent(ZombieManager.Instance.transform);
			clipController.loopMode = SwfClipController.LoopModes.Once;
			clipController.clip.sequence = "dead1";
			break;
		}
	}

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		walkGridNum++;
	}

	private IEnumerator MoveOut()
	{
		while (anim.localPosition.y < animY)
		{
			yield return new WaitForSeconds(0.02f);
			anim.Translate(new Vector2(0f, 1.33f) * Time.deltaTime * 5f);
		}
		clipController.rateScale = 1f;
		theKing.MoveUpOver();
	}

	protected override void HypnoEvent()
	{
		LeaveKing();
	}
}
