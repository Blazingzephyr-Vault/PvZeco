using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	[SerializeField]
	public static MapManager Instance;

	private List<Vector2> pointList = new List<Vector2>();

	public List<MapBase> mapList = new List<MapBase>();

	public List<Iceroad> iceroads = new List<Iceroad>();

	public List<Puddle> puddles = new List<Puddle>();

	private void Awake()
	{
		Instance = this;
	}

	public void ResetScence()
	{
		List<Iceroad> list = new List<Iceroad>(iceroads);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Dead();
		}
		iceroads.Clear();
		for (int j = 0; j < puddles.Count; j++)
		{
			puddles[j].Destroy();
		}
		puddles.Clear();
		for (int k = 0; k < mapList.Count; k++)
		{
			List<Grid> gridList = mapList[k].GridList;
			for (int l = 0; l < gridList.Count; l++)
			{
				gridList[l].DesToryGrid();
			}
			mapList[k].StopAllCoroutines();
			Object.Destroy(mapList[k].gameObject);
		}
		mapList.Clear();
	}

	public void CreateMap(List<GameObject> prefabs)
	{
		for (int i = 0; i < prefabs.Count; i++)
		{
			MapBase component = Object.Instantiate(prefabs[i]).GetComponent<MapBase>();
			component.transform.position = new Vector3(0f, 30 * i);
			mapList.Add(component);
		}
	}

	public void GameOverPause()
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			mapList[i].StopAllCoroutines();
		}
	}

	private void Start()
	{
	}

	public MapBase GetCurrMap(Vector3 pos)
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			float num = mapList[i].transform.position.x - mapList[i].MapHalfLengthWidth.x;
			float num2 = mapList[i].transform.position.y - mapList[i].MapHalfLengthWidth.y;
			float num3 = mapList[i].transform.position.x + mapList[i].MapHalfLengthWidth.x;
			float num4 = mapList[i].transform.position.y + mapList[i].MapHalfLengthWidth.y;
			if (pos.x > num && pos.x < num3 && pos.y > num2 && pos.y < num4)
			{
				return mapList[i];
			}
		}
		return null;
	}

	private void CreateGirdsBaseColl()
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<BoxCollider2D>().size = new Vector2(1f, 1.5f);
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.position = base.transform.position;
		gameObject.name = 0 + "-" + 0;
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				Object.Instantiate(gameObject, base.transform.position + new Vector3(1.33f * (float)i, 1.63f * (float)j, 0f), Quaternion.identity, base.transform).name = i + "-" + j;
			}
		}
	}

	private void CreateGridsBasePointList()
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				pointList.Add(base.transform.position + new Vector3(1.33f * (float)i, 1.63f * (float)j, 0f));
			}
		}
	}

	public Grid GetGridPointByMouse()
	{
		return GetGridByWorldPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}

	public Grid GetGridByWorldPos(Vector2 worldPos)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = 99999f;
		Grid result = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (Vector2.Distance(worldPos, gridList[i].Position) < num)
			{
				num = Vector2.Distance(worldPos, gridList[i].Position);
				result = gridList[i];
			}
		}
		return result;
	}

	public Grid GetGridByWorldPos(Vector2 worldPos, int line)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = 99999f;
		Grid result = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y == line && Vector2.Distance(worldPos, gridList[i].Position) < num)
			{
				num = Vector2.Distance(worldPos, gridList[i].Position);
				result = gridList[i];
			}
		}
		return result;
	}

	public float GetLineY(Vector2 pos, int line)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return 0f;
		}
		if (line < 0 || line >= currMap.MapGridNum.y)
		{
			return 0f;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y == line)
			{
				return gridList[i].Position.y;
			}
		}
		return 0f;
	}

	public void LightGrid(Vector2 pos, Vector2Int Gridpos, int x, int y, bool isLight)
	{
		MapBase currMap = GetCurrMap(pos);
		Vector2Int vector2Int = Gridpos - new Vector2Int(x, y);
		int num = 2 * x + 1;
		int num2 = 2 * y + 1;
		for (int i = 0; i < currMap.GridList.Count; i++)
		{
			if (currMap.GridList[i].Point.x >= vector2Int.x && currMap.GridList[i].Point.x < vector2Int.x + num && currMap.GridList[i].Point.y >= vector2Int.y && currMap.GridList[i].Point.y < vector2Int.y + num2)
			{
				if (isLight)
				{
					currMap.GridList[i].LightNum++;
				}
				else
				{
					currMap.GridList[i].LightNum--;
				}
			}
		}
	}

	public Grid GetFarestGrid(Vector2 worldPos, bool getLeft, int line)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = float.MinValue;
		if (getLeft)
		{
			num = float.MaxValue;
		}
		Grid result = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y != line)
			{
				continue;
			}
			if (getLeft)
			{
				if (gridList[i].Position.x < num)
				{
					num = gridList[i].Position.x;
					result = gridList[i];
				}
			}
			else if (gridList[i].Position.x > num)
			{
				num = gridList[i].Position.x;
				result = gridList[i];
			}
		}
		return result;
	}

	public Grid GetRandomGrid()
	{
		List<Grid> gridList = mapList[Random.Range(0, mapList.Count)].GridList;
		return gridList[Random.Range(0, gridList.Count)];
	}

	public PlantBase GetMinDisPlant(Vector2 worldPos, int line, bool getHypno)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = float.MaxValue;
		Grid grid = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y != line)
			{
				continue;
			}
			if (getHypno)
			{
				if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && gridList[i].CurrPlantBase.isHypno && Vector2.Distance(worldPos, gridList[i].Position) < num)
				{
					num = Vector2.Distance(worldPos, gridList[i].Position);
					grid = gridList[i];
				}
			}
			else if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && !gridList[i].CurrPlantBase.isHypno && Vector2.Distance(worldPos, gridList[i].Position) < num)
			{
				num = Vector2.Distance(worldPos, gridList[i].Position);
				grid = gridList[i];
			}
		}
		return grid?.CurrPlantBase;
	}

	public PlantBase GetMinDisPlant(Vector2 worldPos, int line, bool getLeft, bool getHypno)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = float.MaxValue;
		Grid grid = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y != line)
			{
				continue;
			}
			if (getHypno)
			{
				if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && gridList[i].CurrPlantBase.isHypno && ((gridList[i].Position.x < worldPos.x && getLeft) || (gridList[i].Position.x >= worldPos.x && !getLeft)) && Vector2.Distance(worldPos, gridList[i].Position) < num)
				{
					num = Vector2.Distance(worldPos, gridList[i].Position);
					grid = gridList[i];
				}
			}
			else if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && !gridList[i].CurrPlantBase.isHypno && ((gridList[i].Position.x < worldPos.x && getLeft) || (gridList[i].Position.x >= worldPos.x && !getLeft)) && Vector2.Distance(worldPos, gridList[i].Position) < num)
			{
				num = Vector2.Distance(worldPos, gridList[i].Position);
				grid = gridList[i];
			}
		}
		return grid?.CurrPlantBase;
	}

	public Grid GetLastPlantGrid(Vector2 worldPos, int line, bool getLeft, bool getHyp)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = 0f;
		Grid result = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y != line)
			{
				continue;
			}
			if (getHyp)
			{
				if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && gridList[i].CurrPlantBase.isHypno && ((gridList[i].Position.x < worldPos.x && getLeft) || (gridList[i].Position.x >= worldPos.x && !getLeft)) && Vector2.Distance(worldPos, gridList[i].Position) > num)
				{
					num = Vector2.Distance(worldPos, gridList[i].Position);
					result = gridList[i];
				}
			}
			else if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && !gridList[i].CurrPlantBase.isHypno && ((gridList[i].Position.x < worldPos.x && getLeft) || (gridList[i].Position.x >= worldPos.x && !getLeft)) && Vector2.Distance(worldPos, gridList[i].Position) > num)
			{
				num = Vector2.Distance(worldPos, gridList[i].Position);
				result = gridList[i];
			}
		}
		return result;
	}

	public Grid GetNextGrid(Grid grid, bool isRight = false)
	{
		if (grid == null)
		{
			return null;
		}
		MapBase currMap = GetCurrMap(grid.Position);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		if (grid == null)
		{
			return null;
		}
		int num = gridList.IndexOf(grid);
		int num2 = 1;
		if (!isRight)
		{
			num2 = -1;
		}
		if (num + num2 < gridList.Count && num + num2 >= 0 && gridList[num + num2].Point.y == grid.Point.y)
		{
			return gridList[num + num2];
		}
		return null;
	}

	public List<Grid> GetAroundGrid(Grid grid, int radius)
	{
		List<Grid> list = new List<Grid>();
		MapBase currMap = GetCurrMap(grid.Position);
		if (currMap == null)
		{
			return list;
		}
		List<Grid> gridList = currMap.GridList;
		Vector2 vector = grid.Point - new Vector2Int(radius, radius);
		int num = 2 * radius + 1;
		List<Vector2> list2 = new List<Vector2>();
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				list2.Add(vector + new Vector2Int(i, j));
			}
		}
		for (int k = 0; k < gridList.Count; k++)
		{
			if (list2.Contains(gridList[k].Point))
			{
				list.Add(gridList[k]);
				list2.Remove(gridList[k].Point);
			}
		}
		return list;
	}

	public List<Grid> GetLineAllGrid(Vector2 pos, int lineNum)
	{
		List<Grid> list = new List<Grid>();
		List<Grid> gridList = GetCurrMap(pos).GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y == lineNum)
			{
				list.Add(gridList[i]);
			}
		}
		return list;
	}

	public List<PlantBase> GetAllPlant(Vector3 pos, bool getHyp)
	{
		MapBase currMap = GetCurrMap(pos);
		List<PlantBase> list = new List<PlantBase>();
		if (currMap == null)
		{
			return list;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].CurrPlantBase != null)
			{
				if (getHyp && gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
				else if (!getHyp && !gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
			}
		}
		return list;
	}

	public List<PlantBase> GetAroundPlant(Vector3 pos, float dis, bool getHyp)
	{
		List<PlantBase> list = new List<PlantBase>();
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return list;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (Vector2.Distance(gridList[i].Position, pos) < dis && (bool)gridList[i].CurrPlantBase)
			{
				if (getHyp && gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
				else if (!getHyp && !gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
			}
		}
		return list;
	}

	public List<PlantBase> GetLinePlant(Vector3 pos, int CurrLine, float dis, bool getHyp)
	{
		MapBase currMap = GetCurrMap(pos);
		List<PlantBase> list = new List<PlantBase>();
		if (currMap == null)
		{
			return list;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y == CurrLine && Vector2.Distance(gridList[i].Position, pos) < dis && (bool)gridList[i].CurrPlantBase)
			{
				if (getHyp && gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
				else if (!getHyp && !gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
			}
		}
		return list;
	}

	public Grid GetHaveIceFirstGrid(Vector3 pos, int line)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y >= line && gridList[i].Point.x == 8 && gridList[i].IceRoadNum > 0)
			{
				return gridList[i];
			}
		}
		for (int j = 0; j < gridList.Count; j++)
		{
			if (gridList[j].Point.x == 8 && gridList[j].IceRoadNum > 0)
			{
				return gridList[j];
			}
		}
		return null;
	}

	public Grid GetWaterGrid()
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			List<Grid> gridList = mapList[i].GridList;
			List<Grid> list = new List<Grid>();
			for (int j = 0; j < gridList.Count; j++)
			{
				if (gridList[j].isWaterGrid)
				{
					list.Add(gridList[j]);
				}
			}
			if (list.Count > 0)
			{
				int index = Random.Range(0, list.Count);
				if (list.Count <= 0)
				{
					return null;
				}
				return list[index];
			}
		}
		return null;
	}

	public Grid GetWaterGrid(Vector3 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		List<Grid> list = new List<Grid>();
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].isWaterGrid)
			{
				list.Add(gridList[i]);
			}
		}
		int index = Random.Range(0, list.Count);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[index];
	}

	public int GetNoWaterGrid(Vector3 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return -1;
		}
		List<Grid> gridList = currMap.GridList;
		List<int> list = new List<int>();
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].isWaterGrid && !list.Contains(gridList[i].Point.y))
			{
				list.Add(gridList[i].Point.y);
			}
		}
		int y = currMap.MapGridNum.y;
		List<int> list2 = new List<int>();
		for (int j = 0; j < y; j++)
		{
			if (!list.Contains(j))
			{
				list2.Add(j);
			}
		}
		int index = Random.Range(0, list2.Count);
		if (list2.Count <= 0)
		{
			return -1;
		}
		return list2[index];
	}

	public Grid GetHavePlantGrid(Vector3 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		List<Grid> list = new List<Grid>();
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].CurrPlantBase != null)
			{
				list.Add(gridList[i]);
			}
		}
		int index = Random.Range(0, list.Count);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[index];
	}

	public float GetMapYHighest(Vector2 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return -1f;
		}
		return currMap.transform.position.y + currMap.MapHalfLengthWidth.y;
	}

	public Vector2 GetMapPos(Vector3 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return new Vector2(0f, 0f);
		}
		return currMap.transform.position;
	}

	public Grid GetGridByVertical(Vector3 pos, int lineNum)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point == new Vector2(8f, lineNum))
			{
				return gridList[i];
			}
		}
		return null;
	}

	public GameObject GraveStoneUp(Grid grid)
	{
		GameObject obj = Object.Instantiate(GameManager.Instance.GameConf.GraveStone);
		obj.GetComponent<GraveStone>().CreateInit(grid);
		return obj;
	}

	public GameObject CreateCrater(Grid grid)
	{
		GameObject obj = Object.Instantiate(GameManager.Instance.GameConf.Crater);
		obj.transform.position = grid.Position + new Vector2(0f, -0.3f);
		obj.GetComponent<Crater>().CreateInit(grid.isWaterGrid, grid.Point.y * 100 + 1, grid);
		return obj;
	}

	private IEnumerator MoveUp(GameObject gameObject, float Y)
	{
		while (gameObject.transform.position.y < Y)
		{
			yield return new WaitForSeconds(0.02f);
			gameObject.transform.Translate(new Vector2(0f, 1f) * Time.deltaTime * 5f);
		}
	}

	public void PlantFlash(PlantType plantType)
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			List<Grid> gridList = mapList[i].GridList;
			for (int j = 0; j < gridList.Count; j++)
			{
				if (gridList[j].CurrPlantBase != null)
				{
					if (gridList[j].CurrPlantBase.GetPlantType() == plantType)
					{
						gridList[j].CurrPlantBase.StartFlash();
					}
					else if (gridList[j].CurrPlantBase.CarryPlant != null && gridList[j].CurrPlantBase.CarryPlant.GetPlantType() == plantType)
					{
						gridList[j].CurrPlantBase.CarryPlant.StartFlash();
					}
				}
			}
		}
	}

	public void PlantnoFlash(PlantType plantType)
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			List<Grid> gridList = mapList[i].GridList;
			for (int j = 0; j < gridList.Count; j++)
			{
				if (gridList[j].CurrPlantBase != null)
				{
					if (gridList[j].CurrPlantBase.GetPlantType() == plantType)
					{
						gridList[j].CurrPlantBase.StopFlash();
					}
					else if (gridList[j].CurrPlantBase.CarryPlant != null && gridList[j].CurrPlantBase.CarryPlant.GetPlantType() == plantType)
					{
						gridList[j].CurrPlantBase.CarryPlant.StopFlash();
					}
				}
			}
		}
	}
}
