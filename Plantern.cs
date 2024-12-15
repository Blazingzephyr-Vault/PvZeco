using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Plantern : PlantBase
{
	public Texture2D lightOut;

	private bool isLight;

	public Light2D light2d;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Plantern;

	protected override void DeadEvent()
	{
		if (isLight)
		{
			isLight = false;
			MapManager.Instance.GetCurrMap(base.transform.position).fog.LightFog(currGrid.Point, 1, 1, isLight: false);
			MapManager.Instance.LightGrid(base.transform.position, currGrid.Point, 1, 1, isLight: false);
		}
	}

	protected override void OnInitForPlace()
	{
		if (!isLight && !isSleeping)
		{
			light2d.enabled = true;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.plantern, base.transform.position);
			MapManager.Instance.GetCurrMap(base.transform.position).fog.LightFog(currGrid.Point, 1, 1, isLight: true);
			MapManager.Instance.LightGrid(base.transform.position, currGrid.Point, 1, 1, isLight: true);
			isLight = true;
		}
	}

	protected override void GoAwakeSpecial()
	{
		if (!isLight)
		{
			light2d.enabled = true;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.plantern, base.transform.position);
			MapManager.Instance.GetCurrMap(base.transform.position).fog.LightFog(currGrid.Point, 1, 1, isLight: true);
			MapManager.Instance.LightGrid(base.transform.position, currGrid.Point, 1, 1, isLight: true);
			isLight = true;
		}
	}

	protected override void GoSleepSpecial()
	{
		REnderer.material.SetTexture("_EyeTex", lightOut);
		light2d.enabled = false;
		if (isLight)
		{
			isLight = false;
			MapManager.Instance.GetCurrMap(base.transform.position).fog.LightFog(currGrid.Point, 1, 1, isLight: false);
			MapManager.Instance.LightGrid(base.transform.position, currGrid.Point, 1, 1, isLight: false);
		}
	}
}
