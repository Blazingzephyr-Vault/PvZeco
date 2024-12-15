using System.Collections.Generic;
using FTRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class FumeShroom : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(1.2f, 0.24f);

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.FumeShroom;

	protected override int attackValue => 20;

	protected override bool isShroom => true;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "shoot")
			{
				if (swfClip.currentFrame == 50)
				{
					CreateFume();
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
			if (swfClip.currentFrame == 40 || swfClip.currentFrame == swfClip.frameCount - 1)
			{
				CheckAttack();
			}
		}
	}

	private void CheckAttack()
	{
		if (!isSleeping && currGrid != null)
		{
			ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
			PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
			if ((zombieByLineMinDistance != null && Mathf.Abs(zombieByLineMinDistance.transform.position.x - base.transform.position.x) < 5.6f) || (minDisPlant != null && Mathf.Abs(minDisPlant.transform.position.x - base.transform.position.x) < 5.6f))
			{
				clipController.clip.sequence = "shoot";
				clipController.rateScale = 2.5f * base.SpeedRate;
			}
		}
	}

	private void CreateFume()
	{
		if (currGrid == null)
		{
			return;
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 5.6f, isHypno, needCapsule: true);
		List<PlantBase> linePlant = MapManager.Instance.GetLinePlant(base.transform.position, currGrid.Point.y, 15f, !isHypno);
		for (int i = 0; i < zombies.Count; i++)
		{
			if (base.IsFacingLeft && zombies[i].transform.position.x < base.transform.position.x)
			{
				zombies[i].Hurt(attackValue, Vector2.zero);
			}
			else if (!base.IsFacingLeft && zombies[i].transform.position.x > base.transform.position.x)
			{
				zombies[i].Hurt(attackValue, Vector2.zero);
			}
		}
		for (int j = 0; j < linePlant.Count; j++)
		{
			if (base.IsFacingLeft && linePlant[j].transform.position.x < base.transform.position.x)
			{
				linePlant[j].Hurt(attackValue, null);
			}
			else if (!base.IsFacingLeft && linePlant[j].transform.position.x > base.transform.position.x)
			{
				linePlant[j].Hurt(attackValue, null);
			}
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Fume, base.transform.position);
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ShroomFumeParticle);
		obj.transform.localScale = new Vector3(Mathf.Abs(obj.transform.localScale.x), obj.transform.localScale.y);
		obj.GetComponent<SortingGroup>().sortingOrder = GetBulletSortOrder();
		if (base.IsFacingLeft)
		{
			obj.transform.position = base.transform.position + MyTool.ReverseX(creatBulletOffsetPos);
			obj.transform.localScale = new Vector3(0f - obj.transform.localScale.x, obj.transform.localScale.y);
		}
		else
		{
			obj.transform.position = base.transform.position + creatBulletOffsetPos;
		}
	}
}
