using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class Cactus : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.6f, 0.2f);

	private ZombieBase TargetZombie;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Cactus;

	protected override int attackValue => 20;

	private void CheckAttack()
	{
		if (isSleeping || currGrid == null)
		{
			return;
		}
		FindMinDisBalloon();
		if (TargetZombie == null)
		{
			ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
			PlantBase plantBase = null;
			if (zombieByLineMinDistance == null)
			{
				plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
			}
			if (zombieByLineMinDistance == null && plantBase == null)
			{
				clipController.clip.sequence = "idel";
			}
			else
			{
				clipController.clip.sequence = "shoot";
			}
		}
		else
		{
			clipController.clip.sequence = "rise";
		}
	}

	private void FindMinDisBalloon()
	{
		TargetZombie = null;
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno, needCapsule: false);
		if (zombiesByLine.Count <= 0)
		{
			return;
		}
		List<ZombieBase> list = new List<ZombieBase>();
		for (int i = 0; i < zombiesByLine.Count; i++)
		{
			if (zombiesByLine[i] is BalloonZombie && zombiesByLine[i].GetComponent<BalloonZombie>().IsFly())
			{
				list.Add(zombiesByLine[i]);
			}
		}
		float num = 999999f;
		for (int j = 0; j < list.Count; j++)
		{
			if (Vector2.Distance(base.transform.position, list[j].transform.position) < num && list[j].Hp > 0)
			{
				num = Vector2.Distance(base.transform.position, list[j].transform.position);
				TargetZombie = list[j];
			}
		}
	}

	private void HighCheckAttack()
	{
		FindMinDisBalloon();
		if (TargetZombie == null)
		{
			clipController.clip.sequence = "down";
		}
	}

	private void CreateThron(Vector3 pos)
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
			Thron component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Thron).GetComponent<Thron>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(attackValue, base.transform.position + MyTool.ReverseX(pos), currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno, TargetZombie);
			}
			else
			{
				component.Init(attackValue, base.transform.position + pos, currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno, TargetZombie);
			}
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "idel":
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			if (swfClip.currentFrame == 0 && currGrid != null)
			{
				CheckAttack();
			}
			break;
		case "shoot":
			if (swfClip.currentFrame == 55)
			{
				CreateThron(creatBulletOffsetPos);
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1 && currGrid != null)
			{
				CheckAttack();
			}
			break;
		case "rise":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "shoothigh";
			}
			break;
		case "down":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "idel";
			}
			break;
		case "shoothigh":
			if (swfClip.currentFrame == 36)
			{
				CreateThron(new Vector3(1.15f, 1.3f));
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1 && currGrid != null)
			{
				HighCheckAttack();
			}
			break;
		}
	}
}
