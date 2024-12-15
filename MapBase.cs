using System.Collections.Generic;
using UnityEngine;

public abstract class MapBase : MonoBehaviour
{
	public Fog fog;

	public SpriteRenderer ChangeSprite;

	public Sprite Day;

	public Sprite Sunset;

	public Sprite Night;

	public Sprite GotoSprite;

	public int GraveStoneNum;

	public int GraveStoneLine;

	public Color puddleColor = new Color(1f, 1f, 1f);

	public List<GameObject> MapLights = new List<GameObject>();

	protected List<LawnMower> mowers = new List<LawnMower>();

	public abstract Vector2Int MapGridNum { get; }

	public abstract Vector2 MapHalfLengthWidth { get; }

	public abstract List<Grid> GridList { get; }

	public virtual float EndLine { get; } = -7.6f;

	public abstract void InitMap();

	public abstract void TimeInitMap();

	public abstract void TimeChange(int time);

	public virtual void ZombieDeadEvent(ZombieBase zombie)
	{
	}

	public virtual void SpawnMower()
	{
	}

	public virtual List<Vector2> GetShowZombiePos(int PosNum)
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < PosNum; i++)
		{
			list.Add(new Vector3(Random.Range(8.5f, 12.8f), Random.Range(-5f, 3.2f)) + base.transform.position);
		}
		list.Sort((Vector2 x, Vector2 y) => -x.y.CompareTo(y.y));
		return list;
	}

	public void LightOpenClose()
	{
		if (MapLights.Count == 0)
		{
			return;
		}
		if (!GetIsDay() || GobalLight.Instance.gobalLight.intensity < 0.5f)
		{
			if (!MapLights[0].activeSelf)
			{
				for (int i = 0; i < MapLights.Count; i++)
				{
					MapLights[i].SetActive(value: true);
				}
			}
		}
		else if (MapLights[0].activeSelf)
		{
			for (int j = 0; j < MapLights.Count; j++)
			{
				MapLights[j].SetActive(value: false);
			}
		}
	}

	public bool GetIsDay()
	{
		if (SkyManager.Instance.Time < 1140 && SkyManager.Instance.Time > 340)
		{
			return true;
		}
		return false;
	}

	public void SynMower(int OnlineId)
	{
		for (int i = 0; i < mowers.Count; i++)
		{
			if (mowers[i].OnlineID == OnlineId)
			{
				mowers[i].Launch(synClient: true);
				break;
			}
		}
	}
}
