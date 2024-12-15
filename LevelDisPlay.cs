using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelDisPlay : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public Image MapImage;

	public Text LevelText;

	public Image REnderer;

	public LVInfo lVInfo;

	public void OpenInit(LVInfo info)
	{
		lVInfo = info;
		if (info == null)
		{
			LevelText.text = "暂无关卡";
			return;
		}
		MapImage.sprite = info.LevelSprite;
		LevelText.text = info.LevelName;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!(lVInfo == null) && !(LevelSelector.Instance.SelectedDis == this))
		{
			if (eventData != null)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
			}
			LevelSelector.Instance.SelectThis(this);
			REnderer.color = new Color32(150, 150, 150, byte.MaxValue);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!(lVInfo == null))
		{
			REnderer.color = new Color32(150, 150, 150, byte.MaxValue);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!(LevelSelector.Instance.SelectedDis == this))
		{
			REnderer.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
	}
}
