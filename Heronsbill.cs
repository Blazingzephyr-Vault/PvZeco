using FTRuntime;
using SocketSave;
using UnityEngine;

public class Heronsbill : PlantBase
{
	private float createSunTime = 24f;

	private float lightTime = 1.5f;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Heronsbill;

	public override bool ZombieCanEat => false;

	protected override Vector2 offSet => new Vector2(0f, -0.1f);

	public override bool CanProtect => false;

	protected override void OnInitForPlace()
	{
		InvokeRepeating("CreateSun", createSunTime, createSunTime);
	}

	private void CreateSun()
	{
		if (isSleeping)
		{
			return;
		}
		int shadowNum = SkyManager.Instance.GetShadowNum(currGrid);
		if (Random.Range(0, shadowNum) > shadowNum - 3)
		{
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
		}
	}

	private void InstantiateSun()
	{
		if (currGrid != null)
		{
			SkyManager.Instance.CreatePlantSun(base.transform.position, 10 + currGrid.LightNum * 2, isSun: true, PlacePlayer);
		}
	}

	public override void OnlineSynPlant(SynItem syn)
	{
		base.OnlineSynPlant(syn);
		if (syn.SynCode[0] == 0)
		{
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}
}
