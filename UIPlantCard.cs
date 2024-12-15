using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPlantCard : MonoBehaviour
{
	public int CardId;

	private Image maskImg;

	private Image image;

	private Sprite sprite;

	private Text WantSunText;

	public int NeedSun;

	public bool isNeedSun;

	public float CDTime;

	public PlantType CardPlantType;

	public float currTimeForCd;

	private bool canPlace;

	private bool wantPlace;

	private PlantBase plant;

	private PlantBase plantInGrid;

	private CardState cardState;

	public bool isChoosed;

	private int currOrderNum = 3;

	public UIPlantCardNC myPlantCard;

	private Coroutine CDCoroutine;

	private bool isImitater;

	public OnlineSeedBank ownerSeedBank;

	public CardState CardState
	{
		get
		{
			return cardState;
		}
		set
		{
			if (cardState != value)
			{
				switch (value)
				{
				case CardState.CanPlace:
					maskImg.fillAmount = 0f;
					image.color = Color.white;
					break;
				case CardState.NotCD:
					image.color = new Color(0.75f, 0.75f, 0.75f);
					break;
				case CardState.NotSun:
					maskImg.fillAmount = 0f;
					image.color = new Color(0.75f, 0.75f, 0.75f);
					break;
				case CardState.NotAll:
					image.color = new Color(0.5f, 0.5f, 0.5f);
					break;
				}
				cardState = value;
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
			if (value > 30)
			{
				currOrderNum = 3;
			}
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
				plant = PlantManager.Instance.GetNewPlant(CardPlantType);
				plant.transform.SetParent(PlantManager.Instance.transform);
			}
			else if (plant != null)
			{
				plant.Dead();
				plant = null;
			}
		}
	}

	private void Start()
	{
		maskImg = base.transform.Find("mask").GetComponent<Image>();
		WantSunText = base.transform.Find("CardSunNumText").GetComponent<Text>();
		image = GetComponent<Image>();
		maskImg.material = Object.Instantiate(maskImg.material);
		image.material = Object.Instantiate(image.material);
		sprite = image.sprite;
		PlayerManager.Instance.AddSunNumUpdateActionListener(CheckState);
		LVManager.Instance.AddLVStartActionListenr(OnLVStartAction);
		isChoosed = false;
		CanPlace = true;
	}

	private void OnLVStartAction()
	{
		if (CDTime > 7.5f)
		{
			CanPlace = false;
			CDEnter();
		}
		else
		{
			CanPlace = true;
		}
	}

	private void CheckState()
	{
		if (canPlace && PlayerManager.Instance.GetSunNum(isNeedSun, ownerSeedBank.OwnerShow.nameText.name) >= (float)NeedSun)
		{
			CardState = CardState.CanPlace;
		}
		else if (!canPlace && PlayerManager.Instance.GetSunNum(isNeedSun, ownerSeedBank.OwnerShow.nameText.name) >= (float)NeedSun)
		{
			CardState = CardState.NotCD;
		}
		else if (canPlace && PlayerManager.Instance.GetSunNum(isNeedSun, ownerSeedBank.OwnerShow.nameText.name) < (float)NeedSun)
		{
			CardState = CardState.NotSun;
		}
		else if (!canPlace && PlayerManager.Instance.GetSunNum(isNeedSun, ownerSeedBank.OwnerShow.nameText.name) < (float)NeedSun)
		{
			CardState = CardState.NotAll;
		}
	}

	public void CDEnter()
	{
		if (CDCoroutine != null)
		{
			StopCoroutine(CDCoroutine);
		}
		CanPlace = false;
		maskImg.fillAmount = 1f;
		CDCoroutine = StartCoroutine(CalCD());
	}

	private IEnumerator CalCD()
	{
		float calCD = 1f / CDTime * 0.1f;
		for (currTimeForCd = CDTime; currTimeForCd >= 0f; currTimeForCd -= 0.1f)
		{
			yield return new WaitForSeconds(0.1f);
			maskImg.fillAmount -= calCD;
		}
		CanPlace = true;
		CDCoroutine = null;
	}

	public void AddChoose(UIPlantCardNC nC)
	{
		isImitater = false;
		maskImg.material.SetInt("_OpenGray", 0);
		image.material.SetInt("_OpenGray", 0);
		myPlantCard = nC;
		isChoosed = true;
		if (myPlantCard.CardPlantType == PlantType.Imitater)
		{
			isImitater = true;
			image.material.SetInt("_OpenGray", 1);
			maskImg.material.SetInt("_OpenGray", 1);
			UIPlantCard preCard = ownerSeedBank.GetPreCard(this);
			if (preCard == null)
			{
				myPlantCard = SeedChooser.Instance.GetCardInfo(1);
			}
			else
			{
				myPlantCard = SeedChooser.Instance.GetCardInfo(preCard.CardPlantType);
			}
		}
		NeedSun = myPlantCard.NeedNum;
		isNeedSun = myPlantCard.isNeedSun;
		CDTime = myPlantCard.CDTime;
		CardPlantType = myPlantCard.CardPlantType;
		image.sprite = myPlantCard.cardImage.sprite;
		maskImg.sprite = myPlantCard.cardImage.sprite;
		WantSunText.text = NeedSun.ToString();
		if (isImitater)
		{
			myPlantCard = SeedChooser.Instance.GetCardInfo(PlantType.Imitater);
		}
	}

	public void ClearChoose()
	{
		image.sprite = sprite;
		maskImg.sprite = sprite;
		WantSunText.text = "";
		isChoosed = false;
		NeedSun = 0;
		CDTime = 0f;
		CardPlantType = PlantType.Nope;
	}

	public void ClearChoose(bool noAnimation)
	{
		myPlantCard.IsChoosed = false;
	}

	public void ClearChooseOk()
	{
	}

	public void DestroyCardSlot()
	{
		Object.Destroy(base.gameObject);
	}
}
