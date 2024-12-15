using UnityEngine;

public class Kernal : BulletPult
{
	protected override float Speed => 6f;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Kernal;

	protected override void HitEvent(ZombieBase zombie, int sortOrder)
	{
		if (Random.Range(0, 2) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.kernalpult1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.kernalpult2, base.transform.position);
		}
	}
}
