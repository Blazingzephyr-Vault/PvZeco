using System.Collections.Generic;
using FTRuntime;

public class Cherry : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Cherry;

	protected override int attackValue => 1800;

	protected override void OnInitForAll()
	{
		needFlatDead = false;
		REnderer.material.SetTexture("_SpecialTex", eye1);
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "attack")
			{
				if (swfClip.currentFrame == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.reverse_explosion, base.transform.position);
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					Dead(isFlat: false, 0f, synClient: true);
				}
			}
		}
		else if (swfClip.currentFrame == 1 && currGrid != null && !isSleeping)
		{
			clipController.clip.sequence = "attack";
		}
	}

	protected override void DeadrattleEvent()
	{
		Boom();
	}

	private void Boom()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CherryParticle).transform.position = base.transform.position;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.CherryBoom, base.transform.position);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, needCapsule: false, isHypno);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, !isHypno);
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
		CameraControl.Instance.ShakeCamera(base.transform.position);
	}

	protected override void GoAwakeSpecial()
	{
		REnderer.material.SetTexture("_SpecialTex", eye1);
	}

	protected override void GoSleepSpecial()
	{
		REnderer.material.SetTexture("_EyeTex", null);
		REnderer.material.SetTexture("_SpecialTex", eye2);
	}
}
