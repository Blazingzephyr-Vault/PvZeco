using SocketSave;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPlantCardNC : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	private bool isChoosed;

	public bool isNeedSun;

	public int NeedNum;

	public float CDTime;

	public PlantType CardPlantType;

	public ZombieType CardZombieType;

	public Image cardImage { get; private set; }

	public bool IsChoosed
	{
		get
		{
			return isChoosed;
		}
		set
		{
			isChoosed = value;
			if (value)
			{
				cardImage.color = new Color(0.3f, 0.3f, 0.3f);
			}
			else
			{
				cardImage.color = new Color(1f, 1f, 1f);
			}
		}
	}

	private void Start()
	{
		cardImage = base.transform.GetComponent<Image>();
		IsChoosed = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!IsChoosed && (CardPlantType != 0 || CardZombieType != 0) && !SeedBank.Instance.isFull && !SeedChooser.Instance.isPrepare)
		{
			if (SeedBank.Instance.ChooseCard(this))
			{
				IsChoosed = true;
				if (Random.Range(0, 2) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
				}
			}
			if (GameManager.Instance.isClient)
			{
				SelectCard selectCard = new SelectCard();
				selectCard.plantType = CardPlantType;
				selectCard.zombieType = CardZombieType;
				selectCard.isBack = false;
				SocketClient.Instance.SelectCard(selectCard);
			}
			if (GameManager.Instance.isServer)
			{
				SelectCard selectCard2 = new SelectCard();
				selectCard2.PlayerName = GameManager.Instance.LocalPlayer.playerName;
				selectCard2.plantType = CardPlantType;
				selectCard2.zombieType = CardZombieType;
				selectCard2.isBack = false;
				SocketServer.Instance.SelectCard(selectCard2);
			}
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
		}
	}
}
