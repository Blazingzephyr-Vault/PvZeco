using System.Collections.Generic;
using FTRuntime;
using SocketSave;
using UnityEngine;

public class Zombie : ZombieBase
{
	public NormalZombieType Type;

	public Sprite cone1;

	public Sprite cone2;

	public Sprite cone3;

	public Sprite bucket1;

	public Sprite bucket2;

	public Sprite bucket3;

	public Sprite door1;

	public Sprite door2;

	public Sprite door3;

	public Sprite normalArm;

	public Sprite lostArm;

	public SpriteRenderer ConeRenderer;

	public SpriteRenderer BucketRenderer;

	public List<SpriteRenderer> armSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> doorArmSprites = new List<SpriteRenderer>();

	public List<Texture2D> DecoratesTex = new List<Texture2D>();

	private float RandomSpeed = 4f;

	private bool needDropHat;

	private bool needDropDoor;

	protected override float DefSpeed => RandomSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => GetTypeHp();

	protected override GameObject Prefab => GameManager.Instance.GameConf.Zombie;

	protected override float AnToSpeed => 5f;

	private int GetTypeHp()
	{
		int result = 270;
		switch (Type)
		{
		case NormalZombieType.Cone:
			result = 640;
			break;
		case NormalZombieType.Bucket:
			result = 1370;
			break;
		case NormalZombieType.DoorAndCone:
			result = 640;
			break;
		case NormalZombieType.DoorAndBucket:
			result = 1370;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		needDropHat = true;
		needDropDoor = true;
		Arm1Renderer.sprite = normalArm;
		Arm2Renderer.enabled = true;
		Arm3Renderer.enabled = true;
		RandomSpeed = Random.Range(2.5f, 5.5f);
		MaxDoorHp = 0;
		ConeRenderer.enabled = false;
		BucketRenderer.enabled = false;
		HpState = new List<int>();
		DoorHpState = new List<int>();
		E1HpStateSprite.Clear();
		DoorHpStateSprite.Clear();
		bool isDoorArm = false;
		switch (Type)
		{
		case NormalZombieType.Cone:
			EquipRenderer = ConeRenderer;
			HpState = new List<int> { 640, 520, 400, 270 };
			E1HpStateSprite = new List<Sprite> { cone1, cone2, cone3, null };
			break;
		case NormalZombieType.Bucket:
			EquipRenderer = BucketRenderer;
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			break;
		case NormalZombieType.Door:
			isDoorArm = true;
			DoorHpState = new List<int> { 1100, 760, 360, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			MaxDoorHp = 1100;
			break;
		case NormalZombieType.DoorAndCone:
			isDoorArm = true;
			EquipRenderer = ConeRenderer;
			HpState = new List<int> { 640, 520, 400, 270 };
			E1HpStateSprite = new List<Sprite> { cone1, cone2, cone3, null };
			DoorHpState = new List<int> { 1100, 760, 360, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			MaxDoorHp = 1100;
			break;
		case NormalZombieType.DoorAndBucket:
			isDoorArm = true;
			EquipRenderer = BucketRenderer;
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			DoorHpState = new List<int> { 1100, 760, 360, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			MaxDoorHp = 1100;
			break;
		}
		IsDoorArm(isDoorArm);
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

	private void SetDecorate()
	{
		if (DecoratesTex.Count == 0)
		{
			return;
		}
		if (DecoratesTex.Count <= 2)
		{
			REnderer.material.SetTexture("_Decorate1Tex", DecoratesTex[0]);
			if (DecoratesTex.Count == 2)
			{
				REnderer.material.SetTexture("_Decorate2Tex", DecoratesTex[1]);
			}
			return;
		}
		int num = Random.Range(0, DecoratesTex.Count);
		int num2 = Random.Range(0, DecoratesTex.Count);
		while (num == num2)
		{
			num2 = Random.Range(0, DecoratesTex.Count);
		}
		REnderer.material.SetTexture("_Decorate1Tex", DecoratesTex[num]);
		REnderer.material.SetTexture("_Decorate2Tex", DecoratesTex[num2]);
	}

	private void EquipDropAn(Sprite equip, Vector3 offset)
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZombieEquipParticle).GetComponent<EquipDropParticle>().InitCreate(base.transform.position + offset, Sorting.sortingOrder, equip);
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "eat1":
			if (swfClip.currentFrame == 32 || swfClip.currentFrame == 80)
			{
				Attack();
			}
			break;
		case "deaddownhand":
			if (swfClip.currentFrame == 0)
			{
				DropHead();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "dead" + Random.Range(1, 3);
			}
			break;
		case "uphand":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "eat1";
			}
			break;
		case "downhand":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				anCanMove = true;
				clipController.clip.sequence = "walk" + Random.Range(1, 3);
			}
			break;
		case "dead1":
			if (swfClip.currentFrame == 104)
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
		case "dead2":
			if (swfClip.currentFrame == 78)
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
		case "waterdead1":
			_ = swfClip.currentFrame;
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				Dead();
			}
			break;
		}
		if (base.State == ZombieState.Walk)
		{
			if (swfClip.currentFrame == 0 || swfClip.currentFrame == 120)
			{
				anCanMove = true;
			}
			if (swfClip.currentFrame == 80 || swfClip.currentFrame == 200)
			{
				anCanMove = false;
			}
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
				if (base.Hp < 180 && Arm3Renderer.enabled)
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
		if (!HitSound)
		{
			return;
		}
		if (isHard && base.Hp > 270)
		{
			switch (Type)
			{
			case NormalZombieType.Cone:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit2, base.transform.position);
				}
				break;
			case NormalZombieType.Bucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case NormalZombieType.DoorAndCone:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit2, base.transform.position);
				}
				break;
			case NormalZombieType.DoorAndBucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case NormalZombieType.Door:
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
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
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
			if (base.DoorHp > 0 && (Type == NormalZombieType.DoorAndCone || Type == NormalZombieType.Door || Type == NormalZombieType.DoorAndBucket))
			{
				IsDoorArm(isDoorArm: true);
			}
			break;
		case ZombieState.Attack:
			animator.SetInteger("Change", 21);
			if (Type == NormalZombieType.DoorAndCone || Type == NormalZombieType.Door || Type == NormalZombieType.DoorAndBucket)
			{
				IsDoorArm(isDoorArm: false);
			}
			break;
		case ZombieState.Dead:
			base.DoorHp = 0;
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
		if (base.DoorHp > 0)
		{
			result = DoorRenderer;
			if (needClearEquip)
			{
				needDropDoor = false;
				base.DoorHp = 0;
			}
		}
		else if (base.Hp > 270 && (sprite == bucket1 || sprite == bucket2 || sprite == bucket3))
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
}
