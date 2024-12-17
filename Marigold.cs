using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using UnityEngine;
using UnityEngine.Events;

public class Marigold : PlantBase
{
	private float createCoinTime = 24f;

	private float lightTime = 1.5f;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Marigold;

	protected override void OnInitForPlace()
	{
		InvokeRepeating("CreateSun", createCoinTime, createCoinTime);
	}

	private void CreateSun()
	{
		if (!isSleeping)
		{
			StartCoroutine(ColorEffect2(lightTime, 0.05f, InstantiateCoin));
		}
	}

	protected IEnumerator ColorEffect2(float wantBright, float delayTime, UnityAction fun)
	{
		float currBright = 1f;
		while (currBright < wantBright)
		{
			yield return new WaitForSeconds(delayTime);
			currBright += 0.05f;
			REnderer.material.SetFloat("_Brightness", currBright);
		}
		REnderer.material.SetFloat("_Brightness", 1f);
		fun?.Invoke();
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}

	private void InstantiateCoin()
    {
        StartCoroutine(ColorEffect3(lightTime, 2f, InstantiateCoin));

        //ZombieManager.Instance.DropCoin(base.transform.position, AllDrop: true);
    }

    protected IEnumerator ColorEffect3(float wantBright, float delayTime, UnityAction fun)
    {
        float currBright = 1f;
		List<ZombieBase> zombies;

        while (currBright < wantBright)
        {
            yield return new WaitForSeconds(delayTime);
            currBright += 0.05f;

            zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 15f, isHypno, needCapsule: false);
			foreach (var z in zombies)
			{
				//z.SetAllBrightness(2f);
			}
        }

        zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 15f, isHypno, needCapsule: false);
        foreach (var z in zombies)
        {
            //z.SetAllBrightness(1);
        }
    }
}
