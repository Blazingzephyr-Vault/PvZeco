using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class ScaredyShroom : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.48f, -0.14f);

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.ScaredyShroom;

	protected override int attackValue => 20;

	protected override bool isShroom => true;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "idel":
			if ((swfClip.currentFrame == 0 || swfClip.currentFrame == 36) && currGrid != null)
			{
				CheackScare();
				CheckAttack();
			}
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			break;
		case "shoot":
			if (swfClip.currentFrame == 42)
			{
				CreatePuff();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				CheckAttack();
				CheackScare();
			}
			break;
		case "scared":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "scaredidel";
			}
			break;
		case "scaredidel":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				CheackNoScare();
			}
			break;
		case "grow":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "idel";
			}
			break;
		}
	}

	private void CheackScare()
	{
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.25f, needCapsule: false, isHypno);
		List<PlantBase> list = null;
		if (zombies.Count == 0)
		{
			list = MapManager.Instance.GetAroundPlant(base.transform.position, 2.25f, !isHypno);
		}
		if (zombies.Count > 0 || list.Count > 0)
		{
			clipController.clip.sequence = "scared";
		}
	}

	private void CheackNoScare()
	{
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.25f, needCapsule: false, isHypno);
		List<PlantBase> list = null;
		if (zombies.Count == 0)
		{
			list = MapManager.Instance.GetAroundPlant(base.transform.position, 2.25f, !isHypno);
		}
		if (zombies.Count == 0 && list.Count == 0)
		{
			clipController.clip.sequence = "grow";
		}
	}

	private void CheckAttack()
	{
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		PlantBase plantBase = null;
		if (zombieByLineMinDistance == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
		}
		if (zombieByLineMinDistance == null && plantBase == null)
		{
			if (clipController.clip.sequence != "scared")
			{
				clipController.clip.sequence = "idel";
			}
		}
		else if (clipController.clip.sequence != "scared")
		{
			clipController.clip.sequence = "shoot";
		}
	}

	private void CreatePuff()
	{
		if (currGrid != null)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Puff, base.transform.position);
			ShroomPuff component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ShroomPuff).GetComponent<ShroomPuff>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(attackValue, base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), currGrid.Point.y, Vector2.left, NeedDis: false, isHypno);
			}
			else
			{
				component.Init(attackValue, base.transform.position + creatBulletOffsetPos, currGrid.Point.y, Vector2.right, NeedDis: false, isHypno);
			}
		}
	}

	protected override void GoSleepSpecial()
	{
		clipController.clip.sequence = "sleep";
	}
}
