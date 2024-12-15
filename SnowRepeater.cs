using FTRuntime;
using UnityEngine;

public class SnowRepeater : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.6f, 0.2f);

	public SwfClipController ExtraClipController;

	public Animator HeadAnimator;

	private int ShootNum;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.SnowRepeater;

	protected override int attackValue => 20;

	public override PlantType BasePlant => PlantType.SnowPea;

	public override int BasePlantSunNum => 175;

	private void CheckAttack()
	{
		if (!isSleeping && currGrid != null)
		{
			ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
			PlantBase plantBase = null;
			if (zombieByLineMinDistance == null)
			{
				plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
			}
			if (zombieByLineMinDistance == null && plantBase == null)
			{
				clipController.rateScale = 1.5f * base.SpeedRate;
				clipController.clip.sequence = "idel";
			}
			else
			{
				clipController.rateScale = 4f * base.SpeedRate;
				clipController.clip.sequence = "shoot";
			}
		}
	}

	protected override void OnInitForAlmanac()
	{
		clipController.clip.sequence = "idel";
		clipController.rateScale = 1.5f * base.SpeedRate;
		HeadAnimator.speed = base.SpeedRate;
	}

	protected override void OnInitForAll()
	{
		HeadAnimator.speed = 0f;
		HeadAnimator.Play("SnowRepeater", 0, 0f);
	}

	protected override void OnInitForPlace()
	{
		clipController.clip.sequence = "idel";
		clipController.rateScale = 1.5f * base.SpeedRate;
		HeadAnimator.speed = base.SpeedRate;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "shoot")
		{
			if (swfClip.currentFrame == 60)
			{
				CreatePea();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				if (ShootNum < 1)
				{
					ShootNum++;
					swfClip.currentFrame = 0;
				}
				else
				{
					clipController.rateScale = 1.5f * base.SpeedRate;
					clipController.clip.sequence = "idel";
				}
			}
		}
		if (swfClip.sequence == "idel")
		{
			if (swfClip.currentFrame == 0 && ShootNum < 1)
			{
				CheckAttack();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				ShootNum = 0;
			}
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
		}
	}

	private void CreatePea()
	{
		if (!isSleeping && currGrid != null)
		{
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
			}
			SnowPea component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.FrozenPea1).GetComponent<SnowPea>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(attackValue, base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno, ShootNum + 1);
			}
			else
			{
				component.Init(attackValue, base.transform.position + creatBulletOffsetPos, currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno, ShootNum + 1);
			}
		}
	}

	protected override void GameOverSpecial()
	{
		HeadAnimator.speed = 0f;
	}
}
