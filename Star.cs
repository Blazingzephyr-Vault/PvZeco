using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Star : MonoBehaviour
{
	public Light2D light2;

	private Rigidbody2D rigibody;

	private bool isHit;

	private bool isHypno;

	private Vector2 Dirction;

	private int attackValue;

	private int CurrLine;

	private Starfruit Starfruit;

	private bool isCheck;

	private float UpLine;

	private float DownLine;

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirc, Starfruit starfruit, bool isCheck, bool isHyp)
	{
		isHypno = isHyp;
		light2.enabled = !isCheck;
		UpLine = MapManager.Instance.GetMapPos(pos).y + MapManager.Instance.GetCurrMap(pos).MapHalfLengthWidth.y;
		DownLine = MapManager.Instance.GetMapPos(pos).y - MapManager.Instance.GetCurrMap(pos).MapHalfLengthWidth.y;
		Starfruit = starfruit;
		this.isCheck = isCheck;
		if (isCheck)
		{
			base.transform.GetComponent<SpriteRenderer>().enabled = false;
		}
		else
		{
			base.transform.GetComponent<SpriteRenderer>().enabled = true;
		}
		base.transform.position = pos;
		Dirction = dirc;
		CurrLine = currLine;
		rigibody = GetComponent<Rigidbody2D>();
		rigibody.velocity = Vector2.zero;
		if (base.transform.localScale.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
		}
		rigibody.velocity = dirc.normalized * 6f;
		this.attackValue = attackValue;
		isHit = false;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
	}

	private void Update()
	{
		if (isHit)
		{
			return;
		}
		base.transform.Rotate(new Vector3(0f, 0f, 3f));
		if (MapManager.Instance.GetCurrMap(base.transform.position) == null)
		{
			Destroy();
		}
		Grid grid = null;
		grid = ((CurrLine != -1) ? MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine) : MapManager.Instance.GetGridByWorldPos(base.transform.position));
		if (grid == null || !(grid.CurrPlantBase != null) || ((grid.CurrPlantBase.isHypno || !isHypno) && (!grid.CurrPlantBase.isHypno || isHypno)) || !(Vector2.Distance(base.transform.position, grid.CurrPlantBase.transform.position) < 0.46f))
		{
			return;
		}
		if (Starfruit != null)
		{
			Starfruit.CheckZombieResult();
		}
		if (!isCheck)
		{
			grid.CurrPlantBase.Hurt(attackValue, null);
			HitEff();
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

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isHit)
		{
			return;
		}
		if (collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			if ((componentInParent.CurrLine == CurrLine || CurrLine == -1) && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				if (Starfruit != null)
				{
					Starfruit.CheckZombieResult();
				}
				if (!isCheck)
				{
					componentInParent.Hurt(attackValue, Dirction);
					HitEff();
				}
			}
		}
		if (!(collision.tag == "Wall") || collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
			return;
		}
		if (!isCheck)
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
		HitEff();
	}

	private void HitEff()
	{
		isHit = true;
		if (!isCheck)
		{
			GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.StarParticle);
			obj.transform.position = base.transform.position + new Vector3(rigibody.velocity.normalized.x * Random.Range(0.1f, 0.2f), rigibody.velocity.normalized.y * Random.Range(0.1f, 0.2f));
			obj.transform.GetComponent<SortingGroup>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
		}
		Destroy();
	}

	private void Destroy()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Star, base.gameObject);
	}
}
