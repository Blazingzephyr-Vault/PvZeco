using FTRuntime;
using UnityEngine;

public class JalapenoFire : MonoBehaviour
{
	private SwfClipController clipController;

	private GameObject PreFab;

	public void CreateInit(Vector2 pos, GameObject prefab, int sort)
	{
		if (clipController == null)
		{
			clipController = GetComponent<SwfClipController>();
			clipController.clip.OnChangeCurrentFrameEvent += FrameChangeEvent;
		}
		clipController.clip.sortingOrder = sort;
		clipController.clip.currentFrame = 0;
		PreFab = prefab;
		base.transform.position = pos + new Vector2(0f, 0.8f);
	}

	public void DisAppear()
	{
		clipController.clip.currentFrame = 16;
	}

	protected void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.currentFrame == 15)
		{
			swfClip.currentFrame = 0;
		}
		if (swfClip.currentFrame == swfClip.frameCount - 1)
		{
			PoolManager.Instance.PushObj(PreFab, base.gameObject);
		}
	}
}
