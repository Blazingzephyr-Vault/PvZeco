using UnityEngine;

public class Butter : BulletPult
{
	protected override float Speed => 6f;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Butter;

	protected override void HitEvent(ZombieBase zombie, int sortOrder)
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.butter, base.transform.position);
		if (zombie != null)
		{
			zombie.Butter();
		}
	}
}
