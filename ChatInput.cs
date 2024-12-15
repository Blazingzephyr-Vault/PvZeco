using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatInput : MonoBehaviour
{
	public static ChatInput Instance;

	public InputField InputField;

	private List<string> oldChat = new List<string>();

	private int oldChatIndex;

	private int OldChatIndex
	{
		get
		{
			return oldChatIndex;
		}
		set
		{
			if (value < oldChat.Count && value >= 0)
			{
				oldChatIndex = value;
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

	private void Update()
	{
		if (!UIManager.Instance.IsChatBoxOpen)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
		{
			SendContent();
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			OldChatIndex--;
			if (oldChat.Count > 0)
			{
				InputField.text = oldChat[OldChatIndex];
			}
			InputField.MoveTextEnd(shift: false);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			OldChatIndex++;
			if (oldChat.Count > 0)
			{
				InputField.text = oldChat[OldChatIndex];
			}
			InputField.MoveTextEnd(shift: false);
		}
	}

	public void SendContent()
	{
		if (InputField.text != "")
		{
			if (InputField.text.IndexOf("/") == 0)
			{
				Command(InputField.text);
			}
			else
			{
				SendMessageToAll(InputField.text);
			}
			if (oldChat.Count > 20)
			{
				oldChat.Remove(oldChat[0]);
			}
			if (oldChat.Count == 0)
			{
				oldChat.Add(InputField.text);
			}
			if (oldChat.Count > 0 && oldChat[oldChat.Count - 1] != InputField.text)
			{
				oldChat.Add(InputField.text);
			}
		}
		UIManager.Instance.IsChatBoxOpen = false;
	}

	public void SlashOpen()
	{
		StartCoroutine(Slash());
	}

	private IEnumerator Slash()
	{
		yield return new WaitForFixedUpdate();
		InputField.text = "/";
		InputField.MoveTextEnd(shift: false);
	}

	public void ClearInput()
	{
		InputField.text = "";
		oldChatIndex = oldChat.Count;
	}

	private void SendMessageToAll(string content)
	{
		string text = "<" + GameManager.Instance.LocalPlayer.playerName + ">" + content;
		AddMessage(text);
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendChatMsg(text);
		}
		if (GameManager.Instance.isClient)
		{
			SocketClient.Instance.SendChatMsg(text);
		}
	}

	public void AddMessage(string content)
	{
		ChatTextGroup.Instance.InputContent(content);
		OutChatTextGroup.Instance.InputContent(content);
	}

	public void AddMessage(string content, Color32 color)
	{
		ChatTextGroup.Instance.InputContent(content, color);
		OutChatTextGroup.Instance.InputContent(content, color);
	}

	private void Command(string InputText)
	{
		InputText = InputText.Remove(0, 1);
		string[] array = InputText.Split(" ");
		try
		{
			switch (array[0].ToLower())
			{
			case "sun":
				SunCommand(array);
				break;
			case "summon":
				CommandError("Summon暂不可用");
				break;
			case "time":
				TimeCommand(array);
				break;
			case "cd":
				CDCommand(array);
				break;
			case "weather":
				WeatherCommand(array);
				break;
			case "fill":
				FillCommand(array);
				break;
			case "msg":
				MsgCommand(array);
				break;
			case "kick":
				KickCommand(array);
				break;
			case "kill":
				KillCommand(array);
				break;
			case "gamerule":
				GameruleCommand(array);
				break;
			case "win":
				WinCommand(array);
				break;
			case "card":
				CardCommand(array);
				break;
			default:
				CommandError();
				break;
			}
		}
		catch
		{
			CommandError();
		}
	}

	private void SunCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (str[1].ToLower() == "set")
		{
			if (int.TryParse(str[2], out var result) && result >= 0)
			{
				PlayerManager.Instance.SetSunNum(result, isSun: true, null);
				PlayerManager.Instance.SetSunNum(result, isSun: false, null);
			}
			else
			{
				CommandError();
			}
		}
		else if (str[1].ToLower() == "add")
		{
			if (int.TryParse(str[2], out var result2))
			{
				PlayerManager.Instance.AddSunNum(result2, isSun: true, null);
				PlayerManager.Instance.AddSunNum(result2, isSun: false, null);
			}
			else
			{
				CommandError();
			}
		}
		else if (str[1].ToLower() == "infinite")
		{
			PlayerManager.Instance.SunInfinite = !PlayerManager.Instance.SunInfinite;
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (PlayerManager.Instance.SunInfinite)
			{
				AddMessage("无限阳光已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("无限阳光已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "auto")
		{
			SkyManager.Instance.SunAutoCollect = !SkyManager.Instance.SunAutoCollect;
			if (SkyManager.Instance.SunAutoCollect)
			{
				AddMessage("阳光收集已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("阳光收集已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else
		{
			CommandError();
		}
	}

	private void SummonCommand(string[] str)
	{
		int result;
		int result2;
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (int.TryParse(str[1], out result) && int.TryParse(str[2], out result2))
		{
			if (str.Length > 3 && int.TryParse(str[3], out var result3))
			{
				Grid grid = null;
				List<Grid> gridList = CameraControl.Instance.CurrMap.GridList;
				for (int i = 0; i < gridList.Count; i++)
				{
					if (gridList[i].Point.x == result2 - 1 && gridList[i].Point.y == result3 - 1)
					{
						grid = gridList[i];
					}
				}
				if (grid != null)
				{
					ZombieManager.Instance.SummonZombie(result, grid);
				}
			}
			else
			{
				ZombieManager.Instance.SummonZombie(result, result2 - 1, CameraControl.Instance.transform.position);
			}
		}
		else
		{
			CommandError();
		}
	}

	private void TimeCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (str[1].ToLower() == "set")
		{
			if (int.TryParse(str[2], out var result))
			{
				SkyManager.Instance.DirectSetTime(result);
			}
			else if (str[2].ToLower() == "day")
			{
				SkyManager.Instance.DirectSetTime(480);
			}
			else if (str[2].ToLower() == "night")
			{
				SkyManager.Instance.DirectSetTime(1140);
			}
			else
			{
				CommandError();
			}
		}
		else
		{
			CommandError();
		}
	}

	private void CDCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (str[1].ToLower() == "clear")
		{
			SeedBank.Instance.ClearAllCD();
			AddMessage("已清除自己的CD", new Color32(123, 123, 123, byte.MaxValue));
		}
		else if (str[1].ToLower() == "infinite")
		{
			SeedBank.Instance.isNoCD = !SeedBank.Instance.isNoCD;
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (SeedBank.Instance.isNoCD)
			{
				AddMessage("无CD已开启", new Color32(123, 123, 123, byte.MaxValue));
				SeedBank.Instance.ClearAllCD();
			}
			else
			{
				AddMessage("无CD已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else
		{
			CommandError();
		}
	}

	private void WeatherCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (str[1].ToLower() == "clear")
		{
			SkyManager.Instance.IsThunder = false;
			SkyManager.Instance.RainScale = 0;
			AddMessage("天气更换为晴天", new Color32(123, 123, 123, byte.MaxValue));
		}
		else if (str[1].ToLower() == "rain")
		{
			SkyManager.Instance.RainScale = 10;
			AddMessage("天气更换为雨天", new Color32(123, 123, 123, byte.MaxValue));
		}
		else if (str[1].ToLower() == "thunder")
		{
			SkyManager.Instance.RainScale = 10;
			SkyManager.Instance.IsThunder = true;
			AddMessage("天气更换为雷雨天", new Color32(123, 123, 123, byte.MaxValue));
		}
		else
		{
			CommandError();
		}
	}

	private void FillCommand(string[] str)
	{
		int result;
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (int.TryParse(str[1], out result))
		{
			PlantType cardType = SeedChooser.Instance.GetCardType(result);
			if (cardType != 0 && CameraControl.Instance.CurrMap != null)
			{
				if (int.TryParse(str[2], out var result2) && int.TryParse(str[3], out var result3) && int.TryParse(str[4], out var result4) && int.TryParse(str[5], out var result5))
				{
					int num = 0;
					List<Grid> gridList = CameraControl.Instance.CurrMap.GridList;
					int num2 = ((result2 < result4) ? result2 : result4);
					int num3 = ((result2 > result4) ? result2 : result4);
					int num4 = ((result3 < result5) ? result3 : result5);
					int num5 = ((result3 > result5) ? result3 : result5);
					for (int i = 0; i < gridList.Count; i++)
					{
						if (gridList[i].Point.x >= num2 - 1 && gridList[i].Point.x < num3 && gridList[i].Point.y >= num4 - 1 && gridList[i].Point.y < num5)
						{
							PlantBase newPlant = PlantManager.Instance.GetNewPlant(cardType);
							if (SeedBank.Instance.CheckPlant(newPlant, gridList[i], -2, null))
							{
								num++;
								SeedBank.Instance.PlantConfirm(newPlant, gridList[i], -2, 0, null);
							}
							else
							{
								Object.Destroy(newPlant.gameObject);
							}
						}
					}
					AddMessage("已种植" + num + "个植物", new Color32(123, 123, 123, byte.MaxValue));
				}
				else
				{
					CommandError();
				}
			}
			else
			{
				CommandError();
			}
		}
		else
		{
			CommandError();
		}
	}

	private void MsgCommand(string[] str)
	{
		if (str.Length > 2)
		{
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendPrivateChatMsg(str[1], str[2], GameManager.Instance.LocalPlayer.playerName);
			}
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.SendPrivateChatMsg(str[1], str[2]);
			}
			string content = "你悄悄对玩家" + str[1] + "说:" + str[2];
			AddMessage(content, new Color32(123, 123, 123, byte.MaxValue));
		}
		else
		{
			CommandError();
		}
	}

	private void KickCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.KickPlayer(str[1]);
		}
	}

	private void WinCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
			return;
		}
		MoneyBag component = Object.Instantiate(GameManager.Instance.GameConf.Moneybag).GetComponent<MoneyBag>();
		component.transform.position = CameraControl.Instance.transform.position;
		component.transform.SetParent(MapManager.Instance.GetCurrMap(component.transform.position).transform);
		component.OnMouseDown();
	}

	private void CardCommand(string[] str)
	{
		int result;
		int result2;
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (GameManager.Instance.isOnline)
		{
			CommandError("多人暂不可用");
		}
		else if (int.TryParse(str[1], out result) && int.TryParse(str[2], out result2))
		{
			SeedBank.Instance.ChangeCard(result2, result);
		}
		else
		{
			CommandError();
		}
	}

	private void KillCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (str[1].ToLower() == "plant")
		{
			if (str[2].ToLower() == "all")
			{
				AddMessage("已杀死" + PlantManager.Instance.plants.Count + "个植物", new Color32(123, 123, 123, byte.MaxValue));
				PlantManager.Instance.ClearAllPlant(synClient: false);
			}
			else if (str[2].ToLower() == "there")
			{
				AddMessage("已杀死" + PlantManager.Instance.ClearMapPlant(CameraControl.Instance.CurrMap) + "个植物", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				CommandError();
			}
		}
		else if (str[1].ToLower() == "zombie")
		{
			if (str[2].ToLower() == "all")
			{
				AddMessage("已杀死" + ZombieManager.Instance.BigHurtAllZombie() + "个僵尸", new Color32(123, 123, 123, byte.MaxValue));
			}
			else if (str[2].ToLower() == "there")
			{
				AddMessage("已杀死" + ZombieManager.Instance.BigHurtMapZombie(CameraControl.Instance.CurrMap) + "个僵尸", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				CommandError();
			}
		}
		else
		{
			CommandError();
		}
	}

	private void GameruleCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (str[1].ToLower() == "plantinvincible")
		{
			PlantManager.Instance.PlantInvincible = !PlantManager.Instance.PlantInvincible;
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (PlantManager.Instance.PlantInvincible)
			{
				AddMessage("植物无敌已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("植物无敌已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "zombieinvincible")
		{
			ZombieManager.Instance.ZombieInvincible = !ZombieManager.Instance.ZombieInvincible;
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (ZombieManager.Instance.ZombieInvincible)
			{
				AddMessage("僵尸无敌已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("僵尸无敌已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "plantdontsleep")
		{
			PlantManager.Instance.PlantDontSleep = !PlantManager.Instance.PlantDontSleep;
			if (PlantManager.Instance.PlantDontSleep)
			{
				AddMessage("植物不睡觉已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("植物不睡觉已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "dontfail")
		{
			LVManager.Instance.LvDontFail = !LVManager.Instance.LvDontFail;
			if (LVManager.Instance.LvDontFail)
			{
				AddMessage("关卡不失败已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("关卡不失败已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "daylightcycle")
		{
			SkyManager.Instance.DayLightCycle = !SkyManager.Instance.DayLightCycle;
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (SkyManager.Instance.DayLightCycle)
			{
				AddMessage("日夜循环已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("日夜循环已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else
		{
			CommandError();
		}
	}

	private void CommandError(string content = "")
	{
		if (content == "")
		{
			AddMessage("请输入正确的命令", new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
		}
		else
		{
			AddMessage(content, new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
		}
	}
}
