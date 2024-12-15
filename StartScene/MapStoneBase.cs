using System.Collections.Generic;
using UnityEngine;

namespace StartScene;

public class MapStoneBase : MonoBehaviour
{
	public Sprite MapSprite;

	public Sprite LightSprite;

	public List<LVInfo> LVInfoList = new List<LVInfo>();

	public Sprite normalSprite;

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject() && !(SelectMap.Instance.SelectedStone == this))
		{
			GetComponent<SpriteRenderer>().sprite = LightSprite;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
		}
	}

	private void OnMouseExit()
	{
		if (SelectMap.Instance.SelectedStone != this)
		{
			GetComponent<SpriteRenderer>().sprite = normalSprite;
		}
	}

	private void OnMouseDown()
	{
		if (!MyTool.IsPointerOverGameObject() && !(SelectMap.Instance.SelectedStone == this))
		{
			SelectThis();
		}
	}

	public void SelectThis()
	{
		GetComponent<SpriteRenderer>().sprite = LightSprite;
		SelectMap.Instance.SelectStone(this);
	}

	public void ClearSelect()
	{
		if (!(SelectMap.Instance.SelectedStone == this))
		{
			GetComponent<SpriteRenderer>().sprite = normalSprite;
		}
	}
}
