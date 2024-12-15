using FTRuntime;
using UnityEngine;

public class GoldMagnet : PlantBase
{
	public Texture2D Magnetism;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.GoldMagnet;

	public override int BasePlantSunNum => 100;

	public override PlantType BasePlant => PlantType.Magnetshroom;

	protected override void OnInitForAll()
	{
		REnderer.material.SetTexture("_SpecialTex", Magnetism);
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.currentFrame == swfClip.frameCount - 1)
		{
			CheckCoin();
		}
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "attract" && swfClip.currentFrame == 65)
			{
				GetCoin();
			}
		}
		else if (swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}

	private void CheckCoin()
	{
		if (currGrid == null || isSleeping)
		{
			return;
		}
		for (int i = 0; i < PlayerManager.Instance.Coins.Count; i++)
		{
			if (Vector3.Distance(PlayerManager.Instance.Coins[i].transform.position, base.transform.position) < 15f)
			{
				clipController.clip.sequence = "attract";
				return;
			}
		}
		clipController.clip.sequence = "idel";
	}

	private void GetCoin()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.magnetshroom, base.transform.position);
		int num = 0;
		for (int i = 0; i < PlayerManager.Instance.Coins.Count; i++)
		{
			if (Vector3.Distance(PlayerManager.Instance.Coins[i].transform.position, base.transform.position) < 15f)
			{
				PlayerManager.Instance.Coins[i].DoFlytoMagnet(base.transform.position);
				num++;
			}
			if (num > 4)
			{
				break;
			}
		}
	}

	protected override void GoAwakeSpecial()
	{
		REnderer.material.SetTexture("_SpecialTex", Magnetism);
	}

	protected override void GoSleepSpecial()
	{
		REnderer.material.SetTexture("_SpecialTex", null);
	}
}
