using System.Collections;
using UnityEngine;

public abstract class Allcoin : MonoBehaviour
{
	private bool isFlying;

	protected abstract int money { get; }

	protected abstract AudioClip sound { get; }

	protected abstract AudioClip dropSound { get; }

	public void OnMouseDown()
	{
		if (!isFlying)
		{
			isFlying = true;
			Coinbank.Instance.ShowCoinbank();
			Vector3 vector = Camera.main.ScreenToWorldPoint(Coinbank.Instance.GetCoinbankTextPos());
			vector = new Vector3(vector.x, vector.y, 0f);
			FlyAnimation(vector);
			AudioManager.Instance.PlayEFAudio(sound, base.transform.position);
		}
	}

	private void FlyAnimation(Vector3 pos)
	{
		StartCoroutine(DoFly(pos));
	}

	private IEnumerator DoFly(Vector3 pos)
	{
		Vector3 direction = (pos - base.transform.position).normalized;
		while (Vector3.Distance(pos, base.transform.position) > 0.5f)
		{
			yield return new WaitForSeconds(0.02f);
			base.transform.Translate(direction);
		}
		PlayerManager.Instance.Money += money;
		Destroy();
	}

	public void InitForItem(Vector2 pos)
	{
		base.transform.position = pos;
		StartCoroutine(DoJump());
		Invoke("Destroy", 10f);
	}

	private IEnumerator DoJump()
	{
		bool num = Random.Range(0, 2) == 0;
		Vector3 startPos = base.transform.position;
		float x = ((!num) ? 0.008f : (-0.008f));
		float speed = 0f;
		while ((double)base.transform.position.y <= (double)startPos.y + 0.6)
		{
			yield return new WaitForSeconds(0.005f);
			speed += 0.0005f;
			base.transform.Translate(new Vector3(x, 0.015f + speed, 0f));
		}
		while ((double)base.transform.position.y >= (double)startPos.y - 0.2)
		{
			yield return new WaitForSeconds(0.005f);
			speed += 0.0005f;
			base.transform.Translate(new Vector3(x, -0.015f - speed, 0f));
		}
		AudioManager.Instance.PlayEFAudio(dropSound, base.transform.position);
		PlayerManager.Instance.Coins.Add(this);
	}

	public void DoFlytoMagnet(Vector3 pos)
	{
		if (!isFlying)
		{
			isFlying = true;
			StartCoroutine(Fly(pos));
		}
	}

	private IEnumerator Fly(Vector3 pos)
	{
		Vector3 direction = (pos - base.transform.position).normalized;
		while (Vector3.Distance(pos, base.transform.position) > 0.2f)
		{
			yield return new WaitForSeconds(0.02f);
			base.transform.Translate(direction * 0.2f);
		}
		PlayerManager.Instance.Money += money;
		Destroy();
	}

	public void Destroy()
	{
		PlayerManager.Instance.Coins.Remove(this);
		StopAllCoroutines();
		CancelInvoke();
		Object.Destroy(base.gameObject);
	}
}
