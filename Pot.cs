using UnityEngine;

public class Pot : PlantBase
{
	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Pot;

	public override bool CanCarryOtherPlant => true;

	protected override bool HaveShadow => false;

	protected override Vector2 offSet => new Vector2(0f, -0.05f);

	public override Vector2 CarryOffset => new Vector2(0f, 0.3f);

	public override bool CanPlaceOnHardGround => true;
}
