using UnityEngine;

namespace StartScene;

public class Store : MonoBehaviour
{
	public Renderer REnderer;

	private void OnMouseEnter()
	{
		MyTool.IsPointerOverGameObject();
	}

	private void OnMouseExit()
	{
		REnderer.material.SetFloat("_Brightness", 1f);
	}

	private void OnMouseDown()
	{
		MyTool.IsPointerOverGameObject();
	}
}
