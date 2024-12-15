using FTRuntime;
using UnityEngine;

public class Cabbagepult : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Cabbagepult;

	protected override int attackValue => 40;

	private void CheckAttack()
	{
		if (!isSleeping && currGrid != null)
		{
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
			}
			else
			{
				clipController.clip.sequence = "shoot";
				clipController.rateScale = 3f * base.SpeedRate;
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
				if (swfClip.currentFrame == 60)
				{
					CreateCabbage();
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

	private void CreateCabbage()
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
			Cabbage component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Cabbage).GetComponent<Cabbage>();
			component.transform.SetParent(null);
			Vector3 vector;
			if (base.IsFacingLeft)
			{
				vector = MyTool.ReverseX(new Vector3(-0.27f, 0.75f, 0f));
				component.transform.rotation = Quaternion.Euler(0f, 0f, 123f);
			}
			else
			{
				vector = new Vector3(-0.27f, 0.75f, 0f);
				component.transform.rotation = Quaternion.Euler(0f, 0f, -57f);
			}
			component.Init(plantBase, zombieBase, base.transform.position + vector, attackValue, GetBulletSortOrder(), isHypno);
		}
	}
}
