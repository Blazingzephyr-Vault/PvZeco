using FTRuntime;
using UnityEngine;

public class Tanglekelp : PlantBase
{
	private ZombieBase zombie;

	private bool isAttack;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Tanglekelp;

	public override bool ZombieCanEat => isSleeping;

	public override bool CanPlaceOnWater => true;

	protected override int attackValue => 1800;

	protected override Vector2 offSet => new Vector2(0f, -0.2f);

	public override bool CanPlaceOnGrass => false;

	public override bool CanCarryed => false;

	protected override bool HaveShadow => false;

	public override bool CanProtect => false;

	protected override void OnInitForPlace()
	{
		isAttack = false;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "idel")
		{
			if (swfClip.currentFrame == 1)
			{
				Attack();
			}
			if (swfClip.currentFrame == 20)
			{
				Attack();
			}
			if (swfClip.currentFrame == 40)
			{
				Attack();
			}
			if (swfClip.currentFrame == 60)
			{
				Attack();
			}
			if (swfClip.currentFrame == 80)
			{
				Attack();
			}
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
		}
	}

	private void Attack()
	{
		if (currGrid != null && !isSleeping && !isAttack && currGrid != null)
		{
			zombie = ZombieManager.Instance.GetZombieByLineMinDisNoDir(currGrid.Point.y, base.transform.position, isHypno);
			if (!(zombie == null) && zombie.InWater && Mathf.Abs(zombie.transform.position.x - base.transform.position.x) < 0.8f)
			{
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Tanglekelpgrab).GetComponent<Tanglekelpgrab>().Init(this, zombie, attackValue);
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.EFObj).GetComponent<EFObj>().CreateInit(new Vector2(zombie.transform.position.x - 0.8f, base.transform.position.y + 0.6f), 3, new Color(1f, 1f, 1f, 1f), GetBulletSortOrder());
				isAttack = true;
			}
		}
	}

	protected override void DeadrattleEvent()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.EFObj).GetComponent<EFObj>().CreateInit(base.transform.position + new Vector3(-0.75f, 0.85f), 3, new Color(1f, 1f, 1f, 1f), GetBulletSortOrder());
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
		}
	}
}
