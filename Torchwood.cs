using FTRuntime;
using UnityEngine;

public class Torchwood : PlantBase
{
	public Collider2D FireCollider;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Torchwood;

	public int lineNum => currGrid.Point.y;

	protected override void OnInitForCreate()
	{
		FireCollider.enabled = false;
	}

	protected override void OnInitForPlace()
	{
		FireCollider.enabled = true;
        /*MapBase currMap = MapManager.Instance.GetCurrMap(base.transform.position);
        Fog fog = currMap.fog;

        fog.IsOpen = true;
        fog.CreateFog(10);
        fog.MoveBack();*/
    }

    protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "idel" && swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}

	protected override void GoAwakeSpecial()
	{
		FireCollider.enabled = true;
	}

	protected override void GoSleepSpecial()
	{
		FireCollider.enabled = false;
	}

	protected override void DeadEvent()
	{
		FireCollider.enabled = false;
	}
}
