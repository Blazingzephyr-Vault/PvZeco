using System.Collections;
using SocketSave;
using UnityEngine;

public class PlantCard : MonoBehaviour
{
	public int CardId;

	private Sprite sprite;

	private TextMesh WantSunText;

	public Transform mask;

	public SpriteRenderer grayImg;

	public SpriteRenderer ImgRenderer;

	public int NeedSun;

	public bool isNeedSun;

	public float CDTime;

	public PlantType CardPlantType;

	public ZombieType CardZombieType;

	public float currTimeForCd;

	private bool canPlace;

	private bool wantPlace;

	private PlantBase plant;

	private PlantBase plantInGrid;

	private ZombieBase zombie;

	private ZombieBase zombieInGrid;

	private Grid InGridGrid;

	private CardState cardState;

	public bool isChoosed;

	private UIPlantCardNC myPlantCard;

	private Coroutine CdCoroutine;

	private bool secondClick;

	public bool isImitater;

	private bool isStarted => LVManager.Instance.GameIsStart;

	public CardState CardState
	{
		get
		{
			return cardState;
		}
		set
		{
			if (cardState == value)
			{
				return;
			}
			switch (value)
			{
			case CardState.CanPlace:
				mask.localScale = new Vector3(mask.localScale.x, 2.2f);
				ImgRenderer.color = Color.white;
				break;
			case CardState.NotCD:
				grayImg.color = new Color(0.7f, 0.7f, 0.7f);
				ImgRenderer.color = new Color(0.6f, 0.6f, 0.6f);
				CDEnter();
				if (WantPlace)
				{
					CancelPlace();
				}
				break;
			case CardState.NotSun:
				mask.localScale = new Vector3(mask.localScale.x, 2.2f);
				ImgRenderer.color = new Color(0.6f, 0.6f, 0.6f);
				if (WantPlace)
				{
					CancelPlace();
				}
				break;
			case CardState.NotAll:
				grayImg.color = new Color(0.6f, 0.6f, 0.6f);
				ImgRenderer.color = new Color(0.4f, 0.4f, 0.4f);
				CDEnter();
				if (WantPlace)
				{
					CancelPlace();
				}
				break;
			}
			cardState = value;
		}
	}

	public bool CanPlace
	{
		get
		{
			return canPlace;
		}
		set
		{
			canPlace = value;
			if (SeedBank.Instance.isNoCD)
			{
				canPlace = true;
			}
			CheckState();
		}
	}

	public bool WantPlace
	{
		get
		{
			return wantPlace;
		}
		set
		{
			wantPlace = value;
			if (wantPlace)
			{
				if (CardPlantType != 0)
				{
					plant = PlantManager.Instance.GetNewPlant(CardPlantType);
					plant.transform.SetParent(PlantManager.Instance.transform);
					plant.InitForCreate(inGrid: false, null, isImitater);
				}
				else if (CardZombieType != 0)
				{
					zombie = ZombieManager.Instance.GetNewZombie(CardZombieType);
					zombie.transform.SetParent(ZombieManager.Instance.transform);
					zombie.CreateInit(inGrid: false, null, SeedBank.Instance.isRatZombie);
				}
				ImgRenderer.color = new Color(0.75f, 0.75f, 0.75f);
			}
			else
			{
				ImgRenderer.color = new Color(1f, 1f, 1f);
			}
		}
	}

	private void Start()
	{
		WantSunText = base.transform.Find("SunNumText").GetComponent<TextMesh>();
		sprite = ImgRenderer.sprite;
		PlayerManager.Instance.AddSunNumUpdateActionListener(CheckState);
		LVManager.Instance.AddLVStartActionListenr(OnLVStartAction);
		isChoosed = false;
	}

