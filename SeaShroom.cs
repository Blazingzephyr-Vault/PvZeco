using FTRuntime;
using UnityEngine;

public class SeaShroom : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.3f, -0.14f);

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.SeaShroom;

	protected override int attackValue => 20;

	protected override Vector2 offSet => new Vector2(0f, -0.35f);

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => true;

	protected override bool HaveShadow => false;

	public override bool CanCarryed => false;

	public override bool CanProtect => false;

	protected override bool isShroom => true;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "shoot")
			{
				if (swfClip.currentFrame == 36)
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
			if (swfClip.currentFrame == 61 || swfClip.currentFrame == swfClip.frameCount - 1)
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
		if (!isSleeping && currGrid != null)
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
