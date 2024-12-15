using UnityEngine;

public class ShroomPuff : MonoBehaviour
{
	private Rigidbody2D rigibody;

	private int attackValue;

	private Vector2 Dirction;

	private int CurrLine;

	private bool isHypno;

	private float createX;

	private bool needDis;

	public Transform particle;

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirct, bool NeedDis, bool isHyp)
	{
		isHypno = isHyp;
		needDis = NeedDis;
		createX = pos.x;
		CurrLine = currLine;
		Dirction = dirct;
		base.transform.position = pos;
		rigibody = GetComponent<Rigidbody2D>();
		rigibody.velocity = Vector2.zero;
		rigibody.velocity = dirct * 6f;
		this.attackValue = attackValue;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		particle.localScale = new Vector3(Mathf.Abs(particle.localScale.x), particle.localScale.y, particle.localScale.z);
		if (dirct.x < 0f)
		{
			particle.localScale = new Vector3(0f - particle.localScale.x, particle.localScale.y, particle.localScale.z);
		}
	}

	private void Update()
	{
		if (needDis && Mathf.Abs(base.transform.position.x - createX) > 4.9f)
		{
			Destroy();
			return;
		}
		if (MapManager.Instance.GetCurrMap(base.transform.position) == null)
		{
			Destroy();
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
			if (componentInParent.CurrLine == CurrLine && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				componentInParent.Hurt(attackValue, Dirction);
				HitEff();
			}
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
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PuffParticle).transform.position = base.transform.position + new Vector3(rigibody.velocity.normalized.x * Random.Range(0.1f, 0.2f), rigibody.velocity.normalized.y * Random.Range(0.1f, 0.2f));
		Destroy();
	}

	private void Destroy()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.ShroomPuff, base.gameObject);
	}
}
