using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class Spike : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Spike;

	public override bool ZombieCanEat => false;

	protected override Vector2 offSet => new Vector2(0f, -0.1f);

	protected override int attackValue => 20;

	protected override bool HaveShadow => false;

	public override bool CanCarryed => false;

	public override bool CanProtect => false;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "attack")
		{
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "idel";
			}
			if (swfClip.currentFrame == 20)
			{
				Attack();
			}
		}
		if (swfClip.sequence == "idel")
		{
			if ((swfClip.currentFrame == 51 || swfClip.currentFrame == swfClip.frameCount - 1) && currGrid != null)
			{
				CheckAttack();
			}
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
		}
	}

	private void CheckAttack()
	{
		if (currGrid != null && !isSleeping && ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 0.66f, isHypno, needCapsule: true).Count != 0)
		{
			clipController.clip.sequence = "attack";
		}
	}

	private void Attack()
	{
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 0.66f, isHypno, needCapsule: true);
		for (int i = 0; i < zombies.Count; i++)
		{
			zombies[i].Hurt(attackValue, Vector2.up, isHard: false);
		}
	}
}
