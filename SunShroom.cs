using System.Collections;
using FTRuntime;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class SunShroom : PlantBase
{
	private float createSunTime;

	private float lightTime;

	private int Sunsum;

	private int growTime;

	private bool isBig;

	private Coroutine GrowCoroutine;

	public Texture2D sleepEye;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.SunShroom;

	protected override bool isShroom => true;

	protected override void OnInitForPlace()
	{
		isBig = false;
		growTime = 10;
		Sunsum = 150;
		createSunTime = 8f;
		lightTime = 1.5f;
        GrowCoroutine = StartCoroutine(Grow());

		if (!GameManager.Instance.isClient)
		{
			InvokeRepeating("CreateSun", createSunTime, createSunTime);
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "grow" && swfClip.currentFrame == swfClip.frameCount - 1)
			{
				isBig = true;
				clipController.clip.sequence = "bigidel";

                clipController.clip.transform.localScale = new Vector3(1.2f, 1.2f);
                //clipController.clip.transform.localScale = new Vector3(0.7f, 0.7f);
			}
		}
		else if (swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}

	private void CreateSun()
	{
		if (isSleeping)
		{
			return;
		}
		if (SkyManager.Instance.GetIsDay() && Random.Range(0, 4) > 2)
		{
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
		}
		else if (!SkyManager.Instance.GetIsDay())
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

	public override void OnlineSynPlant(SynItem syn)
	{
		base.OnlineSynPlant(syn);
		if (syn.SynCode[0] == 0)
		{
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
		}
	}

	protected IEnumerator ColorEffect2(float wantBright, Color targetColor, float delayTime, UnityAction fun)
	{
		float currBright = 1f;
		while (currBright < wantBright)
		{
			yield return new WaitForSeconds(delayTime);
			currBright += 0.05f;
			REnderer.material.SetFloat("_Brightness", currBright);
		}
		REnderer.material.SetFloat("_Brightness", 1f);
		fun?.Invoke();
	}

	private void InstantiateSun()
	{
		if (currGrid != null)
		{
			SkyManager.Instance.CreatePlantSun(base.transform.position, 50, isSun: true, PlacePlayer);
            SkyManager.Instance.CreatePlantSun(base.transform.position, 50, isSun: true, PlacePlayer);
            SkyManager.Instance.CreatePlantSun(base.transform.position, 50, isSun: true, PlacePlayer);
            SkyManager.Instance.CreatePlantSun(base.transform.position, 50, isSun: true, PlacePlayer);
            SkyManager.Instance.CreatePlantSun(base.transform.position, 50, isSun: true, PlacePlayer);
            SkyManager.Instance.CreatePlantSun(base.transform.position, 50, isSun: true, PlacePlayer);
            SkyManager.Instance.CreatePlantSun(base.transform.position, Sunsum, isSun: true, PlacePlayer);
        }
	}

	private IEnumerator Grow()
	{
		while (growTime > 0)
		{
			yield return new WaitForSeconds(1f);
			growTime--;
		}
		Sunsum = 15;
		clipController.clip.sequence = "grow";
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.plantgrow, base.transform.position);
	}

	protected override void GoAwakeSpecial()
	{
		if (!isBig)
		{
			GrowCoroutine = StartCoroutine(Grow());
		}
	}

	protected override void GoSleepSpecial()
	{
		REnderer.material.SetTexture("_EyeTex", sleepEye);
		if (isBig)
		{
			clipController.clip.sequence = "bigidel";
		}
		else if (GrowCoroutine != null)
		{
			StopCoroutine(GrowCoroutine);
		}
	}
}
