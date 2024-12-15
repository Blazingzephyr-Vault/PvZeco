using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class BungiZombie : ZombieBase
{
	public float CreateY;

	public Grid SpawnGrid;

	private PlantBase GetPlantBase;

	private GameObject Target;

	private bool isHitUmbrella;

	protected override GameObject Prefab => GameManager.Instance.GameConf.BungiZombie;

	protected override float AnToSpeed => 1f;

	protected override float DefSpeed => 1f;

	protected override float attackValue => 0f;

	public override int MaxHP => 500;

	public override void InitZombieHpState()
	{
		if (IsOVer)
		{
			return;
		}
		base.transform.Find("BungeeCord").localScale = Vector3.one;
		if (LVManager.Instance.CurrLVState == LVState.Fighting)
		{
			base.State = ZombieState.Idel;
			isHitUmbrella = false;
			anCanMove = false;
			dontChangeState = true;
			Shadow.enabled = false;
			capsuleCollider2D.enabled = false;
			canIce = false;
			canButter = false;
			if (!GameManager.Instance.isClient)
			{
				CreateY = MapManager.Instance.GetMapYHighest(SpawnPos) + 1f;
				if (SpawnGrid == null)
				{
					SpawnGrid = MapManager.Instance.GetGridByWorldPos(SpawnPos);
				}
				base.transform.position = new Vector2(SpawnGrid.Position.x, CreateY);
				base.CurrGrid = SpawnGrid;
				base.CurrGrid.isZombieSigned = true;
				if (SpawnGrid.CurrPlantBase != null)
				{
					Sorting.sortingOrder = SpawnGrid.CurrPlantBase.SortingOrder - 1;
				}
				base.transform.Find("BungeeCord").GetComponent<SpriteRenderer>().sortingOrder = Sorting.sortingOrder - 1;
				StartCoroutine(MoveDown());
				StartCoroutine(SetTarget());
				animator.Play("drop");
			}
		}
		else
		{
			base.State = ZombieState.Idel;
			base.transform.position = new Vector3(base.transform.position.x, base.CurrGrid.Position.y + 0.2f, base.transform.position.z);
			base.transform.Find("BungeeCord").GetComponent<SpriteRenderer>().sortingOrder = Sorting.sortingOrder - 1;
		}
	}

	protected override void CreateInitZombie()
	{
		base.transform.Find("BungeeCord").localScale = Vector3.zero;
	}

	protected override void ClientInit(ZombieSpawn spawnInfo)
	{
		if (LVManager.Instance.CurrLVState == LVState.Fighting)
		{
			SpawnGrid = MapManager.Instance.GetGridByWorldPos(spawnInfo.SpawnPos);
			CreateY = MapManager.Instance.GetMapYHighest(SpawnGrid.Position) + 1f;
			base.transform.position = new Vector2(SpawnGrid.Position.x, CreateY);
			base.CurrGrid = SpawnGrid;
			base.CurrGrid.isZombieSigned = true;
			if (SpawnGrid.CurrPlantBase != null)
			{
				Sorting.sortingOrder = SpawnGrid.CurrPlantBase.SortingOrder - 1;
			}
			base.transform.Find("BungeeCord").GetComponent<SpriteRenderer>().sortingOrder = Sorting.sortingOrder - 1;
			StartCoroutine(MoveDown());
			StartCoroutine(SetTarget());
			animator.Play("drop");
		}
	}

	protected override void ServerInitInfo(ZombieSpawn spawnInfo)
	{
		if (SpawnGrid != null)
		{
			spawnInfo.SpawnPos = SpawnGrid.Position;
		}
	}

	protected override void CheckState()
	{
		animator.Play("idel1");
		if (base.State == ZombieState.Dead)
		{
			Dead(canDropItem: true, 0f);
		}
	}

	private IEnumerator MoveDown()
	{
		yield return new WaitForSeconds(1f);
		switch (Random.Range(0, 3))
		{
		case 0:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bungee_scream1, SpawnGrid.Position);
			break;
		case 1:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bungee_scream2, SpawnGrid.Position);
			break;
		default:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bungee_scream3, SpawnGrid.Position);
			break;
		}
		yield return new WaitForSeconds(1f);
		while (base.transform.position.y > SpawnGrid.Position.y + 0.9f)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(0f, -2f) * Time.deltaTime * 6f);
		}
		List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(MapManager.Instance.GetGridByWorldPos(SpawnGrid.Position), 1);
		for (int i = 0; i < aroundGrid.Count; i++)
		{
			if (!(aroundGrid[i].CurrPlantBase != null))
			{
				continue;
			}
			if (aroundGrid[i].CurrPlantBase is Umbrellaleaf)
			{
				if (aroundGrid[i].CurrPlantBase.GetComponent<Umbrellaleaf>().Block())
				{
					isHitUmbrella = true;
				}
			}
			else if (aroundGrid[i].CurrPlantBase.CarryPlant is Umbrellaleaf && aroundGrid[i].CurrPlantBase.CarryPlant.GetComponent<Umbrellaleaf>().Block())
			{
				isHitUmbrella = true;
			}
		}
		if (!isHitUmbrella)
		{
			while (base.transform.position.y > SpawnGrid.Position.y + 0.4f)
			{
				yield return new WaitForSeconds(0.02f);
				base.transform.Translate(new Vector2(0f, -2f) * Time.deltaTime * 8f);
			}
			capsuleCollider2D.enabled = true;
			animator.SetInteger("Change", 41);
			dontChangeState = false;
			canIce = true;
			canButter = true;
			yield return new WaitForSeconds(3f);
			animator.SetInteger("Change", 42);
		}
		else
		{
			StartCoroutine(MoveUp());
		}
	}

	private IEnumerator MoveUp()
	{
		canIce = false;
		canButter = false;
		capsuleCollider2D.enabled = false;
		dontChangeState = true;
		while (base.transform.position.y < CreateY)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(0f, 3f) * Time.deltaTime * 5f);
		}
		base.CurrGrid.isZombieSigned = false;
		if (GetPlantBase != null)
		{
			GetPlantBase.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
		}
		DirectDead(canDropItem: false, 0f);
	}

	private IEnumerator SetTarget()
	{
		Target = Object.Instantiate(GameManager.Instance.GameConf.BungeeTarget);
		Target.GetComponent<SpriteRenderer>().sortingOrder = Sorting.sortingOrder + 100;
		Target.transform.position = base.transform.position;
		while (Target.transform.position.y > SpawnGrid.Position.y + 0.1f)
		{
			yield return new WaitForFixedUpdate();
			Target.transform.Translate(new Vector2(0f, -2f) * Time.deltaTime * 5f);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.grassstep, Target.transform.position);
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if (Target != null)
		{
			Object.Destroy(Target.gameObject);
		}
		SpawnGrid = null;
	}

	public override void SpecialAnimEvent1()
	{
		if (LVManager.Instance.CurrLVState == LVState.Start)
		{
			clipController.clip.sequence = "idel1";
			return;
		}
		if (!GameManager.Instance.isClient)
		{
			if (SpawnGrid.CurrPlantBase != null)
			{
				if (SpawnGrid.CurrPlantBase.CarryPlant != null)
				{
					GetPlantBase = SpawnGrid.CurrPlantBase.CarryPlant;
				}
				else if (SpawnGrid.CurrPlantBase.CanCarryOtherPlant && SpawnGrid.CurrPlantBase.ProtectPlant != null)
				{
					GetPlantBase = SpawnGrid.CurrPlantBase.ProtectPlant;
				}
				else
				{
					GetPlantBase = SpawnGrid.CurrPlantBase;
				}
			}
			if (GetPlantBase != null)
			{
				if (GameManager.Instance.isServer)
				{
					SynItem synItem = new SynItem();
					synItem.OnlineId = OnlineId;
					synItem.Type = 2;
					synItem.SynCode[0] = 2;
					synItem.SynCode[1] = 1;
					SocketServer.Instance.SendSynBag(synItem);
				}
				GetPlantBase.Dead(isFlat: false, 10f, synClient: true, deadRattle: false);
				GetPlantBase.transform.SetParent(base.transform);
			}
		}
		StartCoroutine(MoveUp());
		if (Target != null)
		{
			Target.transform.SetParent(base.transform);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.floop, SpawnGrid.Position);
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
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

	public override void OnlineSynZombie(SynItem syn)
	{
		base.OnlineSynZombie(syn);
		if (syn.SynCode[0] != 2 || syn.SynCode[1] != 1)
		{
			return;
		}
		if (SpawnGrid.CurrPlantBase != null)
		{
			if (SpawnGrid.CurrPlantBase.CarryPlant != null)
			{
				GetPlantBase = SpawnGrid.CurrPlantBase.CarryPlant;
			}
			else if (SpawnGrid.CurrPlantBase.CanCarryOtherPlant && SpawnGrid.CurrPlantBase.ProtectPlant != null)
			{
				GetPlantBase = SpawnGrid.CurrPlantBase.ProtectPlant;
			}
			else
			{
				GetPlantBase = SpawnGrid.CurrPlantBase;
			}
		}
		if (GetPlantBase != null)
		{
			GetPlantBase.Dead(isFlat: false, 10f, synClient: true, deadRattle: false);
			GetPlantBase.transform.SetParent(base.transform);
		}
	}
}
