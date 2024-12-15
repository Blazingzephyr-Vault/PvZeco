using UnityEngine;

public class PvPButton : MonoBehaviour
{
	public Renderer REnderer;

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject() && GameManager.Instance.isOnline)
		{
			REnderer.material.SetFloat("_Brightness", 1.3f);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
		}
	}

	private void OnMouseExit()
	{
		REnderer.material.SetFloat("_Brightness", 1f);
	}

	private void OnMouseDown()
	{
		if (!MyTool.IsPointerOverGameObject() && GameManager.Instance.isOnline)
		{
			PvPSelector.Instance.OpenAndInit();
		}
	}
}
