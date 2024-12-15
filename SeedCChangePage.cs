using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeedCChangePage : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	private Image LightImage;

	public bool isNextPage;

	private void Awake()
	{
		LightImage = base.transform.Find("Light").GetComponent<Image>();
		LightImage.transform.localScale = Vector3.zero;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		LightImage.transform.localScale = Vector3.one;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		LightImage.transform.localScale = Vector3.zero;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
		if (isNextPage)
		{
			SeedChooser.Instance.CurrPage++;
		}
		else
		{
			SeedChooser.Instance.CurrPage--;
		}
	}
}
