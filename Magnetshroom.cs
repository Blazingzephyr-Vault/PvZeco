using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class Magnetshroom : PlantBase
{
	private bool CDover;

	private SpriteRenderer Equip;

	public Texture2D sleep;

	public Texture2D eye;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Magnetshroom;

	protected override bool isShroom => true;

	protected override void OnInitForAll()
	{
		REnderer.material.SetTexture("_SpecialTex", eye);
	}

	protected override void OnInitForPlace()
	{
		isClosingEye = false;
		CDover = true;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (!(sequence == "attract"))
			{
				return;
			}
			if (swfClip.currentFrame == 46)
			{
				GetIron();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				if (CDover)
				{
					clipController.clip.sequence = "idel";
					return;
				}
				clipController.clip.sequence = "absorb";
				Invoke("CdOver", 15f);
			}
		}
		else
		{
			if (swfClip.currentFrame == 1)
			{
				CheckIron();
			}
			if (swfClip.currentFrame == closeEyeFrame)
			{
				OwnerCloseEyes();
			}
		}
	}

	private void CheckIron()
	{
		if (currGrid == null || isSleeping)
		{
			return;
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 4.5f, needCapsule: false, isHypno);
		for (int i = 0; i < zombies.Count; i++)
		{
			if (zombies[i].GetEquipSprite(needClearEquip: false) != null)
			{
				clipController.clip.sequence = "attract";
				break;
			}
		}
	}

	private void GetIron()
	{
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 4.5f, needCapsule: false, isHypno);
		for (int i = 0; i < zombies.Count; i++)
		{
			if (zombies[i].GetEquipSprite(needClearEquip: false) != null)
			{
				SpriteRenderer equipSprite = zombies[i].GetEquipSprite(needClearEquip: false);
				Equip = Object.Instantiate(NormalSprite.Instance.SpriteDisplay).GetComponent<SpriteRenderer>();
				Equip.sprite = equipSprite.sprite;
				Equip.transform.localScale = equipSprite.transform.localScale;
				Equip.transform.rotation = equipSprite.transform.rotation;
				Equip.transform.position = equipSprite.transform.position;
				CDover = false;
				DoFly();
				zombies[i].GetEquipSprite(needClearEquip: true);
				break;
			}
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.magnetshroom, base.transform.position);
	}

	private void CdOver()
	{
		CDover = true;
		Object.Destroy(Equip.gameObject);
		Equip = null;
		clipController.clip.sequence = "idel";
	}

	public void DoFly()
	{
		StartCoroutine(Fly(base.transform.position + new Vector3(0.3f, 0.3f)));
		Equip.sortingOrder = 2200;
	}

	private IEnumerator Fly(Vector3 pos)
	{
		while (Vector3.Distance(pos, Equip.transform.position) > 0.1f)
		{
			yield return new WaitForFixedUpdate();
			Equip.transform.position = Vector2.MoveTowards(Equip.transform.position, pos, 8f * Time.deltaTime);
		}
		Equip.sortingOrder = clipController.clip.sortingOrder + 1;
	}

	protected override void DeadEvent()
	{
		if (Equip != null)
		{
			Object.Destroy(Equip.gameObject);
		}
		Equip = null;
	}

	protected void OwnerCloseEyes()
	{
		if (!isSleeping && Random.Range(0, idelFrameCount) <= idelFrameCount / 4)
		{
			closeEyeFrame = Random.Range(0, idelFrameCount);
			if (!isClosingEye)
			{
				StartCoroutine(CloseEyes());
			}
		}
	}

	private IEnumerator CloseEyes()
	{
		isClosingEye = true;
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_SpecialTex", eye1);
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_SpecialTex", eye2);
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_SpecialTex", eye1);
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_SpecialTex", eye);
		isClosingEye = false;
	}

	protected override void GoAwakeSpecial()
	{
		REnderer.material.SetTexture("_SpecialTex", eye);
	}

	protected override void GoSleepSpecial()
	{
		REnderer.material.SetTexture("_SpecialTex", sleep);
	}
}
