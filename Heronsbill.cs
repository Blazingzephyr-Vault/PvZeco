using FTRuntime;
using SocketSave;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Heronsbill : PlantBase
{
    private Vector3 creatBulletOffsetPos = new Vector2(0f, -0.15f);
    public override float MaxHp => 200f;
    protected override PlantType plantType => PlantType.Heronsbill;
    public override bool isProtectPlant => true;
    protected override Vector2 offSet => new Vector2(0f, -0.1f);
    public override bool CanProtect => false;

    protected override void OnInitForPlace()
    {
        int num = clipController.clip.sortingOrder / 100 * 100;
        clipController.clip.sortingOrder = num + 51;
        InvokeRepeating("CreateSun", 20f, 20f);
    }

    private float lightTime = 1.5f;
    private void CreateSun()
    {
        if (isSleeping)
        {
            return;
        }
        int shadowNum = SkyManager.Instance.GetShadowNum(currGrid);
        if (Random.Range(0, shadowNum) > shadowNum - 3)
        {
            if (GameManager.Instance.isServer)
            {
                SynItem synItem = new SynItem();
                synItem.OnlineId = OnlineId;
                synItem.Type = 1;
                SocketServer.Instance.SendSynBag(synItem);
            }
            StartCoroutine(BrightnessEffect2(lightTime, CreateFume));
        }
    }

    private void CreateFume()
    {
        if (currGrid == null)
        {
            return;
        }
        List<ZombieBase> zombies = ZombieManager.Instance.GetZombiesByLine(currGrid.Point.y, base.transform.position, IsFacingLeft, isHypno);
        List<PlantBase> linePlant = MapManager.Instance.GetLinePlant(base.transform.position, currGrid.Point.y, 15f, !isHypno);
        for (int i = 0; i < zombies.Count; i++)
        {
            if (base.IsFacingLeft && zombies[i].transform.position.x < base.transform.position.x)
            {
                zombies[i].Hurt(attackValue, Vector2.zero);
                zombies[i].ApplyScaredyStall(1f, 10);
            }
            else if (!base.IsFacingLeft && zombies[i].transform.position.x > base.transform.position.x)
            {
                zombies[i].Hurt(attackValue, Vector2.zero);
                zombies[i].ApplyScaredyStall(1f, 10);
            }
        }
        for (int j = 0; j < linePlant.Count; j++)
        {
            if (base.IsFacingLeft && linePlant[j].transform.position.x < base.transform.position.x)
            {
                linePlant[j].Hurt(attackValue, null);
                linePlant[j].ApplyScaredyStall(1f, 10);
            }
            else if (!base.IsFacingLeft && linePlant[j].transform.position.x > base.transform.position.x)
            {
                linePlant[j].Hurt(attackValue, null);
                linePlant[j].ApplyScaredyStall(1f, 10);
            }
        }

        EffectPanel.Instance.Spark(new Color(0.98f, 0.29f, 0.95f, 0.8f), 0.1f, base.transform.position);
        AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FrozenPea, base.transform.position);
        PoolManager.Instance.GetObj(GameManager.Instance.GameConf.IceParticle).transform.position = base.transform.position;

        //Object.Instantiate(GameManager.Instance.GameConf.JalapenoBoom).GetComponent<JalapenoBoom>().CreateInit(currGrid, GetBulletSortOrder());
        CameraControl.Instance.ShakeCamera(base.transform.position);
        /*
        GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ShroomFumeParticle);
        obj.transform.localScale = new Vector3(Mathf.Abs(obj.transform.localScale.x), obj.transform.localScale.y);
        obj.GetComponent<SortingGroup>().sortingOrder = GetBulletSortOrder();
        if (base.IsFacingLeft)
        {
            obj.transform.position = base.transform.position + MyTool.ReverseX(creatBulletOffsetPos);
            obj.transform.localScale = new Vector3(0f - obj.transform.localScale.x, obj.transform.localScale.y);
        }
        else
        {
            obj.transform.position = base.transform.position + creatBulletOffsetPos;
        }*/
    }

    protected override void FrameChangeEvent(SwfClip swfClip)
    {
        if (swfClip.currentFrame == closeEyeFrame)
        {
            StartCloseEyes();
        }
    }

    /*private float createSunTime = 24f;

	private float lightTime = 1.5f;

	protected override void OnInitForPlace()
	{
		InvokeRepeating("CreateSun", createSunTime, createSunTime);
	}

	private void CreateSun()
	{
		if (isSleeping)
		{
			return;
		}
		int shadowNum = SkyManager.Instance.GetShadowNum(currGrid);
		if (Random.Range(0, shadowNum) > shadowNum - 3)
		{
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
		}
	}

	private void InstantiateSun()
	{
		if (currGrid != null)
		{
			SkyManager.Instance.CreatePlantSun(base.transform.position, 10 + currGrid.LightNum * 2, isSun: true, PlacePlayer);
		}
	}

	public override void OnlineSynPlant(SynItem syn)
	{
		base.OnlineSynPlant(syn);
		if (syn.SynCode[0] == 0)
		{
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
		}
	}*/
}
