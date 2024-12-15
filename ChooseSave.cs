using System.Collections.Generic;
using System.IO;
using SaveClass;
using UnityEngine;

public class ChooseSave : MonoBehaviour
{
	public static ChooseSave Instance;

	public GameObject SaveOption;

	public GameObject Content;

	public AddUser AddUser;

	public EditUser EditUser;

	public SaveOption SelectedSaveOption;

	private List<SaveOption> saveOptions = new List<SaveOption>();

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		AddUser.gameObject.SetActive(value: false);
		EditUser.gameObject.SetActive(value: false);
		base.transform.localScale = new Vector3(0f, 0f, 0f);
	}

	public bool CheckNameRepeat(string Name)
	{
		bool result = false;
		for (int i = 0; i < saveOptions.Count; i++)
		{
			if (saveOptions[i].SaveNameText.text == Name)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public void ShowEditUser()
	{
		if (!(SelectedSaveOption == null))
		{
			UIManager.Instance.OpenAndFocusUI(EditUser.transform);
			EditUser.inputField.text = SelectedSaveOption.userSave.playerName;
		}
	}

	public void ShowAddUser()
	{
		AddUser.Display(canCancel: true);
	}

	public void LoadSave(UserSave saveUser, string path)
	{
		GameManager.Instance.LoadSave(saveUser, path);
		Cancel();
	}

	public void Confirm()
	{
		LoadSave(SelectedSaveOption.userSave, SelectedSaveOption.Path.ToString());
	}

	public void Cancel()
	{
		ClearOptions();
		base.transform.localScale = new Vector3(0f, 0f, 0f);
		UIManager.Instance.CloseUI();
	}

	private void ClearOptions()
	{
		for (int i = 0; i < saveOptions.Count; i++)
		{
			Object.Destroy(saveOptions[i].gameObject);
		}
		saveOptions.Clear();
		SelectedSaveOption = null;
	}

	public void LoadSavegroup()
	{
		ClearOptions();
		DirectoryInfo[] directories = new DirectoryInfo(GameManager.Instance.SavePath).GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			if (File.Exists(directories[i]?.ToString() + "/Winfo.d"))
			{
				StreamReader streamReader = new StreamReader(directories[i]?.ToString() + "/Winfo.d");
				string json = streamReader.ReadToEnd();
				streamReader.Close();
				UserSave saveUser = JsonUtility.FromJson<UserSave>(json);
				SaveOption component = Object.Instantiate(SaveOption).GetComponent<SaveOption>();
				saveOptions.Add(component);
				component.GetSaveinfo(saveUser, directories[i]);
				component.transform.SetParent(Content.transform);
				component.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		for (int j = 0; j < saveOptions.Count; j++)
		{
			if (saveOptions[j].Path.ToString() == GameManager.Instance.lastSavePath)
			{
				SelectedSaveOption = saveOptions[j];
				SelectedSaveOption.OnPointerClick(null);
				SelectedSaveOption.transform.SetSiblingIndex(0);
				StartSceneManager.Instance.changeUser.NameText.text = saveOptions[j].userSave.playerName + "!";
				break;
			}
		}
		if (saveOptions.Count == 0)
		{
			Cancel();
			AddUser.Display(canCancel: false);
		}
	}

	public void RenewSelect(SaveOption saveOption)
	{
		SelectedSaveOption = saveOption;
		for (int i = 0; i < saveOptions.Count; i++)
		{
			saveOptions[i].RenewSelect();
		}
	}
}
