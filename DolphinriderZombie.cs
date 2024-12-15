using FTRuntime;
using UnityEngine;

public class DolphinriderZombie : ZombieBase
{
	public Texture2D arm;

	public Texture2D lostArm;

	private Transform anim;

	private bool isJump;

	protected override GameObject Prefab => GameManager.Instance.GameConf.DolphinriderZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 50f;

	public override int MaxHP => 500;

	public override void InitZombieHpState()
	{
		anim = base.transform.Find("Animation");
		dontChangeState = true;
		needInWater = false;
		isJump = false;
		REnderer.material.SetTexture("_ArmTex", arm);
		if (LVManager.Instance.CurrLVState == LVState.Fighting)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.dolphin_appear, base.transform.position);
		}
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			clipController.clip.sequence = "idel1";
			base.Speed = 5f;
			break;
		case ZombieState.Walk:
			if (isJump)
			{
				clipController.clip.sequence = "walk1";
			}
			else
			{
				clipController.clip.sequence = "run";
			}
			break;
		case ZombieState.Attack:
			if (isJump)
			{
				clipController.clip.sequence = "eat1";
				break;
			}
			canButter = false;
			canIce = false;
			clipController.clip.sequence = "jump";
			break;
		case ZombieState.Dead:
			capsuleCollider2D.enabled = false;
			clipController.clip.sequence = "dead1";
			break;
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "walk2":
			break;
		case "eat1":
			if (swfClip.currentFrame == 50 || swfClip.currentFrame == 100)
			{
				Attack();
			}
			break;
		case "walk1":
			if (base.CurrGrid.isWaterGrid && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.8f && nextGrid != null && nextGrid.isWaterGrid)
			{
				anCanMove = false;
				swfClip.sequence = "jump1";
			}
			break;
		case "dead1":
			if (swfClip.currentFrame == 90)
			{
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Zombiefail1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Zombiefail2, base.transform.position);
				}
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				Dead();
			}
			break;
		case "jump1":
			if (swfClip.currentFrame == 50)
			{
				capsuleCollider2D.enabled = false;
			}
			if (swfClip.currentFrame == 120)
			{
				Shadow.enabled = false;
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				base.Speed = 0.8f;
				clipController.rateScale = 1f;
				capsuleCollider2D.enabled = true;
				capsuleCollider2D.offset = new Vector2(1f, 0f);
				clipController.clip.sequence = "ride";
				base.InWater = true;
				anCanMove = true;
			}
			break;
		case "jump2":
			if (swfClip.currentFrame == 1)
			{
				capsuleCollider2D.enabled = false;
				Shadow.enabled = false;
			}
			if (swfClip.currentFrame == 50)
			{
				anCanMove = false;
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				base.Speed = 5f;
				anCanMove = true;
				dontChangeState = false;
				capsuleCollider2D.enabled = true;
				capsuleCollider2D.offset = new Vector2(0.16f, 0f);
				clipController.clip.sequence = "swim";
				base.transform.position -= new Vector3(2.75f, 0.94f);
				base.InWater = true;
			}
			break;
		case "ride":
			if (nextGrid != null && nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.ZombieCanEat && base.transform.position.x - nextGrid.CurrPlantBase.transform.position.x < 1.3f)
			{
				swfClip.sequence = "jump2";
			}
			if (base.CurrGrid.Position.x - base.transform.position.x > 0.6f && (nextGrid == null || !nextGrid.isWaterGrid) && base.InWater)
			{
				base.Speed = DefSpeed;
				anim.position -= new Vector3(1.2f, 0f);
				swfClip.sequence = "walk1";
			}
			break;
		case "swim":
			if (base.CurrGrid.Position.x - base.transform.position.x > 0.6f && (nextGrid == null || !nextGrid.isWaterGrid) && base.InWater)
			{
				swfClip.sequence = "walk2";
			}
			break;
		}
	}
}
