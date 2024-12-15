using System.IO;
using SaveClass;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public string SavePath;

	public string lastSavePath;

	public bool isOnline;

	public UserSave LocalPlayer;

	public bool isAndroid;

	public int VersionCode = 2003031;

	public string HostName;

	public GameConf GameConf { get; private set; }

	public AudioConf AudioConf { get; private set; }

	public bool isServer
	{
		get
		{
			if (isOnline)
			{
				return SocketServer.Instance.isServerOpen;
			}
			return false;
		}
	}

	public bool isClient
	{
		get
		{
			if (isOnline)
			{
				return !SocketServer.Instance.isServerOpen;
			}
			return false;
		}
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Application.targetFrameRate = 100;
			GameConf = Resources.Load<GameConf>("GameConf");
			AudioConf = Resources.Load<AudioConf>("AudioConf");
			SavePath = Application.persistentDataPath + "/saves";
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (isAndroid)
		{
			Screen.SetResolution(1920, 1080, fullscreen: false);
		}
		LoadSetting();
		if (lastSavePath != null && File.Exists(lastSavePath + "/Winfo.d"))
		{
			StreamReader streamReader = new StreamReader(lastSavePath + "/Winfo.d");
			string json = streamReader.ReadToEnd();
			streamReader.Close();
			UserSave saveUser = JsonUtility.FromJson<UserSave>(json);
			LoadSave(saveUser, lastSavePath);
			return;
		}
		if (!Directory.Exists(SavePath))
		{
			Directory.CreateDirectory(SavePath);
		}
		bool flag = false;
		DirectoryInfo[] directories = new DirectoryInfo(SavePath).GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			if (File.Exists(directories[i]?.ToString() + "/Winfo.d"))
			{
				StreamReader streamReader2 = new StreamReader(directories[i]?.ToString() + "/Winfo.d");
				string json2 = streamReader2.ReadToEnd();
				streamReader2.Close();
				UserSave saveUser2 = JsonUtility.FromJson<UserSave>(json2);
				LoadSave(saveUser2, directories[i].ToString());
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			ChooseSave.Instance.AddUser.Display(canCancel: false);
		}
	}

	public void LoadSave(UserSave saveUser, string loadingPath)
	{
		if (lastSavePath != loadingPath)
		{
			lastSavePath = loadingPath;
			SaveSetting();
		}
		LocalPlayer = saveUser;
		StartSceneManager.Instance.changeUser.NameText.text = saveUser.playerName + "!";
	}

	private void LoadSetting()
	{
		if (File.Exists(SavePath + "/Setting.d"))
		{
			StreamReader streamReader = new StreamReader(SavePath + "/Setting.d");
			string json = streamReader.ReadToEnd();
			streamReader.Close();
			SettingSave settingSave = JsonUtility.FromJson<SettingSave>(json);
			UIManager.Instance.SetPanel.VolumeInit(settingSave);
			lastSavePath = settingSave.lastLoadSavePath;
			Screen.fullScreen = settingSave.isFullScreen;
		}
	}

	public void SaveSetting()
	{
		SettingSave obj = new SettingSave
		{
			bgmVolume = AudioManager.Instance.BgmVolume,
			soundVolume = AudioManager.Instance.SoundVolume,
			lastLoadSavePath = lastSavePath,
			isFullScreen = Screen.fullScreen,
			isF1080P = UIManager.Instance.SetPanel.is1080P
		};
		if (!Directory.Exists(SavePath))
		{
			Directory.CreateDirectory(SavePath);
		}
		string value = JsonUtility.ToJson(obj);
		StreamWriter streamWriter = new StreamWriter(SavePath + "/Setting.d");
		streamWriter.Write(value);
		streamWriter.Close();
	}

	public void ResetPoolObJ()
	{
	}
}
