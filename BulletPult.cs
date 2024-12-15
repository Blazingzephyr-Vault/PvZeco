using UnityEngine;

public abstract class BulletPult : MonoBehaviour
{
	private PlantBase targetplant;

	private ZombieBase targetzombie;

	private Vector2 startPos;

	private Vector2 midPos;

	private Vector2 lastTargetPos;

	private Vector2 zombieDeadPos;

	protected float percent;

	private float percentSpeed;

	private bool isHit;

	private bool targetDead;

	protected bool isHypno;

	private float rotationNum;

	protected int attackValue;

	protected abstract float Speed { get; }

	protected virtual bool NeedPeaAudio { get; }

	protected abstract GameObject Prefab { get; }

	public void Init(PlantBase plant, ZombieBase zombie, Vector2 pos, int attackValue, int sortOrder, bool isHyp)
	{
		if (plant != null)
		{
			targetplant = plant;
			targetzombie = null;
			lastTargetPos = targetplant.transform.position;
		}
		else
		{
			targetplant = null;
			targetzombie = zombie;
			lastTargetPos = targetzombie.transform.position;
		}
		isHypno = isHyp;
		targetDead = false;
		percent = 0f;
		isHit = false;
		this.attackValue = attackValue;
		startPos = pos;
		GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
		percentSpeed = Speed / (lastTargetPos - startPos).magnitude;
		if (percentSpeed > Speed / 6f)
		{
			percentSpeed = Speed / 6f;
		}
		base.transform.position = pos;
		rotationNum = Random.Range(2f, 3f);
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
	}

	private void Update()
	{
		if (isHit)
		{
			return;
		}
		if (targetplant == null)
		{
			if (!targetDead && targetzombie.Hp > 0 && targetzombie.capsuleCollider2D.enabled)
			{
				lastTargetPos = targetzombie.transform.position + new Vector3(0f, 0.5f);
				zombieDeadPos = targetzombie.transform.position;
			}
			else if (!targetDead)
			{
				targetDead = true;
				lastTargetPos = zombieDeadPos + new Vector2(0f, -1f);
				if ((targetzombie.transform.position.x < startPos.x && !isHypno) || (targetzombie.transform.position.x > startPos.x && isHypno))
				{
					targetDead = true;
					lastTargetPos = zombieDeadPos + new Vector2(0f, -1f);
				}
			}
		}
		else if (!targetDead && targetplant.Hp > 0f)
		{
			lastTargetPos = targetplant.transform.position;
			zombieDeadPos = targetplant.transform.position;
		}
		else if (!targetDead)
		{
			targetDead = true;
			lastTargetPos = zombieDeadPos + new Vector2(0f, -1f);
			if ((targetplant.transform.position.x < startPos.x && !isHypno) || (targetplant.transform.position.x > startPos.x && isHypno))
			{
				targetDead = true;
				lastTargetPos = zombieDeadPos + new Vector2(0f, -1f);
			}
		}
		if (percent >= 1f)
		{
			if (!targetDead)
			{
				if (targetplant != null)
				{
					if ((!isHypno && targetplant.isHypno) || (isHypno && !targetplant.isHypno))
					{
						targetplant.Hurt(attackValue, null);
						if (NeedPeaAudio)
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
				}
				else if (targetzombie != null && targetzombie.gameObject.activeSelf && ((isHypno && targetzombie.isHypno) || (!isHypno && !targetzombie.isHypno)))
				{
					targetzombie.Hurt(attackValue, Vector2.down);
				}
			}
			HitEvent(targetzombie, GetComponent<SpriteRenderer>().sortingOrder);
			Destroy();
		}
		if (base.transform.position.y < startPos.y - 1.6f)
		{
			HitEvent(null, GetComponent<SpriteRenderer>().sortingOrder);
			Destroy();
		}
		percent += percentSpeed * Time.deltaTime;
		if (percent > 1f)
		{
			percent = 1f;
		}
		base.transform.Rotate(new Vector3(0f, 0f, 0f - rotationNum));
		if (percent < 0.6f)
		{
			midPos = Vector2.Lerp(startPos, lastTargetPos, 0.5f);
			midPos.y += Mathf.Abs(startPos.x - lastTargetPos.x);
			if (midPos.y < startPos.y + 6.5f)
			{
				midPos.y = startPos.y + 6.5f;
			}
		}
		base.transform.position = MyTool.Bezier(percent, startPos, midPos, lastTargetPos);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(collision.transform != targetzombie.transform) && collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			isHit = true;
			HitEvent(componentInParent, GetComponent<SpriteRenderer>().sortingOrder);
			if (componentInParent != null && componentInParent.gameObject.activeSelf && ((isHypno && componentInParent.isHypno) || (!isHypno && !componentInParent.isHypno)))
			{
				componentInParent.Hurt(attackValue, Vector2.down);
			}
			Destroy();
		}
	}

	protected abstract void HitEvent(ZombieBase zombie, int sortOrder);

	private void Destroy()
	{
		PoolManager.Instance.PushObj(Prefab, base.gameObject);
	}
}
