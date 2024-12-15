using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class Jalapeno : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Jalapeno;

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
			if (sequence == "explode")
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
		else if (currGrid != null && swfClip.currentFrame == 1 && !isSleeping)
		{
			clipController.clip.sequence = "explode";
		}
	}

	protected override void DeadrattleEvent()
	{
		Boom();
	}

	private void Boom()
	{
		if (currGrid == null)
		{
			return;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Jalapenoboom, base.transform.position);
		MapBase currMap = MapManager.Instance.GetCurrMap(base.transform.position);
		List<Iceroad> list = new List<Iceroad>();
		for (int i = 0; i < MapManager.Instance.iceroads.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(MapManager.Instance.iceroads[i].transform.position);
			if (!(currMap != currMap2) && MapManager.Instance.iceroads[i].CurrLine == currGrid.Point.y)
			{
				list.Add(MapManager.Instance.iceroads[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].Dead();
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 15f, isHypno, needCapsule: false);
		List<PlantBase> linePlant = MapManager.Instance.GetLinePlant(base.transform.position, currGrid.Point.y, 15f, !isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int k = 0; k < linePlant.Count; k++)
			{
				linePlant[k].Hurt(attackValue / linePlant.Count, null);
			}
			for (int l = 0; l < zombies.Count; l++)
			{
				zombies[l].BoomHurt(attackValue / zombies.Count);
			}
		}
		else
		{
			for (int m = 0; m < zombies.Count; m++)
			{
				zombies[m].BoomHurt(attackValue);
			}
			for (int n = 0; n < linePlant.Count; n++)
			{
				linePlant[n].Hurt(attackValue, null);
			}
		}
		Object.Instantiate(GameManager.Instance.GameConf.JalapenoBoom).GetComponent<JalapenoBoom>().CreateInit(currGrid, GetBulletSortOrder());
		CameraControl.Instance.ShakeCamera(base.transform.position);
	}

	protected override void GoAwakeSpecial()
	{
		clipController.clip.sequence = "explode";
		REnderer.material.SetTexture("_SpecialTex", eye1);
	}

	protected override void GoSleepSpecial()
	{
		REnderer.material.SetTexture("_EyeTex", null);
		REnderer.material.SetTexture("_SpecialTex", eye2);
	}
}
