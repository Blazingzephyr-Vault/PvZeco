public class HypnoShroom : PlantBase
{
	public override float MaxHp => 300f;

	protected override bool isShroom => true;

	protected override PlantType plantType => PlantType.HypnoShroom;

	protected override void GoSleepSpecial()
	{
		clipController.clip.sequence = "sleep";
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (zombie != null && !isSleeping && !isFlat && !GameManager.Instance.isClient)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.mindControlled, base.transform.position);
			zombie.Hypno();
			Dead();
		}
	}
}