	private void Update()
	{
		if (!isChoosed || !isStarted)
		{
			return;
		}
		if (Input.GetMouseButtonDown(1))
		{
			GoCancelPlant();
		}
		if (!WantPlace)
		{
			return;
		}
		Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(vector);
		if (gridByWorldPos == null)
		{
			return;
		}
		if (plant != null)
		{
			plant.transform.position = new Vector3(vector.x, vector.y, 0f);
		}
		if (zombie != null)
		{
			zombie.transform.position = new Vector3(vector.x, vector.y, 0f);
		}
		float num = Vector2.Distance(vector, gridByWorldPos.Position);
		if (num < 1f)
		{
			if (plant != null && SeedBank.Instance.CheckPlant(plant, gridByWorldPos, NeedSun, GameManager.Instance.LocalPlayer.playerName))
			{
				planting(gridByWorldPos);
			}
			else if (zombie != null && SeedBank.Instance.CheckZombie(CardZombieType, gridByWorldPos, NeedSun, GameManager.Instance.LocalPlayer.playerName))
			{
				Zombieing(gridByWorldPos);
			}
			else
			{
				ClearInGrid();
			}
			return;
		}
		if (Input.GetMouseButtonDown(0) && num > 1.5f)
		{
			if (secondClick)
			{
				GoCancelPlant();
			}
			secondClick = true;
		}
		ClearInGrid();
	}

