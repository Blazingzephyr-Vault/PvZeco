using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class SocketClient : MonoBehaviour
{
	public static SocketClient Instance;

	private Socket clientSocket;

	private List<UnityAction> actions = new List<UnityAction>();

	private bool needLog;

	private bool needLog2;

	private Coroutine WaitConnect;

	private bool OnlineCheck;

	public TextMesh text;

	private void Awake()
	{
		Instance = this;
		Application.runInBackground = true;
	}

	private void Update()
	{
		try
		{
			while (actions.Count > 0)
			{
				actions[0]();
				actions.RemoveAt(0);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("主动断线==" + ex);
			actions.RemoveAt(0);
		}
	}

	public void JoinGame(IPAddress ip, int port)
	{
		if (GameManager.Instance.isOnline)
		{
			return;
		}
		if (WaitConnect != null)
		{
			StopCoroutine(WaitConnect);
		}
		WaitConnect = StartCoroutine(WaitLog());
		clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		IPEndPoint endport = new IPEndPoint(ip, port);
		PlayerInfo playerInfo = new PlayerInfo();
		playerInfo.Name = GameManager.Instance.LocalPlayer.playerName;
		playerInfo.CheckId = GameManager.Instance.VersionCode;
		needLog = true;
		needLog2 = false;
		new Thread((ThreadStart)delegate
		{
			try
			{
				clientSocket.Connect(endport);
				SendMsg(JsonUtility.ToJson(playerInfo), 0, 1);
				ReseviceMsg(clientSocket);
			}
			catch (Exception)
			{
			}
		}).Start();
	}

	private void ReseviceMsg(Socket clientSocket)
	{
		new Thread((ThreadStart)delegate
		{
			byte[] array = new byte[1];
			int num = 0;
			bool flag = false;
			while (true)
			{
				try
				{
					byte[] array2 = new byte[4194304];
					int num2 = clientSocket.Receive(array2);
					List<byte[]> list = new List<byte[]>();
					int num3 = 0;
					int num4 = num2 - num;
					if (flag)
					{
						byte[] array3 = array2.Skip(0).Take(num).ToArray();
						byte[] array4 = new byte[array.Length + array3.Length];
						array.CopyTo(array4, 0);
						array3.CopyTo(array4, array.Length);
						list.Add(array4);
						num3 += num;
					}
					num = 0;
					flag = false;
					while (num4 > 0)
					{
						int num5 = BitConverter.ToInt32(new byte[4]
						{
							array2[num3],
							array2[num3 + 1],
							array2[num3 + 2],
							array2[num3 + 3]
						});
						byte[] item = array2.Skip(num3 + 4).Take(num5).ToArray();
						num4 -= 4 + num5;
						if (num4 >= 0)
						{
							list.Add(item);
						}
						else
						{
							flag = true;
							int count = num5 + num4;
							num = -num4;
							array = array2.Skip(num3 + 4).Take(count).ToArray();
						}
						num3 += 4 + num5;
					}
					for (int i = 0; i < list.Count; i++)
					{
						byte b = list[i][0];
						byte b2 = list[i][1];
						string getmsg = Encoding.UTF8.GetString(list[i], 2, list[i].Length - 2);
						switch (b)
						{
						case 0:
							if (b2 == 0)
							{
								Debug.Log("0-0");
							}
							switch (b2)
							{
							case 1:
								needLog = false;
								clientSocket.Close();
								actions.Add(delegate
								{
									UIManager.Instance.LogPanel.DisplayLog(getmsg, delegate
									{
										if (GameManager.Instance.isOnline)
										{
											UIManager.Instance.CloseUI();
										}
										else
										{
											UIManager.Instance.OpenAndFocusUI(UIManager.Instance.JoinGame);
										}
									});
								});
								goto end_IL_08f8;
							case 2:
							{
								OnlinePlayerInfo playerInfo = JsonUtility.FromJson<OnlinePlayerInfo>(getmsg);
								actions.Add(delegate
								{
									if (!GameManager.Instance.isOnline)
									{
										if (WaitConnect != null)
										{
											StopCoroutine(WaitConnect);
										}
										needLog2 = true;
										Application.runInBackground = true;
										GameManager.Instance.isOnline = true;
										OnlineCheck = false;
										StartCoroutine(CheckConnect());
										UIManager.Instance.ConnectSuccess();
									}
									PlayerList.Instance.UpdatePlayerList(playerInfo.HostPlayer, playerInfo.players);
									BattlePlayerList.Instance.UpdatePlayerList(playerInfo.HostPlayer, playerInfo.players);
								});
								break;
							}
							case byte.MaxValue:
								OnlineCheck = true;
								break;
							}
							break;
						case 1:
							switch (b2)
							{
							case 0:
							{
								LoadLVBag LvBag = JsonUtility.FromJson<LoadLVBag>(getmsg);
								actions.Add(delegate
								{
									if (LvBag.LoadType == 0)
									{
										LVManager.Instance.StartGame(LvBag);
									}
									else if (LvBag.LoadType == 1)
									{
										LVManager.Instance.ReStartGame();
									}
									else if (LvBag.LoadType == 2)
									{
										LVManager.Instance.QuitBattleGame();
									}
								});
								break;
							}
							case 1:
								actions.Add(delegate
								{
									SeedChooser.Instance.StartRunLv(synClient: true);
									ZombieChooser.Instance.StartRunLv(synClient: true);
								});
								break;
							case 2:
								actions.Add(delegate
								{
									LVManager.Instance.ClientShowBigWave(JsonUtility.FromJson<WaveComing>(getmsg));
								});
								break;
							case 3:
								actions.Add(delegate
								{
									PlayerManager.Instance.ClientUpdateSunNum(JsonUtility.FromJson<SunNumBag>(getmsg));
								});
								break;
							case 4:
								actions.Add(delegate
								{
									SynItem(JsonUtility.FromJson<SynItem>(getmsg));
								});
								break;
							case 5:
							{
								PlayerMap apply6 = JsonUtility.FromJson<PlayerMap>(getmsg);
								actions.Add(delegate
								{
									BattlePlayerList.Instance.UpdateMapSprite(apply6.PlayerName, apply6.Pos);
								});
								break;
							}
							case 6:
							{
								SelectCard apply5 = JsonUtility.FromJson<SelectCard>(getmsg);
								actions.Add(delegate
								{
									if (apply5.PlayerName != GameManager.Instance.LocalPlayer.playerName)
									{
										if (apply5.isBack)
										{
											BattlePlayerList.Instance.CancelCard(apply5.PlayerName, apply5.cardId);
										}
										else
										{
											BattlePlayerList.Instance.SelectCard(apply5.PlayerName, apply5.plantType, apply5.zombieType);
										}
									}
								});
								break;
							}
							case 7:
							{
								SelectPrepare apply4 = JsonUtility.FromJson<SelectPrepare>(getmsg);
								actions.Add(delegate
								{
									BattlePlayerList.Instance.UpdateState(apply4.PlayerName, apply4.isPrepare);
								});
								break;
							}
							case 8:
							{
								GameOver over4 = JsonUtility.FromJson<GameOver>(getmsg);
								actions.Add(delegate
								{
									if (LV.Instance.CurrLVType == LVType.PvP)
									{
										LVManager.Instance.GameOver(over4.pos, over4.isRedFail);
									}
									else
									{
										LVManager.Instance.GameOver(over4.pos);
									}
								});
								break;
							}
							case 9:
							{
								PvPTeamList over3 = JsonUtility.FromJson<PvPTeamList>(getmsg);
								actions.Add(delegate
								{
									PvPSelector.Instance.ClientSynTeam(over3);
								});
								break;
							}
							case 10:
							{
								PvPModeSyn over2 = JsonUtility.FromJson<PvPModeSyn>(getmsg);
								actions.Add(delegate
								{
									PvPSelector.Instance.ClientSynMode(over2);
								});
								break;
							}
							case 11:
							{
								SpectList over = JsonUtility.FromJson<SpectList>(getmsg);
								actions.Add(delegate
								{
									SpectatorList.Instance.ClientSynList(over.names);
								});
								break;
							}
							}
							break;
						case 2:
							switch (b2)
							{
							case 0:
							{
								PlantSpawn spawn7 = JsonUtility.FromJson<PlantSpawn>(getmsg);
								actions.Add(delegate
								{
									PlantBase newPlant = PlantManager.Instance.GetNewPlant(spawn7.plantType);
									if (spawn7.SPcode == 2)
									{
										newPlant.InitForCreate(inGrid: false, null, isBlcWhi: false);
									}
									if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(spawn7.PlacePlayer))
									{
										spawn7.GridPos = new Vector2(0f - spawn7.GridPos.x, spawn7.GridPos.y);
									}
									newPlant.OnlineId = spawn7.OnlineId;
									Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(spawn7.GridPos);
									SeedBank.Instance.PlantConfirm(newPlant, gridByWorldPos, -1, spawn7.SPcode, spawn7.PlacePlayer);
								});
								break;
							}
							case 1:
								actions.Add(delegate
								{
									SkyManager.Instance.ClientSpawnSun(JsonUtility.FromJson<SunSpawn>(getmsg));
								});
								break;
							case 2:
								actions.Add(delegate
								{
									SkyManager.Instance.OnlineCollectSun(JsonUtility.FromJson<ClickedSun>(getmsg));
								});
								break;
							case 3:
							{
								PlantPreview apply3 = JsonUtility.FromJson<PlantPreview>(getmsg);
								actions.Add(delegate
								{
									BattlePlayerList.Instance.PreviewPlant(apply3);
								});
								break;
							}
							case 4:
							{
								UpdateCardCD apply2 = JsonUtility.FromJson<UpdateCardCD>(getmsg);
								actions.Add(delegate
								{
									if (apply2.name == GameManager.Instance.LocalPlayer.playerName)
									{
										if (apply2.OK)
										{
											SeedBank.Instance.PlantFailClearCD(apply2.CardId);
										}
									}
									else
									{
										BattlePlayerList.Instance.UpdateCardCD(apply2.name, apply2.CardId, apply2.OK);
									}
								});
								break;
							}
							case 5:
							{
								ShovelPreview apply = JsonUtility.FromJson<ShovelPreview>(getmsg);
								actions.Add(delegate
								{
									BattlePlayerList.Instance.PreviewShovel(apply.PlayerName, apply.GridPos, apply.isShow);
								});
								break;
							}
							}
							break;
						case 3:
							switch (b2)
							{
							case 0:
							{
								ZombieSpawn spawn6 = JsonUtility.FromJson<ZombieSpawn>(getmsg);
								actions.Add(delegate
								{
									ZombieManager.Instance.UpdateZombie(spawn6);
								});
								break;
							}
							case 1:
							{
								GraveStoneSpawn spawn5 = JsonUtility.FromJson<GraveStoneSpawn>(getmsg);
								actions.Add(delegate
								{
									MapManager.Instance.GetGridByWorldPos(spawn5.MapPos).HaveGraveStone = spawn5.isHave;
								});
								break;
							}
							case 2:
							{
								PuddleSpawn spawn4 = JsonUtility.FromJson<PuddleSpawn>(getmsg);
								actions.Add(delegate
								{
									List<Grid> list2 = new List<Grid>();
									for (int j = 0; j < spawn4.MapPos.Count; j++)
									{
										list2.Add(MapManager.Instance.GetGridByWorldPos(spawn4.MapPos[j]));
									}
									Puddle component = UnityEngine.Object.Instantiate(GameManager.Instance.GameConf.Puddle).GetComponent<Puddle>();
									component.CreateInit(list2, spawn4.InitPos, spawn4.OnlineId);
									MapManager.Instance.puddles.Add(component);
								});
								break;
							}
							case 3:
							{
								LightingSpawn spawn3 = JsonUtility.FromJson<LightingSpawn>(getmsg);
								actions.Add(delegate
								{
									SkyManager.Instance.LightningThis(MapManager.Instance.GetGridByWorldPos(spawn3.Pos));
								});
								break;
							}
							case 4:
							{
								MowerLaunch spawn2 = JsonUtility.FromJson<MowerLaunch>(getmsg);
								actions.Add(delegate
								{
									MapManager.Instance.GetCurrMap(spawn2.pos).SynMower(spawn2.id);
								});
								break;
							}
							case 5:
							{
								ZombiePreview spawn = JsonUtility.FromJson<ZombiePreview>(getmsg);
								actions.Add(delegate
								{
									BattlePlayerList.Instance.PreviewZombie(spawn);
								});
								break;
							}
							}
							break;
						case 4:
							switch (b2)
							{
							case 0:
								actions.Add(delegate
								{
									ChatInput.Instance.AddMessage(getmsg);
								});
								break;
							case 1:
								actions.Add(delegate
								{
									ChatInput.Instance.AddMessage(getmsg, new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue));
								});
								break;
							case 2:
							{
								PrivateChatMsg chatMsg = JsonUtility.FromJson<PrivateChatMsg>(getmsg);
								actions.Add(delegate
								{
									string content = "玩家" + chatMsg.PlayerName + "悄悄对你说:" + chatMsg.content;
									ChatInput.Instance.AddMessage(content, new Color32(123, 123, 123, byte.MaxValue));
								});
								break;
							}
							case 3:
							{
								CommandBag bag2 = JsonUtility.FromJson<CommandBag>(getmsg);
								actions.Add(delegate
								{
									PlantManager.Instance.PlantInvincible = bag2.Pinv;
									ZombieManager.Instance.ZombieInvincible = bag2.Zinv;
									SkyManager.Instance.DayLightCycle = bag2.DLiCy;
									PlayerManager.Instance.SunInfinite = bag2.SnInf;
									SeedBank.Instance.isNoCD = bag2.CdCle;
								});
								break;
							}
							case 4:
							{
								TimeWeatherCmd bag = JsonUtility.FromJson<TimeWeatherCmd>(getmsg);
								actions.Add(delegate
								{
									if (bag.time >= 10000)
									{
										bag.time -= 10000;
										SkyManager.Instance.DirectSetTime(bag.time, synClient: true);
									}
									SkyManager.Instance.RainScale = bag.RnSc;
									SkyManager.Instance.IsThunder = bag.isTud;
								});
								break;
							}
							}
							break;
						}
						continue;
						end_IL_08f8:
						break;
					}
				}
				catch (Exception message)
				{
					Debug.Log(message);
					break;
				}
			}
			clientSocket.Close();
			actions.Add(delegate
			{
				ConnectOver();
			});
		}).Start();
	}

	private void SendMsg(string content, byte type1, byte type2)
	{
		try
		{
			List<byte> list = new List<byte>();
			list.Add(type1);
			list.Add(type2);
			if (content != "")
			{
				list.AddRange(Encoding.UTF8.GetBytes(content));
			}
			list.InsertRange(0, BitConverter.GetBytes(list.Count));
			byte[] buffer = list.ToArray();
			clientSocket.Send(buffer);
		}
		catch (Exception message)
		{
			clientSocket.Close();
			Debug.Log("1253");
			actions.Add(delegate
			{
				ConnectOver();
			});
			Debug.Log(message);
		}
	}

	private IEnumerator CheckConnect()
	{
		while (GameManager.Instance.isOnline)
		{
			yield return new WaitForSeconds(3f);
			SendMsg("", 0, byte.MaxValue);
			if (!OnlineCheck)
			{
				ConnectOver();
				Debug.Log("123");
			}
			OnlineCheck = false;
		}
	}

	private IEnumerator WaitLog()
	{
		yield return new WaitForSeconds(5f);
		clientSocket.Close();
		UIManager.Instance.LogPanel.Confirm();
		WaitConnect = null;
	}

	public void CloseClient()
	{
		clientSocket.Close();
		needLog = false;
		needLog2 = false;
	}

	private void ConnectOver()
	{
		if (!GameManager.Instance.isOnline)
		{
			return;
		}
		GameManager.Instance.isOnline = false;
		if (WaitConnect != null)
		{
			StopCoroutine(WaitConnect);
		}
		if (LVManager.Instance.InGame)
		{
			LVManager.Instance.QuitBattleGame();
		}
		SpectatorList.Instance.ClientSynList(new List<string>());
		PvPSelector.Instance.ResetPvPInfo();
		PlayerList.Instance.UpdatePlayerList(null, new List<PlayerInfo>());
		BattlePlayerList.Instance.UpdatePlayerList(null, new List<PlayerInfo>());
		if (needLog && needLog2)
		{
			UIManager.Instance.LogPanel.DisplayLog("与服务器断开连接。", delegate
			{
				UIManager.Instance.CloseUI();
			});
		}
		else if (needLog)
		{
			UIManager.Instance.LogPanel.DisplayLog("连接超时。", delegate
			{
				UIManager.Instance.OpenAndFocusUI(UIManager.Instance.JoinGame.transform);
			});
		}
	}

	public void SendChatMsg(string msg)
	{
		SendMsg(msg, 2, 0);
	}

	public void SendPrivateChatMsg(string name, string content)
	{
		PrivateChatMsg privateChatMsg = new PrivateChatMsg();
		privateChatMsg.PlayerName = name;
		privateChatMsg.content = content;
		SendMsg(JsonUtility.ToJson(privateChatMsg), 2, 1);
	}

	public void ChangeMap(PlayerMap map)
	{
		SendMsg(JsonUtility.ToJson(map), 1, 3);
	}

	public void SelectCard(SelectCard card)
	{
		SendMsg(JsonUtility.ToJson(card), 1, 4);
	}

	public void SelectPrepare(SelectPrepare prepare)
	{
		SendMsg(JsonUtility.ToJson(prepare), 1, 5);
	}

	public void ApplyShovel(ShovelApply apply)
	{
		SendMsg(JsonUtility.ToJson(apply), 1, 1);
	}

	public void UpdateCD(int cardID, bool Ok)
	{
		UpdateCardCD updateCardCD = new UpdateCardCD();
		updateCardCD.CardId = cardID;
		updateCardCD.OK = Ok;
		SendMsg(JsonUtility.ToJson(updateCardCD), 1, 8);
	}

	public void ClickedSun(ClickedSun sun)
	{
		SendMsg(JsonUtility.ToJson(sun), 1, 2);
	}

	public void SynItem(SynItem syn)
	{
		if (syn.Type == 0 || syn.Type == 1)
		{
			List<PlantBase> plants = PlantManager.Instance.plants;
			for (int i = 0; i < plants.Count; i++)
			{
				if (plants[i].OnlineId == syn.OnlineId)
				{
					plants[i].OnlineSynPlant(syn);
					break;
				}
			}
		}
		if (syn.Type == 0 || syn.Type == 2)
		{
			List<ZombieBase> list = new List<ZombieBase>(ZombieManager.Instance.GetAllZombies());
			List<ZombieBase> allHypZombies = ZombieManager.Instance.GetAllHypZombies();
			list.AddRange(allHypZombies);
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].OnlineId == syn.OnlineId)
				{
					list[j].OnlineSynZombie(syn);
					break;
				}
			}
		}
		if (syn.Type != 0 && syn.Type != 3)
		{
			return;
		}
		List<Puddle> puddles = MapManager.Instance.puddles;
		for (int k = 0; k < puddles.Count; k++)
		{
			if (puddles[k].OnlineId == syn.OnlineId)
			{
				puddles[k].StartDisappear();
				break;
			}
		}
	}

	public void ApplyPlacePlant(PlantSpawn spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 0);
	}

	public void ApplyPlaceZombie(ZombieSpawnApply spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 10);
	}

	public void ApplyPlacePreview(PlantPreview spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 7);
	}

	public void ApplyShovelPreview(ShovelPreview spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 9);
	}

	public void ApplyZombiePreview(ZombiePreview spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 11);
	}

	public void ApplyJoinTeam(JoinTeamApply spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 12);
	}

	public void ApplyJoinSpect(JoinSpecApply spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 13);
	}

	public void SendSynBag(SynItem syn)
	{
		SendMsg(JsonUtility.ToJson(syn), 1, 6);
	}
}
