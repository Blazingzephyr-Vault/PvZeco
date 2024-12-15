using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class BackYard : MapBase
{
	private List<Grid> gridList = new List<Grid>();

	public ParticleSystem PoolSpark;

	public override Vector2Int MapGridNum => new Vector2Int(9, 6);

	public override List<Grid> GridList => gridList;

	public override Vector2 MapHalfLengthWidth => new Vector2(14.3f, 6.2f);

	private List<Puddle> puddles => MapManager.Instance.puddles;

	public override void InitMap()
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				GridList.Add(new Grid(new Vector2Int(j, i), base.transform.position + new Vector3(-5.9f, 2.7f) + new Vector3(1.33f * (float)j, -1.5f * (float)i)));
			}
		}
		for (int k = 0; k < 2; k++)
		{
			for (int l = 0; l < 9; l++)
			{
				GridList.Add(new Grid(new Vector2Int(l, k + 2), base.transform.position + new Vector3(-5.9f, -0.45f) + new Vector3(1.33f * (float)l, -1.2f * (float)k)));
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
		}
		for (int m = 0; m < 2; m++)
		{
			for (int n = 0; n < 9; n++)
			{
				GridList.Add(new Grid(new Vector2Int(n, m + 4), base.transform.position + new Vector3(-5.9f, -3.4f) + new Vector3(1.33f * (float)n, -1.2f * (float)m)));
			}
		}
		TimeInitMap();
		TimeChange(SkyManager.Instance.Time);
		StartCoroutine(RenewRain());
		StartCoroutine(SpawnSkySun());
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
		PoolSpark.gameObject.SetActive(SkyManager.Instance.Time > 420 && SkyManager.Instance.Time < 1080 && SkyManager.Instance.RainScale < 6);
		ChangeSprite.color = new Color(1f, 1f, 1f, 1f);
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
				int num3 = Random.Range(0, 4);
				Vector3 position = Vector3.zero;
				switch (num3)
				{
				case 0:
					position = new Vector3(Random.Range(-8f, 7f) + base.transform.position.x, Random.Range(3f, 0.3f) + base.transform.position.y);
					break;
				case 1:
					position = new Vector3(Random.Range(-8f, 7f) + base.transform.position.x, Random.Range(-2.7f, -6f) + base.transform.position.y);
					break;
				case 2:
					position = new Vector3(Random.Range(9f, 14f) + base.transform.position.x, Random.Range(-5f, 2f) + base.transform.position.y);
					break;
				case 3:
				{
					position = new Vector3(Random.Range(-6.3f, 5f) + base.transform.position.x, Random.Range(-0.5f, -2.2f) + base.transform.position.y);
					GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_circle);
					obj.transform.SetParent(base.transform);
					obj.transform.position = position;
					break;
				}
				}
				if (num3 < 3)
				{
					GameObject obj2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_splash);
					obj2.transform.SetParent(base.transform);
					obj2.transform.position = position;
				}
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

	public override void TimeChange(int time)
	{
		LightOpenClose();
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
		if (SkyManager.Instance.Time > 420 && SkyManager.Instance.Time < 1080 && SkyManager.Instance.RainScale < 6)
		{
			PoolSpark.gameObject.SetActive(value: true);
		}
		if (time == 1080 || SkyManager.Instance.RainScale > 6)
		{
			PoolSpark.gameObject.SetActive(value: false);
		}
		for (int i = 0; i < MapManager.Instance.puddles.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(MapManager.Instance.puddles[i].transform.position) == this)
			{
				MapManager.Instance.puddles[i].spriteRenderer.color = new Color(puddleColor.r, puddleColor.g, puddleColor.b, MapManager.Instance.puddles[i].spriteRenderer.color.a);
			}
		}
		if (!LVManager.Instance.GameIsStart || GameManager.Instance.isClient)
		{
			return;
		}
		if (Random.Range(0, 10) >= 8)
		{
			int num4 = 0;
			List<Grid> list = new List<Grid>();
			for (int j = 0; j < gridList.Count; j++)
			{
				if (gridList[j].HaveGraveStone)
				{
					num4++;
				}
				if (gridList[j].Point.x >= GraveStoneLine && !gridList[j].HaveGraveStone && !gridList[j].isWaterGrid)
				{
					list.Add(gridList[j]);
				}
			}
			if (num4 < GraveStoneNum && list.Count > 0)
			{
				list[Random.Range(0, list.Count)].HaveGraveStone = true;
			}
		}
		if (SkyManager.Instance.RainScale == 0 && puddles.Count > 0 && Random.Range(0, 15) > 10)
		{
			int index = Random.Range(0, puddles.Count);
			if (MapManager.Instance.GetCurrMap(puddles[index].transform.position) == this)
			{
				puddles[index].StartDisappear();
			}
		}
		if (SkyManager.Instance.RainScale <= 2)
		{
			return;
		}
		int num5 = 80 - SkyManager.Instance.RainScale * 5;
		if (num5 < 10)
		{
			num5 = 10;
		}
		if (Random.Range(0, num5) <= num5 - 2)
		{
			return;
		}
		int num6 = Random.Range(0, gridList.Count);
		if (gridList[num6].isHavePuddle || gridList[num6].isWaterGrid || puddles.Count >= 8)
		{
			return;
		}
		bool flag = false;
		Vector2 vector = Vector2.zero;
		List<Grid> list2 = new List<Grid>();
		switch (Random.Range(0, 3))
		{
		case 0:
			if (num6 > 0 && num6 < gridList.Count - 1)
			{
				if ((!gridList[num6 - 1].isHavePuddle || gridList[num6 - 1].Point.y != gridList[num6].Point.y) && (!gridList[num6 + 1].isHavePuddle || gridList[num6 + 1].Point.y != gridList[num6].Point.y))
				{
					flag = true;
				}
			}
			else if (num6 == 0)
			{
				if (!gridList[1].isHavePuddle || gridList[1].Point.y != gridList[num6].Point.y)
				{
					flag = true;
				}
			}
			else if (num6 == gridList.Count - 1 && (!gridList[num6 - 1].isHavePuddle || gridList[num6 - 1].Point.y != gridList[num6].Point.y))
			{
				flag = true;
			}
			if (flag)
			{
				list2.Add(gridList[num6]);
				gridList[num6].isHavePuddle = true;
				vector = gridList[num6].Position;
			}
			break;
		case 1:
			if (num6 > 1 && num6 < gridList.Count - 1 && !gridList[num6 - 1].isHavePuddle && gridList[num6 - 1].Point.y == gridList[num6].Point.y && (!gridList[num6 - 2].isHavePuddle || gridList[num6 - 2].Point.y != gridList[num6].Point.y) && (!gridList[num6 + 1].isHavePuddle || gridList[num6 + 1].Point.y != gridList[num6].Point.y))
			{
				flag = true;
				list2.Add(gridList[num6]);
				list2.Add(gridList[num6 - 1]);
				gridList[num6].isHavePuddle = true;
				gridList[num6 - 1].isHavePuddle = true;
				vector = (gridList[num6 - 1].Position + gridList[num6].Position) * 0.5f;
			}
			break;
		case 2:
			if (num6 > 1 && num6 < gridList.Count - 2 && !gridList[num6 - 1].isHavePuddle && gridList[num6 - 1].Point.y == gridList[num6].Point.y && (!gridList[num6 - 2].isHavePuddle || gridList[num6 - 2].Point.y != gridList[num6].Point.y) && !gridList[num6 + 1].isHavePuddle && gridList[num6 + 1].Point.y == gridList[num6].Point.y && (!gridList[num6 + 2].isHavePuddle || gridList[num6 + 2].Point.y != gridList[num6].Point.y))
			{
				flag = true;
				list2.Add(gridList[num6]);
				list2.Add(gridList[num6 + 1]);
				list2.Add(gridList[num6 - 1]);
				gridList[num6 + 1].isHavePuddle = true;
				gridList[num6 - 1].isHavePuddle = true;
				gridList[num6].isHavePuddle = true;
				vector = gridList[num6].Position;
			}
			break;
		}
		if (!flag)
		{
			return;
		}
		PuddleSpawn puddleSpawn = new PuddleSpawn();
		if (GameManager.Instance.isServer)
		{
			puddleSpawn.OnlineId = SocketServer.Instance.ItemId;
			for (int k = 0; k < list2.Count; k++)
			{
				puddleSpawn.MapPos.Add(list2[k].Position);
			}
			puddleSpawn.InitPos = vector + new Vector2(0f, -0.3f);
			SocketServer.Instance.SpawnPuddle(puddleSpawn);
		}
		Puddle component = Object.Instantiate(GameManager.Instance.GameConf.Puddle).GetComponent<Puddle>();
		component.CreateInit(list2, vector + new Vector2(0f, -0.2f), puddleSpawn.OnlineId);
		MapManager.Instance.puddles.Add(component);
	}

	public override void SpawnMower()
	{
		for (int i = 0; i < MapGridNum.y; i++)
		{
			LawnMower lawnMower = null;
			Grid farestGrid = MapManager.Instance.GetFarestGrid(base.transform.position, getLeft: true, i);
			lawnMower = ((!farestGrid.isWaterGrid) ? PoolManager.Instance.GetObj(GameManager.Instance.GameConf.LawnMover).GetComponent<LawnMower>() : PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PoolCleaner).GetComponent<LawnMower>());
			lawnMower.Init(farestGrid, new Vector2(farestGrid.Position.x - 1.2f, farestGrid.Position.y - 0.2f));
			lawnMower.transform.SetParent(base.transform);
			lawnMower.OnlineID = i;
			mowers.Add(lawnMower);
		}
	}
}
