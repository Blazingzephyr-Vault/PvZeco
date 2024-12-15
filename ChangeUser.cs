using FTRuntime;
using UnityEngine;

public class ChangeUser : MonoBehaviour
{
	public Texture2D EnterGreen;

	public SwfClipController clipController;

	public Animator animator;

	public TextMesh NameText;

	public Collider2D Collider;

	private void Start()
	{
		clipController.clip.OnChangeCurrentFrameEvent += FrameChange;
	}

	public void PlayAnimation()
	{
		Collider.enabled = false;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodSignRoll_in, base.transform.position, isAll: true);
		animator.Play("WoodsignName", 0, 0f);
		clipController.GotoAndPlay(0);
	}

	private void FrameChange(SwfClip swfClip)
	{
		if (swfClip.currentFrame == 40)
		{
			Collider.enabled = true;
		}
	}

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			base.transform.GetComponent<Renderer>().material.SetTexture("_EyeTex", EnterGreen);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
		}
	}

	private void OnMouseExit()
	{
		base.transform.GetComponent<Renderer>().material.SetTexture("_EyeTex", null);
	}

	private void OnMouseDown()
	{
		if (!GameManager.Instance.isOnline && !MyTool.IsPointerOverGameObject())
		{
			ChooseSave.Instance.transform.localScale = new Vector3(1f, 1f);
			ChooseSave.Instance.LoadSavegroup();
			UIManager.Instance.OpenAndFocusUI(ChooseSave.Instance.transform);
		}
	}
}
