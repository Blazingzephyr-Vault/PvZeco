using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SeedBank : MonoBehaviour
{
    ///
    public List<PlantCard> SlotList => slotList;
    ///

    public int CardNum = 4;

	private int DecidedCardNum;

	private int choosedNum;

	public bool isFull;

	public bool isCanClick;

	public bool isNoCD;

	public bool isRatZombie;

	public GameObject SunPos;

	public GameObject MoonPos;

	public GameObject MoonBank;

	public Sprite SunBankSpite;

	public Sprite MoonBankSpite;

	public Sprite SunAndMoonBankSpite;

	private SpriteRenderer seedBank;

	public TextMesh sunNumText;

	public TextMesh moonNumText;

	private List<PlantCard> slotList = new List<PlantCard>();

	public static SeedBank Instance;

	private int currOrderNum = 14;

	public bool NeedSummonSun
	{
		get
		{
			if (LV.Instance.CurrSeedBankType != 0)
			{
				return LV.Instance.CurrSeedBankType == SeedBankType.SunAndMoonBank;
			}
			return true;
		}
	}

	public bool NeedSummonMoon
	{
		get
		{
			if (LV.Instance.CurrSeedBankType != SeedBankType.MoonBank)
			{
				return LV.Instance.CurrSeedBankType == SeedBankType.SunAndMoonBank;
			}
			return true;
		}
	}

	public int noBasePlantExtra => 50;

	public int ChoosedNum
	{
		get
		{
			return choosedNum;
		}
		set
		{
			choosedNum = value;
			if (choosedNum >= CardNum)
			{
				isFull = true;
			}
			else
			{
				isFull = false;
			}
		}
	}

	public int CurrOrderNum
	{
		get
		{
			return currOrderNum;
		}
		set
		{
			currOrderNum = value;
			if (value > 50)
			{
				currOrderNum = 14;
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		seedBank = base.transform.GetComponent<SpriteRenderer>();
		sunNumText = base.transform.Find("SunNumText").GetComponent<TextMesh>();
		isFull = false;
		isCanClick = true;
	}

	public void UpdateSeedBankType(SeedBankType type)
	{
		UIManager.Instance.SetChooserType(type);
		switch (type)
		{
		case SeedBankType.SunBank:
			seedBank.sprite = SunBankSpite;
			MoonBank.transform.localScale = Vector3.zero;
			MoonBank.transform.localPosition = new Vector3(0.65f, MoonBank.transform.localPosition.y);
			break;
		case SeedBankType.MoonBank:
			seedBank.sprite = MoonBankSpite;
			MoonBank.transform.localScale = Vector3.one;
			MoonBank.transform.localPosition = new Vector3(0.65f, MoonBank.transform.localPosition.y);
			break;
		case SeedBankType.SunAndMoonBank:
			seedBank.sprite = SunAndMoonBankSpite;
			MoonBank.transform.localScale = Vector3.one;
			MoonBank.transform.localPosition = new Vector3(-0.5f, MoonBank.transform.localPosition.y);
			break;
		}
	}

	public void PlantFailClearCD(int cardID)
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].CardId == cardID)
			{
				slotList[i].currTimeForCd = 0f;
				break;
			}
		}
	}

	public void ChangeCard(int plantId, int cardId)
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (i == cardId - 1)
			{
				UIPlantCardNC cardInfo = SeedChooser.Instance.GetCardInfo(plantId);
				slotList[i].AddChoose(cardInfo);
				slotList[i].currTimeForCd = 0f;
			}
		}
	}

	public void UpdateSunNum(string sunNum)
	{
		sunNumText.text = sunNum;
	}

	public void UpdateMoonNum(string moonNum)
	{
		moonNumText.text = moonNum;
	}

	public void SpawnCardSlot()
	{
		ClearCardSlot();
		if (CardNum == 0)
		{
			isFull = true;
			seedBank.size = new Vector2(4.9f, seedBank.size.y);
		}
		else
		{
			seedBank.size = new Vector2(4.9f + (float)(CardNum - 4) * 0.85f, seedBank.size.y);
		}
		if (seedBank.size.x < 5.8f)
		{
			seedBank.size = new Vector2(5.8f, seedBank.size.y);
		}
		for (int i = 0; i < CardNum; i++)
		{
			PlantCard component = Object.Instantiate(GameManager.Instance.GameConf.CardSlot).GetComponent<PlantCard>();
			component.transform.SetParent(Instance.transform);
			component.transform.localPosition = new Vector3(1.75f + 0.85f * (float)i, 0f);
			component.transform.localScale = new Vector3(0.8f, 0.8f);
			component.CardId = i;
			slotList.Add(component);
		}
	}

	public void ClearCardSlot()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].isChoosed)
			{
				slotList[i].ClearChoose(noAnimation: true);
				ChoosedNum--;
			}
			slotList[i].DestroyCardSlot();
		}
		slotList.Clear();
		isFull = false;
	}

	public void AllCancelPlace()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			slotList[i].CancelPlace();
		}
	}

	public bool ChooseCard(UIPlantCardNC nC)
	{
		if (slotList.Count < 1)
		{
			return false;
		}
		for (int i = 0; i < slotList.Count; i++)
		{
			if (!slotList[i].isChoosed)
			{
				DecidedCardNum = i;
				break;
			}
		}
		slotList[DecidedCardNum].isChoosed = true;
		ChoosedNum++;
		Vector2 vector = Camera.main.WorldToScreenPoint(slotList[DecidedCardNum].transform.position);
		UIPlantCardAnimation component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CardSlotAnimation).GetComponent<UIPlantCardAnimation>();
		component.transform.SetParent(UIManager.Instance.transform);
		component.CreateInit(nC.transform.position, nC, slotList[DecidedCardNum]);
		component.PlayChooseAnimation(vector, isBack: false);
		return true;
	}

	public PlantCard GetPreCard(PlantCard pC)
	{
		int num = slotList.IndexOf(pC);
		if (num <= 0)
		{
			return null;
		}
		return slotList[num - 1];
	}

	public void ClearChoose(UIPlantCardNC nC, PlantCard pC)
	{
		ChoosedNum--;
		UIPlantCardAnimation component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CardSlotAnimation).GetComponent<UIPlantCardAnimation>();
		component.transform.SetParent(UIManager.Instance.transform);
		Vector2 initPos = Camera.main.WorldToScreenPoint(pC.transform.position);
		component.CreateInit(initPos, nC, pC);
		component.PlayChooseAnimation(nC.transform.position, isBack: true);
	}

	public bool ClearCD(PlantType type)
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].CardPlantType == type && slotList[i].currTimeForCd > 0f)
			{
				slotList[i].currTimeForCd = 0f;
				if (GameManager.Instance.isServer)
				{
					SocketServer.Instance.SendHostCD(slotList[i].CardId, isOK: true);
				}
				if (GameManager.Instance.isClient)
				{
					SocketClient.Instance.UpdateCD(slotList[i].CardId, Ok: true);
				}
				return true;
			}
		}
		return false;
	}

	public void ClearAllCD()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			slotList[i].currTimeForCd = 0f;
		}
	}

	public bool CheckPlant(PlantBase plant, Grid grid, int NeedSun, string PlacePlayer)
	{
		if (NeedSun == -1)
		{
			UIPlantCardNC cardInfo = SeedChooser.Instance.GetCardInfo(plant.GetPlantType());
			if (cardInfo == null)
			{
				cardInfo = ZombieChooser.Instance.GetCardInfo(plant.GetPlantType());
				NeedSun = cardInfo.NeedNum;
			}
			else
			{
				NeedSun = cardInfo.NeedNum;
			}
		}
		if (plant.IsZombiePlant)
		{
			if (PlayerManager.Instance.GetSunNum(isSun: false, PlacePlayer) < (float)NeedSun)
			{
				return false;
			}
		}
		else if (PlayerManager.Instance.GetSunNum(isSun: true, PlacePlayer) < (float)NeedSun)
		{
			return false;
		}
		if (plant != null)
		{
			if (LV.Instance.CurrLVType == LVType.PvP && grid.CurrGridType != 0)
			{
				bool flag = PvPSelector.Instance.RedTeamNames.Contains(PlacePlayer);
				if (flag && grid.CurrGridType == GridType.BlueTeam)
				{
					return false;
				}
				if (!flag && grid.CurrGridType == GridType.RedTeam)
				{
					return false;
				}
			}
			if (grid.IceRoadNum > 0)
			{
				return false;
			}
			if (grid.HaveCrater)
			{
				return false;
			}
			if (grid.isOccupied)
			{
				return false;
			}
			if (grid.HaveGraveStone)
			{
				if (plant.GetPlantType() == PlantType.Gravebuster && grid.CurrPlantBase == null)
				{
					return true;
				}
				return false;
			}
			if (!plant.isHaveSpecialCheck)
			{
				if (plant.BasePlant == PlantType.Nope)
				{
					return NormalCheck(plant, grid);
				}
				if (grid.CurrPlantBase != null)
				{
					if (grid.CurrPlantBase.GetPlantType() == plant.BasePlant && grid.CurrPlantBase.CarryPlant == null)
					{
						return true;
					}
					if (grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() == plant.BasePlant)
					{
						return true;
					}
				}
				if (plant.BasePlantSunNum < 0 || NeedSun == -2)
				{
					return true;
				}
				int num = NeedSun + plant.BasePlantSunNum + Instance.noBasePlantExtra;
				if (PlayerManager.Instance.GetSunNum(isSun: true, PlacePlayer) >= (float)num)
				{
					return NormalCheck(plant, grid);
				}
			}
			else if (plant.SpecialPlantCheck(grid, NeedSun, PlacePlayer))
			{
				return true;
			}
		}
		return false;
	}

	private bool NormalCheck(PlantBase plant, Grid grid)
	{
		if (grid.CurrPlantBase != null)
		{
			if (!plant.CanCarryed && !plant.isProtectPlant)
			{
				return false;
			}
			if (grid.isWaterGrid && grid.CurrPlantBase.CanCarryOtherPlant && !plant.CanPlaceOnWaterCarry)
			{
				return false;
			}
			if (grid.CurrPlantBase.CanCarryOtherPlant && !plant.CanCarryOtherPlant && grid.CurrPlantBase.CarryPlant == null && !plant.isProtectPlant)
			{
				return true;
			}
			if (plant.isProtectPlant && grid.CurrPlantBase.CanProtect && grid.CurrPlantBase.ProtectPlant == null && (grid.CurrPlantBase.CarryPlant == null || (grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.CanProtect)))
			{
				return true;
			}
			if (grid.CurrPlantBase.isProtectPlant && plant.CanProtect && plant.CanPlaceOnGrass)
			{
				return true;
			}
			if (!grid.CurrPlantBase.CanCarryOtherPlant && plant.CanCarryOtherPlant && grid.CurrPlantBase.CanCarryed && grid.CurrPlantBase.GetPlantType() != PlantType.CobCannon)
			{
				if (plant.CanPlaceOnWater && (grid.isHavePuddle || grid.isWaterGrid) && !grid.CurrPlantBase.CanPlaceOnWater && grid.CurrPlantBase.GetPlantType() != PlantType.PotatoMine)
				{
					return true;
				}
				if (plant.CanPlaceOnGrass && !grid.isWaterGrid)
				{
					return true;
				}
			}
		}
		else
		{
			if (!grid.isWaterGrid && !grid.isHavePuddle && !grid.isHardGrid && plant.CanPlaceOnGrass)
			{
				return true;
			}
			if (grid.isWaterGrid && plant.CanPlaceOnWater)
			{
				return true;
			}
			if (grid.isHardGrid && plant.CanPlaceOnHardGround)
			{
				return true;
			}
			if (grid.isHavePuddle && plant.CanCarryOtherPlant)
			{
				return true;
			}
		}
		return false;
	}

	public void PlantConfirm(PlantBase plant, Grid grid, int NeedSun, int SPcode, string PlacePlayer)
	{
		plant.PlacePlayer = PlacePlayer;
		if (GameManager.Instance.isServer)
		{
			PlantSpawn plantSpawn = new PlantSpawn();
			plantSpawn.OnlineId = SocketServer.Instance.ItemId;
			plantSpawn.plantType = plant.GetPlantType();
			plantSpawn.PlacePlayer = PlacePlayer;
			plantSpawn.GridPos = grid.Position;
			plant.OnlineId = plantSpawn.OnlineId;
			plantSpawn.SPcode = SPcode;
			if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(PlacePlayer))
			{
				plantSpawn.GridPos = new Vector2(0f - grid.Position.x, grid.Position.y);
			}
			SocketServer.Instance.SpawnPlant(plantSpawn);
		}
		if (SPcode == 2)
		{
			Imitater component = PlantManager.Instance.GetNewPlant(PlantType.Imitater).GetComponent<Imitater>();
			component.CopyPlantInfo(plant);
			component.PlacePlayer = PlacePlayer;
			plant.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
			plant = component;
		}
		PlantManager.Instance.plants.Add(plant);
		if (NeedSun == -1)
		{
			UIPlantCardNC cardInfo = SeedChooser.Instance.GetCardInfo(plant.GetPlantType());
			if (cardInfo == null)
			{
				cardInfo = ZombieChooser.Instance.GetCardInfo(plant.GetPlantType());
				NeedSun = cardInfo.NeedNum;
			}
			else
			{
				NeedSun = cardInfo.NeedNum;
			}
		}
		plant.InitForPlace(grid, CurrOrderNum);
		if (!plant.isHaveSpecialCheck)
		{
			if (plant.BasePlant != 0)
			{
				bool flag = true;
				MapManager.Instance.PlantnoFlash(plant.BasePlant);
				if (grid.CurrPlantBase != null && grid.CurrPlantBase.CanCarryOtherPlant && grid.CurrPlantBase.GetPlantType() != plant.BasePlant)
				{
					if (grid.CurrPlantBase.CarryPlant != null)
					{
						grid.CurrPlantBase.CarryPlant.Dead();
						flag = false;
					}
					plant.transform.SetParent(grid.CurrPlantBase.transform);
					grid.CurrPlantBase.CarryPlant = plant;
				}
				else if (grid.CurrPlantBase != null)
				{
					if (grid.CurrPlantBase.GetPlantType() != plant.BasePlant && grid.CurrPlantBase.isProtectPlant)
					{
						plant.ProtectPlant = grid.CurrPlantBase;
						plant.ProtectPlant.transform.SetParent(plant.transform);
					}
					else if (grid.CurrPlantBase.ProtectPlant != null)
					{
						plant.ProtectPlant = grid.CurrPlantBase.ProtectPlant;
						grid.CurrPlantBase.ProtectPlant = null;
						plant.ProtectPlant.transform.SetParent(plant.transform);
						grid.CurrPlantBase.Dead();
						flag = false;
					}
					else
					{
						grid.CurrPlantBase.Dead();
						flag = false;
					}
					grid.CurrPlantBase = plant;
				}
				if (flag && plant.BasePlantSunNum >= 0 && NeedSun != -2)
				{
					PlayerManager.Instance.AddSunNum(-(plant.BasePlantSunNum + noBasePlantExtra), isSun: true, PlacePlayer);
				}
			}
			else if (grid.CurrPlantBase != null && grid.CurrPlantBase.CanCarryOtherPlant && !plant.isProtectPlant)
			{
				plant.transform.SetParent(grid.CurrPlantBase.transform);
				grid.CurrPlantBase.CarryPlant = plant;
			}
			else if (plant.isProtectPlant)
			{
				if (grid.CurrPlantBase != null)
				{
					grid.CurrPlantBase.ProtectPlant = plant;
					plant.transform.SetParent(grid.CurrPlantBase.transform);
				}
			}
			else if (grid.CurrPlantBase != null && grid.CurrPlantBase.isProtectPlant && !plant.CanCarryOtherPlant)
			{
				grid.CurrPlantBase.transform.SetParent(plant.transform);
				plant.ProtectPlant = grid.CurrPlantBase;
				grid.CurrPlantBase = plant;
			}
			else if (grid.CurrPlantBase != null && !grid.CurrPlantBase.CanCarryOtherPlant && plant.CanCarryOtherPlant)
			{
				grid.CurrPlantBase.transform.SetParent(plant.transform);
				if (grid.CurrPlantBase.isProtectPlant)
				{
					plant.ProtectPlant = grid.CurrPlantBase;
				}
				else
				{
					plant.CarryPlant = grid.CurrPlantBase;
				}
				grid.CurrPlantBase.transform.position += new Vector3(plant.CarryOffset.x, plant.CarryOffset.y);
				grid.CurrPlantBase = plant;
			}
			if (grid.CurrPlantBase == null)
			{
				grid.CurrPlantBase = plant;
			}
		}
		CurrOrderNum++;
		if (NeedSun == -2)
		{
			NeedSun = 0;
		}
		if (plant.IsZombiePlant)
		{
			PlayerManager.Instance.AddSunNum(-NeedSun, isSun: false, PlacePlayer);
		}
		else
		{
			PlayerManager.Instance.AddSunNum(-NeedSun, isSun: true, PlacePlayer);
		}
		if (SPcode == 1)
		{
			plant.OpenBlackAndWhite(isOpen: true);
		}
		if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(PlacePlayer))
		{
			plant.RatThis(synClient: true);
		}
	}

	public bool CheckZombie(ZombieType type2, Grid grid, int NeedMoon, string PlacePlayer)
	{
		if (NeedMoon == -1)
		{
			NeedMoon = ZombieChooser.Instance.GetCardInfo(type2).NeedNum;
		}
		if (PlayerManager.Instance.GetSunNum(isSun: false, PlacePlayer) < (float)NeedMoon)
		{
			return false;
		}
		if (LV.Instance.CurrLVType == LVType.PvP && grid.CurrGridType != 0 && PlacePlayer != null)
		{
			bool flag = PvPSelector.Instance.RedTeamNames.Contains(PlacePlayer);
			if (type2 == ZombieType.BungiZombie)
			{
				if (!flag && grid.CurrGridType == GridType.BlueTeam)
				{
					return false;
				}
				if (flag && grid.CurrGridType == GridType.RedTeam)
				{
					return false;
				}
				if (grid.CurrGridType == GridType.AllTeam)
				{
					return false;
				}
			}
			else
			{
				if (flag && grid.CurrGridType == GridType.BlueTeam)
				{
					return false;
				}
				if (!flag && grid.CurrGridType == GridType.RedTeam)
				{
					return false;
				}
				if (grid.CurrGridType == GridType.AllTeam)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void ZombieConfirm(ZombieType type, ZombieBase zombie, Grid grid, int NeedMoon, string PlacePlayer)
	{
		if (NeedMoon == -1)
		{
			NeedMoon = ZombieChooser.Instance.GetCardInfo(type).NeedNum;
		}
		zombie.PlacePlayer = PlacePlayer;
		ZombieManager.Instance.UpdateZombie(type, zombie, grid.Position, grid.Point.y);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			if (PvPSelector.Instance.IsSameTeam(PlacePlayer))
			{
				zombie.RatThis(synClient: true);
			}
		}
		else if (isRatZombie)
		{
			zombie.RatThis();
		}
		PlayerManager.Instance.AddSunNum(-NeedMoon, isSun: false, PlacePlayer);
	}

	public void StartMove(SeedBankType type)
	{
		UpdateSeedBankType(type);
		base.transform.localScale = new Vector3(1.1f, 1.1f);
		StartCoroutine(DoMove(5f));
	}

	public void StartMoveBack(bool fade)
	{
		if (fade)
		{
			StartCoroutine(DoMove(6.8f));
			return;
		}
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, 6.8f, base.transform.localPosition.z);
		base.transform.localScale = Vector3.zero;
	}

	private IEnumerator DoMove(float targetPosY)
	{
		Vector3 target = new Vector3(base.transform.localPosition.x, targetPosY, base.transform.localPosition.z);
		Vector2 dir = (target - base.transform.localPosition).normalized;
		while (Vector2.Distance(target, base.transform.localPosition) > 0.2f)
		{
			yield return new WaitForSeconds(0.01f);
			base.transform.Translate(dir * 0.1f);
		}
	}
}
