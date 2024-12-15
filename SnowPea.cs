using UnityEngine;
using UnityEngine.Rendering;

public class SnowPea : MonoBehaviour
{
	private Rigidbody2D rigibody;

	private int attackValue;

	private bool isHit;

	private bool isHypno;

	private Vector2 Dirction;

	private int CurrLine;

	private int FrozenLvl;

	public SortingGroup SnowSort;

	public void Init(int attackValue, Vector2 pos, int line, Vector2 dirction, int sortOrder, bool isHyp, int frozenLvl = 1)
	{
		SnowSort.sortingOrder = sortOrder;
		GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
		SortingGroup component = base.transform.Find("SnowFlakeParticle").GetComponent<SortingGroup>();
		component.sortingOrder = sortOrder;
		component.transform.localScale = new Vector3(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y);
		base.transform.localScale = new Vector3(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y);
		if (dirction.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
			component.transform.localScale = new Vector3(0f - Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y);
		}
		FrozenLvl = frozenLvl;
		isHit = false;
		isHypno = isHyp;
		CurrLine = line;
		Dirction = dirction;
		base.transform.position = pos;
		rigibody = GetComponent<Rigidbody2D>();
		rigibody.velocity = Vector2.zero;
		rigibody.velocity = dirction.normalized * 6f;
		this.attackValue = attackValue;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FrozenPea, base.transform.position);
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
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine);
		if (gridByWorldPos != null && gridByWorldPos.CurrPlantBase != null && ((!gridByWorldPos.CurrPlantBase.isHypno && isHypno) || (gridByWorldPos.CurrPlantBase.isHypno && !isHypno)) && Mathf.Abs(base.transform.position.x - gridByWorldPos.Position.x) < 0.2f)
		{
			gridByWorldPos.CurrPlantBase.Hurt(attackValue, null);
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
		if (collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			if (componentInParent == null)
			{
				return;
			}
			if (componentInParent.CurrLine == CurrLine && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				collision.GetComponentInParent<ZombieBase>().Frozen(Dirction, isAudio: true, FrozenLvl);
				collision.GetComponentInParent<ZombieBase>().Hurt(attackValue, Dirction);
				HitEff();
			}
		}
		if (collision.tag == "Torchwood")
		{
			Invoke("melt", 0.2f);
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
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
			HitEff();
		}
	}

	private void HitEff()
	{
		isHit = true;
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SnowpeaParticle);
		obj.transform.position = base.transform.position + new Vector3(rigibody.velocity.normalized.x * Random.Range(0.1f, 0.2f), rigibody.velocity.normalized.y * Random.Range(0.1f, 0.2f));
		obj.transform.GetComponent<SortingGroup>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
		Destroy();
	}

	private void melt()
	{
		Pea component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
		component.transform.SetParent(null);
		component.Init(attackValue, base.transform.position, CurrLine, Dirction, GetComponent<SpriteRenderer>().sortingOrder, isHypno);
		Destroy();
	}

	private void Destroy()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.FrozenPea1, base.gameObject);
	}
}
