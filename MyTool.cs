using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class MyTool
{
	public static Vector2 Bezier(float t, Vector2 start, Vector2 mid, Vector2 end)
	{
		Vector2 a = Vector2.Lerp(start, mid, t);
		Vector2 b = Vector2.Lerp(mid, end, t);
		return Vector2.Lerp(a, b, t);
	}

	public static Vector2 GetMiddlePosition(Vector2 start, Vector2 end)
	{
		Vector2 result = Vector2.Lerp(start, end, 0.5f);
		result.y += Mathf.Abs(start.x - end.x);
		if (result.y < start.y + 6.5f)
		{
			result.y = start.y + 6.5f;
		}
		return result;
	}

	public static Vector2 ReverseX(Vector2 vector)
	{
		return new Vector2(0f - vector.x, vector.y);
	}

	public static Vector3 ReverseX(Vector3 vector)
	{
		return new Vector3(0f - vector.x, vector.y);
	}

	public static Sprite MergeTex(Texture2D[] texs)
	{
		if (texs.Length < 1)
		{
			return null;
		}
		Texture2D texture2D = new Texture2D(texs[0].width, texs[0].height, TextureFormat.ARGB32, mipChain: true);
		Color[] array = new Color[texture2D.width * texture2D.height];
		for (int i = 0; i < texs.Length; i++)
		{
			float num = 1f;
			float num2 = 1f;
			if (texs[i].width != texture2D.width)
			{
				num = (float)texs[i].width / (float)texture2D.width;
			}
			if (texs[i].height != texture2D.height)
			{
				num2 = (float)texs[i].height / (float)texture2D.height;
			}
			for (int j = 0; j < texture2D.width; j++)
			{
				for (int k = 0; k < texture2D.height; k++)
				{
					Color pixel = texs[i].GetPixel((int)((float)k * num2), (int)((float)j * num));
					int num3 = j * texture2D.width + k;
					_ = ref array[num3];
					if (pixel.a > 0f)
					{
						array[num3] = pixel;
					}
					else
					{
						array[num3] = new Color(0f, 0f, 0f, 0f);
					}
				}
			}
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
	}

	public static bool IsPointerOverGameObject()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.pressPosition = Input.mousePosition;
		pointerEventData.position = Input.mousePosition;
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		return list.Count > 0;
	}
}
