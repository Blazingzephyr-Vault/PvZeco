using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GraveStone : MonoBehaviour
{
	public SpriteRenderer REnderer;

	public ParticleSystem DirtPs;

	public List<Sprite> StoneSprites = new List<Sprite>();

	public SpriteMask mask;

	public void CreateInit(Grid grid)
	{
		mask.enabled = true;
		base.transform.position = grid.Position;
		DirtPs.Play();
		DirtPs.transform.localScale = new Vector3(1f, 1f, 1f);
		DirtPs.transform.position = grid.Position + new Vector2(0f, -0.7f);
		DirtPs.GetComponent<SortingGroup>().sortingOrder = grid.Point.y * 200 + 101;
		REnderer.transform.position = grid.Position + new Vector2(0f, -1.5f);
		REnderer.sprite = StoneSprites[Random.Range(0, StoneSprites.Count)];
		REnderer.sortingOrder = grid.Point.y * 200 + 100;
		mask.frontSortingOrder = REnderer.sortingOrder;
		mask.backSortingOrder = REnderer.sortingOrder - 1;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position);
		StartCoroutine(MoveUp(grid.Position.y - 0.1f));
	}

	private IEnumerator MoveUp(float Y)
	{
		while (REnderer.transform.position.y < Y)
		{
			yield return new WaitForSeconds(0.02f);
			REnderer.transform.Translate(new Vector2(0f, 1f) * Time.deltaTime * 5f);
		}
		DirtPs.gameObject.SetActive(value: false);
		mask.enabled = false;
	}

	public void DestoryThis()
	{
		DirtPs.transform.SetParent(base.transform);
		DirtPs.gameObject.SetActive(value: false);
		Object.Destroy(base.gameObject);
	}
}
