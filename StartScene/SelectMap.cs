using System.Collections.Generic;
using UnityEngine;

namespace StartScene;

public class SelectMap : MonoBehaviour
{
	public static SelectMap Instance;

	public TextMesh Text;

	public SpriteRenderer MapSprite;

	public List<MapStoneBase> Stones = new List<MapStoneBase>();

	public MapStoneBase SelectedStone;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Stones[0].SelectThis();
		LevelSelector.Instance.OpenAndInit();
		LevelSelector.Instance.CloseSelector();
	}

	public void SelectStone(MapStoneBase selectedStone)
	{
		SelectedStone = selectedStone;
		for (int i = 0; i < Stones.Count; i++)
		{
			Stones[i].ClearSelect();
		}
		MapSprite.sprite = SelectedStone.MapSprite;
	}

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			Text.color = Color.white;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
		}
	}

	private void OnMouseExit()
	{
		Text.color = Color.black;
	}

	private void OnMouseDown()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			LevelSelector.Instance.OpenAndInit();
		}
	}
}
