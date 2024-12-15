using UnityEngine;

public class Sled : MonoBehaviour
{
	public Sprite Sled2;

	public Sprite Sled3;

	public Sprite Sled4;

	public Transform SledInn;

	private int hp = 400;

	public int Hp
	{
		get
		{
			return hp;
		}
		set
		{
			hp = value;
			if (hp < 100)
			{
				base.transform.GetComponent<SpriteRenderer>().sprite = Sled4;
			}
			else if (hp < 200)
			{
				base.transform.GetComponent<SpriteRenderer>().sprite = Sled3;
			}
			else if (hp < 300)
			{
				base.transform.GetComponent<SpriteRenderer>().sprite = Sled2;
			}
		}
	}

	public void CreateInit(int order)
	{
		base.transform.GetComponent<SpriteRenderer>().sortingOrder = order + 5;
		SledInn.GetComponent<SpriteRenderer>().sortingOrder = order + 4;
	}

	public void Jump()
	{
		SledInn.GetComponent<SpriteRenderer>().sortingOrder -= 5;
	}
}
