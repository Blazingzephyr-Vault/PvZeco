using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Pea : MonoBehaviour
{
	private Rigidbody2D rigibody;

	private bool isHit;

	private bool isHypno;

	private Vector2 Dirction;

	private int attackValue;

	private int CurrLine;

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirct, int sortOrder, bool isHyp)
	{
		GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
		base.transform.position = pos;
		isHypno = isHyp;
		Dirction = dirct;
		CurrLine = currLine;
		rigibody = GetComponent<Rigidbody2D>();
		rigibody.velocity = Vector2.zero;
		rigibody.velocity = Dirction.normalized * 6f;
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
		base.transform.Rotate(new Vector3(0f, 0f, -1.5f));
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isHit)
		{
			return;
		}
		if (collision.tag == "Zombie")
		{
			ZombieBase component = collision.GetComponent<ZombieBase>();
			if (component.CurrLine == CurrLine && ((component.isHypno && isHypno) || (!component.isHypno && !isHypno)))
			{
				component.Hurt(attackValue, Dirction);
				HitEff();
			}
		}
		if (collision.tag == "Torchwood" && collision.GetComponent<Torchwood>().lineNum == CurrLine)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
			FirePea component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.FirePea1).GetComponent<FirePea>();
			component2.transform.SetParent(null);
			component2.Init(base.transform.position, attackValue, CurrLine, Dirction, GetComponent<SpriteRenderer>().sortingOrder, isHypno);
			Destroy();
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
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PeaParticle);
		obj.transform.position = base.transform.position + new Vector3(rigibody.velocity.normalized.x * Random.Range(0.1f, 0.2f), rigibody.velocity.normalized.y * Random.Range(0.1f, 0.2f));
		obj.transform.GetComponent<SortingGroup>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
		Destroy();
	}

	public void StartVerticalMove(float distance)
	{
		StartCoroutine(VerticalMove(distance));
	}

	private IEnumerator VerticalMove(float distance)
	{
		float goal = base.transform.position.y + distance;
		if (distance >= 0f)
		{
			float a = 0.06f;
			while (base.transform.position.y < goal)
			{
				yield return new WaitForSeconds(0.005f);
				base.transform.position += new Vector3(0f, a, 0f);
			}
		}
		else
		{
			float a = -0.06f;
			while (base.transform.position.y > goal)
			{
				yield return new WaitForSeconds(0.005f);
				base.transform.position += new Vector3(0f, a, 0f);
			}
		}
	}

	private void Destroy()
	{
		StopAllCoroutines();
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Pea, base.gameObject);
	}
}
