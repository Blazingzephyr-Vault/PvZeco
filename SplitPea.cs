using FTRuntime;
using UnityEngine;

public class SplitPea : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.6f, 0.2f);

	public SwfClipController ExtraClipController;

	public Animator HeadAnimator;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.SplitPea;

	protected override int attackValue => 20;

	private void CheckAttack()
	{
		if (isSleeping || currGrid == null)
		{
			return;
		}
		ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(currGrid.Point.y, base.transform.position, isHypno);
		PlantBase plantBase = null;
		if (zombieByLineMinDisNoDir == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
			if (plantBase == null)
			{
				plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, !base.IsFacingLeft, !isHypno);
			}
		}
		if (zombieByLineMinDisNoDir == null && plantBase == null)
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
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "shoot")
			{
				if (swfClip.currentFrame == 60)
				{
					CreateFrontPea();
				}
				if (swfClip.currentFrame == 45 || swfClip.currentFrame == 105)
				{
					CreateBackPea();
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					clipController.rateScale = 1.5f * base.SpeedRate;
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
			if (swfClip.currentFrame == swfClip.frameCount - 1 && currGrid != null)
			{
				CheckAttack();
			}
		}
	}

	private void CreateFrontPea()
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

	private void CreateBackPea()
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
			if (!base.IsFacingLeft)
			{
				component.Init(attackValue, base.transform.position + new Vector3(-0.9f, 0.28f, 0f), currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(attackValue, base.transform.position + MyTool.ReverseX(new Vector3(-0.9f, 0.28f, 0f)), currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno);
			}
		}
	}

	protected override void GameOverSpecial()
	{
		HeadAnimator.speed = 0f;
	}
}
