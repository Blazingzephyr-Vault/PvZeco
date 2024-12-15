using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
	public ParticleSystem MainLightning;

	public ParticleSystem SubLightning;

	public GameObject MainSpotLight;

	public GameObject AllLight;

	public void LightGrid(Grid grid)
	{
		base.transform.position = grid.Position;
		MainSpotLight.transform.position = grid.Position + new Vector2(0f, 11f);
		MainLightning.Play();
		SubLightning.Play();
		AllLight.gameObject.SetActive(value: false);
		switch (Random.Range(0, 4))
		{
		case 0:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Thunder, base.transform.position, isAll: true);
			break;
		case 1:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Thunder1, base.transform.position, isAll: true);
			break;
		case 2:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Thunder2, base.transform.position, isAll: true);
			break;
		case 3:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Thunder3, base.transform.position, isAll: true);
			break;
		}
		MapBase currMap = MapManager.Instance.GetCurrMap(grid.Position);
		SubLightning.transform.position = new Vector3(grid.Position.x, currMap.transform.position.y + currMap.MapHalfLengthWidth.y);
		StartCoroutine(waitLight(grid));
	}

	private IEnumerator waitLight(Grid grid)
	{
		yield return new WaitForSeconds(0.3f);
		AllLight.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(0.3f);
		List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(grid, 1);
		for (int i = 0; i < aroundGrid.Count; i++)
		{
			if (aroundGrid[i].CurrPlantBase != null && Random.Range(0, 8) > 6)
			{
				aroundGrid[i].CurrPlantBase.Hurt(300f, null);
			}
			else if (aroundGrid[i].CurrPlantBase != null)
			{
				aroundGrid[i].CurrPlantBase.Hurt(150f, null);
			}
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(grid.Position, 2.6f);
		for (int j = 0; j < zombies.Count; j++)
		{
			zombies[j].BoomHurt(400);
		}
		yield return new WaitForSeconds(0.1f);
		base.transform.gameObject.SetActive(value: false);
	}
}
