using SocketSave;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shovel : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public static Shovel Instance;

	private Transform shovelImg;

	private bool isShovel;

	private Grid CurrGrid;

	public bool IsShovel
	{
		get
		{
			return isShovel;
		}
		set
		{
			if (LVManager.Instance.GameIsStart && !SpectatorList.Instance.LocalIsSpectator)
			{
				SeedBank.Instance.AllCancelPlace();
				CobCannonTarget.Instance.StopAim();
				isShovel = value;
				if (IsShovel)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Shovel, base.transform.position, isAll: true);
					shovelImg.localRotation = Quaternion.Euler(0f, 0f, -10f);
				}
				else
				{
					UpdateOnlinePreview(default(Vector2), isShow: false);
					shovelImg.localRotation = Quaternion.Euler(0f, 0f, 0f);
					shovelImg.transform.position = base.transform.position;
				}
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	public void CancelShovel()
	{
		isShovel = false;
		shovelImg.localRotation = Quaternion.Euler(0f, 0f, 0f);
		shovelImg.transform.position = base.transform.position;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		IsShovel = !IsShovel;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		shovelImg.transform.localScale = new Vector2(1.2f, 1.2f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		shovelImg.transform.localScale = new Vector2(1f, 1f);
	}

	private void Start()
	{
		shovelImg = base.transform.Find("shovel");
	}

	private void Update()
	{
		if (!UIManager.Instance.IsChatBoxOpen && Input.GetKeyDown(KeyCode.Alpha1))
		{
			IsShovel = !IsShovel;
		}
		if (!IsShovel)
		{
			return;
		}
		shovelImg.position = Input.mousePosition;
		Grid gridPointByMouse = MapManager.Instance.GetGridPointByMouse();
		if (gridPointByMouse == null)
		{
			return;
		}
		Vector2 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (Vector2.Distance(vector, gridPointByMouse.Position) < 1f)
		{
			if (CurrGrid == null)
			{
				CurrGrid = gridPointByMouse;
				UpdateOnlinePreview(gridPointByMouse.Position, isShow: true);
			}
			else if (CurrGrid != gridPointByMouse)
			{
				CurrGrid = gridPointByMouse;
				UpdateOnlinePreview(gridPointByMouse.Position, isShow: true);
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (gridPointByMouse.CurrPlantBase == null)
			{
				return;
			}
			if (gridPointByMouse.CurrPlantBase != null && Vector2.Distance(vector, gridPointByMouse.Position) < 1f)
			{
				if (GameManager.Instance.isClient)
				{
					ShovelApply shovelApply = new ShovelApply();
					shovelApply.GridPos = gridPointByMouse.Position;
					SocketClient.Instance.ApplyShovel(shovelApply);
				}
				else
				{
					ClearPlant(gridPointByMouse, vector, GameManager.Instance.LocalPlayer.playerName);
				}
				IsShovel = false;
			}
			else if (Vector2.Distance(vector, gridPointByMouse.Position) > 1.6f)
			{
				IsShovel = false;
			}
		}
		if (Input.GetMouseButtonDown(1) && IsShovel)
		{
			IsShovel = false;
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
			}
		}
	}

	private void UpdateOnlinePreview(Vector2 pos, bool isShow)
	{
		if (GameManager.Instance.isOnline)
		{
			ShovelPreview shovelPreview = new ShovelPreview();
			shovelPreview.GridPos = pos;
			shovelPreview.isShow = isShow;
			shovelPreview.PlayerName = GameManager.Instance.LocalPlayer.playerName;
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.ApplyShovelPreview(shovelPreview);
			}
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.ShovelPreview(shovelPreview, null);
			}
		}
	}

	public void ClearPlant(Grid grid, Vector2 ClickedPos, string applicant)
	{
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			if (!PvPSelector.Instance.IsSameTeam(applicant))
			{
				grid = MapManager.Instance.GetGridByWorldPos(MyTool.ReverseX(grid.Position));
				ClickedPos = MyTool.ReverseX(ClickedPos);
			}
			if (grid.CurrPlantBase != null && !PvPSelector.Instance.IsSameTeam(grid.CurrPlantBase.PlacePlayer, applicant))
			{
				return;
			}
		}
		BattlePlayerList.Instance.PlayShovelAnimation(grid.Position);
		if (!(grid.CurrPlantBase != null) || !(Vector2.Distance(ClickedPos, grid.CurrPlantBase.transform.position) < 1.5f))
		{
			return;
		}
		if (grid.CurrPlantBase.CarryPlant == null)
		{
			if (grid.CurrPlantBase.ProtectPlant != null && grid.CurrPlantBase.CanPlaceOnWater)
			{
				grid.CurrPlantBase.ProtectPlant.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
			}
			else if (grid.CurrPlantBase.ProtectPlant != null && grid.CurrPlantBase.CanCarryOtherPlant)
			{
				grid.CurrPlantBase.ProtectPlant.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
			}
			else
			{
				grid.CurrPlantBase.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
			}
		}
		else
		{
			grid.CurrPlantBase.CarryPlant.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
		}
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Plant1, base.transform.position, isAll: true);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Plant2, base.transform.position, isAll: true);
		}
	}
}
