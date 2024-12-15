using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCob : MonoBehaviour
{
	private float UpLine;

	private Vector2 targetPos;

	private SpriteRenderer BlastMark;

	private int attackValue;

	private bool isHypno;

	public void CreateInit(Vector2 pos, Vector2 target, int attackvalue, bool isHyp)
	{
		isHypno = isHyp;
		attackValue = attackvalue;
		base.transform.GetComponent<SpriteRenderer>().enabled = true;
		BlastMark = base.transform.Find("BlastMark").GetComponent<SpriteRenderer>();
		BlastMark.enabled = false;
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(target);
		BlastMark.sortingOrder = gridByWorldPos.Point.y * 200 + 1;
		UpLine = MapManager.Instance.GetMapPos(pos).y + MapManager.Instance.GetCurrMap(pos).MapHalfLengthWidth.y;
		base.transform.position = pos;
		targetPos = target;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		StartCoroutine(MoveUp());
	}

	private IEnumerator MoveUp()
	{
		while (base.transform.position.y < UpLine + 5f)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(1f, 0f) * Time.deltaTime / 0.1f);
		}
		StartCoroutine(MoveDown());
	}

	private IEnumerator MoveDown()
	{
		base.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
		MapBase currMap = MapManager.Instance.GetCurrMap(targetPos);
		float num = currMap.transform.position.y + currMap.MapHalfLengthWidth.y;
		base.transform.position = new Vector3(targetPos.x, num + 5f);
		float seconds = 2f;
		if (currMap == MapManager.Instance.GetCurrMap(targetPos))
		{
			seconds = 0.5f;
		}
		yield return new WaitForSeconds(seconds);
		while (base.transform.position.y > targetPos.y)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(1f, 0f) * Time.deltaTime * 10f);
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, needCapsule: false, isHypno);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, !isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int i = 0; i < aroundPlant.Count; i++)
			{
				aroundPlant[i].Hurt(attackValue / aroundPlant.Count, null);
			}
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(attackValue / zombies.Count);
			}
		}
		else
		{
			for (int k = 0; k < zombies.Count; k++)
			{
				zombies[k].BoomHurt(attackValue);
			}
			for (int l = 0; l < aroundPlant.Count; l++)
			{
				aroundPlant[l].Hurt(attackValue, null);
			}
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PopcornParticle).transform.position = base.transform.position;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Doomm, base.transform.position);
		base.transform.GetComponent<SpriteRenderer>().enabled = false;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		BlastMark.enabled = true;
		CameraControl.Instance.ShakeCamera(base.transform.position);
		yield return new WaitForSeconds(1f);
		Object.Destroy(base.gameObject);
	}
}
