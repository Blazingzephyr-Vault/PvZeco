using UnityEngine;

public class NormalSprite : MonoBehaviour
{
	public static NormalSprite Instance;

	public GameObject SpriteDisplay;

	public Sprite CheckBox;

	public Sprite CheckBoxYes;

	public Sprite CheckBoxNo;

	private void Awake()
	{
		Instance = this;
	}
}
