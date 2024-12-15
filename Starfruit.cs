using FTRuntime;
using UnityEngine;

public class Starfruit : PlantBase
{
	private bool NeedShoot;

	protected override int attackValue => 20;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Starfruit;

	protected override void OnInitForPlace()
	{
		NeedShoot = false;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "shoot")
			{
				if (swfClip.currentFrame == 50)
				{
					CreateStar(isCheck: false);
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					NeedShoot = false;
					clipController.clip.sequence = "idel";
				}
			}
			return;
		}
		if (swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
		if (NeedShoot)
		{
			clipController.clip.sequence = "shoot";
		}
		if (swfClip.currentFrame == 5)
		{
			if (NeedShoot)
			{
				clipController.clip.sequence = "shoot";
			}
			else
			{
				CreateStar(isCheck: true);
			}
		}
	}

	public void CheckZombieResult()
	{
		NeedShoot = true;
	}

	private void CreateStar(bool isCheck)
	{
		if (currGrid != null && !isSleeping && currGrid != null)
		{
			Star component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			component.Init(attackValue, base.transform.position, -1, Vector2.up, this, isCheck, isHypno);
			component.transform.SetParent(null);
			Star component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			component2.Init(attackValue, base.transform.position, -1, Vector2.down, this, isCheck, isHypno);
			component2.transform.SetParent(null);
			Star component3 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			if (base.IsFacingLeft)
			{
				component3.Init(attackValue, base.transform.position, currGrid.Point.y, Vector2.right, this, isCheck, isHypno);
			}
			else
			{
				component3.Init(attackValue, base.transform.position, currGrid.Point.y, Vector2.left, this, isCheck, isHypno);
			}
			component3.transform.SetParent(null);
			Star component4 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			Vector2 dirc = new Vector2(1.5f, 1f);
			if (base.IsFacingLeft)
			{
				dirc = new Vector2(-1.5f, 1f);
			}
			component4.Init(attackValue, base.transform.position, -1, dirc, this, isCheck, isHypno);
			component4.transform.SetParent(null);
			Star component5 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			dirc = new Vector2(1.5f, -1f);
			if (base.IsFacingLeft)
			{
				dirc = new Vector2(-1.5f, -1f);
			}
			component5.Init(attackValue, base.transform.position, -1, dirc, this, isCheck, isHypno);
			component5.transform.SetParent(null);
		}
	}
}
