using FTRuntime;

public class Umbrellaleaf : PlantBase
{
	private int SortOrder;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Umbrellaleaf;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		string sequence = swfClip.sequence;
		if (!(sequence == "idel"))
		{
			if (sequence == "block")
			{
				if (swfClip.currentFrame == 2)
				{
					clipController.clip.sortingOrder = 2000;
				}
				if (swfClip.currentFrame == 44)
				{
					clipController.clip.sortingOrder = SortOrder;
				}
				if (swfClip.currentFrame == swfClip.frameCount - 1)
				{
					swfClip.sequence = "idel";
				}
			}
		}
		else if (swfClip.currentFrame == closeEyeFrame)
		{
			StartCloseEyes();
		}
	}

	public bool Block()
	{
		if (!isSleeping)
		{
			clipController.clip.sequence = "block";
		}
		return !isSleeping;
	}

	protected override void OnInitForPlace()
	{
		SortOrder = clipController.clip.sortingOrder;
	}
}
