using System.Collections;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering;

public class LawnMower : MonoBehaviour
{
	public int OnlineID;

	public Animator animator;

	public SpriteRenderer Shadow;

	protected bool IsRun;

	private bool IsFacingLeft;

	private bool going;

	private bool inWater;

	private float NormalHigh;

	private Grid currGrid;

	private Grid nextGrid;

	private Vector2 MoveTarget;

	protected virtual bool CanInWater { get; }

	public Grid CurrGrid
	{
		get
		{
			return currGrid;
		}
		set
		{
			if (value != currGrid && value != null)
			{
				currGrid = value;
				nextGrid = MapManager.Instance.GetNextGrid(currGrid, isRight: true);
				ResetMoveTarget();
			}
		}
	}

	public bool InWater
	{
		get
		{
			return inWater;
		}
		protected set
		{
			inWater = value;
			InWaterChangeEvent();
		}
	}

	protected void ResetMoveTarget()
	{
		if (nextGrid == null)
		{
			if (IsFacingLeft)
			{
				MoveTarget = new Vector2(-50f, currGrid.Position.y);
			}
			else
			{
				MoveTarget = new Vector2(50f, currGrid.Position.y);
			}
		}
		else if (currGrid.isSlope)
		{
			if (nextGrid.isSlope)
			{
				MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, (currGrid.Position.y + nextGrid.Position.y) / 2f);
			}
			else
			{
				MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, nextGrid.Position.y);
			}
		}
		else if (nextGrid.isSlope)
		{
			MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, currGrid.Position.y);
		}
		else
		{
			MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, nextGrid.Position.y);
		}
		if (IsFacingLeft)
		{
			MoveTarget -= new Vector2(0.1f, 0f);
		}
		else
		{
			MoveTarget += new Vector2(0.1f, 0f);
		}
	}

	private void Update()
	{
		UpdateEvent();
		if (!IsRun)
		{
			return;
		}
		float y = 0f;
		if (InWater)
		{
			y = -0.4f;
		}
		CurrGrid = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrGrid.Point.y);
		base.transform.position = Vector2.MoveTowards(base.transform.position, MoveTarget + new Vector2(0f, y), Time.deltaTime * 5f);
		if (base.transform.position.x > 8f)
		{
			DestroyMower();
		}
		if (CurrGrid.isWaterGrid && Mathf.Abs(base.transform.position.x - currGrid.Position.x) < 0.4f && !InWater && !going)
		{
			StartCoroutine(MoveInWater());
		}
		if (Mathf.Abs(CurrGrid.Position.x - base.transform.position.x) > 0.65f && (nextGrid == null || !nextGrid.isWaterGrid) && InWater && !going)
		{
			if (IsFacingLeft && CurrGrid.Position.x - base.transform.position.x > 0.65f)
			{
				StartCoroutine(MoveOutWater());
			}
			else if (!IsFacingLeft && base.transform.position.x - CurrGrid.Position.x > 0.65f)
			{
				StartCoroutine(MoveOutWater());
			}
		}
	}

	private void FixedUpdate()
	{
		ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(CurrGrid.Point.y, base.transform.position, getHyp: false, needCapsule: false);
		if (zombieByLineMinDisNoDir == null)
		{
			return;
		}
		if (IsRun)
		{
			if (Mathf.Abs(zombieByLineMinDisNoDir.transform.position.x - base.transform.position.x) < 0.2f)
			{
				zombieByLineMinDisNoDir.CleanerDead();
				KillZombieEvent();
			}
		}
		else if (Mathf.Abs(zombieByLineMinDisNoDir.transform.position.x - base.transform.position.x) < 0.2f)
		{
			Launch(synClient: false);
		}
	}

	public void Init(Grid grid, Vector2 pos)
	{
		IsRun = false;
		CurrGrid = grid;
		IsFacingLeft = false;
		base.transform.position = pos;
		animator.speed = 0f;
		NormalHigh = pos.y;
		animator.transform.GetComponent<SortingGroup>().sortingOrder = grid.Point.y * 200 + 198;
	}

	public void Launch(bool synClient)
	{
		if (!GameManager.Instance.isClient || synClient)
		{
			IsRun = true;
			animator.speed = 1f;
			LaunchEvent();
			if (GameManager.Instance.isServer)
			{
				MowerLaunch mowerLaunch = new MowerLaunch();
				mowerLaunch.id = OnlineID;
				mowerLaunch.pos = base.transform.position;
				SocketServer.Instance.SendMowerLaunch(mowerLaunch);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!IsRun && collision.tag == "Zombie")
		{
			ZombieBase component = collision.GetComponent<ZombieBase>();
			if (component.CurrLine == CurrGrid.Point.y && !component.isHypno && !(component is BungiZombie))
			{
				Launch(synClient: false);
			}
		}
	}

	private IEnumerator MoveInWater()
	{
		going = true;
		Shadow.enabled = false;
		while (base.transform.position.y > NormalHigh - 0.4f)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(0f, -1f) * Time.deltaTime * 5f);
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.EFObj).GetComponent<EFObj>().CreateInit(base.transform.position + new Vector3(-0.6f, 1f), 3, new Color(1f, 1f, 1f, 1f), CurrGrid.Point.y * 200 + 195);
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
		}
		if (!CanInWater)
		{
			DestroyMower();
		}
		InWater = true;
		going = false;
	}

	private IEnumerator MoveOutWater()
	{
		going = true;
		while (base.transform.position.y < CurrGrid.Position.y)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(0f, 1f) * Time.deltaTime * 5f);
		}
		Shadow.enabled = true;
		InWater = false;
		going = false;
	}

	public void DestroyMower()
	{
		Object.Destroy(base.gameObject);
	}

	protected virtual void UpdateEvent()
	{
	}

	protected virtual void InWaterChangeEvent()
	{
	}

	protected virtual void KillZombieEvent()
	{
	}

	protected virtual void LaunchEvent()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Lawnmower, base.transform.position);
	}
}
