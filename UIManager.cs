using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	public Transform UIBackground;

	public LogPanel LogPanel;

	public ConfirmPanel ConfirmPanel;

	public Transform BattleUI;

	private LVStartEF LVStartEF;

	public SetPanel SetPanel;

	public OverPanel OverPanel;

	public Transform HostGame;

	public InputField IpInput;

	public InputField PortInput;

	public Transform JoinGame;

	public InputField JoinIpInput;

	public SeedChooser SeedChooser;

	public ZombieChooser ZombieChooser;

	private bool isChatBoxOpen;

	public ChatInput chatInput;

	public Transform ChatBox;

	public Transform OutChatBox;

	public Transform OpenChatButton;

	public bool IsChatBoxOpen
	{
		get
		{
			return isChatBoxOpen;
		}
		set
		{
			isChatBoxOpen = value;
			if (isChatBoxOpen)
			{
				chatInput.InputField.ActivateInputField();
				ChatBox.localScale = new Vector3(1f, 1f, 1f);
				OutChatBox.localScale = new Vector3(0f, 0f, 0f);
			}
			else
			{
				chatInput.ClearInput();
				ChatBox.localScale = new Vector3(0f, 0f, 0f);
				OutChatBox.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

	private void Awake()
	{
		Instance = this;
		LVStartEF = BattleUI.Find("LVEF").GetComponent<LVStartEF>();
		OverPanel = BattleUI.Find("OverPanel").GetComponent<OverPanel>();
		SetPanel = base.transform.Find("SetPanel").GetComponent<SetPanel>();
	}

	private void Start()
	{
		if (GameManager.Instance.isAndroid)
		{
			OpenChatButton.gameObject.SetActive(value: true);
		}
		else
		{
			OpenChatButton.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (SetPanel.isOpen || isChatBoxOpen)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (SetPanel.isOpen)
				{
					SetPanel.CloseSetPanel();
				}
				else if (isChatBoxOpen)
				{
					IsChatBoxOpen = false;
				}
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape) && LVManager.Instance.InGame && !SetPanel.isOpen)
		{
			ShowBattleSetPanel();
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			IsChatBoxOpen = true;
		}
		if (Input.GetKeyDown(KeyCode.Slash))
		{
			IsChatBoxOpen = true;
			chatInput.SlashOpen();
		}
	}

	public void SetChooserType(SeedBankType type)
	{
		switch (type)
		{
		case SeedBankType.SunBank:
			SeedChooser.transform.localScale = Vector3.one;
			ZombieChooser.transform.localScale = Vector3.zero;
			SeedChooser.ChangeChooserBtn.localScale = Vector3.zero;
			break;
		case SeedBankType.MoonBank:
			SeedChooser.transform.localScale = Vector3.zero;
			ZombieChooser.transform.localScale = Vector3.one;
			ZombieChooser.ChangeChooserBtn.localScale = Vector3.zero;
			break;
		case SeedBankType.SunAndMoonBank:
			SeedChooser.transform.localScale = Vector3.one;
			ZombieChooser.transform.localScale = Vector3.zero;
			SeedChooser.ChangeChooserBtn.localScale = Vector3.one;
			ZombieChooser.ChangeChooserBtn.localScale = Vector3.one;
			break;
		}
	}

	public void ChangeChooser()
	{
		if (SeedChooser.transform.localScale.x == 0f)
		{
			SeedChooser.transform.localScale = Vector3.one;
			ZombieChooser.transform.localScale = Vector3.zero;
		}
		else
		{
			SeedChooser.transform.localScale = Vector3.zero;
			ZombieChooser.transform.localScale = Vector3.one;
		}
	}

	public void OpenChatBtn()
	{
		if (IsChatBoxOpen)
		{
			chatInput.SendContent();
		}
		else
		{
			IsChatBoxOpen = true;
		}
	}

	public void ShowLVStartEF()
	{
		LVStartEF.Show();
	}

	public void StopLVStartEF()
	{
		LVStartEF.StopAll();
	}

	public void ShowBigWaveEF()
	{
		LVStartEF.ShowBigWave();
	}

	public void ShowFinalWaveEF()
	{
		LVStartEF.ShowFinalWave();
	}

	public void ShowSetPanel()
	{
		SetPanel.ShowPanel(isShow: true, isBattle: false);
		OpenAndFocusUI(null);
	}

	public void ShowBattleSetPanel()
	{
		SetPanel.ShowPanel(isShow: true, isBattle: true);
		OpenAndFocusUI(null);
	}

	public void ConfirmOpenServer()
	{
		if (IPAddress.TryParse(IpInput.text, out var address) && int.TryParse(PortInput.text, out var result))
		{
			if (result < 1025 || result > 65535)
			{
				LogPanel.DisplayLog("请输入正确的端口", delegate
				{
					OpenAndFocusUI(HostGame);
				});
			}
			else
			{
				SocketServer.Instance.StartServer(address, result);
				CloseHostGame();
			}
		}
		else
		{
			LogPanel.DisplayLog("请输入正确的端口", delegate
			{
				OpenAndFocusUI(HostGame);
			});
		}
	}

	public void ConfirmJoinGame()
	{
		string[] array = JoinIpInput.text.Split(":");
		int result;
		if (array.Length < 2)
		{
			LogPanel.DisplayLog("请输入正确的地址", delegate
			{
				OpenAndFocusUI(JoinGame);
			});
		}
		else if (int.TryParse(array[1], out result))
		{
			LogPanel.DisplayLog("连接中...", delegate
			{
				OpenAndFocusUI(JoinGame);
			});
			LogPanel.ButtonText.text = "取消";
			LogPanel.CancelConfirm();
			SocketClient.Instance.JoinGame(Dns.GetHostAddresses(array[0])[0], result);
		}
		else
		{
			LogPanel.DisplayLog("请输入正确的地址", delegate
			{
				OpenAndFocusUI(JoinGame);
			});
		}
	}

	public void ConnectSuccess()
	{
		LogPanel.gameObject.SetActive(value: false);
		CloseJoinGame();
	}

	public void CloseHostGame()
	{
		IpInput.text = "127.0.0.1";
		PortInput.text = "45678";
		HostGame.gameObject.SetActive(value: false);
		CloseUI();
	}

	public void CloseJoinGame()
	{
		JoinGame.gameObject.SetActive(value: false);
		CloseUI();
	}

	public void OpenAndFocusUI(Transform transform)
	{
		if (transform != null)
		{
			transform.gameObject.SetActive(value: true);
			UIBackground.SetSiblingIndex(transform.GetSiblingIndex() - 1);
		}
		UIBackground.gameObject.SetActive(value: true);
	}

	public void CloseUI()
	{
		UIBackground.gameObject.SetActive(value: false);
		UIBackground.SetSiblingIndex(0);
	}
}
