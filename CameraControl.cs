using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CameraControl : MonoBehaviour
{
	public static CameraControl Instance;

	public MapBase CurrMap;

	private Coroutine ShakeCoroutine;

	private float velocityX;

	private float velocityY;

	private Coroutine MoveCoroutine;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
	}

	public void SetPosition(Vector2 pos)
	{
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		if (MoveCoroutine != null)
		{
			StopCoroutine(MoveCoroutine);
		}
		base.transform.position = new Vector3(pos.x, pos.y, -10f);
		CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
	}

	public void MoveForLVStart(UnityAction action)
	{
		base.transform.position = new Vector3(-3.5f, 0f, -10f);
		CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
		if (MoveCoroutine != null)
		{
			StopCoroutine(MoveCoroutine);
		}
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		MoveCoroutine = StartCoroutine(DoMove(new Vector2(3.5f, base.transform.position.y), 0.6f, 20f, action));
	}

	public void MoveBackForLVStart(UnityAction action)
	{
		if (MoveCoroutine != null)
		{
			StopCoroutine(MoveCoroutine);
		}
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		MoveCoroutine = StartCoroutine(DoMove(new Vector2(-3.5f, base.transform.position.y), 0.6f, 20f, action));
	}

	public void MoveTo(Vector2 pos, UnityAction action)
	{
		if (MoveCoroutine != null)
		{
			StopCoroutine(MoveCoroutine);
		}
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		MoveCoroutine = StartCoroutine(DoMove(pos, 0f, 8f, action));
	}

	private IEnumerator DoMove(Vector2 targetPos, float waitTime, float speed, UnityAction action)
	{
		yield return new WaitForSeconds(waitTime);
		Vector3 target = new Vector3(targetPos.x, targetPos.y, -10f);
		do
		{
			yield return new WaitForFixedUpdate();
			float x = Mathf.SmoothDamp(base.transform.position.x, target.x, ref velocityX, speed * Time.deltaTime);
			float y = Mathf.SmoothDamp(base.transform.position.y, target.y, ref velocityY, speed * Time.deltaTime);
			base.transform.position = new Vector3(x, y, -10f);
		}
		while (!(Vector2.Distance(target, base.transform.position) < 0.01f));
		base.transform.position = target;
		action?.Invoke();
		MoveCoroutine = null;
	}

	public bool GoOtherYard(int mapId)
	{
		if (mapId < 0 || MapManager.Instance.mapList.Count <= mapId)
		{
			return false;
		}
		if (MoveCoroutine != null)
		{
			return false;
		}
		base.transform.position = new Vector3(base.transform.position.x, MapManager.Instance.mapList[mapId].transform.position.y, -10f);
		CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
		SkyManager.Instance.clickedSunNum = 0;
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		return true;
	}

	public void ShakeCamera(Vector3 pos)
	{
		if (!(MapManager.Instance.GetCurrMap(pos) != CurrMap) && ShakeCoroutine == null)
		{
			ShakeCoroutine = StartCoroutine(Shake());
		}
	}

	private IEnumerator Shake()
	{
		float NormalX = base.transform.position.x + 0.08f;
		float NormalY = base.transform.position.y + 0.08f;
		while (base.transform.position.y < NormalY)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, new Vector3(NormalX, NormalY, base.transform.position.z), 20f * Time.deltaTime);
			yield return new WaitForFixedUpdate();
		}
		NormalX = base.transform.position.x - 0.08f;
		NormalY = base.transform.position.y - 0.08f;
		while (base.transform.position.y > NormalY)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, new Vector3(NormalX, NormalY, base.transform.position.z), 20f * Time.deltaTime);
			yield return new WaitForFixedUpdate();
		}
		ShakeCoroutine = null;
	}
}
