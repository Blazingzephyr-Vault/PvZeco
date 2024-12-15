using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChooser : MonoBehaviour
{
	public static ZombieChooser Instance;

	public StartButton startButton;

	public List<Transform> Pages = new List<Transform>();

	private int currPage;

	public UIPlantCardNC ForGetSizeNc;

	public Transform ChangeChooserBtn;

	public bool isPrepare;

	public int CurrPage
	{
		get
		{
			return currPage;
		}
		set
		{
			int index = currPage;
			if (value > Pages.Count - 1)
			{
				currPage = 0;
			}
			else if (value < 0)
			{
				currPage = Pages.Count - 1;
			}
			else
			{
				currPage = value;
			}
			Pages[index].localScale = new Vector3(0f, 0f);
			Pages[currPage].localScale = new Vector3(1f, 1f);
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		for (int i = 0; i < Pages.Count; i++)
		{
			Pages[i].localScale = new Vector3(0f, 0f);
		}
		CurrPage = 0;
	}

	public UIPlantCardNC[] GetCardInfos(int pageIndex)
	{
		return Pages[pageIndex].GetComponentsInChildren<UIPlantCardNC>();
	}

	public UIPlantCardNC GetCardInfo(PlantType plantType)
	{
		if (plantType == PlantType.Nope)
		{
			return null;
		}
		for (int i = 0; i < Pages.Count; i++)
		{
			UIPlantCardNC[] componentsInChildren = Pages[i].GetComponentsInChildren<UIPlantCardNC>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j].CardPlantType == plantType)
				{
					return componentsInChildren[j];
				}
			}
		}
		return null;
	}

	public UIPlantCardNC GetCardInfo(ZombieType zombieType)
	{
		if (zombieType == ZombieType.Nope)
		{
			return null;
		}
		for (int i = 0; i < Pages.Count; i++)
		{
			UIPlantCardNC[] componentsInChildren = Pages[i].GetComponentsInChildren<UIPlantCardNC>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j].CardZombieType == zombieType)
				{
					return componentsInChildren[j];
				}
			}
		}
		return null;
	}

	public UIPlantCardNC GetCardInfo(int cardId)
	{
		int num = 0;
		for (int i = 0; i < Pages.Count; i++)
		{
			UIPlantCardNC[] componentsInChildren = Pages[i].GetComponentsInChildren<UIPlantCardNC>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				num++;
				if (num == cardId)
				{
					return componentsInChildren[j];
				}
			}
		}
		return null;
	}

	public PlantType GetCardType(int cardId)
	{
		int num = 0;
		for (int i = 0; i < Pages.Count; i++)
		{
			UIPlantCardNC[] componentsInChildren = Pages[i].GetComponentsInChildren<UIPlantCardNC>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				num++;
				if (num == cardId)
				{
					return componentsInChildren[j].CardPlantType;
				}
			}
		}
		return PlantType.Nope;
	}

	public void ClearAllChoose()
	{
		for (int i = 0; i < Pages.Count; i++)
		{
			UIPlantCardNC[] componentsInChildren = Pages[i].GetComponentsInChildren<UIPlantCardNC>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].IsChoosed = false;
			}
		}
	}

	public void ResetPos()
	{
		CurrPage = 0;
		ClearAllChoose();
		base.transform.localPosition = new Vector3(-1500f, -60f, 0f);
	}

	public void StartMove()
	{
		if (GameManager.Instance.isClient)
		{
			if (SpectatorList.Instance.LocalIsSpectator)
			{
				isPrepare = true;
				startButton.StartText.text = "旁观中";
			}
			else
			{
				isPrepare = false;
				if (isPrepare)
				{
					startButton.StartText.text = "取消准备";
				}
				else
				{
					startButton.StartText.text = "准备";
				}
			}
		}
		else
		{
			if (SpectatorList.Instance.LocalIsSpectator)
			{
				isPrepare = true;
			}
			else
			{
				isPrepare = false;
			}
			startButton.StartText.text = "开始战斗";
		}
		StartCoroutine(DoMove(((Vector2)Camera.main.WorldToScreenPoint(SeedBank.Instance.transform.position + new Vector3(5f, 0f))).x));
	}

	public void StartRunLv(bool synClient = false)
	{
		if (!GameManager.Instance.isClient || synClient)
		{
			StartCoroutine(DoMove(-450f));
		}
	}

	public void Prepare()
	{
		isPrepare = !isPrepare;
		if (isPrepare)
		{
			startButton.StartText.text = "取消准备";
		}
		else
		{
			startButton.StartText.text = "准备";
		}
	}

	private IEnumerator DoMove(float targetPosX)
	{
		Vector3 vector = new Vector3(targetPosX, base.transform.position.y, base.transform.position.z);
		Vector2 dir = (vector - base.transform.position).normalized;
		if (targetPosX > base.transform.position.x)
		{
			while (targetPosX > base.transform.position.x)
			{
				yield return new WaitForSeconds(0.01f);
				base.transform.Translate(dir * 80f);
			}
		}
		else
		{
			while (targetPosX < base.transform.position.x)
			{
				yield return new WaitForSeconds(0.01f);
				base.transform.Translate(dir * 80f);
			}
		}
	}
}
