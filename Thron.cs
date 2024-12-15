using System.Collections.Generic;
using UnityEngine;

public class Thron : MonoBehaviour
{
	private Rigidbody2D rigibody;

	private bool isHit;

	private Vector2 Dirction;

	private int attackValue;

	private int CurrLine;

	private bool isHypno;

	private BalloonZombie TargetZombie;

	private int HitNum;

	private List<Grid> hitOverGrids = new List<Grid>();

	private List<ZombieBase> hitOverZombies = new List<ZombieBase>();

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirct, int sortOrder, bool isHyp, ZombieBase zombie = null)
	{
		GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
		isHypno = isHyp;
		base.transform.localScale = new Vector3(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y);
		if (dirct.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
		}
		HitNum = 0;
		hitOverGrids.Clear();
		hitOverZombies.Clear();
		TargetZombie = null;
		if (zombie != null)
		{
			TargetZombie = zombie.GetComponent<BalloonZombie>();
		}
		base.transform.position = pos;
		Dirction = dirct;
		CurrLine = currLine;
		rigibody = GetComponent<Rigidbody2D>();
		rigibody.velocity = Vector2.zero;
		rigibody.velocity = Dirction.normalized * 6f;
		this.attackValue = attackValue;
		isHit = false;
	}

	private void Update()
	{
		if (isHit)
		{
			return;
		}
		if (base.transform.position.x > 15f)
		{
			Destroy();
			return;
		}
		if (base.transform.position.x < -15f)
		{
			Destroy();
			return;
		}
		if (TargetZombie != null && TargetZombie.CurrLine == CurrLine && TargetZombie.IsFly() && TargetZombie.Hp > 0 && Mathf.Abs(TargetZombie.transform.position.x - base.transform.position.x) < 0.1f)
		{
			TargetZombie.Hurt(attackValue, Vector2.zero);
			Destroy();
			return;
		}
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine);
		if (!hitOverGrids.Contains(gridByWorldPos) && gridByWorldPos != null && gridByWorldPos.CurrPlantBase != null && ((!gridByWorldPos.CurrPlantBase.isHypno && isHypno) || (gridByWorldPos.CurrPlantBase.isHypno && !isHypno)) && Mathf.Abs(base.transform.position.x - gridByWorldPos.Position.x) < 0.2f)
		{
			hitOverGrids.Add(gridByWorldPos);
			gridByWorldPos.CurrPlantBase.Hurt(attackValue, null);
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
			HitNum += 3;
			if (HitNum > 5)
			{
				Destroy();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isHit)
		{
			return;
		}
		if (collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			if (componentInParent.CurrLine == CurrLine && !hitOverZombies.Contains(componentInParent) && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				hitOverZombies.Add(componentInParent);
				componentInParent.Hurt(attackValue, Vector2.zero);
				HitNum++;
				if (HitNum > 5)
				{
					Destroy();
				}
			}
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
			isHit = true;
			Destroy();
		}
	}

	private void Destroy()
	{
		StopAllCoroutines();
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Thron, base.gameObject);
	}
}
