using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iceroad : MonoBehaviour
{
	public int CurrLine;

	private Transform Mask;

	private Transform Icecap;

	private float lastX = float.MaxValue;

	private float lastX2 = float.MinValue;

	private Grid LastGrid;

	private List<Grid> barrierGrid = new List<Grid>();

	public void CreateInit(Vector3 pos, int line, bool isFacingLeft)
	{
		CurrLine = line;
		Mask = base.transform.Find("Mask");
		Icecap = base.transform.Find("ice_cap");
		Icecap.localPosition = new Vector3(5.3f, Icecap.localPosition.y);
		Mask.localScale = new Vector3(0f, Mask.localScale.y);
		if (isFacingLeft)
		{
			base.transform.position = new Vector3(pos.x - 6.6f, pos.y - 0.3f);
		}
		else
		{
			base.transform.position = new Vector3(pos.x + 6.6f, pos.y - 0.3f);
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
		MapManager.Instance.iceroads.Add(this);
	}

	public void startDisappear()
	{
		StartCoroutine(Disappear());
	}

	private IEnumerator Disappear()
	{
		yield return new WaitForSeconds(30f);
		Dead();
	}

	public void Dead()
	{
		for (int i = 0; i < barrierGrid.Count; i++)
		{
			barrierGrid[i].IceRoadNum--;
		}
		MapManager.Instance.iceroads.Remove(this);
		Object.Destroy(base.gameObject);
	}

	public void ChangeLong(float x)
	{
		float num = 0f;
		if (base.transform.localScale.x > 0f)
		{
			if (x > lastX)
			{
				return;
			}
			lastX = x;
			if (x - base.transform.position.x != 6.6f)
			{
				num = Mathf.Abs(x - base.transform.position.x - 6.6f) / 15.8f;
			}
			Icecap.localPosition = new Vector3(5.3f - 10.6f * num, Icecap.localPosition.y);
		}
		else
		{
			if (x < lastX2)
			{
				return;
			}
			lastX2 = x;
			if (x - base.transform.position.x != 6.6f)
			{
				num = Mathf.Abs(base.transform.position.x - x - 6.6f) / 15.8f;
			}
			Icecap.localPosition = new Vector3(5.3f - 10.6f * num, Icecap.localPosition.y);
		}
		Mask.localScale = new Vector3(num, Mask.localScale.y);
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(Icecap.transform.position, CurrLine);
		if (gridByWorldPos != LastGrid)
		{
			LastGrid = gridByWorldPos;
			if (!barrierGrid.Contains(gridByWorldPos))
			{
				barrierGrid.Add(gridByWorldPos);
				gridByWorldPos.IceRoadNum++;
			}
		}
	}
}
