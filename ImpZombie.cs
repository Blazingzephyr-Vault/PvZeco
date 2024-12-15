using System.Collections;
using FTRuntime;
using UnityEngine;

public class ImpZombie : ZombieBase
{
	public Sprite arm;

	public Sprite lostArm;

	private Vector3 scale;

	protected override GameObject Prefab => GameManager.Instance.GameConf.ImpZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 50f;

	public override int MaxHP => 270;

	protected override float DropHeadScale => 0.55f;

	public override void InitZombieHpState()
	{
		Arm1Renderer.sprite = arm;
		Arm2Renderer.enabled = true;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			EatCheck();
			if (swfClip.currentFrame == 51 || swfClip.currentFrame == 109)
			{
				Attack();
			}
			break;
		case "dead1":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				Dead();
			}
			break;
		case "throw1":
			if (swfClip.currentFrame == 104)
			{
				capsuleCollider2D.enabled = true;
				dontChangeState = false;
				base.State = ZombieState.Walk;
				canIce = true;
			}
			break;
		}
		if (base.State == ZombieState.Walk)
		{
			if (swfClip.currentFrame == 0 || swfClip.currentFrame == 50 || swfClip.currentFrame == 125)
			{
				anCanMove = true;
			}
			if (swfClip.currentFrame == 40 || swfClip.currentFrame == 104)
			{
				anCanMove = false;
			}
		}
	}

	public void ThrowInit(Vector2 goal, bool haveDuckytube = false)
	{
		scale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
		canIce = false;
		dontChangeState = true;
		animator.Play("throw");
		StartCoroutine(MoveToGround(goal));
	}

	private IEnumerator MoveToGround(Vector3 vector)
	{
		yield return new WaitForFixedUpdate();
		base.transform.localScale = scale;
		anCanMove = false;
		capsuleCollider2D.enabled = false;
		Vector3 vec = (vector - base.transform.position).normalized;
		while (base.transform.position.y > vector.y)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(vec * Time.deltaTime * 3f);
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180 && Arm2Renderer.enabled)
		{
			DropArm(new Vector3(0.07f, -0.13f));
			Arm1Renderer.sprite = lostArm;
			Arm2Renderer.enabled = false;
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
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().InitCharred(5, base.transform.position, new Vector3(-0.85f, 1.46f), Sorting.sortingOrder, base.IsFacingLeft);
		Dead(canDropItem: true, 0f);
	}

	public override void SpecialAnimEvent1()
	{
		capsuleCollider2D.enabled = true;
		dontChangeState = false;
		base.State = ZombieState.Walk;
		animator.Play("walk1");
		canIce = true;
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if (base.transform.localScale.x == 0f)
		{
			base.transform.localScale = scale;
		}
	}
}
