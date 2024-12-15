using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class DoomShroom : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.DoomShroom;

	protected override int attackValue => 1800;

	protected override bool isShroom => true;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "explode")
			{
				if (swfClip.currentFrame == 1)
				{
					clipController.rateScale = 3f * base.SpeedRate;
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.reverse_explosion, base.transform.position);
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					Dead(isFlat: false, 0.01f, synClient: true);
				}
			}
		}
		else if (currGrid != null && swfClip.currentFrame == 1 && !isSleeping)
		{
			clipController.clip.sequence = "explode";
		}
	}

	protected override void DeadrattleEvent()
	{
		if (isSleeping)
		{
			return;
		}
		EffectPanel.Instance.Spark(new Color(1f, 0.8f, 1f, 0.8f), 0.1f, base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Doomm, base.transform.position);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 5f, needCapsule: false, isHypno);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 5f, !isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int i = 0; i < aroundPlant.Count; i++)
			{
				aroundPlant[i].Hurt(attackValue / aroundPlant.Count, null);
			}
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(attackValue / zombies.Count);
			}
		}
		else
		{
			for (int k = 0; k < zombies.Count; k++)
			{
				zombies[k].BoomHurt(attackValue);
			}
			for (int l = 0; l < aroundPlant.Count; l++)
			{
				aroundPlant[l].Hurt(attackValue, null);
			}
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.EFObj).GetComponent<EFObj>().CreateInit(base.transform.position + new Vector3(-3.14f, 5.96f, 0f), 1, new Color(1f, 1f, 1f, 1f), GetBulletSortOrder());
		Grid grid = currGrid;
		CameraControl.Instance.ShakeCamera(base.transform.position);
		grid.HaveCrater = true;
	}

	protected override void GoAwakeSpecial()
	{
		needFlatDead = false;
		clipController.rateScale = 3f;
		clipController.clip.sequence = "explode";
	}

	protected override void GoSleepSpecial()
	{
		needFlatDead = true;
		clipController.rateScale = 1f;
	}
}
