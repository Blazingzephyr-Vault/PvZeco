using UnityEngine;

namespace StartScene;

public class MultiplayerBackButton : MonoBehaviour
{
	public Renderer REnderer;

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			REnderer.material.SetFloat("_Brightness", 1.3f);
		}
	}

	private void OnMouseExit()
	{
		REnderer.material.SetFloat("_Brightness", 1f);
	}

	private void OnMouseDown()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			CameraControl.Instance.MoveTo(new Vector2(0f, -30f), null);
		}
	}
}
