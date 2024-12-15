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

public class SocketServer : MonoBehaviour
{
	public static SocketServer Instance;

	private Socket socketWatch;

	public bool isServerOpen;

	private List<UnityAction> actions = new List<UnityAction>();

	private PlayerInfo HostPlayer;

	private List<PlayerInfo> players = new List<PlayerInfo>();

	private List<Socket> sockets = new List<Socket>();

	private int itemId;

	public int noHostPlayerNum => players.Count;

	public int ItemId
	{
		get
		{
			itemId++;
			return itemId;
		}
	}

	private void Awake()
	{
		Instance = this;
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
			Debug.Log("服务器主动断开" + ex);
			actions.RemoveAt(0);
		}
	}

	public void StartServer(IPAddress ip, int port)
	{
		if (!GameManager.Instance.isClient)
		{
			socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint localEP = new IPEndPoint(ip, port);
			socketWatch.Bind(localEP);
			socketWatch.Listen(4);
			new Thread(Recevice).Start(socketWatch);
			isServerOpen = true;
			HostPlayer = new PlayerInfo();
			HostPlayer.Name = GameManager.Instance.LocalPlayer.playerName;
			GameManager.Instance.isOnline = true;
			PlayerList.Instance.UpdatePlayerList(HostPlayer, null);
			BattlePlayerList.Instance.UpdatePlayerList(HostPlayer, null);
			StartCoroutine(CheckConnect());
		}
	}

	private void Recevice(object obj)
	{
		Debug.Log("启动成功。");
		Socket socket = obj as Socket;
		while (isServerOpen)
		{
			try
			{
				_ = string.Empty;
				Socket socket2 = socket.Accept();
				string text = socket2.RemoteEndPoint.ToString();
				ReceseMsgGoing(socket2);
				Debug.Log(text + ":连接到服务器。");
			}
			catch (Exception)
			{
				if (!isServerOpen)
				{
					Debug.Log("Error:1");
				}
			}
		}
	}

	private void ReceseMsgGoing(Socket TxSocket)
	{
		new Thread((ThreadStart)delegate
		{
			PlayerInfo playerInfo = new PlayerInfo();
			playerInfo.CheckId = 1;
			byte[] array = new byte[1];
			int num = 0;
			bool flag = false;
			while (true)
			{
				try
				{
					byte[] array2 = new byte[4194304];
					int num2 = TxSocket.Receive(array2);
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
						Debug.Log("断包x");
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
					if (list.Count == 0)
					{
						break;
					}
					for (int i = 0; i < list.Count; i++)
					{
						byte b = list[i][0];
						byte b2 = list[i][1];
						string getmsg = Encoding.UTF8.GetString(list[i], 2, list[i].Length - 2);
						switch (b)
						{
						case 0:
						{
							if (b2 != 1)
							{
								break;
							}
							playerInfo = JsonUtility.FromJson<PlayerInfo>(getmsg);
							bool flag2 = true;
							if (playerInfo.CheckId == GameManager.Instance.VersionCode)
							{
								if (players.Count >= 3)
								{
									flag2 = false;
									SendMsg(0, 1, "人数过多，加入失败。", TxSocket);
								}
								else if (HostPlayer.Name == playerInfo.Name)
								{
									flag2 = false;
									SendMsg(0, 1, "与在线玩家重名，加入失败。", TxSocket);
								}
								else if (LVManager.Instance.InGame)
								{
									flag2 = false;
									SendMsg(0, 1, "游戏已开始，加入失败。", TxSocket);
								}
								else
								{
									for (int j = 0; j < players.Count; j++)
									{
										if (players[j].Name == playerInfo.Name)
										{
											flag2 = false;
											SendMsg(0, 1, "与在线玩家重名，加入失败。", TxSocket);
											break;
										}
									}
								}
								if (flag2)
								{
									sockets.Add(TxSocket);
									actions.Add(delegate
									{
										AddNewPlayer(playerInfo);
										SendCommandBag(TxSocket);
										PvPModeSyn syn = new PvPModeSyn
										{
											Mode = PvPSelector.Instance.CurrMode
										};
										SynPvPMode(syn, TxSocket);
										PvPSelector.Instance.ServerSynTeam();
										SpectatorList.Instance.ServerSynSpectList();
									});
									break;
								}
								TxSocket.Close();
							}
							else
							{
								SendMsg(0, 1, "与服务器游戏版本不同。", TxSocket);
								TxSocket.Close();
							}
							goto end_IL_089e;
						}
						case 1:
							switch (b2)
							{
							case 0:
							{
								PlantSpawn spawn2 = JsonUtility.FromJson<PlantSpawn>(getmsg);
								actions.Add(delegate
								{
									UpdateCardCD updateCardCD2 = new UpdateCardCD
									{
										CardId = spawn2.CardId,
										name = playerInfo.Name
									};
									if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(playerInfo.Name))
									{
										spawn2.GridPos = new Vector2(0f - spawn2.GridPos.x, spawn2.GridPos.y);
									}
									PlantBase newPlant = PlantManager.Instance.GetNewPlant(spawn2.plantType);
									Grid gridByWorldPos3 = MapManager.Instance.GetGridByWorldPos(spawn2.GridPos);
									if (SeedBank.Instance.CheckPlant(newPlant, gridByWorldPos3, -1, playerInfo.Name))
									{
										int needSun = -1;
										updateCardCD2.OK = false;
										if (spawn2.SPcode == 2)
										{
											newPlant.InitForCreate(inGrid: false, null, isBlcWhi: false);
											needSun = SeedChooser.Instance.GetCardInfo(newPlant.GetPlantType()).NeedNum;
										}
										SeedBank.Instance.PlantConfirm(newPlant, gridByWorldPos3, needSun, spawn2.SPcode, playerInfo.Name);
										SendMsg(2, 4, JsonUtility.ToJson(updateCardCD2));
										BattlePlayerList.Instance.UpdateCardCD(updateCardCD2.name, updateCardCD2.CardId, updateCardCD2.OK);
									}
									else
									{
										updateCardCD2.OK = true;
										SendMsg(2, 4, JsonUtility.ToJson(updateCardCD2), TxSocket);
										UnityEngine.Object.Destroy(newPlant.gameObject);
									}
								});
								break;
							}
							case 1:
							{
								ShovelApply apply12 = JsonUtility.FromJson<ShovelApply>(getmsg);
								actions.Add(delegate
								{
									Grid gridByWorldPos2 = MapManager.Instance.GetGridByWorldPos(apply12.GridPos);
									Shovel.Instance.ClearPlant(gridByWorldPos2, apply12.GridPos, playerInfo.Name);
								});
								break;
							}
							case 2:
							{
								ClickedSun apply11 = JsonUtility.FromJson<ClickedSun>(getmsg);
								actions.Add(delegate
								{
									SkyManager.Instance.OnlineCollectSun(apply11);
								});
								break;
							}
							case 3:
							{
								PlayerMap apply10 = JsonUtility.FromJson<PlayerMap>(getmsg);
								apply10.PlayerName = playerInfo.Name;
								actions.Add(delegate
								{
									ChangeMap(apply10);
									BattlePlayerList.Instance.UpdateMapSprite(apply10.PlayerName, apply10.Pos);
								});
								break;
							}
							case 4:
							{
								SelectCard apply9 = JsonUtility.FromJson<SelectCard>(getmsg);
								actions.Add(delegate
								{
									apply9.PlayerName = playerInfo.Name;
									SelectCard(apply9);
									if (apply9.isBack)
									{
										BattlePlayerList.Instance.CancelCard(apply9.PlayerName, apply9.cardId);
									}
									else
									{
										BattlePlayerList.Instance.SelectCard(apply9.PlayerName, apply9.plantType, apply9.zombieType);
									}
								});
								break;
							}
							case 5:
							{
								SelectPrepare apply8 = JsonUtility.FromJson<SelectPrepare>(getmsg);
								actions.Add(delegate
								{
									apply8.PlayerName = playerInfo.Name;
									SelectPrepare(apply8);
									BattlePlayerList.Instance.UpdateState(apply8.PlayerName, apply8.isPrepare);
								});
								break;
							}
							case 6:
							{
								SynItem apply7 = JsonUtility.FromJson<SynItem>(getmsg);
								actions.Add(delegate
								{
									SynItem(apply7);
								});
								break;
							}
							case 7:
							{
								PlantPreview apply6 = JsonUtility.FromJson<PlantPreview>(getmsg);
								actions.Add(delegate
								{
									BattlePlayerList.Instance.PreviewPlant(apply6);
									PlacePreview(apply6, TxSocket);
								});
								break;
							}
							case 8:
							{
								UpdateCardCD apply5 = JsonUtility.FromJson<UpdateCardCD>(getmsg);
								actions.Add(delegate
								{
									apply5.name = playerInfo.Name;
									BattlePlayerList.Instance.UpdateCardCD(apply5.name, apply5.CardId, apply5.OK);
									SendMsg(2, 4, JsonUtility.ToJson(apply5), TxSocket, OutThis: true);
								});
								break;
							}
							case 9:
							{
								ShovelPreview apply4 = JsonUtility.FromJson<ShovelPreview>(getmsg);
								actions.Add(delegate
								{
									BattlePlayerList.Instance.PreviewShovel(apply4.PlayerName, apply4.GridPos, apply4.isShow);
									ShovelPreview(apply4, TxSocket);
								});
								break;
							}
							case 10:
							{
								ZombieSpawnApply spawn = JsonUtility.FromJson<ZombieSpawnApply>(getmsg);
								actions.Add(delegate
								{
									UpdateCardCD updateCardCD = new UpdateCardCD
									{
										CardId = spawn.CardId,
										name = playerInfo.Name
									};
									if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(playerInfo.Name))
									{
										spawn.GridPos = new Vector2(0f - spawn.GridPos.x, spawn.GridPos.y);
									}
									ZombieBase newZombie = ZombieManager.Instance.GetNewZombie(spawn.Type);
									Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(spawn.GridPos);
									if (SeedBank.Instance.CheckZombie(spawn.Type, gridByWorldPos, -1, playerInfo.Name))
									{
										updateCardCD.OK = false;
										SeedBank.Instance.ZombieConfirm(spawn.Type, newZombie, gridByWorldPos, -1, playerInfo.Name);
										SendMsg(2, 4, JsonUtility.ToJson(updateCardCD));
										BattlePlayerList.Instance.UpdateCardCD(updateCardCD.name, updateCardCD.CardId, updateCardCD.OK);
									}
									else
									{
										updateCardCD.OK = true;
										SendMsg(2, 4, JsonUtility.ToJson(updateCardCD), TxSocket);
										UnityEngine.Object.Destroy(newZombie.gameObject);
									}
								});
								break;
							}
							case 11:
							{
								ZombiePreview apply3 = JsonUtility.FromJson<ZombiePreview>(getmsg);
								actions.Add(delegate
								{
									BattlePlayerList.Instance.PreviewZombie(apply3);
									ZombiePreview(apply3, TxSocket);
								});
								break;
							}
							case 12:
							{
								JoinTeamApply apply2 = JsonUtility.FromJson<JoinTeamApply>(getmsg);
								actions.Add(delegate
								{
									if (apply2.isRed)
									{
										PvPSelector.Instance.JoinRed(playerInfo.Name);
									}
									else
									{
										PvPSelector.Instance.JoinBlue(playerInfo.Name);
									}
								});
								break;
							}
							case 13:
							{
								JoinSpecApply apply = JsonUtility.FromJson<JoinSpecApply>(getmsg);
								actions.Add(delegate
								{
									SpectatorList.Instance.ClientJoinSpect(playerInfo.Name, apply.isJoin);
								});
								break;
							}
							}
							break;
						case 2:
							switch (b2)
							{
							case 0:
								actions.Add(delegate
								{
									ChatInput.Instance.AddMessage(getmsg);
									SendMsg(4, 0, getmsg, TxSocket, OutThis: true);
								});
								break;
							case 1:
							{
								PrivateChatMsg chatMsg = JsonUtility.FromJson<PrivateChatMsg>(getmsg);
								actions.Add(delegate
								{
									if (chatMsg.PlayerName == GameManager.Instance.LocalPlayer.playerName)
									{
										string content = "玩家" + playerInfo.Name + "悄悄对你说:" + chatMsg.content;
										ChatInput.Instance.AddMessage(content, new Color32(123, 123, 123, byte.MaxValue));
									}
									else
									{
										SendPrivateChatMsg(chatMsg.PlayerName, chatMsg.content, playerInfo.Name);
									}
								});
								break;
							}
							}
							break;
						}
						continue;
						end_IL_089e:
						break;
					}
				}
				catch (Exception message)
				{
					Debug.Log(message);
					if (sockets.Contains(TxSocket))
					{
						sockets.Remove(TxSocket);
					}
					TxSocket.Close();
					if (playerInfo.CheckId != 1)
					{
						actions.Add(delegate
						{
							RemovePlayer(playerInfo);
						});
					}
					break;
				}
			}
		}).Start();
	}

	private void SendMsg(byte type1, byte type2, string content = "", Socket socket = null, bool OutThis = false)
	{
		List<byte> list = new List<byte>();
		list.Add(type1);
		list.Add(type2);
		list.AddRange(Encoding.UTF8.GetBytes(content));
		list.InsertRange(0, BitConverter.GetBytes(list.Count));
		byte[] buffer = list.ToArray();
		if (socket == null)
		{
			for (int i = 0; i < sockets.Count; i++)
			{
				try
				{
					sockets[i].Send(buffer);
				}
				catch (Exception)
				{
					PlayerInfo playerInfo = null;
					if (sockets.Contains(sockets[i]))
					{
						playerInfo = players[sockets.IndexOf(sockets[i])];
					}
					sockets[i].Close();
					sockets.Remove(sockets[i]);
					if (playerInfo != null && playerInfo.CheckId != 1)
					{
						actions.Add(delegate
						{
							RemovePlayer(playerInfo);
						});
					}
					Debug.Log("677" + playerInfo.Name);
				}
			}
			return;
		}
		try
		{
			if (OutThis)
			{
				for (int j = 0; j < sockets.Count; j++)
				{
					if (sockets[j] != socket)
					{
						sockets[j].Send(buffer);
					}
				}
			}
			else
			{
				socket.Send(buffer);
			}
		}
		catch (Exception)
		{
			PlayerInfo playerInfo2 = players[sockets.IndexOf(socket)];
			socket.Close();
			sockets.Remove(socket);
			if (playerInfo2.CheckId != 1)
			{
				actions.Add(delegate
				{
					RemovePlayer(playerInfo2);
				});
			}
		}
	}

	private IEnumerator CheckConnect()
	{
		while (GameManager.Instance.isOnline)
		{
			yield return new WaitForSeconds(1f);
			SendMsg(0, byte.MaxValue);
		}
	}

	public void CloseServer()
	{
		HostPlayer = null;
		for (int i = 0; i < sockets.Count; i++)
		{
			sockets[i].Close();
		}
		sockets.Clear();
		socketWatch.Close();
		socketWatch = null;
		isServerOpen = false;
		GameManager.Instance.isOnline = false;
		BattlePlayerList.Instance.UpdatePlayerList(null, players);
		PlayerList.Instance.UpdatePlayerList(null, players);
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

	public List<string> GetAllPlayerNameList()
	{
		List<string> list = new List<string>();
		list.Add(HostPlayer.Name);
		for (int i = 0; i < players.Count; i++)
		{
			list.Add(players[i].Name);
		}
		return list;
	}

	public void SendHostCD(int CardID, bool isOK)
	{
		UpdateCardCD updateCardCD = new UpdateCardCD();
		updateCardCD.name = GameManager.Instance.LocalPlayer.playerName;
		updateCardCD.CardId = CardID;
		updateCardCD.OK = isOK;
		SendMsg(2, 4, JsonUtility.ToJson(updateCardCD));
	}

	public void SendChatMsg(string content)
	{
		SendMsg(4, 0, content);
	}

	public void SendPrivateChatMsg(string name, string content, string sender)
	{
		PrivateChatMsg privateChatMsg = new PrivateChatMsg();
		privateChatMsg.PlayerName = sender;
		privateChatMsg.content = content;
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].Name == name && sockets.Count > i)
			{
				SendMsg(4, 2, JsonUtility.ToJson(privateChatMsg), sockets[i]);
				break;
			}
		}
	}

	public void KickPlayer(string name)
	{
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].Name == name && sockets.Count > i)
			{
				SendMsg(0, 1, "你被踢出了游戏。", sockets[i]);
				sockets[i].Close();
				break;
			}
		}
	}

	public void GameOver(GameOver over)
	{
		SendMsg(1, 8, JsonUtility.ToJson(over));
	}

	public void SynTeamList(PvPTeamList over)
	{
		SendMsg(1, 9, JsonUtility.ToJson(over));
	}

	public void SynPvPMode(PvPModeSyn syn, Socket socket = null)
	{
		SendMsg(1, 10, JsonUtility.ToJson(syn), socket);
	}

	public void SynSpectList(SpectList over)
	{
		SendMsg(1, 11, JsonUtility.ToJson(over));
	}

	public void LoadLv(LoadLVBag loadLV)
	{
		SendMsg(1, 0, JsonUtility.ToJson(loadLV));
	}

	public void StartRunLv()
	{
		SendMsg(1, 1);
	}

	public void BigWaveComing(WaveComing bigWave)
	{
		SendMsg(1, 2, JsonUtility.ToJson(bigWave));
	}

	public void UpdateSunNum(SunNumBag SunNum)
	{
		SendMsg(1, 3, JsonUtility.ToJson(SunNum));
	}

	public void SendSynBag(SynItem syn)
	{
		SendMsg(1, 4, JsonUtility.ToJson(syn));
	}

	public void ChangeMap(PlayerMap map)
	{
		SendMsg(1, 5, JsonUtility.ToJson(map));
	}

	public void SelectCard(SelectCard card)
	{
		SendMsg(1, 6, JsonUtility.ToJson(card));
	}

	public void SelectPrepare(SelectPrepare prepare)
	{
		SendMsg(1, 7, JsonUtility.ToJson(prepare));
	}

	public void SpawnSun(SunSpawn spawn)
	{
		SendMsg(2, 1, JsonUtility.ToJson(spawn));
	}

	public void ClickedSun(ClickedSun sun)
	{
		SendMsg(2, 2, JsonUtility.ToJson(sun));
	}

	public void SpawnPlant(PlantSpawn spawn)
	{
		SendMsg(2, 0, JsonUtility.ToJson(spawn));
	}

	public void PlacePreview(PlantPreview spawn, Socket socket)
	{
		SendMsg(2, 3, JsonUtility.ToJson(spawn), socket, OutThis: true);
	}

	public void ZombiePreview(ZombiePreview spawn, Socket socket)
	{
		SendMsg(3, 5, JsonUtility.ToJson(spawn), socket, OutThis: true);
	}

	public void ShovelPreview(ShovelPreview spawn, Socket socket)
	{
		SendMsg(2, 5, JsonUtility.ToJson(spawn), socket, OutThis: true);
	}

	public void SpawnZombie(ZombieSpawn spawn)
	{
		SendMsg(3, 0, JsonUtility.ToJson(spawn));
	}

	public void SpawnGraveStone(GraveStoneSpawn spawn)
	{
		SendMsg(3, 1, JsonUtility.ToJson(spawn));
	}

	public void SpawnPuddle(PuddleSpawn spawn)
	{
		SendMsg(3, 2, JsonUtility.ToJson(spawn));
	}

	public void SpawnLightning(LightingSpawn spawn)
	{
		SendMsg(3, 3, JsonUtility.ToJson(spawn));
	}

	public void SendCommandBag(Socket socket = null)
	{
		CommandBag commandBag = new CommandBag();
		commandBag.Pinv = PlantManager.Instance.PlantInvincible;
		commandBag.Zinv = ZombieManager.Instance.ZombieInvincible;
		commandBag.DLiCy = SkyManager.Instance.DayLightCycle;
		commandBag.SnInf = PlayerManager.Instance.SunInfinite;
		commandBag.CdCle = SeedBank.Instance.isNoCD;
		SendMsg(4, 3, JsonUtility.ToJson(commandBag), socket);
	}

	public void SendTimeWeatherCmd(TimeWeatherCmd cmd)
	{
		SendMsg(4, 4, JsonUtility.ToJson(cmd));
	}

	public void SendMowerLaunch(MowerLaunch cmd)
	{
		SendMsg(3, 4, JsonUtility.ToJson(cmd));
	}

	private void AddNewPlayer(PlayerInfo player)
	{
		players.Add(player);
		BattlePlayerList.Instance.UpdatePlayerList(HostPlayer, players);
		PlayerList.Instance.UpdatePlayerList(HostPlayer, players);
		string content = player.Name + "加入了游戏";
		ChatInput.Instance.AddMessage(content, new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue));
		SendMsg(4, 1, content);
		OnlinePlayerInfo onlinePlayerInfo = new OnlinePlayerInfo();
		onlinePlayerInfo.HostPlayer = HostPlayer;
		onlinePlayerInfo.players = players;
		SendMsg(0, 2, JsonUtility.ToJson(onlinePlayerInfo));
	}

	private void RemovePlayer(PlayerInfo player)
	{
		players.Remove(player);
		BattlePlayerList.Instance.UpdatePlayerList(HostPlayer, players);
		PlayerList.Instance.UpdatePlayerList(HostPlayer, players);
		string content = player.Name + "退出了游戏";
		PvPSelector.Instance.ClearQuitPlayer(player.Name);
		SpectatorList.Instance.ClearPlayer(player.Name);
		ChatInput.Instance.AddMessage(content, new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue));
		SendMsg(4, 1, content);
		OnlinePlayerInfo onlinePlayerInfo = new OnlinePlayerInfo();
		onlinePlayerInfo.HostPlayer = HostPlayer;
		onlinePlayerInfo.players = players;
		SendMsg(0, 2, JsonUtility.ToJson(onlinePlayerInfo));
	}
}
