using System.Collections;
using FTRuntime;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hepatica : PlantBase
{
	private Vector2 TargetPos;

	private bool ChargeOver;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Hepatica;

	protected override void OnInitForPlace()
	{
		ChargeOver = true;
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
		clipController.clip.sequence = "nochargeidel";
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "idel":
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			break;
		case "nochargeidel":
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1 && ChargeOver)
			{
				ChargeOver = false;
				clipController.clip.sequence = "charge";
			}
			break;
		case "shoot":
			if (swfClip.currentFrame == 52)
			{
				CreateIcecone();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				StartCoroutine(WaitCharge());
				clipController.clip.sequence = "nochargeidel";
			}
			break;
		case "charge":
			_ = swfClip.currentFrame;
			_ = 34;
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "idel";
				base.transform.GetComponent<CapsuleCollider2D>().enabled = true;
			}
			break;
		}
	}

	private void OnMouseOver()
	{
		if (!EventSystem.current.IsPointerOverGameObject() && currGrid != null && Input.GetMouseButtonDown(0))
		{
			CobCannonTarget.Instance.StartAim(this, delegate(Vector2 pos)
			{
				Shoot(pos);
			});
		}
	}

	private IEnumerator WaitCharge()
	{
		yield return new WaitForSeconds(35f);
		ChargeOver = true;
	}

	private void CreateIcecone()
	{
		Object.Instantiate(GameManager.Instance.GameConf.CannonCob).GetComponent<CannonCob>().CreateInit(base.transform.position + new Vector3(-0.1f, 3f), TargetPos, attackValue, isHypno);
	}

	public void Shoot(Vector2 pos)
	{
		clipController.clip.sequence = "shoot";
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
		TargetPos = pos;
	}
}
