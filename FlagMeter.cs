using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagMeter : MonoBehaviour
{
	public static FlagMeter Instance;

	private Image Head;

	private Image MaskImg;

	private int Allweight;

	public List<LVFlag> FlagList = new List<LVFlag>();

	public RectTransform ForSizeFlag;

	public RectTransform ForSizeFlag2;

	public Text name1;

	public Text name2;

	public Text name3;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Head = base.transform.Find("Head").GetComponent<Image>();
		MaskImg = base.transform.Find("FullMeter").GetComponent<Image>();
		Head.transform.localPosition = new Vector3(183f, 0f, 0f);
		MaskImg.fillAmount = 0f;
	}

	public void SetLvlName(string name)
	{
		name1.text = name;
		name2.text = name;
		name3.text = name;
	}

	private void ResetFlagMeter()
	{
		Head.transform.localPosition = new Vector3(183f, 0f, 0f);
		MaskImg.fillAmount = 0f;
		for (int i = 0; i < FlagList.Count; i++)
		{
			FlagList[i].Destroy();
		}
		FlagList.Clear();
	}

	public void UpdateHead(int weight)
	{
		if (!LVManager.Instance.isBigWave)
		{
			float num = (Mathf.Abs(ForSizeFlag.transform.position.x - ForSizeFlag2.transform.position.x) - 10f) / (float)Allweight;
			Head.transform.position += new Vector3((0f - num) * (float)weight, 0f, 0f);
			MaskImg.fillAmount += 1f / (float)Allweight * (float)weight;
		}
	}

	public void CreateFlag(int allWeight)
	{
		Allweight = allWeight;
		if (Allweight == 0)
		{
			base.transform.localScale = Vector3.zero;
		}
		else
		{
			base.transform.localScale = Vector3.one;
		}
		if (Allweight == 0)
		{
			return;
		}
		ResetFlagMeter();
		float num = Mathf.Abs(ForSizeFlag.transform.position.x - ForSizeFlag2.transform.position.x) / (float)Allweight;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < LV.Instance.Weights[0].Count; i++)
		{
			if (i == LV.Instance.BigWaveNum[num2])
			{
				LVFlag component = Object.Instantiate(GameManager.Instance.GameConf.LVFlag).GetComponent<LVFlag>();
				FlagList.Add(component);
				component.GetComponent<RectTransform>().sizeDelta = new Vector2(ForSizeFlag.rect.width, ForSizeFlag.rect.height);
				component.transform.SetParent(base.transform);
				component.transform.position = new Vector3(ForSizeFlag2.transform.position.x - num * (float)num3, base.transform.position.y, 0f);
				component.transform.SetAsLastSibling();
				num2++;
			}
			else
			{
				for (int j = 0; j < LV.Instance.Weights.Count; j++)
				{
					num3 += LV.Instance.Weights[j][i];
				}
			}
		}
		Head.transform.SetSiblingIndex(Head.transform.parent.childCount - 1);
		for (int k = 0; k < FlagList.Count; k++)
		{
			FlagList[k].GetComponent<RectTransform>().localScale = Vector3.one;
		}
	}
}
