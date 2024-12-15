using FTRuntime;
using UnityEngine;

public class Mint : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Mint;

	public override bool ZombieCanEat => false;

	protected override bool HaveShadow => false;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => false;

	public override bool isHaveSpecialCheck => true;

	protected override Vector2 offSet => new Vector2(0f, 0.8f);

	public override bool SpecialPlantCheck(Grid grid, int NeedSun, string Player)
	{
		if (grid.CurrFloatPlant == null)
		{
			return true;
		}
		return false;
	}

	protected override void OnInitForPlace()
	{
		clipController.clip.sortingOrder = clipController.clip.sortingOrder / 100 * 100 + 51;
		currGrid.CurrFloatPlant = this;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "twitch")
			{
				if (swfClip.currentFrame == 58)
				{
					OverAwake();
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					Dead();
				}
			}
		}
		else
		{
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			if (currGrid != null && swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "twitch";
			}
		}
	}

	private void OverAwake()
	{
		if (currGrid.CurrPlantBase != null)
		{
			if (currGrid.CurrPlantBase.CarryPlant != null && SeedBank.Instance.ClearCD(currGrid.CurrPlantBase.CarryPlant.GetPlantType()))
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
			}
			else if (currGrid.CurrPlantBase != null && SeedBank.Instance.ClearCD(currGrid.CurrPlantBase.GetPlantType()))
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
			}
			else if (currGrid.CurrPlantBase.ProtectPlant != null && SeedBank.Instance.ClearCD(currGrid.CurrPlantBase.ProtectPlant.GetPlantType()))
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
			}
		}
	}

	protected override void DeadEvent()
	{
		currGrid.CurrFloatPlant = null;
	}

	protected override void GoSleepSpecial()
	{
		ZZZ.gameObject.SetActive(value: false);
	}
}
