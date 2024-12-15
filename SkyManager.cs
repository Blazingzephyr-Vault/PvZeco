using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
	private int onlineSunId;

	public static SkyManager Instance;

	public List<Sun> sunList = new List<Sun>();

	public int clickedSunNum;

	private Coroutine RainChangeCoroutine;

	public Lightning lightning;

	public ParticleSystem RainParticle;

	public bool DayLightCycle;

	private ParticleSystem.EmissionModule emissionModule;

	private bool isThunder;

	private bool canLightning;

	private int rainScale;

	private int time;

	public bool SunAutoCollect;

	public int Time
	{
		get
		{
			return time;
		}
		private set
		{
			int num = value;
			if (!DayLightCycle)
			{
				num = time;
			}
			if (LVManager.Instance.GameIsStart)
			{
				if (GetIsDay(time) && !GetIsDay(num))
				{
					AudioManager.Instance.FadeBgAndPlayNew(LV.Instance.NightBgm);
				}
				else if (!GetIsDay(time) && GetIsDay(num))
				{
					AudioManager.Instance.FadeBgAndPlayNew(LV.Instance.DayBgm);
				}
			}
			time = num;
			Timetable.Instance.UpdateTime(Time);
			if (Time > 1439)
			{
				time = 0;
			}
			if (LV.Instance.TimeAction.ContainsKey(Time))
			{
				LV.Instance.TimeAction[Time]();
			}
			clickedSunNum = 0;
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].TimeChange(time);
			}
			GobalLight.Instance.TimeChange();
			if (IsThunder && !GameManager.Instance.isClient && canLightning && Random.Range(0, 10) > 8)
			{
				lightning.gameObject.SetActive(value: true);
				Grid randomGrid = MapManager.Instance.GetRandomGrid();
				lightning.LightGrid(randomGrid);
				StartCoroutine(WaitLightning());
				if (GameManager.Instance.isServer)
				{
					LightingSpawn lightingSpawn = new LightingSpawn();
					lightingSpawn.Pos = randomGrid.Position;
					SocketServer.Instance.SpawnLightning(lightingSpawn);
				}
			}
		}
	}

	public int OnlineSunId
	{
		get
		{
			onlineSunId++;
			return onlineSunId;
		}
		private set
		{
			onlineSunId = value;
		}
	}

	public int RainScale
	{
		get
		{
			return rainScale;
		}
		set
		{
			if (LVManager.Instance.InGame && rainScale != value)
			{
				rainScale = value;
				if (rainScale == 0)
				{
					RainParticle.gameObject.SetActive(value: false);
				}
				else
				{
					RainParticle.gameObject.SetActive(value: true);
				}
				if (RainChangeCoroutine != null)
				{
					StopCoroutine(RainChangeCoroutine);
				}
				RainChangeCoroutine = StartCoroutine(ChangeRain(rainScale * 30));
				GobalLight.Instance.RainScaleChange();
				if (GameManager.Instance.isServer)
				{
					TimeWeatherCmd timeWeatherCmd = new TimeWeatherCmd();
					timeWeatherCmd.RnSc = RainScale;
					timeWeatherCmd.isTud = IsThunder;
					SocketServer.Instance.SendTimeWeatherCmd(timeWeatherCmd);
				}
			}
		}
	}

	public bool IsThunder
	{
		get
		{
			return isThunder;
		}
		set
		{
			if (LVManager.Instance.InGame && isThunder != value)
			{
				isThunder = value;
				GobalLight.Instance.ThunderChange();
				if (GameManager.Instance.isServer)
				{
					TimeWeatherCmd timeWeatherCmd = new TimeWeatherCmd();
					timeWeatherCmd.RnSc = RainScale;
					timeWeatherCmd.isTud = IsThunder;
					SocketServer.Instance.SendTimeWeatherCmd(timeWeatherCmd);
				}
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		emissionModule = RainParticle.emission;
		emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		AudioManager.Instance.wetherAudio.enabled = false;
		DayLightCycle = true;
		canLightning = false;
		SunAutoCollect = false;
	}

	private IEnumerator WaitLightning()
	{
		canLightning = false;
		yield return new WaitForSeconds(10f);
		canLightning = true;
	}

	private IEnumerator ChangeRain(int scale)
	{
		int i = 1;
		if ((float)scale < emissionModule.rateOverTime.constant)
		{
			i = -1;
		}
		while ((float)scale != emissionModule.rateOverTime.constant)
		{
			yield return new WaitForSeconds(0.05f);
			emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(emissionModule.rateOverTime.constant + (float)i);
			if (emissionModule.rateOverTime.constant == 60f)
			{
				AudioManager.Instance.SetWeatherVolume(isStart: true, needFade: true);
			}
			else if (emissionModule.rateOverTime.constant == 59f)
			{
				AudioManager.Instance.SetWeatherVolume(isStart: false, needFade: true);
			}
		}
	}

	public void DirectSetRainScale(int scale)
	{
		if (LVManager.Instance.InGame)
		{
			rainScale = scale;
			AudioManager.Instance.SetWeatherVolume(scale > 0, needFade: false);
			emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(scale * 30);
			if (scale == 0)
			{
				RainParticle.gameObject.SetActive(value: false);
			}
			else
			{
				RainParticle.gameObject.SetActive(value: true);
			}
		}
	}

	public void DirectSetTime(int time, bool synClient = false)
	{
		if (!GameManager.Instance.isClient || synClient)
		{
			bool dayLightCycle = DayLightCycle;
			DayLightCycle = true;
			int num = time;
			if (time < 0)
			{
				num = -time;
			}
			if (time >= 1440)
			{
				num = time % 1440;
			}
			Time = num;
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].TimeInitMap();
				MapManager.Instance.mapList[i].TimeChange(Time);
			}
			GobalLight.Instance.InitIntensity();
			DayLightCycle = dayLightCycle;
			if (GameManager.Instance.isServer)
			{
				TimeWeatherCmd timeWeatherCmd = new TimeWeatherCmd();
				timeWeatherCmd.time = 10000 + num;
				timeWeatherCmd.RnSc = RainScale;
				timeWeatherCmd.isTud = IsThunder;
				SocketServer.Instance.SendTimeWeatherCmd(timeWeatherCmd);
			}
		}
	}

	public void LightningThis(Grid grid)
	{
		lightning.gameObject.SetActive(value: true);
		lightning.LightGrid(grid);
	}

	public int GetShadowNum(Grid grid)
	{
		if (grid == null)
		{
			return 0;
		}
		int num = 1;
		if (!GetIsDay())
		{
			num += 2;
		}
		if (rainScale > 6)
		{
			num++;
		}
		if (grid.isShadow)
		{
			num += 2;
		}
		return num;
	}

	public void StartTime()
	{
		StartCoroutine(TimeAdd());
	}

	public void ResetAll()
	{
		while (sunList.Count > 0)
		{
			sunList[0].DestroySun();
		}
		canLightning = true;
		sunList.Clear();
		sunList = new List<Sun>();
		GobalLight.Instance.ResetAll();
		IsThunder = false;
		DirectSetRainScale(0);
		StopAllCoroutines();
	}

	public void CollectAllSun()
	{
		if (!GameManager.Instance.isClient)
		{
			for (int i = 0; i < sunList.Count; i++)
			{
				sunList[i].CollectSun();
			}
		}
	}

	public void OnlineCollectSun(ClickedSun sun)
	{
		for (int i = 0; i < sunList.Count; i++)
		{
			if (sunList[i].OnlineSunId == sun.OnlineSunId)
			{
				sunList[i].CollectSun(synClient: true);
				break;
			}
		}
	}

	public void ClientSpawnSun(SunSpawn spawn)
	{
		Sun component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Sun).GetComponent<Sun>();
		component.transform.SetParent(base.transform);
		if (spawn.isSkySun)
		{
			component.InitForSky(spawn.Afloat, spawn.Pos, spawn.isSun);
		}
		else
		{
			if (!SpectatorList.Instance.LocalIsSpectator && LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(GameManager.Instance.HostName))
			{
				spawn.Pos = new Vector2(0f - spawn.Pos.x, spawn.Pos.y);
			}
			component.InitForPlant(spawn.Pos, spawn.Afloat, spawn.isSun, spawn.Player);
		}
		component.OnlineSunId = spawn.OnlineId;
		sunList.Add(component);
	}

	public void CreateSkySun(Vector2 SpawnPos, float DownY, bool isSun = true)
	{
		if (!GameManager.Instance.isClient)
		{
			Sun component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Sun).GetComponent<Sun>();
			component.transform.SetParent(base.transform);
			component.InitForSky(DownY, SpawnPos, isSun);
			sunList.Add(component);
			if (GameManager.Instance.isServer)
			{
				SunSpawn sunSpawn = new SunSpawn();
				sunSpawn.OnlineId = OnlineSunId;
				sunSpawn.Player = null;
				component.OnlineSunId = sunSpawn.OnlineId;
				sunSpawn.Pos = SpawnPos;
				sunSpawn.isSun = isSun;
				sunSpawn.isSkySun = true;
				sunSpawn.Afloat = DownY;
				SocketServer.Instance.SpawnSun(sunSpawn);
			}
		}
	}

	public void CreatePlantSun(Vector2 pos, float SunNum, bool isSun, string Player)
	{
		if (!GameManager.Instance.isClient)
		{
			Sun component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Sun).GetComponent<Sun>();
			component.transform.SetParent(base.transform);
			component.InitForPlant(pos, SunNum, isSun, Player);
			sunList.Add(component);
			if (GameManager.Instance.isServer)
			{
				SunSpawn sunSpawn = new SunSpawn();
				sunSpawn.OnlineId = OnlineSunId;
				sunSpawn.Player = Player;
				component.OnlineSunId = sunSpawn.OnlineId;
				sunSpawn.Pos = pos;
				sunSpawn.isSun = isSun;
				sunSpawn.isSkySun = false;
				sunSpawn.Afloat = SunNum;
				SocketServer.Instance.SpawnSun(sunSpawn);
			}
		}
	}

	private IEnumerator TimeAdd()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			Time++;
		}
	}

	public bool GetIsDay()
	{
		if (Time < 1110 && Time > 330)
		{
			return true;
		}
		return false;
	}

	public bool GetIsDay(int time)
	{
		if (time < 1110 && time > 340)
		{
			return true;
		}
		return false;
	}
}
