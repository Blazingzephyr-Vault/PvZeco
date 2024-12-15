using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class Cattail : PlantBase
{
	private int HitNum;

	private Vector3 creatBulletOffsetPos = new Vector2(0f, 0.86f);

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Cattail;

	public override PlantType BasePlant => PlantType.Lilypad;

	public override int BasePlantSunNum => 100;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => true;

	protected override int attackValue => 20;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "shoot")
			{
				if (swfClip.currentFrame == 48)
				{
					HitNum++;
					CreateBullet();
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1 && HitNum > 1)
				{
					clipController.clip.sequence = "idel";
					clipController.rateScale = base.SpeedRate;
					HitNum = 0;
				}
			}
		}
		else
		{
			if (swfClip.currentFrame == swfClip.frameCount - 1 && currGrid != null)
			{
				CheckAttack();
			}
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
		}
	}

	private void CheckAttack()
	{
		if (isSleeping || currGrid == null)
		{
			return;
		}
		List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno);
		bool flag = false;
		for (int i = 0; i < allZombies.Count; i++)
		{
			if (allZombies[i].capsuleCollider2D.enabled || allZombies[i] is BalloonZombie)
			{
				flag = true;
				break;
			}
		}
		List<PlantBase> list = null;
		if (!flag)
		{
			list = MapManager.Instance.GetAllPlant(base.transform.position, !isHypno);
		}
		if (flag || list.Count > 0)
		{
			clipController.clip.sequence = "shoot";
			clipController.rateScale = 2f * base.SpeedRate;
		}
	}

	private void CreateBullet()
	{
		if (currGrid != null)
		{
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
			}
			TrackBullet component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CattailBullet).GetComponent<TrackBullet>();
			component.transform.SetParent(null);
			component.Init(base.transform.position + creatBulletOffsetPos, attackValue, isHypno);
		}
	}
}
