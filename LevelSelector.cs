using System.Collections.Generic;
using StartScene;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
	public static LevelSelector Instance;

	public LevelDisPlay DisPlay1;

	public LevelDisPlay DisPlay2;

	public Text Title;

	public Text WeatherContent;

	public Text Difficulty;

	public GameObject IconPrefab;

	public Transform IconGroup;

	public bool IsEasy = true;

	private int CurrIndex;

	public LevelDisPlay SelectedDis { get; private set; }

	private List<LVInfo> lVInfoList => SelectMap.Instance.SelectedStone.LVInfoList;

	private void Awake()
	{
		Instance = this;
	}

	public void StartGame()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		if (!GameManager.Instance.isClient)
		{
			LVManager.Instance.StartGame(null);
			CloseSelector();
		}
	}

	public void ChangeHard()
	{
		IsEasy = !IsEasy;
		if (IsEasy)
		{
			Difficulty.text = "简单";
			Difficulty.color = new Color32(101, 244, 36, byte.MaxValue);
		}
		else
		{
			Difficulty.text = "困难";
			Difficulty.color = new Color32(244, 36, 39, byte.MaxValue);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
	}

	public void OpenAndInit()
	{
		SelectedDis = null;
		CurrIndex = 0;
		if (lVInfoList.Count > 0)
		{
			DisPlay1.OpenInit(lVInfoList[0]);
		}
		else
		{
			DisPlay1.OpenInit(null);
		}
		if (lVInfoList.Count > 1)
		{
			DisPlay2.OpenInit(lVInfoList[1]);
		}
		else
		{
			DisPlay2.OpenInit(null);
		}
		base.transform.localScale = Vector3.one;
		UIManager.Instance.OpenAndFocusUI(base.transform);
		DisPlay1.OnPointerClick(null);
	}

	public void CloseSelector()
	{
		base.transform.localScale = Vector3.zero;
		UIManager.Instance.CloseUI();
	}

	public void SelectThis(LevelDisPlay levelDis)
	{
		SelectedDis = null;
		DisPlay1.OnPointerExit(null);
		DisPlay2.OnPointerExit(null);
		SelectedDis = levelDis;
		DisLevelInfo(SelectedDis.lVInfo);
	}

	private void DisLevelInfo(LVInfo info)
	{
		MapInfoIcon[] componentsInChildren = IconGroup.GetComponentsInChildren<MapInfoIcon>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i].gameObject);
		}
		Title.text = info.LevelName;
		for (int j = 0; j < info.MapList.Count; j++)
		{
			MapInfoIcon component = Object.Instantiate(IconPrefab).GetComponent<MapInfoIcon>();
			component.CreateInit(info.MapList[j], isYes: true);
			component.transform.SetParent(IconGroup);
		}
		for (int k = 0; k < info.notMapList.Count; k++)
		{
			MapInfoIcon component2 = Object.Instantiate(IconPrefab).GetComponent<MapInfoIcon>();
			component2.CreateInit(info.notMapList[k], isYes: false);
			component2.transform.SetParent(IconGroup);
		}
		string text = "";
		if (info.HaveRain)
		{
			text += " 有雨";
		}
		if (info.HaveFog)
		{
			text += " 大雾";
		}
		if (info.HaveThunder)
		{
			text += " 有雷";
		}
		if (!info.HaveRain && !info.HaveFog && !info.HaveThunder)
		{
			text = " 晴朗";
		}
		WeatherContent.text = text;
	}

	private void UpdateLevelDis()
	{
		if (lVInfoList.Count > CurrIndex)
		{
			DisPlay1.OpenInit(lVInfoList[CurrIndex]);
		}
		else
		{
			DisPlay1.OpenInit(null);
		}
		if (lVInfoList.Count > CurrIndex + 1)
		{
			DisPlay2.OpenInit(lVInfoList[CurrIndex + 1]);
		}
		else
		{
			DisPlay2.OpenInit(null);
		}
	}

	public void LevelUp()
	{
		if (CurrIndex >= 1)
		{
			CurrIndex -= 2;
			UpdateLevelDis();
			SelectedDis = null;
			DisPlay1.OnPointerClick(null);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		}
	}

	public void LevelDown()
	{
		if (CurrIndex <= lVInfoList.Count - 2)
		{
			CurrIndex += 2;
			UpdateLevelDis();
			SelectedDis = null;
			DisPlay1.OnPointerClick(null);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		}
	}
}
