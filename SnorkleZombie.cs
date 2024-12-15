using System.Collections;
using FTRuntime;
using UnityEngine;

public class SnorkleZombie : ZombieBase
{
	private Transform anim;

	private float animY;

	public Texture2D arm;

	public Texture2D lostArm;

	protected override GameObject Prefab => GameManager.Instance.GameConf.SnorkleZombie;

	protected override float AnToSpeed => 5f;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 40f;

	public override int MaxHP => 270;

	public override void InitZombieHpState()
	{
		needInWater = false;
		anim = base.transform.Find("Animation");
		animY = anim.localPosition.y;
		REnderer.material.SetTexture("_ArmTex", arm);
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			if (swfClip.currentFrame == 50 || swfClip.currentFrame == 100)
			{
				Attack();
			}
			break;
		case "walk1":
			if (base.CurrGrid.isWaterGrid && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.8f && nextGrid != null && nextGrid.isWaterGrid)
			{
				swfClip.sequence = "jump";
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
		case "jump":
			if (swfClip.currentFrame == 20)
			{
				capsuleCollider2D.enabled = false;
				Shadow.enabled = false;
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				capsuleCollider2D.enabled = true;
				clipController.clip.sequence = "swim";
				base.InWater = true;
			}
			break;
		case "uptoeat":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "eat1";
			}
			break;
		case "swim":
			if (base.CurrGrid.Position.x - base.transform.position.x > 0.6f && (nextGrid == null || !nextGrid.isWaterGrid) && base.InWater)
			{
				swfClip.sequence = "walk1";
				anim.position -= new Vector3(0f, 1.6f);
				StartCoroutine(MoveUp());
			}
			break;
		}
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			clipController.clip.sequence = "idel1";
			break;
		case ZombieState.Walk:
			if (base.InWater)
			{
				clipController.clip.sequence = "swim";
			}
			else
			{
				clipController.clip.sequence = "walk1";
			}
			break;
		case ZombieState.Attack:
			clipController.clip.sequence = "uptoeat";
			break;
		case ZombieState.Dead:
			capsuleCollider2D.enabled = false;
			if (base.InWater)
			{
				clipController.clip.sequence = "dead1";
			}
			else
			{
				clipController.clip.sequence = "dead3";
			}
			clipController.loopMode = SwfClipController.LoopModes.Once;
			break;
		}
	}

	private IEnumerator MoveUp()
	{
		while (anim.localPosition.y < animY)
		{
			yield return new WaitForSeconds(0.02f);
			anim.Translate(new Vector2(0f, 1.33f) * (Time.deltaTime / 1f) / 0.1f);
		}
	}
}
