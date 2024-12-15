using FTRuntime;
using UnityEngine;

public class Garlic : PlantBase
{
	public Sprite State2;

	public Sprite State3;

	private int state1;

	private int state2;

	public override float MaxHp => 400f;

	protected override PlantType plantType => PlantType.Garlic;

	protected override void OnInitForAll()
	{
		clipController.clip.NewSprite = null;
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (zombie != null && !isFlat)
		{
			zombie.Yuck();
		}
		if (base.Hp <= (float)state1 && base.Hp >= (float)state2)
		{
			clipController.clip.NewSprite = State2;
		}
		else if (base.Hp <= (float)state2)
		{
			clipController.clip.NewSprite = State3;
		}
		else
		{
			clipController.clip.NewSprite = null;
		}
	}

	protected override void OnInitForPlace()
	{
		state1 = (int)MaxHp / 3 * 2;
		state2 = (int)MaxHp / 3;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}
}
