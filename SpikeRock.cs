using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class SpikeRock : PlantBase
{
	private int noFlatNum = 9;

	public Texture2D State1;

	public Texture2D State2;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.SpikeRock;

	public override bool ZombieCanEat => false;

	protected override Vector2 offSet => new Vector2(0f, -0.3f);

	protected override int attackValue => 25;

	protected override bool HaveShadow => false;

	public override PlantType BasePlant => PlantType.Spike;

	public override bool CanCarryed => false;

	public override bool CanProtect => false;

	public override int BasePlantSunNum => 100;

	protected override void OnInitForAll()
	{
		noFlatNum = 9;
		REnderer.material.SetTexture("_SpecialTex", State1);
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "attack")
		{
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				CheckAttack();
			}
			if (swfClip.currentFrame == 26 || swfClip.currentFrame == 56)
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
		if (currGrid != null && !isSleeping)
		{
			if (ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 0.69f, isHypno, needCapsule: true).Count != 0)
			{
				clipController.clip.sequence = "attack";
			}
			else
			{
				clipController.clip.sequence = "idel";
			}
		}
	}

	private void Attack()
	{
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 0.69f, isHypno, needCapsule: true);
		if (zombies.Count != 0)
		{
			for (int i = 0; i < zombies.Count; i++)
			{
				zombies[i].Hurt(attackValue, Vector2.up, isHard: false);
			}
		}
	}

	protected override float HandleHurt(float hurt, bool isFlat)
	{
		if (isFlat && noFlatNum > 0)
		{
			noFlatNum--;
			if (noFlatNum <= 6 && noFlatNum > 3)
			{
				REnderer.material.SetTexture("_SpecialTex", State2);
			}
			else if (noFlatNum <= 3)
			{
				REnderer.material.SetTexture("_SpecialTex", null);
			}
			if (noFlatNum == 0)
			{
				Dead();
			}
			return 0f;
		}
		return hurt;
	}
}
