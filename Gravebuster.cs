using System.Collections;
using FTRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class Gravebuster : PlantBase
{
	private Coroutine MoveDownCoroutine;

	public ParticleSystem StonePs;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Gravebuster;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => false;

	protected override bool HaveShadow => false;

	protected override Vector2 offSet => new Vector2(0f, 0.7f);

	protected override void OnInitForAll()
	{
		StonePs.gameObject.SetActive(value: false);
	}

	protected override void OnInitForPlace()
	{
		if (!isSleeping)
		{
			StonePs.gameObject.SetActive(value: true);
			clipController.clip.sequence = "land";
		}
		clipController.clip.sortingOrder = clipController.clip.sortingOrder / 100 * 100 + 102;
		StonePs.GetComponent<SortingGroup>().sortingOrder = clipController.clip.sortingOrder + 1;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "land" && swfClip.currentFrame == swfClip.frameCount - 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gravestone_chomp, base.transform.position);
				clipController.clip.sequence = "idel";
				MoveDownCoroutine = StartCoroutine(MoveDown());
			}
		}
		else if (swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}

	private IEnumerator MoveDown()
	{
		while (base.transform.position.y > currGrid.Position.y + 0.1f)
		{
			yield return new WaitForFixedUpdate();
			base.gameObject.transform.Translate(new Vector2(0f, -0.15f) * Time.deltaTime);
		}
		if (!GameManager.Instance.isClient)
		{
			currGrid.HaveGraveStone = false;
		}
		ZombieManager.Instance.DropCoin(base.transform.position, AllDrop: true);
		Dead();
	}

	protected override void GoAwakeSpecial()
	{
	}

	protected override void GoSleepSpecial()
	{
		ZZZ.gameObject.SetActive(value: false);
	}
}
