using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MapBase
{
	private List<Grid> gridList = new List<Grid>();

	public override Vector2Int MapGridNum => new Vector2Int(9, 5);

	public override List<Grid> GridList => gridList;

	public override Vector2 MapHalfLengthWidth => new Vector2(14.3f, 6.2f);

	public override float EndLine => -7.1f;

	public override void InitMap()
	{
		Vector3 vector = base.transform.position + new Vector3(-5.6f, 1.55f);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				GridList.Add(new Grid(new Vector2Int(j, i), vector + new Vector3(1.31f * (float)j, -1.36f * (float)i, 0f)));
				if (j < 5)
				{
					GridList[gridList.Count - 1].Position += new Vector2(0f, 0.33f * (float)(j - 1));
					GridList[gridList.Count - 1].isSlope = true;
				}
				else
				{
					GridList[gridList.Count - 1].Position += new Vector2(0f, 1.25f);
				}
				GridList[gridList.Count - 1].isHardGrid = true;
			}
		}
		TimeInitMap();
		TimeChange(SkyManager.Instance.Time);
		StartCoroutine(RenewRain());
		StartCoroutine(SpawnSkySun());
		StartCoroutine(PlacePot());
	}

	public override void TimeInitMap()
	{
		if (SkyManager.Instance.Time > 300 && SkyManager.Instance.Time < 900)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Night;
			ChangeSprite.sprite = Day;
		}
		else if (SkyManager.Instance.Time > 900 && SkyManager.Instance.Time < 1020)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Day;
			ChangeSprite.sprite = Sunset;
		}
		else
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Sunset;
			ChangeSprite.sprite = Night;
			puddleColor = new Color(0.7f, 0.7f, 1f);
		}
		ChangeSprite.color = new Color(1f, 1f, 1f, 1f);
	}

	private IEnumerator PlacePot()
	{
		yield return new WaitForSeconds(0.5f);
		if (GameManager.Instance.isClient)
		{
			yield break;
		}
		for (int i = 0; i < GridList.Count; i++)
		{
			if (GridList[i].Point.x == 0 || GridList[i].Point.x == 1 || GridList[i].Point.x == 2)
			{
				PlantBase newPlant = PlantManager.Instance.GetNewPlant(PlantType.Pot);
				newPlant.transform.SetParent(PlantManager.Instance.transform);
				SeedBank.Instance.PlantConfirm(newPlant, GridList[i], 0, 0, null);
			}
		}
	}

	public override void TimeChange(int time)
	{
		if (time == 300)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Night;
			ChangeSprite.sprite = Day;
			ChangeSprite.color = new Color(1f, 1f, 1f, 0f);
		}
		if (time >= 300 && time <= 420)
		{
			float num = (float)(time - 300) / 120f;
			ChangeSprite.color = new Color(1f, 1f, 1f, num);
			puddleColor = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
		}
		if (time == 900)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Day;
			ChangeSprite.sprite = Sunset;
			ChangeSprite.color = new Color(1f, 1f, 1f, 0f);
		}
		if (time >= 900 && time <= 1020)
		{
			float num2 = (float)(time - 900) / 120f;
			ChangeSprite.color = new Color(1f, 1f, 1f, num2);
			puddleColor = new Color(1f, 1f, 1f - 0.3f * num2);
		}
		if (time == 1020)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Sunset;
			ChangeSprite.sprite = Night;
			ChangeSprite.color = new Color(1f, 1f, 1f, 0f);
		}
		if (time >= 1020 && time <= 1140)
		{
			float num3 = (float)(time - 1020) / 120f;
			ChangeSprite.color = new Color(1f, 1f, 1f, num3);
			puddleColor = new Color(1f - 0.3f * num3, 1f - 0.3f * num3, 0.7f + 0.3f * num3);
		}
		for (int i = 0; i < MapManager.Instance.puddles.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(MapManager.Instance.puddles[i].transform.position) == this)
			{
				MapManager.Instance.puddles[i].spriteRenderer.color = new Color(puddleColor.r, puddleColor.g, puddleColor.b, MapManager.Instance.puddles[i].spriteRenderer.color.a);
			}
		}
		if (!LVManager.Instance.GameIsStart || GameManager.Instance.isClient || Random.Range(0, 10) < 8)
		{
			return;
		}
		int num4 = 0;
		List<Grid> list = new List<Grid>();
		for (int j = 0; j < gridList.Count; j++)
		{
			if (gridList[j].HaveGraveStone)
			{
				num4++;
			}
			if (gridList[j].Point.x >= GraveStoneLine && !gridList[j].HaveGraveStone)
			{
				list.Add(gridList[j]);
			}
		}
		if (num4 < GraveStoneNum && list.Count > 0)
		{
			list[Random.Range(0, list.Count)].HaveGraveStone = true;
		}
	}

	private IEnumerator RenewRain()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.3f);
			if (!(CameraControl.Instance.CurrMap == this) || SkyManager.Instance.RainScale <= 0)
			{
				continue;
			}
			int num = SkyManager.Instance.RainScale - 5;
			if (num < 0)
			{
				num = 0;
			}
			int num2 = Random.Range(num, SkyManager.Instance.RainScale);
			for (int i = 0; i < num2; i++)
			{
				GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_splash);
				obj.transform.SetParent(base.transform);
				Vector2 vector;
				if (Random.Range(0, 3) > 1)
				{
					vector = new Vector3(Random.Range(0.5f, 3.3f), Random.Range(5.2f, -3.4f)) + base.transform.position;
				}
				else if (Random.Range(0, 2) > 0)
				{
					float num3 = Random.Range(-6.5f, 5.25f);
					float maxInclusive = (num3 - 0.2f) * 0.31f + 3.2f;
					float minInclusive = num3 * 0.21f - 3.6f;
					vector = new Vector3(num3, Random.Range(minInclusive, maxInclusive)) + base.transform.position;
				}
				else
				{
					float num4 = Random.Range(5.7f, 12f);
					float maxInclusive2 = (0.2f - num4) * 0.3f + 3.2f;
					float minInclusive2 = num4 * 0.23f - 4.8f;
					vector = new Vector3(num4, Random.Range(minInclusive2, maxInclusive2)) + base.transform.position;
				}
				obj.transform.position = vector;
			}
		}
	}

	private IEnumerator SpawnSkySun()
	{
		while (true)
		{
			yield return new WaitForSeconds(16f);
			if (LVManager.Instance.GameIsStart && SkyManager.Instance.RainScale < 6)
			{
				if (SkyManager.Instance.GetIsDay() && SeedBank.Instance.NeedSummonSun)
				{
					float downY = Random.Range(-4.4f, 2.6f) + base.transform.position.y;
					float x = Random.Range(-6f, 4.5f);
					SkyManager.Instance.CreateSkySun(new Vector3(x, 7.2f + base.transform.position.y), downY);
				}
				else if (!SkyManager.Instance.GetIsDay() && SeedBank.Instance.NeedSummonMoon)
				{
					float downY2 = Random.Range(-4.4f, 2.6f) + base.transform.position.y;
					float x2 = Random.Range(-6f, 4.5f);
					SkyManager.Instance.CreateSkySun(new Vector3(x2, 7.2f + base.transform.position.y), downY2, isSun: false);
				}
			}
		}
	}

	public override List<Vector2> GetShowZombiePos(int PosNum)
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < PosNum; i++)
		{
			float num = Random.Range(7.5f, 11f);
			float maxInclusive = -0.3f * (num - 7.5f) + 2.3f;
			float minInclusive = -0.3f * (num - 7.5f) - 3.3f;
			list.Add(new Vector3(num, Random.Range(minInclusive, maxInclusive)) + base.transform.position);
		}
		list.Sort((Vector2 x, Vector2 y) => -x.y.CompareTo(y.y));
		return list;
	}

	public override void SpawnMower()
	{
		for (int i = 0; i < MapGridNum.y; i++)
		{
			Grid farestGrid = MapManager.Instance.GetFarestGrid(base.transform.position, getLeft: true, i);
			LawnMower component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.RoofCleaner).GetComponent<LawnMower>();
			component.Init(farestGrid, new Vector2(farestGrid.Position.x - 1.1f - (float)i * 0.08f, farestGrid.Position.y - 0.2f));
			component.transform.SetParent(base.transform);
			component.OnlineID = i;
			mowers.Add(component);
		}
	}
}
