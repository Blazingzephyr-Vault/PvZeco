using FTRuntime;
using UnityEngine;

public class Pumpkin : PlantBase
{
	public Sprite State2;

	public Sprite State3;

	private int state1;

	private int state2;

	private SwfClipController backClipController;

	public override float MaxHp => 4000f;

	protected override PlantType plantType => PlantType.Pumpkin;

	public override bool isProtectPlant => true;

	protected override Vector2 offSet => new Vector2(0f, -0.3f);

	public override bool CanProtect => false;

	protected override void OnInitForAll()
	{
		if (backClipController == null)
		{
			state1 = (int)MaxHp / 3 * 2;
			state2 = (int)MaxHp / 3;
			backClipController = base.transform.Find("Back").GetComponent<SwfClipController>();
		}
		clipController.clip.NewSprite = null;
	}

	protected override void OnInitForCreate()
	{
		backClipController.rateScale = 0f;
		backClipController.clip.currentFrame = 0;
		backClipController.clip.sortingOrder = clipController.clip.sortingOrder - 1;
		backClipController.GetComponent<Renderer>().material.SetColor("_Color", REnderer.material.GetColor("_Color"));
	}

	protected override void OnInitForPlace()
	{
		int num = clipController.clip.sortingOrder / 100 * 100;
		backClipController.rateScale = clipController.rateScale;
		backClipController.clip.sortingOrder = num + 12;
		backClipController.clip.currentFrame = 0;
		clipController.clip.sortingOrder = num + 51;
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
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

	public override void OpenBlackAndWhite(bool isOpen)
	{
		if (backClipController == null)
		{
			state1 = (int)MaxHp / 3 * 2;
			state2 = (int)MaxHp / 3;
			backClipController = base.transform.Find("Back").GetComponent<SwfClipController>();
		}
		if (isOpen)
		{
			backClipController.GetComponent<Renderer>().material.SetInt("_OpenGray", 1);
		}
		else
		{
			backClipController.GetComponent<Renderer>().material.SetInt("_OpenGray", 0);
		}
		base.OpenBlackAndWhite(isOpen);
	}

	protected override void OwnerSetColor(Color color)
	{
		backClipController.GetComponent<Renderer>().material.SetColor("_Color", color);
	}
}
