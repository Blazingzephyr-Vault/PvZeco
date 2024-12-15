using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LogPanel : MonoBehaviour
{
	public Text logText;

	public Text ButtonText;

	public Button Button;

	private UnityAction action;

	public void DisplayLog(string logContent, UnityAction confirmAction)
	{
		UIManager.Instance.OpenAndFocusUI(base.transform);
		Button.gameObject.SetActive(value: true);
		logText.text = logContent;
		ButtonText.text = "确定";
		action = confirmAction;
	}

	public void Confirm()
	{
		if (action != null)
		{
			action();
			action = null;
		}
		Close();
	}

	public void CancelConfirm()
	{
		Button.gameObject.SetActive(value: false);
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}
}
