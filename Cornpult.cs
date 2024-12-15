using FTRuntime;
using SocketSave;
using UnityEngine;

public class Cornpult : PlantBase
{
	public Texture2D butter;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Cornpult;

	protected override int attackValue => 40;

	private void CheckAttack()
	{
		if (isSleeping || currGrid == null)
		{
			return;
		}
		PlantBase plantBase = null;
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (zombieByLineMinDistance == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
		}
		if (zombieByLineMinDistance == null && plantBase == null)
		{
			clipController.clip.sequence = "idel";
			clipController.rateScale = base.SpeedRate;
			REnderer.material.SetTexture("_SpecialTex", null);
			return;
		}
		clipController.clip.sequence = "shoot";
		clipController.rateScale = 2f * base.SpeedRate;
		if (!GameManager.Instance.isClient && Random.Range(0, 5) > 3)
		{
			REnderer.material.SetTexture("_SpecialTex", butter);
		}
		if (GameManager.Instance.isServer && REnderer.material.GetTexture("_SpecialTex") == butter)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = 1;
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	public override void OnlineSynPlant(SynItem syn)
	{
		base.OnlineSynPlant(syn);
		if (syn.SynCode[0] == 0)
		{
			REnderer.material.SetTexture("_SpecialTex", butter);
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "shoot")
			{
				if (swfClip.currentFrame == 51)
				{
					CreateBullet();
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					clipController.clip.sequence = "idel";
					clipController.rateScale = base.SpeedRate;
				}
			}
		}
		else
		{
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1 && currGrid != null)
			{
				CheckAttack();
			}
		}
	}

	private void CreateBullet()
	{
		if (currGrid == null)
		{
			return;
		}
		if (Random.Range(0, 2) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
		}
		PlantBase plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
		ZombieBase zombieBase = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (zombieBase != null && plantBase != null)
		{
			if (Mathf.Abs(plantBase.transform.position.x - base.transform.position.x) >= Mathf.Abs(zombieBase.transform.position.x - base.transform.position.x))
			{
				plantBase = null;
			}
			else
			{
				zombieBase = null;
			}
		}
		if (!(zombieBase != null) && !(plantBase != null))
		{
			return;
		}
		if (REnderer.material.GetTexture("_SpecialTex") == butter)
		{
			Butter component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Butter).GetComponent<Butter>();
			component.transform.SetParent(null);
			component.transform.rotation = Quaternion.Euler(0f, 0f, -63f);
			Vector3 vector = ((!base.IsFacingLeft) ? new Vector3(-0.145f, 1.2f) : MyTool.ReverseX(new Vector3(-0.145f, 1.2f)));
			component.Init(plantBase, zombieBase, base.transform.position + vector, attackValue, GetBulletSortOrder(), isHypno);
			REnderer.material.SetTexture("_SpecialTex", null);
			return;
		}
		Kernal component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Kernal).GetComponent<Kernal>();
		component2.transform.SetParent(null);
		Vector3 vector2;
		if (base.IsFacingLeft)
		{
			vector2 = MyTool.ReverseX(new Vector3(-0.22f, 1.1f));
			component2.transform.rotation = Quaternion.Euler(0f, 0f, 194f);
		}
		else
		{
			vector2 = new Vector3(-0.22f, 1.1f);
			component2.transform.rotation = Quaternion.Euler(0f, 0f, 14f);
		}
		component2.Init(plantBase, zombieBase, base.transform.position + vector2, 20, GetBulletSortOrder(), isHypno);
	}
}
