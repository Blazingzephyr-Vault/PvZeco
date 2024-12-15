using System.Collections;
using UnityEngine;

public class Lilypad : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Lilypad;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => true;

	public override bool CanCarryOtherPlant => true;

	protected override Vector2 offSet => new Vector2(0f, -0.3f);

	protected override bool HaveShadow => false;

	protected override void OnInitForAlmanac()
	{
		StartCoroutine(FloatUp());
	}

	protected override void OnInitForPlace()
	{
		StartCoroutine(FloatUp());
		StartCoroutine(CloseEye());
	}

	private IEnumerator FloatUp()
	{
		Vector3 pos = base.transform.position;
		while (base.transform.position.y < pos.y + 0.06f)
		{
			yield return new WaitForSeconds(0.05f);
			UpFloat();
		}
		StartCoroutine(FloatDown());
	}

	private IEnumerator FloatDown()
	{
		Vector3 pos = base.transform.position;
		while (base.transform.position.y > pos.y - 0.06f)
		{
			yield return new WaitForSeconds(0.05f);
			DownFloat();
		}
		StartCoroutine(FloatUp());
	}

	private void UpFloat()
	{
		base.transform.position += new Vector3(0f, 0.005f, 0f);
	}

	private void DownFloat()
	{
		base.transform.position += new Vector3(0f, -0.005f, 0f);
	}

	private IEnumerator CloseEye()
	{
		while (true)
		{
			int num = Random.Range(4, 11);
			yield return new WaitForSeconds(num);
			StartCloseEyes();
		}
	}
}
