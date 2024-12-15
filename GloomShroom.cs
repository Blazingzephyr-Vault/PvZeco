using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class GloomShroom : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.GloomShroom;

	public override PlantType BasePlant => PlantType.FumeShroom;

	public override int BasePlantSunNum => 75;

	protected override int attackValue => 20;

	protected override bool isShroom => true;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.currentFrame == 0)
		{
			CheckAttack();
		}
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "shoot")
			{
				if (swfClip.currentFrame == 45)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Fume, base.transform.position);
				}
				if (swfClip.currentFrame == 46 || swfClip.currentFrame == 65 || swfClip.currentFrame == 84 || swfClip.currentFrame == 106)
				{
					HurtZombie();
				}
			}
		}
		else if (swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}

	private void CheckAttack()
	{
		if (!isSleeping && currGrid != null)
		{
			List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, needCapsule: true, isHypno);
			List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, !isHypno);
			if (zombies.Count > 0 || aroundPlant.Count > 0)
			{
				clipController.clip.sequence = "shoot";
			}
			else
			{
				clipController.clip.sequence = "idel";
			}
		}
	}

	private void HurtZombie()
	{
		if (currGrid != null)
		{
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CircleFumeParticle).transform.position = base.transform.position;
			List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, needCapsule: true, isHypno);
			for (int i = 0; i < zombies.Count; i++)
			{
				zombies[i].Hurt(attackValue, Vector2.zero, isHard: false);
			}
			List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, !isHypno);
			for (int j = 0; j < aroundPlant.Count; j++)
			{
				aroundPlant[j].Hurt(attackValue, null);
			}
		}
	}

	protected override void GoAwakeSpecial()
	{
	}
}
