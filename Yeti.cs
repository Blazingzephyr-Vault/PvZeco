using FTRuntime;
using UnityEngine;

public class Yeti : ZombieBase
{
	public Texture2D arm;

	public Texture2D lostArm;

	private bool isBack;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 50f;

	public override int MaxHP => 1350;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Yeti;

	protected override float AnToSpeed => 2f;

	public override void InitZombieHpState()
	{
		REnderer.material.SetTexture("_ArmTex", arm);
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if (dropItem)
		{
			for (int i = 0; i < 3; i++)
			{
				Allcoin component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Diamond).GetComponent<Allcoin>();
				component.transform.SetParent(null);
				component.InitForItem(base.transform.position);
			}
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			EatCheck();
			if (swfClip.currentFrame == 1 && !isBack && Random.Range(1, 7) == 1)
			{
				GoBack();
				isBack = true;
				base.Speed = 1.2f;
			}
			if (swfClip.currentFrame == 26 || swfClip.currentFrame == 66)
			{
				Attack();
			}
			break;
		case "dead1":
			if (swfClip.currentFrame == 50)
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
				Dead(canDropItem: false);
			}
			break;
		case "walk1":
			if (isBack || (swfClip.currentFrame != 25 && swfClip.currentFrame != 66))
			{
				break;
			}
			if (base.transform.position.x < 3f && base.transform.position.x >= -4f)
			{
				if (Random.Range(1, 8) == 1)
				{
					GoBack();
					isBack = true;
					base.Speed = 1.2f;
				}
			}
			else if (base.transform.position.x < -4f)
			{
				GoBack();
				isBack = true;
				base.Speed = 1.2f;
			}
			break;
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180)
		{
			REnderer.material.SetTexture("_ArmTex", lostArm);
		}
		if (HitSound)
		{
			if (Random.Range(0, 3) == 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat1, base.transform.position);
			}
			else if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat2, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat3, base.transform.position);
			}
		}
	}
}
