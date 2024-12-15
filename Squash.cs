using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using SocketSave;
using UnityEngine;

public class Squash : PlantBase
{
	private Vector3 direction;

	private GameObject Target;

	private bool isAttack;

	private int LineNum;

	public Texture2D eyeBrow;

	public override float MaxHp => 800f;

	protected override PlantType plantType => PlantType.Squash;

	protected override int attackValue => 1800;

	protected override void OnInitForAll()
	{
		REnderer.material.SetTexture("_SpecialTex", eyeBrow);
	}

	protected override void OnInitForPlace()
	{
		isAttack = false;
		LineNum = currGrid.Point.y;
		clipController.rateScale = base.SpeedRate;
		clipController.loopMode = SwfClipController.LoopModes.Loop;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "lookleft":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "jumpup";
			}
			break;
		case "lookright":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "jumpup";
			}
			break;
		case "jumpup":
			if (swfClip.currentFrame == 0)
			{
				clipController.rateScale = 1.5f * base.SpeedRate;
			}
			if (swfClip.currentFrame == 54)
			{
				FlytoHigh();
			}
			break;
		case "idel":
			if (swfClip.currentFrame == closeEyeFrame)
			{
				OwnerCloseEyes();
			}
			if (swfClip.currentFrame == 1 || swfClip.currentFrame == 45)
			{
				Attack();
			}
			break;
		}
	}

	public override void OnlineSynPlant(SynItem syn)
	{
		base.OnlineSynPlant(syn);
		if (syn.SynCode[0] == 0 && clipController.clip.sequence == "idel" && currGrid != null)
		{
			Attack();
		}
	}

	private void Attack()
	{
		if (currGrid == null || isSleeping || isAttack)
		{
			return;
		}
		Target = null;
		ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(currGrid.Point.y, base.transform.position, isHypno);
		PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, !isHypno);
		if (zombieByLineMinDisNoDir != null && minDisPlant != null)
		{
			float num = Mathf.Abs(base.transform.position.x - minDisPlant.transform.position.x);
			float num2 = Mathf.Abs(base.transform.position.x - zombieByLineMinDisNoDir.transform.position.x);
			if (num > num2)
			{
				Target = zombieByLineMinDisNoDir.gameObject;
			}
			else
			{
				Target = minDisPlant.gameObject;
			}
		}
		else
		{
			if (zombieByLineMinDisNoDir != null)
			{
				Target = zombieByLineMinDisNoDir.gameObject;
			}
			if (minDisPlant != null)
			{
				Target = minDisPlant.gameObject;
			}
		}
		if (Target != null && Target.transform.position.x - base.transform.position.x < 2.4f && Target.transform.position.x - base.transform.position.x >= 0f)
		{
			Emmm();
			if (base.IsFacingLeft)
			{
				clipController.clip.sequence = "lookleft";
			}
			else
			{
				clipController.clip.sequence = "lookright";
			}
			isAttack = true;
			clipController.rateScale = 2f * base.SpeedRate;
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
			Dead(isFlat: false, 10f);
		}
		else if (Target != null && Target.transform.position.x - base.transform.position.x > -2.4f && Target.transform.position.x - base.transform.position.x < 0f)
		{
			Emmm();
			if (base.IsFacingLeft)
			{
				clipController.clip.sequence = "lookright";
			}
			else
			{
				clipController.clip.sequence = "lookleft";
			}
			isAttack = true;
			clipController.rateScale = 2f * base.SpeedRate;
			if (GameManager.Instance.isServer)
			{
				SynItem synItem2 = new SynItem();
				synItem2.OnlineId = OnlineId;
				synItem2.Type = 1;
				SocketServer.Instance.SendSynBag(synItem2);
			}
			Dead(isFlat: false, 10f);
		}
	}

	private void Emmm()
	{
		direction = (Target.transform.position - base.transform.position + new Vector3(0f, 2.1f, 0f)).normalized;
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Squashemm1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Squashemm2, base.transform.position);
		}
	}

	private void FlytoHigh()
	{
		clipController.loopMode = SwfClipController.LoopModes.Once;
		StartCoroutine(DoFly());
	}

	private IEnumerator DoFly()
	{
		float Y = Target.transform.position.y + 2f;
		while (base.transform.position.y < Y)
		{
			yield return new WaitForFixedUpdate();
			if (Vector2.Distance(Target.transform.position, base.transform.position) < 3f)
			{
				direction = (Target.transform.position - base.transform.position + new Vector3(0f, 2.1f, 0f)).normalized;
			}
			base.transform.Translate(direction * 20f * Time.deltaTime);
		}
		clipController.clip.sortingOrder = GetBulletSortOrder();
		yield return new WaitForSeconds(0.2f);
		clipController.clip.sequence = "jumpdown";
		clipController.rateScale = 2f;
		clipController.GotoAndPlay(0);
		StartCoroutine(DoDown());
	}

	private IEnumerator DoDown()
	{
		Grid grid = MapManager.Instance.GetGridByWorldPos(base.transform.position, LineNum);
		float Y = grid.Position.y;
		while (base.transform.position.y > Y)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(0f, -15f * Time.deltaTime, 0f);
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 1f, isHypno, needCapsule: true);
		for (int i = 0; i < zombies.Count; i++)
		{
			zombies[i].Hurt(attackValue, Vector2.down);
		}
		List<PlantBase> linePlant = MapManager.Instance.GetLinePlant(base.transform.position, currGrid.Point.y, 1f, !isHypno);
		for (int j = 0; j < linePlant.Count; j++)
		{
			linePlant[j].Hurt(attackValue, null, isFlat: true);
		}
		CameraControl.Instance.ShakeCamera(base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gargantuar_thump, base.transform.position);
		if (grid.isWaterGrid && Mathf.Abs(grid.Position.x - base.transform.position.x) < 1.3f)
		{
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
			}
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.EFObj).GetComponent<EFObj>().CreateInit(base.transform.position + new Vector3(-0.75f, 0.95f, 0f), 3, new Color(1f, 1f, 1f, 1f), GetBulletSortOrder());
			Dead();
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
			Dead();
		}
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
		REnderer.material.SetTexture("_SpecialTex", null);
		REnderer.material.SetTexture("_EyeTex", eye1);
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_EyeTex", eye2);
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_EyeTex", eye1);
		yield return new WaitForSeconds(0.1f);
		REnderer.material.SetTexture("_EyeTex", null);
		REnderer.material.SetTexture("_SpecialTex", eyeBrow);
		isClosingEye = false;
	}

	protected override void GoAwakeSpecial()
	{
		REnderer.material.SetTexture("_SpecialTex", eyeBrow);
	}

	protected override void GoSleepSpecial()
	{
		REnderer.material.SetTexture("_SpecialTex", null);
	}
}
