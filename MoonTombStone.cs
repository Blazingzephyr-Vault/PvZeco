using SocketSave;
using UnityEngine;

public class MoonTombStone : PlantBase
{
	private float createSunTime = 24f;

	private float lightTime = 1.5f;

	public Sprite State2;

	public Sprite State3;

	private int state1;

	private int state2;

	public override float MaxHp => 500f;

	protected override PlantType plantType => PlantType.MoonTombStone;

	public override bool CanCarryed => false;

	public override bool IsZombiePlant => true;

	public override bool CanProtect => false;

	protected override void OnInitForAll()
	{
		clipController.clip.NewSprite = null;
	}

	protected override void OnInitForPlace()
	{
		state1 = (int)MaxHp / 3 * 2;
		state2 = (int)MaxHp / 3;
		if (!GameManager.Instance.isClient)
		{
			InvokeRepeating("CreateMoon", createSunTime, createSunTime);
			needFlatDead = false;
		}
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (base.Hp <= (float)state1 && base.Hp >= (float)state2)
		{
			clipController.clip.NewSprite = State2;
		}
		else if (base.Hp <= (float)state2)
		{
			clipController.clip.NewSprite = State3;
		}
		else
		{
			clipController.clip.NewSprite = null;
		}
	}

	private void CreateMoon()
	{
		SkyManager.Instance.GetShadowNum(currGrid);
		if (GameManager.Instance.isServer)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = 1;
			SocketServer.Instance.SendSynBag(synItem);
		}
		StartCoroutine(BrightnessEffect2(lightTime, InstantiateMoon));
	}

	public override void OnlineSynPlant(SynItem syn)
	{
		base.OnlineSynPlant(syn);
		if (syn.SynCode[0] == 0)
		{
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateMoon));
		}
	}

	private void InstantiateMoon()
	{
		SkyManager.Instance.CreatePlantSun(base.transform.position, 25f, isSun: false, PlacePlayer);
	}

	protected override void GoSleepSpecial()
	{
		GoAwake();
		ZZZ.gameObject.SetActive(value: false);
	}
}
