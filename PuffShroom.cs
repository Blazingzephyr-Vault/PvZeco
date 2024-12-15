using FTRuntime;
using UnityEngine;

public class PuffShroom : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.3f, -0.34f);

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.PuffShroom;

	protected override int attackValue => 20;

	protected override bool isShroom => true;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "shoot")
			{
				if (swfClip.currentFrame == 42)
				{
					CreatePuff();
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					clipController.clip.sequence = "idel";
				}
			}
		}
		else
		{
			if (swfClip.currentFrame == 41 || swfClip.currentFrame == swfClip.frameCount - 1)
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
		if (currGrid != null && !isSleeping)
		{
			ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
			PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
			if ((zombieByLineMinDistance != null && Mathf.Abs(zombieByLineMinDistance.transform.position.x - base.transform.position.x) < 4.9f) || (minDisPlant != null && Mathf.Abs(minDisPlant.transform.position.x - base.transform.position.x) < 4.9f))
			{
				clipController.clip.sequence = "shoot";
			}
			else
			{
				clipController.clip.sequence = "idel";
			}
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
				component.Init(attackValue, base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), currGrid.Point.y, Vector2.left, NeedDis: true, isHypno);
			}
			else
			{
				component.Init(attackValue, base.transform.position + creatBulletOffsetPos, currGrid.Point.y, Vector2.right, NeedDis: true, isHypno);
			}
		}
	}
}
