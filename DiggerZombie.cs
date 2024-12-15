using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DiggerZombie : ZombieBase
{
	private float ownerSpeed;

	public Sprite hat1;

	public Sprite hat2;

	public Sprite hat3;

	public Sprite arm;

	public Sprite lostArm;

	public SpriteRenderer pickaxe;

	public Renderer dirtRenderer;

	private Transform anim;

	public Animator QuestionMark;

	private bool isDigging;

	public Light2D HatLight;

	public SpriteMask mask;

	private Grid OutGrid;

	protected override GameObject Prefab => GameManager.Instance.GameConf.DiggerZombie;

	protected override float AnToSpeed => 3.5f;

	protected override float DefSpeed => ownerSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => 470;

	protected override float DropHeadScale => 0.76f;

	public override void InitZombieHpState()
	{
		ownerSpeed = 1.2f;
		dontChangeState = true;
		animator.speed = 1f;
		mask.enabled = false;
		mask.frontSortingOrder = Sorting.sortingOrder;
		mask.backSortingOrder = Sorting.sortingOrder - 1;
		dirtRenderer.enabled = false;
		if (base.CurrGrid != null)
		{
			dirtRenderer.GetComponent<SwfClip>().sortingOrder = base.CurrLine * 200 + 195;
			OutGrid = MapManager.Instance.GetFarestGrid(base.transform.position, getLeft: true, base.CurrLine);
		}
		capsuleCollider2D.enabled = false;
		animator.Play("dig");
		isDigging = true;
		anim = base.transform.Find("Animation");
		anim.transform.localPosition = new Vector3(1.39f, 1.6f);
		Arm1Renderer.sprite = arm;
		Arm2Renderer.enabled = true;
		Arm3Renderer.enabled = true;
		pickaxe.enabled = true;
		HatLight.enabled = true;
		HpState = new List<int> { 470, 400, 340, 270 };
		E1HpStateSprite = new List<Sprite> { hat1, hat2, hat3, null };
		if (LVManager.Instance.GameIsStart && !IsOVer)
		{
			Shadow.enabled = false;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.digger, base.transform.position);
		}
		QuestionMark.gameObject.SetActive(value: false);
	}

	private void FixedUpdate()
	{
		if (isDigging && !IsOVer)
		{
			if (!isHypno && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && !base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.PotatoMine)
			{
				anCanMove = false;
			}
			else if (isHypno && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.PotatoMine)
			{
				anCanMove = false;
			}
			else if (!anCanMove)
			{
				anCanMove = true;
			}
			if ((!isHypno && base.transform.position.x < OutGrid.Position.x) || (isHypno && base.transform.position.x > OutGrid.Position.x))
			{
				animator.Play("drill");
				anim.position += new Vector3(0f, -2.4f);
				isDigging = false;
				anCanMove = false;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position);
				StartCoroutine(MoveOut());
			}
		}
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			if (swfClip.currentFrame == 40 || swfClip.currentFrame == 100)
			{
				Attack();
			}
			if (swfClip.currentFrame == 70 && REnderer.material.GetTexture("_Decorate1Tex") == pickaxe)
			{
				Attack();
			}
			break;
		case "dead1":
			if (swfClip.currentFrame == 125)
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
		case "dig":
			if (base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.PotatoMine && base.CurrGrid.CurrPlantBase.ZombieCanEat)
			{
				anCanMove = false;
				if (swfClip.currentFrame == 35 || swfClip.currentFrame == 60)
				{
					base.CurrGrid.CurrPlantBase.Hurt(attackValue, this);
				}
			}
			else
			{
				anCanMove = true;
			}
			if (base.transform.position.x < -6.5f)
			{
				swfClip.sequence = "drill";
				anim.position = new Vector3(anim.position.x, anim.position.y - 2.4f);
				anCanMove = false;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position);
				StartCoroutine(MoveOut());
			}
			break;
		case "landing":
			if (swfClip.currentFrame == 0)
			{
				if (REnderer.material.GetTexture("_Decorate1Tex") == pickaxe)
				{
					GoBack();
				}
				capsuleCollider2D.enabled = true;
				Shadow.enabled = true;
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				swfClip.sequence = "dizzy";
				if (REnderer.material.GetTexture("_Decorate1Tex") == pickaxe)
				{
					swfClip.currentFrame = swfClip.frameCount - 1;
				}
			}
			break;
		case "dizzy":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				anCanMove = true;
				dontChangeState = false;
				base.State = ZombieState.Walk;
				ownerSpeed = 5f;
				base.Speed = DefSpeed;
			}
			break;
		case "walk1":
			if (base.transform.position.x > 8f && swfClip.currentFrame > 20)
			{
				Dead(canDropItem: false, 0f);
			}
			if (swfClip.currentFrame == 35 || swfClip.currentFrame == 120)
			{
				anCanMove = true;
			}
			if (swfClip.currentFrame == 20 || swfClip.currentFrame == 90)
			{
				anCanMove = false;
			}
			break;
		}
	}

	private IEnumerator MoveOut()
	{
		mask.enabled = true;
		dirtRenderer.enabled = true;
		dirtRenderer.GetComponent<SwfClipController>().GotoAndPlay(0);
		while (anim.localPosition.y < 1.6f)
		{
			yield return new WaitForSeconds(0.02f);
			anim.Translate(new Vector2(0f, 1.33f) * (Time.deltaTime / 1f) / 0.1f);
		}
		mask.enabled = false;
		dirtRenderer.enabled = false;
		animator.Play("landing");
		if (pickaxe.enabled)
		{
			GoBack();
		}
		capsuleCollider2D.enabled = true;
		Shadow.enabled = true;
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180 && Arm3Renderer.enabled)
		{
			DropArm(new Vector3(0.12f, 0.18f), 0.56f);
			Arm1Renderer.sprite = lostArm;
			Arm2Renderer.enabled = false;
			Arm3Renderer.enabled = false;
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

	private void EquipDropAn(Sprite equip, Vector3 offset)
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZombieEquipParticle).GetComponent<EquipDropParticle>().InitCreate(base.transform.position + offset, Sorting.sortingOrder, equip);
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		Sprite sprite = EquipRenderer.sprite;
		if ((sprite == hat1 || sprite == hat2 || sprite == hat3) && nextSprite == null)
		{
			EquipDropAn(sprite, new Vector3(-0.45f, 1.28f));
		}
		if (nextSprite == hat3 || nextSprite == null)
		{
			HatLight.enabled = false;
		}
	}

	protected override void PlaceCharred()
	{
		if (!isDigging)
		{
			CharredZombie component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>();
			component.InitCharred(3, base.transform.position, new Vector3(-0.72f, 1.5f), Sorting.sortingOrder, base.IsFacingLeft);
			if (pickaxe.enabled)
			{
				component.ClipController.clip.sequence = "pickaxe";
			}
			else
			{
				component.ClipController.clip.sequence = "nopickaxe";
			}
		}
		Dead(canDropItem: true, 0f);
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		SpriteRenderer result = null;
		if (base.Hp > 0 && pickaxe.enabled)
		{
			result = pickaxe;
			if (needClearEquip)
			{
				pickaxe.enabled = false;
				if (isDigging)
				{
					StartCoroutine(NopickaxeOutDirt());
				}
			}
		}
		return result;
	}

	private IEnumerator NopickaxeOutDirt()
	{
		isDigging = false;
		anCanMove = false;
		animator.speed = 0f;
		QuestionMark.gameObject.SetActive(value: true);
		QuestionMark.Play("", 0, 0f);
		do
		{
			yield return new WaitForFixedUpdate();
		}
		while (!(QuestionMark.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f));
		QuestionMark.gameObject.SetActive(value: false);
		animator.speed = base.SpeedRate;
		animator.Play("drill");
		anim.position = new Vector3(anim.position.x, anim.position.y - 2.4f);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position);
		StartCoroutine(MoveOut());
	}

	public override void SpecialAnimEvent1()
	{
		if (pickaxe.enabled)
		{
			Attack();
		}
	}

	public override void SpecialAnimEvent2()
	{
		if (pickaxe.enabled)
		{
			animator.Play("dizzy");
		}
		else
		{
			SpecialAnimEvent3();
		}
	}

	public override void SpecialAnimEvent3()
	{
		anCanMove = true;
		dontChangeState = false;
		base.State = ZombieState.Walk;
		animator.SetInteger("Change", 41);
		ownerSpeed = 5f;
		base.Speed = DefSpeed;
	}

	public override void SpecialAnimEvent4()
	{
		if (!anCanMove)
		{
			if (!isHypno && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && !base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.PotatoMine)
			{
				base.CurrGrid.CurrPlantBase.Hurt(attackValue, this);
			}
			else if (isHypno && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.PotatoMine)
			{
				base.CurrGrid.CurrPlantBase.Hurt(attackValue, this);
			}
		}
	}

	protected override void ChangeFacingEvent()
	{
		OutGrid = MapManager.Instance.GetFarestGrid(base.transform.position, base.IsFacingLeft, base.CurrLine);
	}
}
