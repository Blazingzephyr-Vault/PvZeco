using SocketSave;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPlantCardNC : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
    #region Major New Logic
    /// <summary>
    /// Edit: patch the seed chooser card's sun cost.
    /// Reasoning: no way to edit the costs via assets at the moment.
    /// </summary>
    public int GetCost()
	{
		return (CardPlantType, CardZombieType) switch
		{
			(PlantType.ScaredyShroom, _)	=>	275,
			_								=>	NeedNum
		};
	}

    /// <summary>
    /// Edit: patch the seed chooser card's cooldown time.
    /// Reasoning: no way to edit the costs via assets at the moment.
    /// </summary>
    public float GetCooldown()
    {
        return (CardPlantType, CardZombieType) switch
        {
            (PlantType.ScaredyShroom, _)	=>	30,
            _								=>	CDTime
        };
    }
    #endregion

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

        // Patch the data after the script is activated.
        // No access to the Editor at the moment.
        NeedNum = GetCost();
        CDTime = GetCooldown();
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
