using SocketSave;
using UnityEngine;

public class Grid
{
	public GridType CurrGridType;

	public Vector2Int Point;

	public Vector2 Position;

	public int LightNum;

	private bool haveGraveStone;

	private GameObject GraveStone;

	private bool haveCrater;

	private GameObject Crater;

	private int iceRoadNum;

	public bool isWaterGrid;

	public bool isHardGrid;

	public bool isSlope;

	public bool isShadow;

	public bool isOccupied;

	public bool needChangeLine;

	public bool isHavePuddle;

	public bool isZombieSigned;

	private PlantBase currPlantBase;

	private PlantBase currFloatPlant;

	public PlantBase CurrPlantBase
	{
		get
		{
			return currPlantBase;
		}
		set
		{
			currPlantBase = value;
		}
	}

	public PlantBase CurrFloatPlant
	{
		get
		{
			return currFloatPlant;
		}
		set
		{
			currFloatPlant = value;
		}
	}

	public int IceRoadNum
	{
		get
		{
			return iceRoadNum;
		}
		set
		{
			iceRoadNum = value;
			if (iceRoadNum < 0)
			{
				iceRoadNum = 0;
			}
			if (iceRoadNum > 0 && CurrPlantBase != null)
			{
				CurrPlantBase.Hurt(99999f, null, isFlat: true);
			}
		}
	}

	public bool HaveGraveStone
	{
		get
		{
			return haveGraveStone;
		}
		set
		{
			if (GameManager.Instance.isServer)
			{
				GraveStoneSpawn graveStoneSpawn = new GraveStoneSpawn();
				graveStoneSpawn.MapPos = Position;
				graveStoneSpawn.isHave = value;
				SocketServer.Instance.SpawnGraveStone(graveStoneSpawn);
			}
			if (!haveGraveStone && value)
			{
				GraveStone = MapManager.Instance.GraveStoneUp(this);
				if (CurrPlantBase != null)
				{
					if (CurrPlantBase.ProtectPlant != null)
					{
						CurrPlantBase.ProtectPlant.Dead();
					}
					CurrPlantBase.Dead();
				}
			}
			if (haveGraveStone && !value)
			{
				Object.Destroy(GraveStone);
			}
			haveGraveStone = value;
		}
	}

	public bool HaveCrater
	{
		get
		{
			return haveCrater;
		}
		set
		{
			if (!haveCrater && value)
			{
				Crater = MapManager.Instance.CreateCrater(this);
				if (CurrPlantBase != null)
				{
					CurrPlantBase.Dead();
				}
			}
			if (haveCrater && !value)
			{
				Object.Destroy(Crater);
			}
			haveCrater = value;
		}
	}

	public Grid(Vector2Int point, Vector2 position)
	{
		Point = point;
		Position = position;
	}

	public void DesToryGrid()
	{
		if (HaveCrater)
		{
			Crater.GetComponent<Crater>().DestoryThis();
		}
		if (HaveGraveStone)
		{
			GraveStone.GetComponent<GraveStone>().DestoryThis();
		}
		if (CurrFloatPlant != null)
		{
			CurrFloatPlant.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
		}
		if (CurrPlantBase != null)
		{
			if (CurrPlantBase.ProtectPlant != null)
			{
				CurrPlantBase.ProtectPlant.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
			}
			CurrPlantBase.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
		}
	}
}
