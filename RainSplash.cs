using FTRuntime;
using UnityEngine;

public class RainSplash : MonoBehaviour
{
	public SwfClipController clipController;

	private void Update()
	{
		if (!clipController.isPlaying)
		{
			clipController.GotoAndPlay(0);
			PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Rain_splash, base.gameObject);
		}
	}
}
