using FTRuntime;
using UnityEngine;

public class GatlingPea : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.6f, 0.2f);

	public SwfClipController ExtraClipController;

	public Animator HeadAnimator;

	private int ShootNum;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.GatlingPea;

	public override PlantType BasePlant => PlantType.Repeater;

	public override int BasePlantSunNum => 200;

	protected override int attackValue => 20;

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

	private void CreatePea()
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
			Pea component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(attackValue, base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(attackValue, base.transform.position + creatBulletOffsetPos, currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno);
			}
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "shoot")
		{
			if (swfClip.currentFrame == 55 || swfClip.currentFrame == 90 || swfClip.currentFrame == 120 || swfClip.currentFrame == 150)
			{
				CreatePea();
			}
			if (swfClip.currentFrame == 151 && ShootNum < 2)
			{
				ShootNum++;
				swfClip.currentFrame = 121;
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.rateScale = 1.5f * base.SpeedRate;
				clipController.clip.sequence = "idel";
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

	protected override void GameOverSpecial()
	{
		HeadAnimator.speed = 0f;
	}
}
