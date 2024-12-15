using System.Collections;
using UnityEngine;

public class MoneyBag : MonoBehaviour
{
	private int money = 500;

	private bool isFlash = true;

	private SpriteRenderer Sprite;

	private void Start()
	{
		Sprite = GetComponent<SpriteRenderer>();
		isFlash = true;
		StartCoroutine(flash());
	}

	public void OnMouseDown()
	{
		if (isFlash)
		{
			isFlash = false;
			Coinbank.Instance.ShowCoinbank();
			Vector3 vector = Camera.main.ScreenToWorldPoint(Coinbank.Instance.GetCoinbankTextPos());
			vector = new Vector3(vector.x + 6f, vector.y + 6f, 0f);
			FlyAnimation(vector);
			PlayerManager.Instance.Money += money;
			SkyManager.Instance.CollectAllSun();
			AudioManager.Instance.StopBgAudio();
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ClickCoin, base.transform.position, isAll: true);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameWin, base.transform.position, isAll: true);
			EffectPanel.Instance.WinGame(this);
		}
	}

	private void FlyGold()
	{
	}

	private void FlyAnimation(Vector3 pos)
	{
		StartCoroutine(DoFly(pos));
	}

	private IEnumerator DoFly(Vector3 pos)
	{
		float a = 1f;
		Vector3 direction = (pos - base.transform.position).normalized;
		while (Vector3.Distance(pos, base.transform.position) > 0.5f)
		{
			yield return new WaitForSeconds(0.02f);
			base.transform.Translate(direction * 0.1f);
			a += 0.01f;
			base.transform.localScale = new Vector3(a, a, 0f);
		}
	}

	private IEnumerator flash()
	{
		float a = 1f;
		while (isFlash)
		{
			if (a >= 0.5f)
			{
				while (a > 0.5f)
				{
					yield return new WaitForSeconds(0.04f);
					a -= 0.05f;
					Sprite.color = new Color(a, a, a);
				}
			}
			else if (a <= 0.5f)
			{
				while (a < 0.95f)
				{
					yield return new WaitForSeconds(0.04f);
					a += 0.05f;
					Sprite.color = new Color(a, a, a);
				}
			}
		}
	}

	public void DestroyThis()
	{
		Object.Destroy(base.gameObject);
	}
}
