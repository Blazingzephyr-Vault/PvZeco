using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Wintermelon : BulletPult
{
	protected override float Speed => 6f;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Wintermelon;

	protected override void HitEvent(ZombieBase zombie, int sortOrder)
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.melonimpact, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.melonimpact2, base.transform.position);
		}
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.WintermelonParticle);
		obj.transform.position = base.transform.position;
		obj.transform.GetComponent<SortingGroup>().sortingOrder = sortOrder;
		if (zombie != null)
		{
			zombie.Frozen(Vector2.down, isAudio: true, 2);
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2f, isHypno);
		for (int i = 0; i < zombies.Count; i++)
		{
			zombies[i].Frozen(Vector2.down, isAudio: true, 3);
			zombies[i].Hurt(attackValue / 2, Vector2.down);
		}
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.25f, !isHypno);
		for (int j = 0; j < aroundPlant.Count; j++)
		{
			aroundPlant[j].Hurt(attackValue / 2, null);
		}
	}
}
