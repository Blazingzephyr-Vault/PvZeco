using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LVFlag : MonoBehaviour
{
	private Image image;

	public void GoRise()
	{
		image = base.transform.Find("Image").GetComponent<Image>();
		StartCoroutine(Rise());
	}

	private IEnumerator Rise()
	{
		float x = image.transform.position.y + 5f;
		while (image.transform.position.y < x)
		{
			image.transform.Translate(new Vector3(0f, 0.5f, 0f));
			yield return new WaitForSeconds(0.02f);
		}
	}

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}
}
