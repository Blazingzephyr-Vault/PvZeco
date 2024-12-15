using FTRuntime;
using UnityEngine;

public class ThreePeater : PlantBase
{
	private bool isAttack;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.ThreePeater;

	protected override int attackValue => 20;

	protected override void OnInitForCreate()
	{
		isAttack = false;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.currentFrame == 0)
		{
			CheckAttack();
		}
		if (swfClip.sequence == "shoot" && swfClip.currentFrame == 32)
		{
			CreatePea();
		}
		if (swfClip.sequence == "idel" && swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}

	private void CheckAttack()
	{
		if (currGrid == null || isSleeping)
		{
			return;
		}
		if (ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y + 1, base.transform.position, base.IsFacingLeft, isHypno) != null)
		{
			isAttack = true;
		}
		if (!isAttack && ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno) != null)
		{
			isAttack = true;
		}
		if (!isAttack && ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y - 1, base.transform.position, base.IsFacingLeft, isHypno) != null)
		{
			isAttack = true;
		}
		if (!isAttack)
		{
			if (MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno) != null)
			{
				isAttack = true;
			}
			if (!isAttack && MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y + 1, base.IsFacingLeft, !isHypno) != null)
			{
				isAttack = true;
			}
			if (!isAttack && MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y - 1, base.IsFacingLeft, !isHypno) != null)
			{
				isAttack = true;
			}
		}
		if (isAttack)
		{
			clipController.clip.sequence = "shoot";
		}
		else
		{
			clipController.clip.sequence = "idel";
		}
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
				component.Init(attackValue, base.transform.position + MyTool.ReverseX(new Vector3(0.25f, 0.45f, 0f)), currGrid.Point.y - 1, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(attackValue, base.transform.position + new Vector3(0.25f, 0.45f, 0f), currGrid.Point.y - 1, Vector2.right, GetBulletSortOrder(), isHypno);
			}
			component.StartVerticalMove(1.4f);
			component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(attackValue, base.transform.position + MyTool.ReverseX(new Vector3(0.35f, 0.2f, 0f)), currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(attackValue, base.transform.position + new Vector3(0.35f, 0.2f, 0f), currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno);
			}
			component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(attackValue, base.transform.position + MyTool.ReverseX(new Vector3(0.8f, -0.12f, 0f)), currGrid.Point.y + 1, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(attackValue, base.transform.position + new Vector3(0.8f, -0.12f, 0f), currGrid.Point.y + 1, Vector2.right, GetBulletSortOrder(), isHypno);
			}
			component.StartVerticalMove(-1.1f);
			isAttack = false;
		}
	}
}
