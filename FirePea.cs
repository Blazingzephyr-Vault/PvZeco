using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class FirePea : MonoBehaviour
{
	private Rigidbody2D rigibody;

	private bool isHit;

	private bool isHypno;

	private int attackValue1;

	private Vector2 Dirction;

	private int CurrLine;

	public Renderer peaRenderer;

	public Renderer fireRenderer;

	public Animator animator;

	public void Init(Vector2 pos, int attackvalue, int line, Vector2 dirct, int sortOrder, bool ishyp)
	{
		fireRenderer.GetComponent<SortingGroup>().sortingOrder = sortOrder;
		peaRenderer.GetComponent<SortingGroup>().sortingOrder = sortOrder;
		peaRenderer.enabled = true;
		fireRenderer.enabled = false;
		isHit = false;
		isHypno = ishyp;
		CurrLine = line;
		Dirction = dirct;
		animator.Play("FirePeaShark");
		base.transform.rotation = Quaternion.FromToRotation(Vector3.right, dirct);
		base.transform.position = pos;
		attackValue1 = attackvalue * 2;
		rigibody = GetComponent<Rigidbody2D>();
		rigibody.velocity = Vector2.zero;
		rigibody.velocity = Dirction.normalized * 6f;
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
			isHit = true;
			gridByWorldPos.CurrPlantBase.Hurt(attackValue1, null);
			StartCoroutine(Fire());
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();

            /*if ((componentInParent.CurrLine == CurrLine || CurrLine == -1) && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
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
            }*/

            if (componentInParent.CurrLine == CurrLine && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				isHit = true;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
				componentInParent.UnFrozen();
				componentInParent.Hurt(attackValue1, Vector2.right, isHard: false, HitSound: false);
				List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 0.65f);
				for (int i = 0; i < zombies.Count; i++)
				{
					zombies[i].Hurt(attackValue1 / 3, Vector2.zero, isHard: false);
				}
				StartCoroutine(Fire());
			}
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
			isHit = true;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
			StartCoroutine(Fire());
		}
	}

	private IEnumerator Fire()
	{
		rigibody.velocity = Vector2.zero;
		animator.Play("FirePeaDead");
		peaRenderer.enabled = false;
		fireRenderer.enabled = true;
		fireRenderer.transform.GetComponent<SwfClipController>().GotoAndPlay(0);
		yield return new WaitForSeconds(1f);
		Destroy();
	}

	private void Destroy()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.FirePea1, base.gameObject);
	}
}
