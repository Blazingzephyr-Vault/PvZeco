using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SwampZombie : ZombieBase
{
	public SwampZombieType Type;

	public Sprite strawHat1;

	public Sprite strawHat2;

	public Sprite cone1;

	public Sprite cone2;

	public Sprite cone3;

	public Sprite bucket1;

	public Sprite bucket2;

	public Sprite bucket3;

	public Sprite door1;

	public Sprite door2;

	public Sprite door3;

	public Sprite arm;

	public Sprite lostArm;

	public SpriteRenderer ConeRenderer;

	public SpriteRenderer BucketRenderer;

	private float RandomSpeed = 4f;

	public List<Sprite> DecoratesSprite = new List<Sprite>();

	public List<SpriteRenderer> armSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> doorArmSprites = new List<SpriteRenderer>();

	private bool needDropHat;

	private bool needDropDoor;

	protected override float DefSpeed => RandomSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => GetTypeHp();

	protected override GameObject Prefab => GameManager.Instance.GameConf.SwampZombie;

	protected override float AnToSpeed => 5f;

	private int GetTypeHp()
	{
		int result = 270;
		switch (Type)
		{
		case SwampZombieType.Normal:
			result = 350;
			break;
		case SwampZombieType.Stool:
			result = 640;
			break;
		case SwampZombieType.Bucket:
			result = 1370;
			break;
		case SwampZombieType.Door:
			result = 350;
			break;
		case SwampZombieType.DoorAndStool:
			result = 640;
			break;
		case SwampZombieType.DoorAndBucket:
			result = 1370;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		needDropHat = true;
		needDropDoor = true;
		RandomSpeed = Random.Range(2.5f, 5.5f);
		MaxDoorHp = 0;
		canButter = true;
		Arm1Renderer.sprite = arm;
		Arm2Renderer.enabled = true;
		Arm3Renderer.enabled = true;
		ConeRenderer.enabled = false;
		BucketRenderer.enabled = false;
		HpState.Clear();
		DoorHpState.Clear();
		E1HpStateSprite.Clear();
		DoorHpStateSprite.Clear();
		bool isDoorArm = false;
		switch (Type)
		{
		case SwampZombieType.Normal:
			EquipRenderer = ConeRenderer;
			HpState = new List<int> { 350, 310, 270 };
			E1HpStateSprite = new List<Sprite> { strawHat1, strawHat2, null };
			break;
		case SwampZombieType.Stool:
			EquipRenderer = BucketRenderer;
			HpState = new List<int> { 840, 720, 500, 270 };
			E1HpStateSprite = new List<Sprite> { cone1, cone2, cone3, null };
			break;
		case SwampZombieType.Bucket:
			EquipRenderer = BucketRenderer;
			canButter = false;
			HpState = new List<int> { 1470, 1100, 740, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			break;
		case SwampZombieType.Door:
			EquipRenderer = ConeRenderer;
			isDoorArm = true;
			HpState = new List<int> { 350, 310, 270 };
			E1HpStateSprite = new List<Sprite> { strawHat1, strawHat2, null };
			DoorHpState = new List<int> { 600, 400, 200, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			MaxDoorHp = 600;
			break;
		case SwampZombieType.DoorAndStool:
			EquipRenderer = BucketRenderer;
			isDoorArm = true;
			HpState = new List<int> { 640, 520, 400, 270 };
			E1HpStateSprite = new List<Sprite> { cone1, cone2, cone3, null };
			DoorHpState = new List<int> { 1100, 760, 360, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			MaxDoorHp = 1100;
			break;
		case SwampZombieType.DoorAndBucket:
			EquipRenderer = BucketRenderer;
			isDoorArm = true;
			canButter = false;
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			DoorHpState = new List<int> { 1100, 760, 360, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			MaxDoorHp = 1100;
			break;
		}
		IsDoorArm(isDoorArm);
	}

	private void EquipDropAn(Sprite equip, Vector3 offset)
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZombieEquipParticle).GetComponent<EquipDropParticle>().InitCreate(base.transform.position + offset, Sorting.sortingOrder, equip);
	}

	private void IsDoorArm(bool isDoorArm)
	{
		for (int i = 0; i < doorArmSprites.Count; i++)
		{
			doorArmSprites[i].enabled = isDoorArm;
		}
		for (int j = 0; j < armSprites.Count; j++)
		{
			armSprites[j].enabled = !isDoorArm;
		}
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		Sprite sprite = EquipRenderer.sprite;
		Sprite sprite2 = DoorRenderer.sprite;
		if (DoorRenderer.enabled && base.DoorHp <= 0 && (sprite2 == door1 || sprite2 == door2 || sprite2 == door3))
		{
			if (nextSprite == null)
			{
				if (needDropDoor)
				{
					EquipDropAn(sprite2, new Vector3(-0.3f, 0.25f));
				}
				IsDoorArm(isDoorArm: false);
				if (base.Hp < 180)
				{
					DropArm(new Vector3(0.1f, 0.3f, 0f));
					Arm1Renderer.sprite = lostArm;
					Arm2Renderer.enabled = false;
					Arm3Renderer.enabled = false;
				}
			}
		}
		else
		{
			if ((sprite == bucket1 || sprite == bucket2 || sprite == bucket3) && nextSprite == null && needDropHat)
			{
				EquipDropAn(sprite, new Vector3(-0.1f, 0.8f));
				canButter = true;
			}
			if ((sprite == cone1 || sprite == cone2 || sprite == cone3) && nextSprite == null && needDropHat)
			{
				EquipDropAn(sprite, new Vector3(-0.1f, 0.8f));
			}
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180 && Arm3Renderer.enabled && base.DoorHp <= 0)
		{
			DropArm(new Vector3(0.1f, 0.3f, 0f));
			Arm1Renderer.sprite = lostArm;
			Arm2Renderer.enabled = false;
			Arm3Renderer.enabled = false;
		}
		if (Type == SwampZombieType.Bucket && base.Hp <= 270 && !canButter)
		{
			canButter = true;
			canIce = true;
			canFrozen = true;
		}
		if (!HitSound)
		{
			return;
		}
		if (isHard && base.Hp > 270)
		{
			switch (Type)
			{
			case SwampZombieType.Stool:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit2, base.transform.position);
				}
				break;
			case SwampZombieType.Bucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case SwampZombieType.DoorAndStool:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit2, base.transform.position);
				}
				break;
			case SwampZombieType.DoorAndBucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case SwampZombieType.Door:
				break;
			}
		}
		else if (Random.Range(0, 3) == 0)
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

	protected override void InWaterChangeEvent()
	{
		if (base.State != ZombieState.Dead)
		{
			if (base.InWater)
			{
				animator.SetInteger("Change", 13);
			}
			else
			{
				animator.SetInteger("Change", 10 + Random.Range(1, 3));
			}
		}
	}

	protected override void DoorHpReduceEvent()
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit2, base.transform.position);
		}
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			animator.Play("idel" + Random.Range(1, 3));
			break;
		case ZombieState.Walk:
			base.Speed = DefSpeed;
			if (base.InWater)
			{
				animator.SetInteger("Change", 13);
			}
			else
			{
				animator.SetInteger("Change", 10 + Random.Range(1, 3));
			}
			if (base.DoorHp > 0 && (Type == SwampZombieType.DoorAndStool || Type == SwampZombieType.Door || Type == SwampZombieType.DoorAndBucket))
			{
				IsDoorArm(isDoorArm: true);
			}
			break;
		case ZombieState.Attack:
			animator.SetInteger("Change", 21);
			if (Type == SwampZombieType.DoorAndStool || Type == SwampZombieType.Door || Type == SwampZombieType.DoorAndBucket)
			{
				IsDoorArm(isDoorArm: false);
			}
			break;
		case ZombieState.Dead:
			base.DoorHp = 0;
			IsDoorArm(isDoorArm: false);
			base.Speed = DefSpeed;
			Shadow.enabled = false;
			capsuleCollider2D.enabled = false;
			if (base.InWater)
			{
				animator.SetInteger("Change", 33);
			}
			else
			{
				animator.SetInteger("Change", 30 + Random.Range(1, 3));
			}
			break;
		}
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().InitCharred(1, base.transform.position, new Vector3(-0.66f, 1.426f), Sorting.sortingOrder, base.IsFacingLeft);
		Dead(canDropItem: true, 0f);
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		Sprite sprite = EquipRenderer.sprite;
		SpriteRenderer result = null;
		if (base.Hp > 270 && (sprite == bucket1 || sprite == bucket2 || sprite == bucket3))
		{
			result = EquipRenderer;
			if (needClearEquip)
			{
				needDropHat = false;
				base.Hp = 270;
			}
		}
		return result;
	}

	protected override void ClientInit(ZombieSpawn spawnInfo)
	{
		RandomSpeed = spawnInfo.DefSpeed;
	}

	protected override void ServerInitInfo(ZombieSpawn spawnInfo)
	{
		spawnInfo.DefSpeed = RandomSpeed;
	}

	protected override int HandleHurt(int attackValue, Vector2 dirction)
	{
		if (base.Hp > 270 && (Type == SwampZombieType.Stool || Type == SwampZombieType.DoorAndStool) && dirction.y < 0f)
		{
			attackValue /= 20;
		}
		return attackValue;
	}
}
