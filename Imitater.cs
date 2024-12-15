using FTRuntime;
using UnityEngine;

public class Imitater : PlantBase
{
	private bool IsProtectPlant;

	private PlantType basePlant;

	private int basePlantSunNum;

	private bool canCarryOtherPlant;

	private bool haveSpecialCheck;

	private PlantType PlacePlant;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Imitater;

	public override bool isProtectPlant => IsProtectPlant;

	public override PlantType BasePlant => basePlant;

	public override int BasePlantSunNum => basePlantSunNum;

	public override bool CanCarryOtherPlant => canCarryOtherPlant;

	public override bool isHaveSpecialCheck => haveSpecialCheck;

	public void CopyPlantInfo(PlantBase plant)
	{
		IsProtectPlant = plant.isProtectPlant;
		basePlant = plant.BasePlant;
		haveSpecialCheck = plant.isHaveSpecialCheck;
		basePlantSunNum = plant.BasePlantSunNum;
		canCarryOtherPlant = plant.CanCarryOtherPlant;
		if (canCarryOtherPlant)
		{
			haveSpecialCheck = true;
		}
		PlacePlant = plant.GetPlantType();
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "turn")
		{
			if (swfClip.currentFrame == 65)
			{
				Object.Instantiate(GameManager.Instance.GameConf.ImitaterParticle).transform.position = base.transform.position;
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1 && !GameManager.Instance.isClient)
			{
				Dead(isFlat: false, 1f);
				PlantBase newPlant = PlantManager.Instance.GetNewPlant(PlacePlant);
				if (SeedBank.Instance.CheckPlant(newPlant, currGrid, -2, PlacePlayer))
				{
					SeedBank.Instance.PlantConfirm(newPlant, currGrid, -2, 1, PlacePlayer);
					if (isHypno && LV.Instance.CurrLVType != LVType.PvP)
					{
						if (needHypnoPurple)
						{
							newPlant.Hypno();
						}
						else
						{
							newPlant.RatThis();
						}
					}
				}
				else
				{
					Object.Destroy(newPlant.gameObject);
				}
				Dead();
			}
		}
		if (swfClip.sequence == "idel" && currGrid != null && swfClip.currentFrame == swfClip.frameCount - 1)
		{
			swfClip.sequence = "turn";
		}
	}

	protected override void GoSleepSpecial()
	{
		GoAwake();
		ZZZ.gameObject.SetActive(value: false);
	}
}
