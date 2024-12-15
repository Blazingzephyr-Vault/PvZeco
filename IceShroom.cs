using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class IceShroom : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.IceShroom;

	protected override int attackValue => 20;

	protected override bool isShroom => true;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "idel" && currGrid != null && swfClip.currentFrame == swfClip.frameCount - 1 && !isSleeping)
		{
			Dead();
		}
	}

	protected override void DeadrattleEvent()
	{
		if (!isSleeping)
		{
			Ice();
		}
	}

	private void Ice()
	{
		if (isSleeping)
		{
			return;
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.IceParticle).transform.position = base.transform.position;
		EffectPanel.Instance.Spark(new Color(0.02f, 1f, 0.96f, 0.3f), 0.05f, base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Frozen, base.transform.position);
		List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno);
		for (int i = 0; i < allZombies.Count; i++)
		{
			allZombies[i].Frozen(Vector2.zero, isAudio: false, 10);
			allZombies[i].Ice();
			if (allZombies[i].capsuleCollider2D.enabled)
			{
				allZombies[i].Hurt(attackValue, Vector2.zero, isHard: false);
			}
		}
		List<PlantBase> allPlant = MapManager.Instance.GetAllPlant(base.transform.position, !isHypno);
		for (int j = 0; j < allPlant.Count; j++)
		{
			allPlant[j].Hurt(attackValue, null);
		}
	}
}
