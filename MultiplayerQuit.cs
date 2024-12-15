using UnityEngine;
using UnityEngine.EventSystems;

public class MultiplayerQuit : MonoBehaviour
{
	public Renderer REnderer;

	private void OnMouseEnter()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
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
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.CloseServer();
			}
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.CloseClient();
			}
		}
	}
}
