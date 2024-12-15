using FTRuntime;
using UnityEngine;

public class Coffeebean : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Coffeebean;

	public override bool ZombieCanEat => false;

	protected override bool HaveShadow => false;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => false;

	public override bool isHaveSpecialCheck => true;

	protected override Vector2 offSet => new Vector2(0f, 0.8f);

	public override bool SpecialPlantCheck(Grid grid, int NeedSun, string Player)
	{
		if (grid.CurrFloatPlant == null && grid.CurrPlantBase != null)
		{
			if (grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.isSleeping)
			{
				return true;
			}
			if (grid.CurrPlantBase.isSleeping)
			{
				return true;
			}
			if (grid.CurrPlantBase.ProtectPlant != null && grid.CurrPlantBase.ProtectPlant.isSleeping)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnInitForPlace()
	{
		clipController.loopMode = SwfClipController.LoopModes.Once;
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
					PlayAudio();
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					OverAwake();
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

	private void PlayAudio()
	{
		if (!(currGrid.CurrPlantBase != null))
		{
			return;
		}
		if (currGrid.CurrPlantBase.CarryPlant == null)
		{
			if (currGrid.CurrPlantBase.isSleeping)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
			}
		}
		else if (currGrid.CurrPlantBase.CarryPlant.isSleeping)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
		}
	}

	private void OverAwake()
	{
		if (currGrid == null)
		{
			return;
		}
		if (currGrid.CurrPlantBase != null)
		{
			if (currGrid.CurrPlantBase.CarryPlant != null && currGrid.CurrPlantBase.CarryPlant.isSleeping)
			{
				currGrid.CurrPlantBase.CarryPlant.GoAwake();
			}
			else if (currGrid.CurrPlantBase.isSleeping)
			{
				currGrid.CurrPlantBase.GoAwake();
			}
			else if (currGrid.CurrPlantBase.ProtectPlant != null && currGrid.CurrPlantBase.ProtectPlant.isSleeping)
			{
				currGrid.CurrPlantBase.ProtectPlant.GoAwake();
			}
		}
		Dead();
	}

	protected override void DeadEvent()
	{
		currGrid.CurrFloatPlant = null;
	}

	protected override void GoSleepSpecial()
	{
		GoAwake();
		ZZZ.gameObject.SetActive(value: false);
	}
}
