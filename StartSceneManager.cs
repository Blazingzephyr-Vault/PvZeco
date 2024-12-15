using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
	public static StartSceneManager Instance;

	public Animator BackLeft;

	public Animator BackCenter;

	public Animator BackRight;

	public ChangeUser changeUser;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		PlayAllAnim();
	}

	public void PlayAllAnim()
	{
		BackLeft.Play("", 0, 0f);
		BackCenter.Play("", 0, 0f);
		BackRight.Play("", 0, 0f);
		changeUser.PlayAnimation();
	}

	public void GoEndLess()
	{
		PoolManager.Instance.Clear();
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
		Invoke("DoGoEndLess", 0.5f);
	}
}
