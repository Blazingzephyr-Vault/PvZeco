using System.Collections;
using SocketSave;
using UnityEngine;

public class PogoZombie : ZombieBase
{
	private Transform anim;

	private float animY;

	private bool isGoUp;

	public int jumpNum;

	private float jumpHigh;

	private float ownerSpeed;

	public SpriteRenderer Pogo;

	public Sprite Pogo1;

	public Sprite Pogo2;

	public Sprite Pogo3;

	public Sprite arm;

	public Sprite lostArm;

	protected override GameObject Prefab => GameManager.Instance.GameConf.PogoZombie;

	protected override float AnToSpeed => ownerSpeed;

	protected override float DefSpeed => ownerSpeed;

	protected override float attackValue => 80f;

	public override int MaxHP => 500;

	protected override float DropHeadScale => 0.75f;

	public override void InitZombieHpState()
	{
		jumpHigh = 0.8f;
		jumpNum = 0;
		anim = base.transform.Find("Animation");
		canIce = false;
		anim.localPosition = new Vector3(-6f, 4f);
		animY = 4.1f;
		Pogo.sprite = Pogo1;
		Arm1Renderer.sprite = arm;
		Arm2Renderer.enabled = true;
		Arm3Renderer.enabled = true;
		Arm3Renderer.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
		dontChangeState = true;
		isGoUp = true;
		animator.SetInteger("Change", 0);
		animator.Play("pogo");
		ownerSpeed = 2f;
		if (!IsOVer)
		{
			animator.speed = 1f;
		}
	}

	private void FixedUpdate()
	{
		if (IsOVer || !dontChangeState || base.State == ZombieState.Dead)
		{
			return;
		}
		if (jumpNum <= 2 && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && ((isHypno && base.CurrGrid.CurrPlantBase.isHypno) || (!isHypno && !base.CurrGrid.CurrPlantBase.isHypno)) && ((base.IsFacingLeft && base.transform.position.x - base.CurrGrid.Position.x >= 0f) || (!base.IsFacingLeft && base.transform.position.x - base.CurrGrid.Position.x <= 0f)) && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.7f)
		{
			anCanMove = false;
		}
		float num = 1f;
		num = 1.2f + (float)jumpNum * 0.7f;
		if (isGoUp && Pogo.enabled)
		{
			if (anim.localPosition.y < animY + jumpHigh * num)
			{
				anim.Translate(new Vector2(0f, 1f) * Time.deltaTime * num);
			}
		}
		else if (anim.localPosition.y > animY)
		{
			anim.Translate(new Vector2(0f, -1.2f) * Time.deltaTime * num);
		}
	}

	public override void OnlineSynZombie(SynItem syn)
	{
		base.OnlineSynZombie(syn);
		if (syn.SynCode[0] == 2)
		{
			if (syn.SynCode[1] == 0)
			{
				anCanMove = false;
			}
			else if (syn.SynCode[1] == 1)
			{
				jumpNum++;
				capsuleCollider2D.enabled = false;
				anCanMove = true;
				base.Speed = 1f;
				animator.speed = 1f;
			}
		}
	}

	private void EquipDropAn()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZombieEquipParticle).GetComponent<EquipDropParticle>().InitCreate(base.transform.position, Sorting.sortingOrder, Pogo.sprite);
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		SpriteRenderer result = null;
		if (base.Hp > 0 && Pogo.enabled)
		{
			result = Pogo;
			if (needClearEquip)
			{
				Pogo.enabled = false;
				base.Speed = 4.5f;
				ownerSpeed = 4.5f;
				canIce = true;
				dontChangeState = false;
				anim.localPosition = new Vector3(-6f, anim.localPosition.y);
				capsuleCollider2D.enabled = true;
				base.State = ZombieState.Walk;
				animator.Play("walk1");
				Shadow.enabled = true;
				StartCoroutine(MoveDown());
			}
		}
		return result;
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			animator.Play("pogo");
			break;
		case ZombieState.Walk:
			animator.SetInteger("Change", 11);
			break;
		case ZombieState.Attack:
			animator.SetInteger("Change", 21);
			break;
		case ZombieState.Dead:
			Shadow.enabled = false;
			capsuleCollider2D.enabled = false;
			StartCoroutine(MoveDown());
			animator.SetInteger("Change", 31);
			break;
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180 && Arm3Renderer.enabled)
		{
			DropArm(new Vector3(0.05f, -0.05f), 0.33f);
			Arm1Renderer.sprite = lostArm;
			Arm2Renderer.enabled = false;
			Arm3Renderer.enabled = false;
			Arm3Renderer.transform.localScale = new Vector3(0f, 0f, 1f);
		}
		if (base.Hp < 160 && Pogo.sprite == Pogo2)
		{
			Pogo.sprite = Pogo3;
		}
		else if (base.Hp < 320 && Pogo.sprite == Pogo1)
		{
			Pogo.sprite = Pogo2;
		}
		if (base.Hp <= 0)
		{
			isGoUp = false;
			jumpNum = 2;
			dontChangeState = false;
			base.State = ZombieState.Dead;
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

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().InitCharred(1, base.transform.position, new Vector3(-0.72f, 1.34f), Sorting.sortingOrder, base.IsFacingLeft);
		Dead(canDropItem: true, 0f);
	}

	public override void SpecialAnimEvent1()
	{
		if (!anCanMove)
		{
			jumpNum++;
		}
		animator.speed = 1f;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.pogo_jump, base.transform.position);
	}

	public override void SpecialAnimEvent2()
	{
		isGoUp = false;
	}

	public override void SpecialAnimEvent3()
	{
		isGoUp = true;
		if (jumpNum > 2)
		{
			if (jumpNum == 4)
			{
				jumpNum = 0;
				capsuleCollider2D.enabled = true;
				base.Speed = DefSpeed;
			}
			if (jumpNum == 3)
			{
				jumpNum++;
				capsuleCollider2D.enabled = false;
				anCanMove = true;
				base.Speed = 1f;
				animator.speed = 1f;
			}
		}
	}

	public override void SpecialAnimEvent4()
	{
		if (base.CurrGrid != null && jumpNum == 4 && ((base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.Tallnut) || (base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.CarryPlant != null && base.CurrGrid.CurrPlantBase.CarryPlant.GetPlantType() == PlantType.Tallnut)))
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bonk, base.transform.position);
			EquipDropAn();
			base.Speed = 4.5f;
			ownerSpeed = 4.5f;
			canIce = true;
			dontChangeState = false;
			anim.localPosition = new Vector3(-6f, anim.localPosition.y);
			capsuleCollider2D.enabled = true;
			base.State = ZombieState.Walk;
			animator.Play("walk1");
			Shadow.enabled = true;
			StartCoroutine(MoveDown());
		}
	}

	private IEnumerator MoveDown()
	{
		while (anim.localPosition.y > 4f)
		{
			yield return new WaitForFixedUpdate();
			anim.Translate(new Vector2(0f, -3f) * Time.deltaTime);
		}
	}

	protected override void AlmanacInitZombie()
	{
		IsOVer = false;
	}
}
