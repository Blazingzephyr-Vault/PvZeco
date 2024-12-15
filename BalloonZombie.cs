using System.Collections;
using UnityEngine;

public class BalloonZombie : ZombieBase
{
	public Sprite arm;

	public Sprite lostArm;

	private Transform anim;

	private bool isFlying;

	private float ownerSpeed;

	protected override GameObject Prefab => GameManager.Instance.GameConf.BalloonZombie;

	protected override float AnToSpeed => 5f;

	protected override float DefSpeed => ownerSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => 350;

	protected override float DropHeadScale => 0.66f;

	public bool IsFly()
	{
		return isFlying;
	}

	public override void InitZombieHpState()
	{
		canIce = false;
		ownerSpeed = 2.2f;
		base.Speed = 2.2f;
		dontChangeState = true;
		animator.speed = 1f;
		isFlying = true;
		animator.Play("idel1");
		anim = base.transform.Find("Animation");
		anim.localPosition = new Vector3(-0.42f, 1f);
		Arm1Renderer.sprite = arm;
		Arm2Renderer.enabled = true;
		Arm3Renderer.enabled = true;
		capsuleCollider2D.enabled = false;
		if (LVManager.Instance.GameIsStart && !IsOVer)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ballooninflate, base.transform.position);
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (isFlying && base.Hp > 0)
		{
			anCanMove = false;
			ownerSpeed = 4.5f;
			base.Speed = 4.5f;
			isFlying = false;
			animator.Play("pop");
			StartCoroutine(MoveDown());
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.balloon_pop, base.transform.position);
		}
		if (base.Hp < 180 && Arm3Renderer.enabled)
		{
			DropArm(new Vector3(-0.32f, -0.02f));
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

	private IEnumerator MoveDown()
	{
		while (anim.localPosition.y > 0.84f)
		{
			yield return new WaitForFixedUpdate();
			anim.Translate(new Vector2(0f, -1.33f) * Time.deltaTime);
		}
	}

	public void Blow()
	{
		StartCoroutine(BlowOut());
	}

	private IEnumerator BlowOut()
	{
		if (isFlying)
		{
			do
			{
				yield return new WaitForFixedUpdate();
				base.transform.Translate(new Vector2(1f, 0f) * Time.deltaTime * 20f);
			}
			while (!(base.transform.position.x > 10f));
			Dead(canDropItem: false, 0f);
		}
	}

	public override void SpecialAnimEvent1()
	{
		capsuleCollider2D.enabled = true;
	}

	public override void SpecialAnimEvent2()
	{
		anCanMove = true;
		canIce = true;
		dontChangeState = false;
		base.State = ZombieState.Walk;
		animator.Play("walk1");
	}
}
