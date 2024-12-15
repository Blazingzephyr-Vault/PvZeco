using UnityEngine;
using UnityEngine.UI;

public class Timetable : MonoBehaviour
{
	public static Timetable Instance;

	private Text TimeText;

	private void Awake()
	{
		Instance = this;
		TimeText = base.transform.Find("Time").GetComponent<Text>();
	}

	public void UpdateTime(int time)
	{
		int num = time / 60;
		int num2 = time % 60;
		if (num < 10)
		{
			if (num2 < 10)
			{
				TimeText.text = "0" + num + ":0" + num2;
			}
			else
			{
				TimeText.text = "0" + num + ":" + num2;
			}
		}
		else if (num2 < 10)
		{
			TimeText.text = num + ":0" + num2;
		}
		else
		{
			TimeText.text = num + ":" + num2;
		}
	}
}
