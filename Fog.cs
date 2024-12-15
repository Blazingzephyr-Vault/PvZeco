using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
	private class SubFrog
	{
		public SpriteRenderer render;

		private int lightNum;

		public Grid grid;

		private Fog Fog2;

		private float alpha;

		private Coroutine coroutine;

		public int LightNum
		{
			get
			{
				return lightNum;
			}
			set
			{
				lightNum = value;
				if (value < 0)
				{
					lightNum = 0;
				}
				if (lightNum > 0)
				{
					if (coroutine != null)
					{
						Fog2.StopCor(coroutine);
					}
					coroutine = Fog2.starcccc(Fade(isDis: true));
				}
				else
				{
					if (coroutine != null)
					{
						Fog2.StopCor(coroutine);
					}
					coroutine = Fog2.starcccc(Fade(isDis: false));
				}
			}
		}

		public SubFrog(SpriteRenderer Render, Fog fog, Grid grid2)
		{
			alpha = Render.color.a;
			render = Render;
			Fog2 = fog;
			grid = grid2;
		}

		public void AllDisplay()
		{
			render.enabled = true;
			render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
		}

		private IEnumerator Fade(bool isDis)
		{
			if (isDis)
			{
				while (render.color.a > 0f && Fog2.isNormalPos)
				{
					render.color = new Color(render.color.r, render.color.g, render.color.b, render.color.a - 2f * Time.deltaTime);
					yield return new WaitForFixedUpdate();
				}
			}
			else
			{
				while (render.color.a < alpha && Fog2.isNormalPos)
				{
					render.color = new Color(render.color.r, render.color.g, render.color.b, render.color.a + 2f * Time.deltaTime);
					yield return new WaitForFixedUpdate();
				}
			}
			if (!Fog2.isNormalPos)
			{
				if (isDis)
				{
					render.enabled = false;
				}
				else
				{
					render.enabled = true;
				}
			}
		}
	}

	public GameObject subFog;

	public MapBase currMap;

	public List<Sprite> sprites = new List<Sprite>();

	private bool isOpen;

	private Coroutine BackCoroutine;

	private bool isNormalPos;

	private float OutX;

	private float velocity;

	private Coroutine MoveCoroutine;

	private Dictionary<Vector2Int, SubFrog> pairs = new Dictionary<Vector2Int, SubFrog>();

	public bool IsOpen
	{
		get
		{
			return isOpen;
		}
		set
		{
			isOpen = value;
		}
	}

	public void CreateFog(int FogNum)
	{
		ClearFog();
		isNormalPos = false;
		base.transform.position = new Vector3(0f, currMap.transform.position.y);
		List<Grid> gridList = currMap.GridList;
		int num = currMap.MapGridNum.x - FogNum;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.x >= num)
			{
				SpriteRenderer component = Object.Instantiate(subFog).GetComponent<SpriteRenderer>();
				component.transform.position = gridList[i].Position;
				component.sprite = sprites[Random.Range(0, 8)];
				component.sortingOrder = 1000 + i;
				component.transform.SetParent(base.transform);
				if (gridList[i].Point.x == num)
				{
					component.color = new Color(1f, 1f, 1f, 0.8f);
				}
				else
				{
					component.color = new Color(1f, 1f, 1f, 0.9f);
				}
				if (pairs.Count == 0)
				{
					OutX = 0f - gridList[i].Position.x + 14f;
				}
				pairs.Add(gridList[i].Point, new SubFrog(component, this, gridList[i]));
			}
		}
		base.transform.position = new Vector3(OutX, base.transform.position.y);
	}

	private void ClearFog()
	{
		foreach (KeyValuePair<Vector2Int, SubFrog> pair in pairs)
		{
			Object.Destroy(pair.Value.render.gameObject);
		}
		pairs.Clear();
	}

	public void LightFog(Vector2Int pos, int x, int y, bool isLight)
	{
		Vector2Int vector2Int = pos - new Vector2Int(x, y);
		int num = 2 * x + 1;
		int num2 = 2 * y + 1;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				if (pairs.ContainsKey(vector2Int + new Vector2Int(i, j)))
				{
					if (isLight)
					{
						pairs[vector2Int + new Vector2Int(i, j)].LightNum++;
					}
					else
					{
						pairs[vector2Int + new Vector2Int(i, j)].LightNum--;
					}
				}
			}
		}
	}

	public void Blow()
	{
		if (isOpen)
		{
			if (MoveCoroutine != null)
			{
				StopCoroutine(MoveCoroutine);
			}
			MoveCoroutine = StartCoroutine(MoveOut(20f));
		}
	}

	public void TimeOut()
	{
		if (isOpen)
		{
			if (MoveCoroutine != null)
			{
				StopCoroutine(MoveCoroutine);
			}
			MoveCoroutine = StartCoroutine(MoveOut(5f));
		}
	}

	private IEnumerator MoveOut(float speed)
	{
		isNormalPos = false;
		while (base.transform.position.x < OutX)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(1f, 0f) * Time.deltaTime * speed);
		}
		if (BackCoroutine != null)
		{
			StopCoroutine(BackCoroutine);
			BackCoroutine = null;
		}
		BackCoroutine = StartCoroutine(MoveBack(25f, 3f));
		foreach (KeyValuePair<Vector2Int, SubFrog> pair in pairs)
		{
			pair.Value.AllDisplay();
		}
	}

	public void MoveBack()
	{
		if (isOpen)
		{
			if (MoveCoroutine != null)
			{
				StopCoroutine(MoveCoroutine);
			}
			MoveCoroutine = StartCoroutine(MoveBack(0f, 2f));
		}
	}

	private IEnumerator MoveBack(float delay, float speed)
	{
		yield return new WaitForSeconds(delay);
		while (base.transform.position.x > 1f)
		{
			yield return new WaitForFixedUpdate();
			float x = Mathf.SmoothDamp(base.transform.position.x, 0f, ref velocity, 100f * Time.deltaTime);
			base.transform.position = new Vector3(x, base.transform.position.y, base.transform.position.z);
		}
		while (base.transform.position.x > 0f)
		{
			base.transform.Translate(new Vector2(-0.5f, 0f) * Time.deltaTime * speed);
			yield return new WaitForFixedUpdate();
		}
		isNormalPos = true;
		foreach (KeyValuePair<Vector2Int, SubFrog> pair in pairs)
		{
			pair.Value.LightNum = pair.Value.grid.LightNum;
		}
	}

	private Coroutine starcccc(IEnumerator enumerator)
	{
		return StartCoroutine(enumerator);
	}

	private void StopCor(Coroutine enumerator)
	{
		StopCoroutine(enumerator);
	}
}
