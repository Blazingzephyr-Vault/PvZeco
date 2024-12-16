using UnityEngine;

namespace StartScene;

public class AlmanacCardSlot : MonoBehaviour
{
	public PlantType plantType;

	public ZombieType zombieType;

	public SpriteRenderer REnderer;

	public TextMesh NeedNumText;

	private Sprite NomalSprite;

	private float CdTime;

	private void Start()
	{
		NomalSprite = REnderer.sprite;
	}

	public void UpdateInfo(UIPlantCardNC nC)
	{
		plantType = nC.CardPlantType;
		zombieType = nC.CardZombieType;
		if (nC.CardPlantType != 0 || nC.CardZombieType != 0)
		{
			REnderer.sprite = nC.cardImage.sprite;
			NeedNumText.text = nC.GetCost().ToString();
			CdTime = nC.GetCooldown();
		}
		else
		{
			REnderer.sprite = NomalSprite;
			NeedNumText.text = "";
			CdTime = 0f;
		}
	}

	public void ResetInfo()
	{
		plantType = PlantType.Nope;
		REnderer.sprite = NomalSprite;
		NeedNumText.text = "";
		CdTime = 0f;
	}

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject() && (plantType != 0 || zombieType != 0))
		{
			REnderer.material.SetFloat("_Brightness", 1.3f);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
		}
	}

	private void OnMouseExit()
	{
		REnderer.material.SetFloat("_Brightness", 1f);
	}

	private void OnMouseDown()
	{
		if ((plantType != 0 || zombieType != 0) && !MyTool.IsPointerOverGameObject())
		{
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
			}
			LoadThis();
		}
	}

	public void LoadThis()
	{
		AlmcPlantLoader.Instance.LoadPlant(plantType, zombieType, NeedNumText.text, CdTime);
	}
}