	private void GoCancelPlant()
	{
		if (WantPlace)
		{
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
			}
			CancelPlace();
		}
	}

	private void UpdateOnlinePreview(Vector2 pos, PlantType type)
	{
		if (GameManager.Instance.isOnline)
		{
			PlantPreview plantPreview = new PlantPreview();
			plantPreview.plantType = type;
			plantPreview.GridPos = pos;
			plantPreview.PlayerName = GameManager.Instance.LocalPlayer.playerName;
			plantPreview.isImtor = isImitater;
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.ApplyPlacePreview(plantPreview);
			}
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.PlacePreview(plantPreview, null);
			}
		}
	}

	private void UpdateOnlinePreview(Vector2 pos, ZombieType type)
	{
		if (GameManager.Instance.isOnline)
		{
			ZombiePreview zombiePreview = new ZombiePreview();
			zombiePreview.zombieType = type;
			zombiePreview.GridPos = pos;
			zombiePreview.PlayerName = GameManager.Instance.LocalPlayer.playerName;
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.ApplyZombiePreview(zombiePreview);
			}
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.ZombiePreview(zombiePreview, null);
			}
		}
	}

	private void ClearInGrid()
	{
		if (plantInGrid != null)
		{
			plantInGrid.Dead(isFlat: false, 0f, synClient: true);
			plantInGrid = null;
			UpdateOnlinePreview(default(Vector2), PlantType.Nope);
		}
		if (zombieInGrid != null)
		{
			zombieInGrid.DirectDead(canDropItem: false, 0f, synClient: true);
			zombieInGrid = null;
			UpdateOnlinePreview(default(Vector2), ZombieType.Nope);
		}
	}

	private void planting(Grid grid)
	{
		if (plantInGrid == null)
		{
			InGridGrid = grid;
			plantInGrid = PlantManager.Instance.GetNewPlant(CardPlantType);
			plantInGrid.transform.SetParent(PlantManager.Instance.transform);
			plantInGrid.InitForCreate(inGrid: true, grid, isImitater);
			UpdateOnlinePreview(grid.Position, CardPlantType);
		}
		else if (grid != InGridGrid)
		{
			InGridGrid = grid;
			plantInGrid.UpdateForCreate(grid);
			UpdateOnlinePreview(grid.Position, CardPlantType);
		}
		if (!Input.GetMouseButtonDown(0) && (!Input.GetMouseButtonUp(0) || !GameManager.Instance.isAndroid))
		{
			return;
		}
		WantPlace = false;
		if (GameManager.Instance.isClient)
		{
			PlantSpawn plantSpawn = new PlantSpawn();
			plantSpawn.plantType = plant.GetPlantType();
			plantSpawn.GridPos = grid.Position;
			plantSpawn.CardId = CardId;
			plantSpawn.SPcode = (isImitater ? 2 : 0);
			SocketClient.Instance.ApplyPlacePlant(plantSpawn);
			plant.Dead(isFlat: false, 0f, synClient: true);
		}
		else
		{
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendHostCD(CardId, isOK: false);
			}
			SeedBank.Instance.PlantConfirm(plant, grid, NeedSun, isImitater ? 2 : 0, GameManager.Instance.LocalPlayer.playerName);
		}
		plant = null;
		ClearInGrid();
		CanPlace = false;
	}

	private void Zombieing(Grid grid)
	{
		if (zombieInGrid == null)
		{
			InGridGrid = grid;
			zombieInGrid = ZombieManager.Instance.GetNewZombie(CardZombieType);
			zombieInGrid.transform.SetParent(ZombieManager.Instance.transform);
			zombieInGrid.CreateInit(inGrid: true, grid, SeedBank.Instance.isRatZombie);
			UpdateOnlinePreview(grid.Position, CardZombieType);
		}
		else if (grid != InGridGrid)
		{
			InGridGrid = grid;
			zombieInGrid.UpdateForCreate(grid);
			UpdateOnlinePreview(grid.Position, CardZombieType);
		}
		if (!Input.GetMouseButtonDown(0) && (!Input.GetMouseButtonUp(0) || !GameManager.Instance.isAndroid))
		{
			return;
		}
		WantPlace = false;
		if (GameManager.Instance.isClient)
		{
			ZombieSpawnApply zombieSpawnApply = new ZombieSpawnApply();
			zombieSpawnApply.Type = CardZombieType;
			zombieSpawnApply.GridPos = grid.Position;
			zombieSpawnApply.CardId = CardId;
			SocketClient.Instance.ApplyPlaceZombie(zombieSpawnApply);
			zombie.DirectDead(canDropItem: false, 0f, synClient: true);
		}
		else
		{
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendHostCD(CardId, isOK: false);
			}
			SeedBank.Instance.ZombieConfirm(CardZombieType, zombie, grid, NeedSun, GameManager.Instance.LocalPlayer.playerName);
		}
		zombie = null;
		ClearInGrid();
		CanPlace = false;
	}

	public void CancelPlace()
	{
		if (WantPlace)
		{
			if (plant != null && plant.BasePlant != 0)
			{
				MapManager.Instance.PlantnoFlash(plant.BasePlant);
			}
			WantPlace = false;
			CheckState();
			if (plant != null)
			{
				plant.Dead(isFlat: false, 0f, synClient: true);
				plant = null;
			}
			if (zombie != null)
			{
				zombie.DirectDead(canDropItem: false, 0f, synClient: true);
				zombie = null;
			}
			ClearInGrid();
		}
	}

	private void OnLVStartAction()
	{
		CancelPlace();
		if (SeedBank.Instance.isNoCD)
		{
			CanPlace = true;
		}
		else if (CDTime > 7.5f)
		{
			CanPlace = false;
		}
		else
		{
			CanPlace = true;
		}
	}

	private void CheckState()
	{
		if (isStarted)
		{
			float sunNum = PlayerManager.Instance.GetSunNum(isNeedSun, GameManager.Instance.LocalPlayer.playerName);
			if (canPlace && sunNum >= (float)NeedSun)
			{
				CardState = CardState.CanPlace;
			}
			else if (!canPlace && sunNum >= (float)NeedSun)
			{
				CardState = CardState.NotCD;
			}
			else if (canPlace && sunNum < (float)NeedSun)
			{
				CardState = CardState.NotSun;
			}
			else if (!canPlace && sunNum < (float)NeedSun)
			{
				CardState = CardState.NotAll;
			}
		}
	}

	private void CDEnter()
	{
		if (CdCoroutine != null)
		{
			return;
		}
		if (CDTime == 0f || CanPlace)
		{
			CanPlace = true;
			return;
		}
		mask.localScale = new Vector3(mask.localScale.x, 2.2f);
		CdCoroutine = StartCoroutine(CalCD());
		if (SeedBank.Instance.isNoCD)
		{
			currTimeForCd = 0f;
		}
	}

	private IEnumerator CalCD()
	{
		float calCD = 2.2f / CDTime * 0.05f;
		for (currTimeForCd = CDTime; currTimeForCd >= 0f; currTimeForCd -= 0.05f)
		{
			yield return new WaitForSeconds(0.05f);
			mask.localScale = new Vector3(mask.localScale.x, mask.localScale.y - calCD);
		}
		CanPlace = true;
		CdCoroutine = null;
	}

	private void OnMouseOver()
	{
		if (MyTool.IsPointerOverGameObject() || !SeedBank.Instance.isCanClick || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		Shovel.Instance.CancelShovel();
		if (isStarted && isChoosed)
		{
			if (cardState != 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
				return;
			}
			if (!WantPlace)
			{
				SeedBank.Instance.AllCancelPlace();
				WantPlace = true;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GetPlant, base.transform.position, isAll: true);
			}
			if (plant != null && plant.BasePlant != 0)
			{
				MapManager.Instance.PlantFlash(plant.BasePlant);
			}
			secondClick = false;
		}
		else if (!isStarted && isChoosed && (CardPlantType != 0 || CardZombieType != 0))
		{
			ClearChoose();
		}
	}

	public void AddChoose(UIPlantCardNC nC)
	{
		isImitater = false;
		grayImg.material.SetInt("_OpenGray", 0);
		ImgRenderer.material.SetInt("_OpenGray", 0);
		myPlantCard = nC;
		isChoosed = true;
		if (nC.CardPlantType == PlantType.Imitater)
		{
			isImitater = true;
			grayImg.material.SetInt("_OpenGray", 1);
			ImgRenderer.material.SetInt("_OpenGray", 1);
			PlantCard preCard = SeedBank.Instance.GetPreCard(this);
			if (preCard != null)
			{
				UIPlantCardNC cardInfo = SeedChooser.Instance.GetCardInfo(preCard.CardPlantType);
				if (cardInfo == null)
				{
					myPlantCard = SeedChooser.Instance.GetCardInfo(1);
				}
				else
				{
					myPlantCard = cardInfo;
				}
			}
			else
			{
				myPlantCard = SeedChooser.Instance.GetCardInfo(1);
			}
		}
		CDTime = myPlantCard.CDTime;
		NeedSun = myPlantCard.NeedNum;
		isNeedSun = myPlantCard.isNeedSun;
		CardPlantType = myPlantCard.CardPlantType;
		CardZombieType = myPlantCard.CardZombieType;
		grayImg.sprite = myPlantCard.cardImage.sprite;
		ImgRenderer.sprite = myPlantCard.cardImage.sprite;
		WantSunText.text = myPlantCard.NeedNum.ToString();
		if (isImitater)
		{
			myPlantCard = SeedChooser.Instance.GetCardInfo(PlantType.Imitater);
		}
	}

	public void ClearChoose()
	{
		SeedBank.Instance.ClearChoose(myPlantCard, this);
		ImgRenderer.sprite = sprite;
		grayImg.sprite = sprite;
		WantSunText.text = "";
		isChoosed = false;
		if (GameManager.Instance.isClient)
		{
			SelectCard selectCard = new SelectCard();
			selectCard.plantType = CardPlantType;
			selectCard.zombieType = CardZombieType;
			selectCard.isBack = true;
			selectCard.cardId = CardId;
			SocketClient.Instance.SelectCard(selectCard);
		}
		if (GameManager.Instance.isServer)
		{
			SelectCard selectCard2 = new SelectCard();
			selectCard2.PlayerName = GameManager.Instance.LocalPlayer.playerName;
			selectCard2.plantType = CardPlantType;
			selectCard2.zombieType = CardZombieType;
			selectCard2.isBack = true;
			selectCard2.cardId = CardId;
			SocketServer.Instance.SelectCard(selectCard2);
		}
	}

	public void ClearChoose(bool noAnimation)
	{
		myPlantCard.IsChoosed = false;
	}

	public void ClearChooseOk()
	{
		myPlantCard.IsChoosed = false;
		NeedSun = 0;
		CDTime = 0f;
		CardPlantType = PlantType.Nope;
	}

	public void DestroyCardSlot()
	{
		CancelPlace();
		if (CdCoroutine != null)
		{
			StopCoroutine(CdCoroutine);
		}
		Object.Destroy(base.gameObject);
	}
}
