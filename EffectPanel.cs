using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EffectPanel : MonoBehaviour
{
    #region New Major Logic


    protected IEnumerator HeronsbillStall(float wantBright, UnityAction fun)
    {
		// Color up, Brightness up
		// Color fast down
		// Brightness down

		Color color = Color.white;

		

        float currBright = 1f;
        while (currBright < wantBright)
        {
            yield return new WaitForSeconds(0.05f);
            currBright += 0.05f;
            REnderer.material.SetFloat("_Brightness", currBright);
            if (ExtraREnderer != null)
            {
                ExtraREnderer.material.SetFloat("_Brightness", currBright);
            }
        }
        REnderer.material.SetFloat("_Brightness", 1f);
        if (ExtraREnderer != null)
        {
            ExtraREnderer.material.SetFloat("_Brightness", 1f);
        }
        fun?.Invoke();
    }

    public void HeronsbillStall(Color color, float time, Vector2 pos)
    {
        if (!Displaying && !(MapManager.Instance.GetCurrMap(pos) != CameraControl.Instance.CurrMap))
        {
            panel.gameObject.SetActive(value: true);
            panel.color = color;
			StartCoroutine(WaitTime(time));
        }
    }

    private IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
        panel.color = new Color(1f, 1f, 1f, 0f);
        panel.gameObject.SetActive(value: false);
    }

    #endregion

    public static EffectPanel Instance;

	private Image panel;

	private bool Displaying;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		panel = base.transform.GetComponent<Image>();
		panel.gameObject.SetActive(value: false);
		panel.color = new Color(1f, 1f, 1f, 0f);
		Displaying = false;
	}

	public void WinGame(MoneyBag moneyBag)
	{
		Displaying = true;
		panel.gameObject.SetActive(value: true);
		StartCoroutine(PanelColorEF(moneyBag));
	}

	private IEnumerator PanelColorEF(MoneyBag moneyBag)
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.LightFill, base.transform.position, isAll: true);
		float a = 0f;
		while (a < 1f)
		{
			panel.color = new Color(1f, 1f, 1f, a);
			a += 0.02f;
			yield return new WaitForSeconds(0.05f);
		}
		yield return new WaitForSeconds(0.2f);
		moneyBag.DestroyThis();
		LVManager.Instance.GameWin();
		while (a > 0f)
		{
			panel.color = new Color(1f, 1f, 1f, a);
			a -= 0.08f;
			yield return new WaitForSeconds(0.05f);
		}
		panel.gameObject.SetActive(value: false);
		Displaying = false;
	}

	public void Spark(Color color, float time, Vector2 pos)
	{
		if (!Displaying && !(MapManager.Instance.GetCurrMap(pos) != CameraControl.Instance.CurrMap))
		{
			panel.gameObject.SetActive(value: true);
			panel.color = color;
			StartCoroutine(WaitTime(time));
		}
	}

	private IEnumerator WaitTime(float time)
	{
		yield return new WaitForSeconds(time);
		panel.color = new Color(1f, 1f, 1f, 0f);
		panel.gameObject.SetActive(value: false);
	}
}
