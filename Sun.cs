using System.Collections;
using FTRuntime;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Sun : MonoBehaviour
{
	public int OnlineSunId;

	public string OwnerPlayer;

	private float downTargetPosY;

	private bool isFromSky;

	private float Sunnum;

	public Renderer REnderer;

	private SphereCollider sphereCollider;

	private Vector3 velocity;

	private bool isClick;

	private bool isSun;

	public SwfClipController clipController;

	public Sprite SunSprite;

	public Sprite MoonSprite;

	public Light2D light2d;

	private void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
	}

	private void Update()
	{
		if (isFromSky && !(base.transform.position.y <= downTargetPosY))
		{
			base.transform.Translate(Vector3.down * Time.deltaTime);
		}
	}

	public void OnMouseOver()
	{
		if (!SpectatorList.Instance.LocalIsSpectator)
		{
			CollectSun();
		}
	}

	public void CollectSun(bool synClient = false)
	{
		if (isClick && !synClient)
		{
			return;
		}
		if (!synClient && GameManager.Instance.isClient)
		{
			isClick = true;
			ClickedSun clickedSun = new ClickedSun();
			clickedSun.OnlineSunId = OnlineSunId;
			SocketClient.Instance.ClickedSun(clickedSun);
			return;
		}
		if (GameManager.Instance.isServer)
		{
			if (isClick)
			{
				return;
			}
			ClickedSun clickedSun2 = new ClickedSun();
			clickedSun2.OnlineSunId = OnlineSunId;
			SocketServer.Instance.ClickedSun(clickedSun2);
		}
		SkyManager.Instance.clickedSunNum++;
		isFromSky = false;
		StopAllCoroutines();
		Vector3 vector = ((!isSun) ? SeedBank.Instance.MoonPos.transform.position : SeedBank.Instance.SunPos.transform.position);
		if (isSun && !SeedBank.Instance.NeedSummonSun)
		{
			vector = base.transform.position;
		}
		else if (!isSun && !SeedBank.Instance.NeedSummonMoon)
		{
			vector = base.transform.position;
		}
		if (LV.Instance.CurrLVType == LVType.PvP && (OwnerPlayer == null || !PvPSelector.Instance.IsSameTeam(OwnerPlayer)) && OwnerPlayer != null)
		{
			vector = base.transform.position;
		}
		vector = new Vector3(vector.x, vector.y, 0f);
		if (MapManager.Instance.GetCurrMap(base.transform.position) == CameraControl.Instance.CurrMap)
		{
			FlyAnimation(vector);
		}
		else
		{
			DestroySun();
		}
		PlayerManager.Instance.AddSunNum(Sunnum, isSun, OwnerPlayer);
		if (SkyManager.Instance.clickedSunNum < 3)
		{
			SunSound();
		}
		else if (Random.Range(0, 3) > 0)
		{
			SunSound();
		}
		isClick = true;
	}

	private void SunSound()
	{
		if (Random.Range(0, 2) > 0)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.SunClick, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.SunClickHigh, base.transform.position);
		}
	}

	public void InitForSky(float downTargetPosY, Vector3 SpawnPos, bool issun)
	{
		OwnerPlayer = null;
		isSun = issun;
		if (isSun)
		{
			clipController.clip.NewSprite = SunSprite;
		}
		else
		{
			clipController.clip.NewSprite = MoonSprite;
		}
		isClick = false;
		Sunnum = 25f;
		sphereCollider.radius = 0.7f;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		light2d.pointLightOuterRadius = 1.6f;
		this.downTargetPosY = downTargetPosY;
		base.transform.position = SpawnPos;
		isFromSky = true;
		Invoke("TimeOutDestory", 20f);
		REnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
	}

	public void InitForPlant(Vector2 pos, float sum, bool issun, string Player)
	{
		OwnerPlayer = Player;
		isClick = false;
		isSun = issun;
		Sunnum = sum;
		if (isSun)
		{
			clipController.clip.NewSprite = SunSprite;
		}
		else
		{
			clipController.clip.NewSprite = MoonSprite;
		}
		float num = sum * 1f / 25f;
		sphereCollider.radius = 0.7f * (sum * 1f / 25f);
		if (sphereCollider.radius < 0.6f)
		{
			sphereCollider.radius = 0.6f;
		}
		base.transform.localScale = new Vector3(num, num, 1f);
		light2d.pointLightOuterRadius = 1.6f * num;
		num = 1f / num;
		sphereCollider.radius *= num;
		base.transform.position = pos;
		isFromSky = false;
		StartCoroutine(DoJump());
		Invoke("TimeOutDestory", 14f);
		REnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
	}

	private IEnumerator DoJump()
	{
		bool num = Random.Range(0, 2) == 0;
		Vector3 startPos = base.transform.position;
		float x = Random.Range(0.2f, 1f);
		x = ((!num) ? (x * Time.deltaTime) : ((0f - x) * Time.deltaTime));
		float speed = 0f;
		float scale = base.transform.localScale.y;
		base.transform.localScale = new Vector3(0.1f, 0.1f);
		sphereCollider.enabled = false;
		while (base.transform.localScale.y < scale)
		{
			yield return new WaitForFixedUpdate();
			base.transform.localScale += new Vector3(0.05f, 0.05f);
			speed += 0.1f;
			base.transform.Translate(new Vector3(x, speed * Time.deltaTime, 0f));
		}
		sphereCollider.enabled = true;
		while ((double)base.transform.position.y >= (double)startPos.y - 0.2)
		{
			yield return new WaitForFixedUpdate();
			speed -= 0.1f;
			base.transform.Translate(new Vector3(x, speed * Time.deltaTime, 0f));
		}
	}

	private void FlyAnimation(Vector3 pos)
	{
		StartCoroutine(DoFly(pos));
	}

	private IEnumerator DoFly(Vector3 pos)
	{
		while (Vector3.Distance(pos, base.transform.position) > 0.1f)
		{
			yield return new WaitForFixedUpdate();
			base.transform.position = Vector3.SmoothDamp(base.transform.position, pos, ref velocity, 0.4f);
		}
		SkyManager.Instance.clickedSunNum--;
		float alp = 1f;
		while (alp > 0.1f)
		{
			yield return new WaitForSeconds(0.02f);
			alp -= 0.1f;
			REnderer.material.SetColor("_Color", new Color(1f, 1f, 1f, alp));
		}
		isClick = false;
		DestroySun();
	}

	private void TimeOutDestory()
	{
		if (!GameManager.Instance.isClient && SkyManager.Instance.SunAutoCollect)
		{
			CollectSun();
		}
		else
		{
			Invoke("DestroySun", 2f);
		}
	}

	public void DestroySun()
	{
		if (!isClick)
		{
			StopAllCoroutines();
			CancelInvoke();
			SkyManager.Instance.sunList.Remove(this);
			PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Sun, base.gameObject);
		}
	}
}
