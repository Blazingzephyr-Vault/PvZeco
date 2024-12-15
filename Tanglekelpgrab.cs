using System.Collections;
using FTRuntime;
using UnityEngine;

public class Tanglekelpgrab : MonoBehaviour
{
	private Tanglekelp Tanglekelp;

	private ZombieBase zombie;

	private int attackValue;

	private bool isGrabZombie;

	public void Init(Tanglekelp tanglekelp, ZombieBase zombie, int attackvalue)
	{
		attackValue = attackvalue;
		SwfClipController component = base.transform.Find("TanglekelpGrab").GetComponent<SwfClipController>();
		component.clip.currentFrame = 0;
		component.clip.sortingOrder = zombie.SortOrder + 1;
		component.GotoAndPlay(0);
		base.transform.position = new Vector2(zombie.transform.position.x - 0.8f, tanglekelp.transform.position.y) + new Vector2(0f, 1f);
		Tanglekelp = tanglekelp;
		this.zombie = zombie;
		isGrabZombie = zombie.Hp <= attackValue;
		StartCoroutine(Grab());
	}

	private IEnumerator Grab()
	{
		if (isGrabZombie)
		{
			zombie.StopAction();
		}
		yield return new WaitForSeconds(0.3f);
		while (base.transform.position.y > Tanglekelp.transform.position.y - 0.8f)
		{
			yield return new WaitForFixedUpdate();
			base.transform.position += new Vector3(0f, -4f * Time.deltaTime, 0f);
			if (isGrabZombie)
			{
				zombie.transform.position += new Vector3(0f, -4f * Time.deltaTime, 0f);
			}
		}
		Over();
	}

	public void Over()
	{
		if (zombie != null && zombie.isActiveAndEnabled)
		{
			if (isGrabZombie)
			{
				zombie.DirectDead(canDropItem: true, 0f);
			}
			else
			{
				zombie.Hurt(500, Vector2.zero, isHard: false);
			}
		}
		if (Tanglekelp != null && Tanglekelp.isActiveAndEnabled)
		{
			Tanglekelp.Dead();
		}
		StopAllCoroutines();
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Tanglekelpgrab, base.gameObject);
	}
}
