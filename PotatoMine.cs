using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class PotatoMine : PlantBase
{
	private bool isGrowOver;

	private int GlowSeconds = 15;

	private Coroutine GrowCoroutine;

	private ZombieBase zombieBoom;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.PotatoMine;

	protected override int attackValue => 1800;

	public override bool CanPlaceOnWaterCarry => false;

	protected override void OnInitForPlace()
	{
		GlowSeconds = 15;
		isGrowOver = false;
		zombieBoom = null;
		if (!isSleeping)
		{
			GrowCoroutine = StartCoroutine(WaitGrow());
		}
		clipController.clip.sequence = "grow";
		clipController.rateScale = 0f;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "idel")
		{
			Attack();
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
		}
		if (swfClip.sequence == "grow" && swfClip.currentFrame == swfClip.frameCount - 1)
		{
			clipController.clip.sequence = "idel";
		}
	}

	private IEnumerator WaitGrow()
	{
		while (GlowSeconds > 0)
		{
			yield return new WaitForSeconds(1f);
			GlowSeconds--;
		}
		isGrowOver = true;
		clipController.rateScale = 1f;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position);
	}

	private void Attack()
	{
		if (currGrid != null)
		{
			ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(currGrid.Point.y, base.transform.position, isHypno);
			if (!(zombieByLineMinDisNoDir == null) && Mathf.Abs(zombieByLineMinDisNoDir.transform.position.x - base.transform.position.x) < 0.7f)
			{
				Boom();
			}
		}
	}

	private void Boom()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PotatoParticle).transform.position = base.transform.position;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PotatoMineboom, base.transform.position);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 1f, needCapsule: false, isHypno);
		for (int i = 0; i < zombies.Count; i++)
		{
			if (zombies[i] != zombieBoom)
			{
				zombies[i].BoomHurt(attackValue);
			}
		}
		CameraControl.Instance.ShakeCamera(base.transform.position);
		Dead();
	}

	protected override void GoAwakeSpecial()
	{
		if (!isGrowOver)
		{
			clipController.clip.sequence = "grow";
			if (GrowCoroutine != null)
			{
				StopCoroutine(GrowCoroutine);
			}
			GrowCoroutine = StartCoroutine(WaitGrow());
		}
	}

	protected override void GoSleepSpecial()
	{
		if (!isGrowOver)
		{
			clipController.clip.sequence = "grow";
			if (GrowCoroutine != null)
			{
				StopCoroutine(GrowCoroutine);
			}
		}
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (zombie != null && isGrowOver && zombie.Hp > 0)
		{
			zombieBoom = zombie;
			zombie.BoomHurt(attackValue);
			Boom();
		}
	}
}
