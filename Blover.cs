using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class Blover : PlantBase
{
	private int loopNum;

	private bool blowZombie;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Blover;

	protected override void OnInitForPlace()
	{
		loopNum = 0;
		if (!isSleeping)
		{
			clipController.rateScale = 1.5f;
			clipController.clip.sequence = "blow";
		}
	}

	protected override void GoAwakeSpecial()
	{
		clipController.rateScale = 1.5f;
		clipController.clip.sequence = "blow";
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (!(swfClip.sequence == "blow"))
		{
			return;
		}
		if (swfClip.currentFrame == 50)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.blover, base.transform.position);
			blowZombie = true;
			if (!base.IsFacingLeft)
			{
				MapManager.Instance.GetCurrMap(base.transform.position).fog.Blow();
			}
			List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno);
			StartCoroutine(BlowZimbieBack(allZombies));
			for (int i = 0; i < allZombies.Count; i++)
			{
				if (allZombies[i] is BalloonZombie)
				{
					allZombies[i].GetComponent<BalloonZombie>().Blow();
				}
			}
		}
		if (swfClip.currentFrame == swfClip.frameCount - 1)
		{
			if (loopNum < 2)
			{
				swfClip.currentFrame = 95;
				loopNum++;
			}
			else
			{
				blowZombie = false;
				Dead();
			}
		}
	}

	private IEnumerator BlowZimbieBack(List<ZombieBase> zombies)
	{
		float move = 0.5f;
		if (base.IsFacingLeft)
		{
			move = -0.5f;
		}
		while (blowZombie)
		{
			yield return new WaitForFixedUpdate();
			for (int i = 0; i < zombies.Count; i++)
			{
				if (zombies[i].Hp > 0 && !(zombies[i] is Gargantuar) && !(zombies[i] is BungiZombie) && !(zombies[i] is PvPTarget))
				{
					zombies[i].transform.Translate(new Vector2(1f, 0f) * Time.deltaTime * move);
				}
			}
		}
	}

	protected override void DeadEvent()
	{
		blowZombie = false;
	}
}
