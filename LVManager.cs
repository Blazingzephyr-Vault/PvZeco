using System;
using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class LVManager : MonoBehaviour
{
	public static LVManager Instance;

	private LVState currLVState;

	public bool isOver;

	private int AllWeight;

	private int currLVWave;

	private int BIGwaveNum;

	private int CurrWeight;

	private int CurrActionNum;

	private List<LawnMower> MowerList = new List<LawnMower>();

	private UnityAction LvStartAction;

	private Coroutine AutoNextWaveCoroutine;

	public bool isBigWave;

	public bool LvDontFail;

	public bool InGame => MapManager.Instance.mapList.Count > 0;

	public bool GameIsStart => currLVState == LVState.Fighting;

	public LVState CurrLVState
	{
		get
		{
			return currLVState;
		}
		set
		{
			currLVState = value;
			switch (currLVState)
			{
			case LVState.Start:
				isOver = false;
				break;
			case LVState.Fighting:
				startRunLV();
				break;
			case LVState.End:
				QuitBattleGame();
				break;
			}
		}
	}

	public int CurrLVWave
	{
		get
		{
			return currLVWave;
		}
		set
		{
			currLVWave = value;
			if (CurrLVWave < LV.Instance.Weights[0].Count)
			{
				AutoStartNextWave();
			}
			else
			{
				isOver = true;
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
	}

	public void StartGame(LoadLVBag loadBag, int LVId = -1)
	{
		if (GameManager.Instance.isClient && loadBag == null)
		{
			return;
		}
		PvPSelector.Instance.CloseSelector();
		LevelSelector.Instance.CloseSelector();
		int num = UnityEngine.Random.Range(1000000, 9999999);
		UnityEngine.Random.InitState(num);
		CurrLVState = LVState.Start;
		if (GameManager.Instance.isClient && loadBag != null)
		{
			UnityEngine.Random.InitState(loadBag.LvSeed);
			LV.Instance.LoadLV(loadBag.LvNum, loadBag.isEasy);
			if (SpectatorList.Instance.LocalIsSpectator)
			{
				SeedBank.Instance.CardNum = 0;
			}
			else
			{
				for (int i = 0; i < loadBag.NameList.Count; i++)
				{
					if (loadBag.NameList[i] == GameManager.Instance.LocalPlayer.playerName)
					{
						SeedBank.Instance.CardNum = loadBag.CardNumList[i];
					}
				}
			}
			BattlePlayerList.Instance.LoadAllSeedBank(loadBag.NameList, loadBag.CardNumList);
			FlagMeter.Instance.SetLvlName(loadBag.Name);
		}
		if (!GameManager.Instance.isClient)
		{
			if (LVId == -1)
			{
				LV.Instance.LoadLV(LevelSelector.Instance.SelectedDis.lVInfo.LevelId, LevelSelector.Instance.IsEasy);
			}
			else
			{
				LV.Instance.LoadLV(LVId, LevelSelector.Instance.IsEasy);
			}
			SeedBank.Instance.CardNum = LV.Instance.CardNum;
		}
		for (int j = 0; j < MapManager.Instance.mapList.Count; j++)
		{
			MapManager.Instance.mapList[j].InitMap();
			MapManager.Instance.mapList[j].SpawnMower();
		}
		if (GameManager.Instance.isServer)
		{
			List<int> list = new List<int>();
			List<string> noSpectatorPlayerList = SpectatorList.Instance.GetNoSpectatorPlayerList();
			if (LV.Instance.CurrLVType == LVType.PvP)
			{
				int cardNum = PvPSelector.Instance.CardNum;
				int num2 = cardNum / PvPSelector.Instance.RedTeamNames.Count;
				int num3 = cardNum % PvPSelector.Instance.RedTeamNames.Count;
				int num4 = cardNum / PvPSelector.Instance.BlueTeamNames.Count;
				int num5 = cardNum % PvPSelector.Instance.BlueTeamNames.Count;
				for (int k = 0; k < noSpectatorPlayerList.Count; k++)
				{
					bool flag = false;
					bool flag2 = false;
					if (PvPSelector.Instance.RedTeamNames.Contains(noSpectatorPlayerList[k]))
					{
						if (!flag)
						{
							flag = true;
							list.Add(num2 + num3);
						}
						else
						{
							list.Add(num2);
						}
					}
					else if (PvPSelector.Instance.BlueTeamNames.Contains(noSpectatorPlayerList[k]))
					{
						if (!flag2)
						{
							flag2 = true;
							list.Add(num4 + num5);
						}
						else
						{
							list.Add(num4);
						}
					}
				}
				SeedBank.Instance.CardNum = 0;
				for (int l = 0; l < noSpectatorPlayerList.Count; l++)
				{
					if (noSpectatorPlayerList[l] == GameManager.Instance.LocalPlayer.playerName)
					{
						SeedBank.Instance.CardNum = list[l];
					}
				}
			}
			else
			{
				int count = noSpectatorPlayerList.Count;
				int num6 = (LV.Instance.CardNum + noSpectatorPlayerList.Count - 1) / count;
				int num7 = (LV.Instance.CardNum + noSpectatorPlayerList.Count - 1) % count;
				if (num6 < 4)
				{
					int num8 = 4 - num6;
					num7 -= num8 * SocketServer.Instance.noHostPlayerNum;
					if (num7 < 0)
					{
						num7 = 0;
					}
					num6 = 4;
				}
				for (int m = 0; m < noSpectatorPlayerList.Count; m++)
				{
					if (m == 0)
					{
						list.Add(num6 + num7);
					}
					else
					{
						list.Add(num6);
					}
				}
				SeedBank.Instance.CardNum = num6 + num7;
			}
			BattlePlayerList.Instance.LoadAllSeedBank(noSpectatorPlayerList, list);
			if (SpectatorList.Instance.LocalIsSpectator)
			{
				SeedBank.Instance.CardNum = 0;
			}
			LoadLVBag loadLVBag = new LoadLVBag();
			if (LVId == -1)
			{
				loadLVBag.LvNum = LevelSelector.Instance.SelectedDis.lVInfo.LevelId;
			}
			else
			{
				loadLVBag.LvNum = LVId;
			}
			loadLVBag.LvSeed = num;
			loadLVBag.CardNumList = list;
			loadLVBag.NameList = noSpectatorPlayerList;
			loadLVBag.isEasy = LevelSelector.Instance.IsEasy;
			loadLVBag.Name = LevelSelector.Instance.SelectedDis.lVInfo.LevelName;
			SocketServer.Instance.LoadLv(loadLVBag);
		}
		UIManager.Instance.BattleUI.localScale = Vector3.one;
		SumWeight();
		FlagMeter.Instance.CreateFlag(AllWeight);
		GobalLight.Instance.InitIntensity();
		PlayerManager.Instance.ResetSunNum();
		GoOtherMap.Instance.LoadInit();
		if (LV.Instance.CurrLVType == LVType.Normal)
		{
			ZombieManager.Instance.ShowZombie();
			CameraControl.Instance.MoveForLVStart(LVStartCameraAction);
		}
		else
		{
			CameraControl.Instance.SetPosition(Vector2.zero);
			StartCoroutine(WaitTimeDo(LVStartCameraAction, 1f));
		}
		if (SkyManager.Instance.GetIsDay())
		{
			AudioManager.Instance.PlayBgAudio(GameManager.Instance.AudioConf.ChooseYourSeeds);
		}
		else
		{
			AudioManager.Instance.PlayBgAudio(GameManager.Instance.AudioConf.NightChooseSeeds);
		}
		if (!GameManager.Instance.isClient)
		{
			FlagMeter.Instance.SetLvlName(LevelSelector.Instance.SelectedDis.lVInfo.LevelName);
		}
		SeedBank.Instance.SpawnCardSlot();
		SeedBank.Instance.isRatZombie = LV.Instance.SeedBankRat;
	}

	public void QuitBattleGame()
	{
		if (GameManager.Instance.isServer)
		{
			LoadLVBag loadLVBag = new LoadLVBag();
			loadLVBag.LoadType = 2;
			SocketServer.Instance.LoadLv(loadLVBag);
		}
		ResetScence();
		PvPSelector.Instance.ResetPvPInfo();
		SeedBank.Instance.StartMoveBack(fade: false);
		SeedChooser.Instance.ResetPos();
		ZombieChooser.Instance.ResetPos();
		UIManager.Instance.CloseUI();
		UIManager.Instance.BattleUI.localScale = Vector3.zero;
		AudioManager.Instance.PlayBgAudio(GameManager.Instance.AudioConf.Nor);
		CameraControl.Instance.SetPosition(new Vector2(0f, -30f));
		StartSceneManager.Instance.PlayAllAnim();
	}

	public void ReStartGame()
	{
		if (GameManager.Instance.isServer)
		{
			LoadLVBag loadLVBag = new LoadLVBag();
			loadLVBag.LoadType = 1;
			SocketServer.Instance.LoadLv(loadLVBag);
		}
		ResetScence();
		UIManager.Instance.CloseUI();
		SeedBank.Instance.StartMoveBack(fade: false);
		SeedChooser.Instance.ResetPos();
		ZombieChooser.Instance.ResetPos();
		StartGame(null);
	}

	private void ResetScence()
	{
		CurrActionNum = 0;
		CurrWeight = 0;
		currLVWave = 0;
		BIGwaveNum = 0;
		AllWeight = 0;
		isBigWave = false;
		LvStartAction = null;
		ClearMower();
		CurrLVState = LVState.Start;
		GobalLight.Instance.gobalLight.intensity = 1f;
		AudioManager.Instance.ChangeMapReset();
		PlayerManager.Instance.ClearMoney();
		CobCannonTarget.Instance.StopAim();
		Shovel.Instance.CancelShovel();
		SeedBank.Instance.isCanClick = true;
		UIManager.Instance.StopLVStartEF();
		PlantManager.Instance.ClearAllPlant();
		UIManager.Instance.OverPanel.gameObject.SetActive(value: false);
		ZombieManager.Instance.ClearAllZombie();
		ZombieManager.Instance.RemoveAllZombieDeadAction(OnAllZombieDeadAction);
		SkyManager.Instance.ResetAll();
		MapManager.Instance.ResetScence();
		PoolManager.Instance.Clear();
		UIManager.Instance.LogPanel.Close();
		StopAllCoroutines();
	}

	public void GameOver(Vector2 overPos)
	{
		if (!LvDontFail)
		{
			if (GameManager.Instance.isServer)
			{
				GameOver gameOver = new GameOver();
				gameOver.pos = overPos;
				SocketServer.Instance.GameOver(gameOver);
			}
			StopAllCoroutines();
			CameraControl.Instance.GoOtherYard(MapManager.Instance.mapList.IndexOf(MapManager.Instance.GetCurrMap(overPos)));
			Shovel.Instance.CancelShovel();
			SkyManager.Instance.ResetAll();
			MapManager.Instance.GameOverPause();
			PlantManager.Instance.GameOverPause();
			ZombieManager.Instance.GameOverPause();
			AudioManager.Instance.StopBgAudio();
			SeedBank.Instance.AllCancelPlace();
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameOver, base.transform.position, isAll: true);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlant1, base.transform.position, isAll: true);
			UIManager.Instance.OverPanel.Over();
		}
	}

	public void GameOver(Vector2 overPos, bool isRedFail)
	{
		if (LvDontFail)
		{
			return;
		}
		if (GameManager.Instance.isServer)
		{
			GameOver gameOver = new GameOver();
			gameOver.pos = overPos;
			gameOver.isRedFail = isRedFail;
			SocketServer.Instance.GameOver(gameOver);
		}
		StopAllCoroutines();
		CameraControl.Instance.GoOtherYard(MapManager.Instance.mapList.IndexOf(MapManager.Instance.GetCurrMap(overPos)));
		Shovel.Instance.CancelShovel();
		SkyManager.Instance.ResetAll();
		MapManager.Instance.GameOverPause();
		PlantManager.Instance.GameOverPause();
		ZombieManager.Instance.GameOverPause();
		AudioManager.Instance.StopBgAudio();
		SeedBank.Instance.AllCancelPlace();
		if (PvPSelector.Instance.LocalIsRedTeam)
		{
			if (isRedFail)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameOver, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameWin, base.transform.position, isAll: true);
			}
		}
		else if (PvPSelector.Instance.LocalIsRedTeam)
		{
			if (isRedFail)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameWin, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameOver, base.transform.position, isAll: true);
			}
		}
		string logContent = "红方获得胜利！";
		if (isRedFail)
		{
			logContent = "蓝方获得胜利！";
		}
		UIManager.Instance.LogPanel.DisplayLog(logContent, delegate
		{
			if (!GameManager.Instance.isClient)
			{
				QuitBattleGame();
			}
		});
		UIManager.Instance.LogPanel.ButtonText.text = "返回主界面";
		if (GameManager.Instance.isClient)
		{
			UIManager.Instance.LogPanel.Button.gameObject.SetActive(value: false);
		}
	}

	public void OverPanelEvent()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		UIManager.Instance.LogPanel.DisplayLog("游戏结束。", delegate
		{
			if (!GameManager.Instance.isClient)
			{
				ReStartGame();
			}
		});
		UIManager.Instance.LogPanel.ButtonText.text = "重新开始";
	}

	public void StartRunLv()
	{
		SeedBank.Instance.isCanClick = false;
		if (LV.Instance.CurrLVType == LVType.Normal)
		{
			CameraControl.Instance.MoveBackForLVStart(LVStartCameraBackAction);
		}
		else
		{
			StartCoroutine(WaitTimeDo(LVStartCameraBackAction, 1f));
		}
	}

	public void LVStartCameraAction()
	{
		SeedBank.Instance.StartMove(LV.Instance.CurrSeedBankType);
		SeedChooser.Instance.ResetPos();
		SeedChooser.Instance.StartMove();
		ZombieChooser.Instance.ResetPos();
		ZombieChooser.Instance.StartMove();
	}

	public void LVStartCameraBackAction()
	{
		UIManager.Instance.ShowLVStartEF();
		if (LV.Instance.CurrLVType != LVType.PvP)
		{
			ZombieManager.Instance.ClearAllZombie();
		}
	}

	public void LVStartEFOver()
	{
		SeedBank.Instance.isCanClick = true;
		CurrLVState = LVState.Fighting;
		SkyManager.Instance.StartTime();
		if (SkyManager.Instance.GetIsDay())
		{
			AudioManager.Instance.PlayBgAudio(LV.Instance.DayBgm);
		}
		else
		{
			AudioManager.Instance.PlayBgAudio(LV.Instance.NightBgm);
		}
		if (LvStartAction != null)
		{
			LvStartAction();
		}
	}

	public void AddLVStartActionListenr(UnityAction action)
	{
		LvStartAction = (UnityAction)Delegate.Combine(LvStartAction, action);
	}

	private void OnAllZombieDeadAction()
	{
		CurrLVWave++;
		ZombieManager.Instance.RemoveAllZombieDeadAction(OnAllZombieDeadAction);
		if (AutoNextWaveCoroutine != null)
		{
			StopCoroutine(AutoNextWaveCoroutine);
		}
		AutoNextWaveCoroutine = null;
	}

	private void startRunLV()
	{
		if (!GameManager.Instance.isClient && LV.Instance.CurrLVType != LVType.PvP)
		{
			float time = UnityEngine.Random.Range(LV.Instance.SetupTime.x, LV.Instance.SetupTime.y);
			StartCoroutine(WaitTimeDo(delegate
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Awooga, base.transform.position, isAll: true);
				AutoStartNextWave();
			}, time));
		}
	}

	private void AutoStartNextWave()
	{
		StartCoroutine(NextWave());
	}

	private IEnumerator AutoNextWave(float time)
	{
		yield return new WaitForSeconds(time);
		OnAllZombieDeadAction();
	}

	private IEnumerator NextWave()
	{
		isBigWave = false;
		List<int> c = new List<int>();
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			c.Add(LV.Instance.Weights[i][CurrLVWave]);
		}
		int time = 0;
		for (int j = 0; j < c.Count; j++)
		{
			time += c[j];
		}
		if (GameManager.Instance.isServer)
		{
			WaveComing waveComing = new WaveComing();
			waveComing.WaveType = 0;
			if (CurrLVWave == LV.Instance.BigWaveNum[BIGwaveNum])
			{
				if (BIGwaveNum == LV.Instance.BigWaveNum.Count - 1)
				{
					waveComing.WaveType = 2;
				}
				else
				{
					waveComing.WaveType = 1;
				}
				waveComing.WaitTime = 4f;
			}
			SocketServer.Instance.BigWaveComing(waveComing);
		}
		if (CurrLVWave == LV.Instance.BigWaveNum[BIGwaveNum])
		{
			if (BIGwaveNum == LV.Instance.BigWaveNum.Count - 1)
			{
				UIManager.Instance.ShowFinalWaveEF();
			}
			else
			{
				UIManager.Instance.ShowBigWaveEF();
			}
			isBigWave = true;
			yield return new WaitForSeconds(4f);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Awooga, base.transform.position, isAll: true);
			FlagMeter.Instance.FlagList[BIGwaveNum].GoRise();
			BIGwaveNum++;
		}
		while (true)
		{
			int num = 0;
			for (int k = 0; k < c.Count; k++)
			{
				num += c[k];
			}
			if (num == 0)
			{
				break;
			}
			for (int l = 0; l < c.Count; l++)
			{
				if (c[l] == 0)
				{
					continue;
				}
				int index;
				do
				{
					index = UnityEngine.Random.Range(0, LV.Instance.ZombieTypes[l].Count);
				}
				while (ZombieManager.Instance.GetZombieWeight(LV.Instance.ZombieTypes[l][index]) > c[l]);
				if (MapManager.Instance.mapList.Count - 1 < l)
				{
					break;
				}
				Vector3 position = MapManager.Instance.mapList[l].transform.position;
				if (ZombieManager.Instance.UpdateZombie(LV.Instance.ZombieTypes[l][index], position))
				{
					c[l] -= ZombieManager.Instance.GetZombieWeight(LV.Instance.ZombieTypes[l][index]);
					CurrWeight += ZombieManager.Instance.GetZombieWeight(LV.Instance.ZombieTypes[l][index]);
					if (CurrActionNum < LV.Instance.WeightForAction.Count && CurrWeight > LV.Instance.WeightForAction[CurrActionNum])
					{
						LV.Instance.WeightAction[CurrActionNum]();
						CurrActionNum++;
					}
					FlagMeter.Instance.UpdateHead(ZombieManager.Instance.GetZombieWeight(LV.Instance.ZombieTypes[l][index]));
				}
				else
				{
					l--;
				}
			}
			if (MapManager.Instance.mapList.Count == 0)
			{
				break;
			}
			if (isBigWave)
			{
				yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f));
			}
			else
			{
				yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
			}
		}
		ZombieManager.Instance.AddAllZombieDeadAction(OnAllZombieDeadAction);
		time *= 2;
		if (time < 60)
		{
			time = 60;
		}
		if (time > 80)
		{
			time = 80;
		}
		AutoNextWaveCoroutine = StartCoroutine(AutoNextWave(time));
	}

	public void ClientShowBigWave(WaveComing bigWave)
	{
		if (bigWave.WaveType == 0)
		{
			isBigWave = false;
		}
		else if (bigWave.WaveType == 1)
		{
			isBigWave = true;
			UIManager.Instance.ShowBigWaveEF();
			StartCoroutine(WaitTimeDo(delegate
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Awooga, base.transform.position, isAll: true);
				FlagMeter.Instance.FlagList[BIGwaveNum].GoRise();
				BIGwaveNum++;
			}, bigWave.WaitTime));
		}
		else if (bigWave.WaveType == 2)
		{
			isBigWave = true;
			UIManager.Instance.ShowFinalWaveEF();
			StartCoroutine(WaitTimeDo(delegate
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Awooga, base.transform.position, isAll: true);
				FlagMeter.Instance.FlagList[BIGwaveNum].GoRise();
				BIGwaveNum++;
			}, bigWave.WaitTime));
		}
	}

	private IEnumerator WaitTimeDo(UnityAction action, float time)
	{
		yield return new WaitForSeconds(time);
		action?.Invoke();
	}

	public void SumWeight()
	{
		if (LV.Instance.Weights.Count == 0)
		{
			AllWeight = 0;
			return;
		}
		int num = 0;
		for (int i = 0; i < LV.Instance.Weights[0].Count; i++)
		{
			if (i == LV.Instance.BigWaveNum[num])
			{
				num++;
				continue;
			}
			for (int j = 0; j < LV.Instance.Weights.Count; j++)
			{
				AllWeight += LV.Instance.Weights[j][i];
			}
		}
	}

	public void GameWin()
	{
		CurrLVState = LVState.End;
	}

	public void RemoveMower(LawnMower lm)
	{
		MowerList.Remove(lm);
	}

	private void ClearMower()
	{
		for (int i = 0; i < MowerList.Count; i++)
		{
			MowerList[i].DestroyMower();
		}
	}
}
