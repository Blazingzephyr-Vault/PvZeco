using UnityEngine;

namespace StartScene;

public class Adventure : MonoBehaviour
{
	public Renderer REnderer;

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject())
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
		if (!MyTool.IsPointerOverGameObject() && !GameManager.Instance.isClient)
		{
			LVManager.Instance.StartGame(null);
		}
	}
}
