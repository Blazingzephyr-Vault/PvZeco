using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
	private UnityAction Action;

	public Text TitleText;

	public Text ContentText;

	public Text WarnText;

	public void InitEvent(UnityAction action, string Title, string Content, string Warn)
	{
		Action = action;
		base.transform.localScale = Vector3.one;
		TitleText.text = Title;
		ContentText.text = Content;
		WarnText.text = Warn;
		UIManager.Instance.OpenAndFocusUI(base.transform);
	}

	public void Confirm()
	{
		if (Action != null)
		{
			Action();
		}
		base.transform.localScale = Vector3.zero;
		UIManager.Instance.CloseUI();
	}

	public void Cancel()
	{
		base.transform.localScale = Vector3.zero;
		UIManager.Instance.CloseUI();
	}
}
