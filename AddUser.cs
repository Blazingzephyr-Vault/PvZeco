using System.Collections;
using System.IO;
using SaveClass;
using UnityEngine;
using UnityEngine.UI;

public class AddUser : MonoBehaviour
{
	public InputField inputField;

	public Text Errortext;

	private Coroutine ErrortextCoroutine;

	private bool CanCancel;

	private void Start()
	{
		Errortext.transform.localScale = new Vector3(0f, 0f, 0f);
	}

	public void Display(bool canCancel)
	{
		CanCancel = canCancel;
		UIManager.Instance.OpenAndFocusUI(base.transform);
	}

	public void Confirm()
	{
		if (inputField.text == "")
		{
			return;
		}
		if (ChooseSave.Instance.CheckNameRepeat(inputField.text))
		{
			if (ErrortextCoroutine != null)
			{
				StopCoroutine(ErrortextCoroutine);
			}
			ErrortextCoroutine = StartCoroutine(RepeatLog());
			return;
		}
		if (!Directory.Exists(GameManager.Instance.SavePath + "/" + inputField.text))
		{
			Directory.CreateDirectory(GameManager.Instance.SavePath + "/" + inputField.text);
		}
		UserSave userSave = new UserSave();
		userSave.playerName = inputField.text;
		string value = JsonUtility.ToJson(userSave);
		StreamWriter streamWriter = new StreamWriter(GameManager.Instance.SavePath + "/" + inputField.text + "/Winfo.d");
		streamWriter.Write(value);
		streamWriter.Close();
		inputField.text = "";
		ChooseSave.Instance.LoadSave(userSave, GameManager.Instance.SavePath + "/" + inputField.text);
		CanCancel = true;
		Cancel();
		UIManager.Instance.CloseUI();
	}

	public void Cancel()
	{
		if (CanCancel)
		{
			StopAllCoroutines();
			Errortext.transform.localScale = new Vector3(0f, 0f, 0f);
			inputField.text = "";
			base.gameObject.SetActive(value: false);
			UIManager.Instance.OpenAndFocusUI(ChooseSave.Instance.transform);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
		}
	}

	private IEnumerator RepeatLog()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
		Errortext.transform.localScale = new Vector3(1f, 1f, 0f);
		yield return new WaitForSeconds(3f);
		Errortext.transform.localScale = new Vector3(0f, 0f, 0f);
	}
}
