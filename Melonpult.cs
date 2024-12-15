using FTRuntime;
using UnityEngine;

public class Melonpult : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.16f, 1.13f);

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Melonpult;

	protected override int attackValue => 54;

	private void CheckAttack()
	{
		if (currGrid != null && !isSleeping)
		{
			PlantBase plantBase = null;
			ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
			if (zombieByLineMinDistance == null)
			{
				plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
			}
			if (zombieByLineMinDistance == null && plantBase == null)
			{
				clipController.rateScale = base.SpeedRate;
				clipController.clip.sequence = "idel";
			}
			else
			{
				clipController.rateScale = 3f * base.SpeedRate;
				clipController.clip.sequence = "shoot";
			}
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "shoot")
			{
				if (swfClip.currentFrame == 62)
				{
					CreateMelon();
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					clipController.rateScale = base.SpeedRate;
					clipController.clip.sequence = "idel";
				}
			}
		}
		else
		{
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			if ((swfClip.currentFrame == 76 || swfClip.currentFrame == swfClip.frameCount - 1) && currGrid != null)
			{
				CheckAttack();
			}
		}
	}

	private void CreateMelon()
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
		if (zombieBase != null || plantBase != null)
		{
			Melon component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Melon).GetComponent<Melon>();
			component.transform.SetParent(null);
			Vector3 vector;
			if (base.IsFacingLeft)
			{
				vector = MyTool.ReverseX(creatBulletOffsetPos);
				component.transform.rotation = Quaternion.Euler(0f, 0f, 93f);
			}
			else
			{
				vector = creatBulletOffsetPos;
				component.transform.rotation = Quaternion.Euler(0f, 0f, 273f);
			}
			component.Init(plantBase, zombieBase, base.transform.position + vector, attackValue, GetBulletSortOrder(), isHypno);
		}
	}
}
