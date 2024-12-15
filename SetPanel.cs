using SaveClass;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : MonoBehaviour
{
	public Slider MusicSlider;

	public Slider SoundSlider;

	public Text ContinueText;

	public Text ContinueText2;

	public Button BackMenu;

	public Button Restart;

	public Button ClientQuit;

	public Image FullScreen;

	public Image T1080P;

	public Sprite CheckBox;

	public Sprite CheckBox2;

	public bool isOpen;

	private bool isFullScreen;

	public bool is1080P;

	private void Start()
	{
		if (GameManager.Instance.isAndroid)
		{
			T1080P.gameObject.SetActive(value: false);
		}
	}

	public void ShowPanel(bool isShow, bool isBattle)
	{
		isOpen = isShow;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
		if (isShow)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			GameManager.Instance.SaveSetting();
			base.transform.localScale = new Vector3(0f, 0f, 0f);
		}
		if (isShow && isBattle)
		{
			if (!GameManager.Instance.isOnline)
			{
				Time.timeScale = 0f;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Pause, base.transform.position, isAll: true);
				AudioManager.Instance.StopBgAudio();
			}
		}
		else if (!GameManager.Instance.isOnline)
		{
			Time.timeScale = 1f;
			AudioManager.Instance.PlayBgAudio(null);
		}
		if (isBattle)
		{
			if (GameManager.Instance.isClient)
			{
				ClientQuit.transform.localScale = Vector3.one;
				BackMenu.transform.localScale = Vector3.zero;
				Restart.transform.localScale = Vector3.zero;
			}
			else
			{
				ClientQuit.transform.localScale = Vector3.zero;
				BackMenu.transform.localScale = Vector3.one;
				if (LV.Instance.CurrLVType != LVType.PvP)
				{
					Restart.transform.localScale = Vector3.one;
				}
				else
				{
					Restart.transform.localScale = Vector3.zero;
				}
			}
			ContinueText.text = "继续游戏";
			ContinueText2.text = "继续游戏";
		}
		else
		{
			BackMenu.transform.localScale = Vector3.zero;
			Restart.transform.localScale = Vector3.zero;
			ClientQuit.transform.localScale = Vector3.zero;
			ContinueText.text = "确定";
			ContinueText2.text = "确定";
		}
		UIManager.Instance.OpenAndFocusUI(base.transform);
	}

	public void VolumeChange()
	{
		AudioManager.Instance.SetVolume(SoundSlider.value, MusicSlider.value);
	}

	public void VolumeInit(SettingSave save)
	{
		MusicSlider.value = save.bgmVolume;
		SoundSlider.value = save.soundVolume;
		isFullScreen = save.isFullScreen;
		is1080P = save.isF1080P;
		if (isFullScreen)
		{
			Sprite sprite = FullScreen.sprite;
			FullScreen.sprite = CheckBox;
			CheckBox = sprite;
		}
		if (is1080P)
		{
			Sprite sprite2 = T1080P.sprite;
			T1080P.sprite = CheckBox2;
			CheckBox2 = sprite2;
		}
	}

	public void RestartScene()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		UIManager.Instance.ConfirmPanel.InitEvent(delegate
		{
			if (!GameManager.Instance.isOnline)
			{
				Time.timeScale = 1f;
				AudioManager.Instance.StopBgAudio();
			}
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
			LVManager.Instance.ReStartGame();
			CloseSetPanel();
		}, "重新开始本关卡？", "你确定要重新开始本关吗？", "");
	}

	public void BackMainScene()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		string warn = "";
		if (LVManager.Instance.GameIsStart)
		{
			warn = "*当前版本无法保存当前进度*";
		}
		UIManager.Instance.ConfirmPanel.InitEvent(delegate
		{
			if (!GameManager.Instance.isOnline)
			{
				Time.timeScale = 1f;
				AudioManager.Instance.StopBgAudio();
			}
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
			LVManager.Instance.QuitBattleGame();
			CloseSetPanel();
		}, "返回到主菜单？", "你确定要返回到主菜单吗？", warn);
	}

	public void DoClientQuit()
	{
		if (GameManager.Instance.isClient)
		{
			SocketClient.Instance.CloseClient();
			CloseSetPanel();
		}
	}

	public void CloseSetPanel()
	{
		ShowPanel(isShow: false, isBattle: false);
		UIManager.Instance.CloseUI();
	}

	public void FullScreenButton()
	{
		isFullScreen = !isFullScreen;
		Sprite sprite = FullScreen.sprite;
		FullScreen.sprite = CheckBox;
		CheckBox = sprite;
		if (isFullScreen)
		{
			Screen.SetResolution(1920, 1080, isFullScreen);
		}
		else if (is1080P)
		{
			Screen.SetResolution(1920, 1080, isFullScreen);
		}
		else
		{
			Screen.SetResolution(1280, 720, isFullScreen);
		}
		Screen.fullScreen = isFullScreen;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void T1080PButton()
	{
		is1080P = !is1080P;
		Sprite sprite = T1080P.sprite;
		T1080P.sprite = CheckBox2;
		CheckBox2 = sprite;
		if (is1080P)
		{
			Screen.SetResolution(1920, 1080, fullscreen: false);
		}
		else
		{
			Screen.SetResolution(1280, 720, fullscreen: false);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}
}
