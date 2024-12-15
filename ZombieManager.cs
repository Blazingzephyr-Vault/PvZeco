using System;
using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class ZombieManager : MonoBehaviour
{
	public static ZombieManager Instance;

	public List<ZombieBase> zombies = new List<ZombieBase>();

	public List<ZombieBase> hypnoZombies = new List<ZombieBase>();

	private int currOrderNum = 1;

	private UnityAction AllZombieDeadAction;

	private float CreateX = 8.2f;

	public bool ZombieInvincible;

	public int CurrOrderNum
	{
		get
		{
			return currOrderNum;
		}
		set
		{
			currOrderNum = value;
			if (value > 90)
			{
				currOrderNum = 1;
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Yell();
	}

	public void GameOverPause()
	{
		for (int i = 0; i < zombies.Count; i++)
		{
			zombies[i].GameOverFakeDeath();
		}
		for (int j = 0; j < hypnoZombies.Count; j++)
		{
			hypnoZombies[j].GameOverFakeDeath();
		}
	}

	public void ClearAllZombie()
	{
		AllZombieDeadAction = null;
		while (zombies.Count > 0)
		{
			zombies[0].DirectDead(canDropItem: false, 0f, synClient: true);
		}
		while (hypnoZombies.Count > 0)
		{
			hypnoZombies[0].DirectDead(canDropItem: false, 0f, synClient: true);
		}
		zombies.Clear();
		hypnoZombies.Clear();
	}

	public int BigHurtAllZombie()
	{
		List<ZombieBase> list = new List<ZombieBase>(zombies);
		int count = zombies.Count;
		for (int i = 0; i < list.Count; i++)
		{
			zombies[i].Hurt(1000000, Vector2.zero, isHard: false, HitSound: false);
		}
		return count;
	}

	public int BigHurtMapZombie(MapBase map)
	{
		List<ZombieBase> list = new List<ZombieBase>(zombies);
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(zombies[i].transform.position) == map)
			{
				num++;
				zombies[i].Hurt(1000000, Vector2.zero, isHard: false, HitSound: false);
			}
		}
		return num;
	}

	public void ShowZombie()
	{
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			List<Vector2> showZombiePos = MapManager.Instance.mapList[i].GetShowZombiePos(LV.Instance.ZombieTypes[i].Count);
			for (int j = 0; j < LV.Instance.ZombieTypes[i].Count; j++)
			{
				int updateLine;
				ZombieBase summonZombie = GetSummonZombie(LV.Instance.ZombieTypes[i][j], showZombiePos[j], needSpawnLimit: false, out updateLine);
				if (summonZombie != null)
				{
					AddZombie(summonZombie);
					summonZombie.transform.SetParent(base.transform);
					summonZombie.Init(0, j, showZombiePos[j], new ZombieSpawn());
				}
			}
		}
		ZombieStartIdel();
	}

	public void SummonZombie(int id1, Grid grid)
	{
	}

	public void SummonZombie(int id1, int lineNum, Vector2 mapPos)
	{
		GetPosByGridVertical(mapPos, lineNum, CreateX);
	}

	public bool UpdateZombie(ZombieType Type, Vector3 mapPos, int updateLine = -1)
	{
		int num = ((updateLine == -1) ? UnityEngine.Random.Range(0, MapManager.Instance.GetCurrMap(mapPos).MapGridNum.y) : updateLine);
		ZombieBase summonZombie = GetSummonZombie(Type, mapPos, needSpawnLimit: true, out updateLine);
		if (updateLine != -1)
		{
			num = updateLine;
		}
		if (summonZombie != null)
		{
			Grid posByGridVertical = GetPosByGridVertical(mapPos, num, CreateX);
			if (posByGridVertical == null)
			{
				return false;
			}
			UpdateZombie(Type, summonZombie, new Vector2(CreateX, posByGridVertical.Position.y), num);
		}
		return summonZombie != null;
	}

	public bool UpdateZombie(ZombieType Type, ZombieBase zombie, Vector2 pos, int UpdateLine)
	{
		ZombieSpawn zombieSpawn = new ZombieSpawn();
		if (zombie != null)
		{
			zombieSpawn.SpawnPos = pos;
			AddZombie(zombie);
			zombie.transform.SetParent(base.transform);
			zombie.Init(UpdateLine, CurrOrderNum, pos, zombieSpawn);
			CurrOrderNum++;
		}
		if (zombie != null && GameManager.Instance.isServer)
		{
			if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(zombie.PlacePlayer))
			{
				zombieSpawn.SpawnPos = new Vector2(0f - zombieSpawn.SpawnPos.x, zombieSpawn.SpawnPos.y);
			}
			for (int i = 0; i < zombie.OnlineIdNum; i++)
			{
				zombieSpawn.OnlineId = SocketServer.Instance.ItemId;
			}
			zombie.OnlineId = zombieSpawn.OnlineId;
			zombieSpawn.PlacePlayer = zombie.PlacePlayer;
			zombieSpawn.Type = Type;
			zombieSpawn.UpdateLine = UpdateLine;
			SocketServer.Instance.SpawnZombie(zombieSpawn);
		}
		return zombie != null;
	}

	public bool UpdateZombie(ZombieSpawn spawnInfo)
	{
		int updateLine;
		ZombieBase summonZombie = GetSummonZombie(spawnInfo.Type, spawnInfo.SpawnPos, needSpawnLimit: false, out updateLine);
		if (summonZombie != null)
		{
			AddZombie(summonZombie);
			summonZombie.transform.SetParent(base.transform);
			summonZombie.OnlineId = spawnInfo.OnlineId;
			summonZombie.PlacePlayer = spawnInfo.PlacePlayer;
			if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(spawnInfo.PlacePlayer))
			{
				spawnInfo.SpawnPos = new Vector2(0f - spawnInfo.SpawnPos.x, spawnInfo.SpawnPos.y);
			}
			if (GetPosByGridVertical(spawnInfo.SpawnPos, spawnInfo.UpdateLine, CreateX) == null)
			{
				return false;
			}
			summonZombie.Init(spawnInfo.UpdateLine, CurrOrderNum, spawnInfo.SpawnPos, spawnInfo);
			CurrOrderNum++;
			if (summonZombie.PlacePlayer == null || summonZombie.PlacePlayer == "")
			{
				FlagMeter.Instance.UpdateHead(GetZombieWeight(spawnInfo.Type));
			}
			if (LV.Instance.CurrLVType == LVType.PvP && PvPSelector.Instance.IsSameTeam(spawnInfo.PlacePlayer))
			{
				summonZombie.RatThis(synClient: true);
			}
		}
		return summonZombie != null;
	}

	public GameObject CreateOneZombie(GameObject prefab, int lineNum, Vector2 vector)
	{
		if (prefab == null)
		{
			return null;
		}
		if (MapManager.Instance.GetCurrMap(vector).MapGridNum.y <= lineNum || lineNum < 0)
		{
			return null;
		}
		ZombieBase component = PoolManager.Instance.GetObj(prefab).GetComponent<ZombieBase>();
		AddZombie(component);
		component.transform.SetParent(base.transform);
		ZombieSpawn spawnInfo = new ZombieSpawn();
		component.Init(lineNum, CurrOrderNum, vector, spawnInfo);
		CurrOrderNum++;
		return component.gameObject;
	}

	public ZombieBase GetNewZombie(ZombieType Type, bool removeList = false)
	{
		int updateLine;
		ZombieBase summonZombie = GetSummonZombie(Type, default(Vector2), needSpawnLimit: false, out updateLine);
		if (removeList)
		{
			RemoveZombie(summonZombie);
		}
		return summonZombie;
	}

	private ZombieBase GetSummonZombie(ZombieType Type, Vector2 mapPos, bool needSpawnLimit, out int updateLine)
	{
		updateLine = -1;
		ZombieBase zombieBase = null;
		switch (Type)
		{
		case ZombieType.PvPTarget:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PvPTarget).GetComponent<ZombieBase>();
			break;
		case ZombieType.NormalZombie:
		{
			Zombie component20 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component20.Type = NormalZombieType.Normal;
			zombieBase = component20;
			break;
		}
		case ZombieType.ImpZpmbie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ImpZombie).GetComponent<ImpZombie>();
			break;
		case ZombieType.SwampNormal:
		{
			SwampZombie component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component2.Type = SwampZombieType.Normal;
			zombieBase = component2;
			break;
		}
		case ZombieType.ConeZombie:
		{
			Zombie component22 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component22.Type = NormalZombieType.Cone;
			zombieBase = component22;
			break;
		}
		case ZombieType.Polevaulter:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Polevaulter).GetComponent<Polevaulter>();
			break;
		case ZombieType.PaperZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PaperZombie).GetComponent<PaperZombie>();
			break;
		case ZombieType.StoolZombie:
		{
			SwampZombie component21 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component21.Type = SwampZombieType.Stool;
			zombieBase = component21;
			break;
		}
		case ZombieType.DiggerZombie:
		{
			int noWaterGrid7 = MapManager.Instance.GetNoWaterGrid(mapPos);
			if (noWaterGrid7 != -1 || !needSpawnLimit)
			{
				DiggerZombie component23 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.DiggerZombie).GetComponent<DiggerZombie>();
				updateLine = noWaterGrid7;
				zombieBase = component23;
			}
			break;
		}
		case ZombieType.BalloonZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BalloonZombie).GetComponent<BalloonZombie>();
			break;
		case ZombieType.SwampDoor:
		{
			SwampZombie component17 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component17.Type = SwampZombieType.Door;
			zombieBase = component17;
			break;
		}
		case ZombieType.Ghost:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GhostZombie).GetComponent<Ghost>();
			break;
		case ZombieType.BucketZombie:
		{
			Zombie component14 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component14.Type = NormalZombieType.Bucket;
			zombieBase = component14;
			break;
		}
		case ZombieType.DoorZombie:
		{
			Zombie component15 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component15.Type = NormalZombieType.Door;
			zombieBase = component15;
			break;
		}
		case ZombieType.Yeti:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Yeti).GetComponent<Yeti>();
			break;
		case ZombieType.JackboxZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.JackboxZombie).GetComponent<JackboxZombie>();
			break;
		case ZombieType.PogoZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PogoZombie).GetComponent<PogoZombie>();
			break;
		case ZombieType.DoorAndCone:
		{
			Zombie component7 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component7.Type = NormalZombieType.DoorAndCone;
			zombieBase = component7;
			break;
		}
		case ZombieType.SwampBucket:
		{
			SwampZombie component4 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component4.Type = SwampZombieType.Bucket;
			zombieBase = component4;
			break;
		}
		case ZombieType.FootballZombie:
		{
			FootballZombie component5 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie_Football).GetComponent<FootballZombie>();
			component5.Type = FootballZombieType.Normal;
			zombieBase = component5;
			break;
		}
		case ZombieType.Zomboni:
		{
			int noWaterGrid8 = MapManager.Instance.GetNoWaterGrid(mapPos);
			if (noWaterGrid8 != -1 || !needSpawnLimit)
			{
				zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZamboniZombie).GetComponent<Zamboni>();
				updateLine = noWaterGrid8;
			}
			break;
		}
		case ZombieType.BungiZombie:
			if (needSpawnLimit)
			{
				Grid havePlantGrid = MapManager.Instance.GetHavePlantGrid(mapPos);
				if (havePlantGrid != null && !havePlantGrid.isZombieSigned && havePlantGrid.CurrPlantBase != null && havePlantGrid.CurrPlantBase.GetPlantType() != PlantType.CobCannon && (!(havePlantGrid.CurrPlantBase.CarryPlant != null) || havePlantGrid.CurrPlantBase.CarryPlant.GetPlantType() != PlantType.CobCannon))
				{
					BungiZombie component18 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BungiZombie).GetComponent<BungiZombie>();
					component18.SpawnGrid = havePlantGrid;
					zombieBase = component18;
				}
			}
			else
			{
				BungiZombie component19 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BungiZombie).GetComponent<BungiZombie>();
				updateLine = 0;
				zombieBase = component19;
			}
			break;
		case ZombieType.GargantuarInjured:
		{
			int noWaterGrid6 = MapManager.Instance.GetNoWaterGrid(mapPos);
			if (noWaterGrid6 != -1 || !needSpawnLimit)
			{
				Gargantuar component16 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
				component16.Type = GargantuarType.Injured;
				zombieBase = component16;
				updateLine = noWaterGrid6;
			}
			break;
		}
		case ZombieType.CatapultZombie:
		{
			int noWaterGrid5 = MapManager.Instance.GetNoWaterGrid(mapPos);
			if (noWaterGrid5 != -1 || !needSpawnLimit)
			{
				CatapultZombie component13 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CatapultZombie).GetComponent<CatapultZombie>();
				component13.Type = CatapultZombieType.Normal;
				zombieBase = component13;
				updateLine = noWaterGrid5;
			}
			break;
		}
		case ZombieType.DoorAndBucket:
		{
			Zombie component12 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component12.Type = NormalZombieType.DoorAndBucket;
			zombieBase = component12;
			break;
		}
		case ZombieType.SwampDoorAndStool:
		{
			SwampZombie component11 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component11.Type = SwampZombieType.DoorAndStool;
			zombieBase = component11;
			break;
		}
		case ZombieType.SwampDoorAndBucket:
		{
			SwampZombie component10 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component10.Type = SwampZombieType.DoorAndBucket;
			zombieBase = component10;
			break;
		}
		case ZombieType.BlackFootball:
		{
			FootballZombie component9 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie_BlackFootball).GetComponent<FootballZombie>();
			component9.Type = FootballZombieType.Black;
			zombieBase = component9;
			break;
		}
		case ZombieType.Gargantuar:
		{
			int noWaterGrid4 = MapManager.Instance.GetNoWaterGrid(mapPos);
			if (noWaterGrid4 != -1 || !needSpawnLimit)
			{
				Gargantuar component8 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
				component8.Type = GargantuarType.Normal;
				zombieBase = component8;
				updateLine = noWaterGrid4;
			}
			break;
		}
		case ZombieType.GargantuarRedeye:
		{
			int noWaterGrid3 = MapManager.Instance.GetNoWaterGrid(mapPos);
			if (noWaterGrid3 != -1 || !needSpawnLimit)
			{
				Gargantuar component6 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
				component6.Type = GargantuarType.Redeye;
				zombieBase = component6;
				updateLine = noWaterGrid3;
			}
			break;
		}
		case ZombieType.GargantuarHelmet:
		{
			int noWaterGrid2 = MapManager.Instance.GetNoWaterGrid(mapPos);
			if (noWaterGrid2 != -1 || !needSpawnLimit)
			{
				Gargantuar component3 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
				component3.Type = GargantuarType.Helmet;
				zombieBase = component3;
				updateLine = noWaterGrid2;
			}
			break;
		}
		case ZombieType.GargantuarHelmetRedeye:
		{
			int noWaterGrid = MapManager.Instance.GetNoWaterGrid(mapPos);
			if (noWaterGrid != -1 || !needSpawnLimit)
			{
				Gargantuar component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
				component.Type = GargantuarType.RedeyeAndHelmet;
				zombieBase = component;
				updateLine = noWaterGrid;
			}
			break;
		}
		default:
			Debug.Log("无该类型僵尸:" + Type);
			break;
		}
		if (zombieBase != null)
		{
			zombieBase.SummonInit();
			zombieBase.PlacePlayer = null;
		}
		return zombieBase;
	}

	public int GetZombieWeight(ZombieType type)
	{
		int num = -1;
		return type switch
		{
			ZombieType.NormalZombie => 1, 
			ZombieType.ConeZombie => 2, 
			ZombieType.BucketZombie => 4, 
			ZombieType.DoorZombie => 4, 
			ZombieType.DoorAndCone => 5, 
			ZombieType.DoorAndBucket => 6, 
			ZombieType.Polevaulter => 2, 
			ZombieType.PaperZombie => 2, 
			ZombieType.FootballZombie => 5, 
			ZombieType.BlackFootball => 8, 
			ZombieType.Zomboni => 5, 
			ZombieType.JackboxZombie => 4, 
			ZombieType.BalloonZombie => 3, 
			ZombieType.DiggerZombie => 3, 
			ZombieType.PogoZombie => 4, 
			ZombieType.BungiZombie => 4, 
			ZombieType.CatapultZombie => 6, 
			ZombieType.Gargantuar => 9, 
			ZombieType.GargantuarHelmet => 14, 
			ZombieType.GargantuarRedeye => 12, 
			ZombieType.GargantuarHelmetRedeye => 19, 
			ZombieType.GargantuarInjured => 5, 
			ZombieType.ImpZpmbie => 4, 
			ZombieType.SwampNormal => 1, 
			ZombieType.StoolZombie => 2, 
			ZombieType.SwampBucket => 4, 
			ZombieType.SwampDoor => 3, 
			ZombieType.SwampDoorAndStool => 4, 
			ZombieType.SwampDoorAndBucket => 5, 
			ZombieType.Ghost => 3, 
			_ => -1, 
		};
	}

	private Grid GetPosByGridVertical(Vector3 pos, int verticalNum, float x)
	{
		return MapManager.Instance.GetGridByVertical(pos, verticalNum);
	}

	private void AddZombie(ZombieBase zombie)
	{
		if (!zombies.Contains(zombie))
		{
			zombies.Add(zombie);
		}
	}

	public void RemoveZombie(ZombieBase zombie)
	{
		if (zombie.isHypno)
		{
			hypnoZombies.Remove(zombie);
		}
		else if (zombies.Remove(zombie))
		{
			CheckAllZombieDeadForLV(zombie);
		}
	}

	public void ZombieHypno(ZombieBase zombie)
	{
		if (zombies.Remove(zombie))
		{
			hypnoZombies.Add(zombie);
			CheckAllZombieDeadForLV(zombie);
		}
		else if (hypnoZombies.Remove(zombie))
		{
			zombies.Add(zombie);
		}
	}

	public ZombieBase GetLastZombieByLine(int lineNum, Vector3 pos, bool getRight = false, bool getHyp = false)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		ZombieBase result = null;
		float num = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
			if (currMap != currMap2)
			{
				continue;
			}
			if (getRight)
			{
				if (list[i].CurrGrid.Point.y == lineNum && list[i].capsuleCollider2D.enabled && list[i].transform.position.x >= pos.x && Vector2.Distance(pos, list[i].transform.position) >= num && list[i].Hp > 0)
				{
					num = Vector2.Distance(pos, list[i].transform.position);
					result = list[i];
				}
			}
			else if (list[i].CurrGrid.Point.y == lineNum && list[i].capsuleCollider2D.enabled && list[i].transform.position.x <= pos.x && Vector2.Distance(pos, list[i].transform.position) >= num && list[i].Hp > 0)
			{
				num = Vector2.Distance(pos, list[i].transform.position);
				result = list[i];
			}
		}
		return result;
	}

	public ZombieBase GetZombieByLineMinDistance(int lineNum, Vector3 pos, bool getLeft, bool getHyp, bool needCapsule = true)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		ZombieBase result = null;
		float num = 10000f;
		for (int i = 0; i < list.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
			if (currMap != currMap2 || (needCapsule && !list[i].capsuleCollider2D.enabled))
			{
				continue;
			}
			if (getLeft)
			{
				if (list[i].transform.position.x > pos.x)
				{
					continue;
				}
			}
			else if (list[i].transform.position.x < pos.x)
			{
				continue;
			}
			if (list[i].CurrGrid.Point.y == lineNum && Vector2.Distance(pos, list[i].transform.position) < num && list[i].Hp > 0)
			{
				num = Vector2.Distance(pos, list[i].transform.position);
				result = list[i];
			}
		}
		return result;
	}

	public ZombieBase GetZombieByLineMinDisNoDir(int lineNum, Vector3 pos, bool getHyp, bool needCapsule = true)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		ZombieBase result = null;
		float num = 10000f;
		for (int i = 0; i < list.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
			if (!(currMap != currMap2) && (!needCapsule || list[i].capsuleCollider2D.enabled) && list[i].CurrGrid.Point.y == lineNum && Vector2.Distance(pos, list[i].transform.position) < num && list[i].Hp > 0)
			{
				num = Vector2.Distance(pos, list[i].transform.position);
				result = list[i];
			}
		}
		return result;
	}

	public List<ZombieBase> GetZombiesByLine(int lineNum, Vector3 pos, bool getLeft, bool getHyp, bool needCapsule = true)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		List<ZombieBase> list2 = new List<ZombieBase>();
		for (int i = 0; i < list.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
			if (currMap != currMap2 || (needCapsule && !list[i].capsuleCollider2D.enabled))
			{
				continue;
			}
			if (getLeft)
			{
				if (list[i].transform.position.x > pos.x)
				{
					continue;
				}
			}
			else if (list[i].transform.position.x < pos.x)
			{
				continue;
			}
			if (list[i].CurrGrid.Point.y == lineNum && list[i].Hp > 0)
			{
				list2.Add(list[i]);
			}
		}
		return list2;
	}

	public List<ZombieBase> GetZombies(int lineNum, Vector2 targetPos, float dis, bool getHyp, bool needCapsule)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(targetPos);
		List<ZombieBase> list2 = new List<ZombieBase>();
		for (int i = 0; i < list.Count; i++)
		{
			if (!needCapsule || list[i].capsuleCollider2D.enabled)
			{
				MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
				if (currMap == currMap2 && list[i].CurrGrid.Point.y == lineNum && Mathf.Abs(targetPos.x - list[i].transform.position.x) < dis)
				{
					list2.Add(list[i]);
				}
			}
		}
		return list2;
	}

	public List<ZombieBase> GetZombies(Vector2 targetPos, float dis, bool needCapsule = false, bool getHyp = false)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(targetPos);
		List<ZombieBase> list2 = new List<ZombieBase>();
		for (int i = 0; i < list.Count; i++)
		{
			if (!needCapsule || list[i].capsuleCollider2D.enabled)
			{
				MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
				if (currMap == currMap2 && Vector2.Distance(targetPos, list[i].transform.position) < dis)
				{
					list2.Add(list[i]);
				}
			}
		}
		return list2;
	}

	public List<ZombieBase> GetAllZombies()
	{
		return zombies;
	}

	public List<ZombieBase> GetAllHypZombies()
	{
		return hypnoZombies;
	}

	public List<ZombieBase> GetAllZombies(Vector2 pos, bool getHyp, bool needCapsule = false)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		List<ZombieBase> list2 = new List<ZombieBase>();
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		for (int i = 0; i < list.Count; i++)
		{
			if ((!needCapsule || list[i].capsuleCollider2D.enabled) && MapManager.Instance.GetCurrMap(list[i].transform.position) == currMap && list[i].Hp > 0)
			{
				list2.Add(list[i]);
			}
		}
		return list2;
	}

	public void ZombieStartIdel()
	{
		if (zombies.Count != 0)
		{
			for (int i = 0; i < zombies.Count; i++)
			{
				zombies[i].StartIdel();
			}
		}
	}

	private void CheckAllZombieDeadForLV(ZombieBase zombie)
	{
		if (!LVManager.Instance.InGame)
		{
			return;
		}
		if (zombies.Count == 0)
		{
			if (AllZombieDeadAction != null)
			{
				AllZombieDeadAction();
			}
			if (LVManager.Instance.isOver)
			{
				MoneyBag component = UnityEngine.Object.Instantiate(GameManager.Instance.GameConf.Moneybag).GetComponent<MoneyBag>();
				component.transform.position = zombie.transform.position;
				Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(component.transform.position);
				if (Vector2.Distance(component.transform.position, gridByWorldPos.Position) > 0.8f)
				{
					component.transform.position = gridByWorldPos.Position;
				}
				component.transform.SetParent(MapManager.Instance.GetCurrMap(component.transform.position).transform);
			}
		}
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			MapManager.Instance.mapList[i].ZombieDeadEvent(zombie);
		}
	}

	public void AddAllZombieDeadAction(UnityAction action)
	{
		AllZombieDeadAction = (UnityAction)Delegate.Combine(AllZombieDeadAction, action);
	}

	public void RemoveAllZombieDeadAction(UnityAction action)
	{
		AllZombieDeadAction = (UnityAction)Delegate.Remove(AllZombieDeadAction, action);
	}

	public void DropCoin(Vector2 pos, bool AllDrop = false)
	{
		int minInclusive = 1;
		if (AllDrop)
		{
			minInclusive = 83;
		}
		int num = UnityEngine.Random.Range(minInclusive, 100);
		if (num > 98)
		{
			InstantiateItem(GameManager.Instance.GameConf.Diamond, pos);
		}
		else if (num > 92)
		{
			InstantiateItem(GameManager.Instance.GameConf.Goldcoin, pos);
		}
		else if (num > 82)
		{
			InstantiateItem(GameManager.Instance.GameConf.Silvercoin, pos);
		}
	}

	private void InstantiateItem(GameObject prefab, Vector2 pos)
	{
		Allcoin component = PoolManager.Instance.GetObj(prefab).GetComponent<Allcoin>();
		component.transform.SetParent(null);
		component.InitForItem(pos);
	}

	private void Yell()
	{
	}

	private IEnumerator DoYell()
	{
		while (true)
		{
			if (zombies.Count > 0 && UnityEngine.Random.Range(0, 10) > 8)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieYell, base.transform.position, isAll: true);
			}
			yield return new WaitForSeconds(5f);
		}
	}
}
