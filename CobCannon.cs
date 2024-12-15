using System.Collections;
using FTRuntime;
using SocketSave;
using UnityEngine;

public class CobCannon : PlantBase
{
	private Grid nextGrid;

	private Vector2 TargetPos;

	private bool ChargeOver;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.CobCannon;

	public override bool isHaveSpecialCheck => true;

	public override PlantType BasePlant => PlantType.Cornpult;

	public override int BasePlantSunNum => 100;

	public override bool CanProtect => false;

	protected override int attackValue => 1800;

	protected override void OnInitForCreate()
	{
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
	}

	protected override void OnInitForPlace()
	{
		if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(PlacePlayer))
		{
			nextGrid = MapManager.Instance.GetNextGrid(currGrid);
		}
		else
		{
			nextGrid = MapManager.Instance.GetNextGrid(currGrid, isRight: true);
		}
		ChargeOver = true;
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
		clipController.clip.sequence = "unarmedidel";
		MapManager.Instance.PlantnoFlash(BasePlant);
		int num = 0;
		int num2 = 0;
		if (currGrid.CurrPlantBase != null && currGrid.CurrPlantBase.GetPlantType() == BasePlant)
		{
			if (currGrid.CurrPlantBase.isSleeping)
			{
				num2++;
			}
			currGrid.CurrPlantBase.Dead();
			currGrid.CurrPlantBase = this;
		}
		else if (currGrid.CurrPlantBase == null)
		{
			currGrid.CurrPlantBase = this;
			num += 100;
		}
		else if (currGrid.CurrPlantBase.CarryPlant != null && currGrid.CurrPlantBase.CarryPlant.GetPlantType() == BasePlant)
		{
			if (currGrid.CurrPlantBase.CarryPlant.isSleeping)
			{
				num2++;
			}
			currGrid.CurrPlantBase.CarryPlant.Dead();
			base.transform.SetParent(currGrid.CurrPlantBase.transform);
			currGrid.CurrPlantBase.CarryPlant = this;
		}
		else if (currGrid.CurrPlantBase.CarryPlant == null)
		{
			base.transform.SetParent(currGrid.CurrPlantBase.transform);
			currGrid.CurrPlantBase.CarryPlant = this;
			num += 100;
		}
		if (nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.GetPlantType() == BasePlant)
		{
			if (nextGrid.CurrPlantBase.isSleeping)
			{
				num2++;
			}
			nextGrid.CurrPlantBase.Dead();
			nextGrid.CurrPlantBase = this;
		}
		else if (nextGrid.CurrPlantBase == null)
		{
			nextGrid.CurrPlantBase = this;
			num += 100;
		}
		else if (nextGrid.CurrPlantBase.CarryPlant != null && nextGrid.CurrPlantBase.CarryPlant.GetPlantType() == BasePlant)
		{
			if (nextGrid.CurrPlantBase.CarryPlant.isSleeping)
			{
				num2++;
			}
			nextGrid.CurrPlantBase.CarryPlant.Dead();
			base.transform.SetParent(nextGrid.CurrPlantBase.transform);
			nextGrid.CurrPlantBase.CarryPlant = this;
		}
		else if (nextGrid.CurrPlantBase.CarryPlant == null)
		{
			base.transform.SetParent(nextGrid.CurrPlantBase.transform);
			nextGrid.CurrPlantBase.CarryPlant = this;
			num += 100;
		}
		if (num != 0)
		{
			num += SeedBank.Instance.noBasePlantExtra;
		}
		PlayerManager.Instance.AddSunNum(-num, isSun: true, PlacePlayer);
	}

	public override bool SpecialPlantCheck(Grid grid, int NeedSun, string Player)
	{
		if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(Player))
		{
			nextGrid = MapManager.Instance.GetNextGrid(grid);
		}
		else
		{
			nextGrid = MapManager.Instance.GetNextGrid(grid, isRight: true);
		}
		if (LV.Instance.CurrLVType == LVType.PvP && nextGrid.CurrGridType != 0)
		{
			bool flag = PvPSelector.Instance.RedTeamNames.Contains(Player);
			if (flag && nextGrid.CurrGridType == GridType.BlueTeam)
			{
				return false;
			}
			if (!flag && nextGrid.CurrGridType == GridType.RedTeam)
			{
				return false;
			}
		}
		int num = 0;
		if (grid.isOccupied || nextGrid.isOccupied)
		{
			return false;
		}
		if (grid.HaveGraveStone || nextGrid.HaveGraveStone)
		{
			return false;
		}
		if (grid.HaveCrater || nextGrid.HaveCrater)
		{
			return false;
		}
		if ((grid.isWaterGrid && grid.CurrPlantBase == null) || (nextGrid.isWaterGrid && nextGrid.CurrPlantBase == null))
		{
			return false;
		}
		if (grid.CurrPlantBase != null)
		{
			if (grid.CurrPlantBase.GetPlantType() != BasePlant && !grid.CurrPlantBase.CanCarryOtherPlant)
			{
				return false;
			}
			if (grid.CurrPlantBase.ProtectPlant != null)
			{
				return false;
			}
			if (grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() != BasePlant)
			{
				return false;
			}
		}
		if (nextGrid.CurrPlantBase != null)
		{
			if (nextGrid.CurrPlantBase.GetPlantType() != BasePlant && !nextGrid.CurrPlantBase.CanCarryOtherPlant)
			{
				return false;
			}
			if (nextGrid.CurrPlantBase.ProtectPlant != null)
			{
				return false;
			}
			if (nextGrid.CurrPlantBase.CarryPlant != null && nextGrid.CurrPlantBase.CarryPlant.GetPlantType() != BasePlant)
			{
				return false;
			}
		}
		if (grid.CurrPlantBase != null && nextGrid.CurrPlantBase != null && grid.CurrPlantBase.GetPlantType() != nextGrid.CurrPlantBase.GetPlantType() && grid.CurrPlantBase.CanCarryOtherPlant && nextGrid.CurrPlantBase.CanCarryOtherPlant)
		{
			return false;
		}
		if (grid.CurrPlantBase == null && nextGrid.CurrPlantBase != null)
		{
			if (nextGrid.CurrPlantBase.GetPlantType() != BasePlant && !nextGrid.isWaterGrid)
			{
				return false;
			}
			if (nextGrid.isWaterGrid && !nextGrid.CurrPlantBase.CanCarryOtherPlant)
			{
				return false;
			}
			if (nextGrid.CurrPlantBase.CanCarryOtherPlant && nextGrid.CurrPlantBase.CarryPlant != null && nextGrid.CurrPlantBase.CarryPlant.GetPlantType() != BasePlant)
			{
				return false;
			}
		}
		if (nextGrid.CurrPlantBase == null && grid.CurrPlantBase != null)
		{
			if (grid.CurrPlantBase.GetPlantType() != BasePlant && !grid.isWaterGrid)
			{
				return false;
			}
			if (grid.isWaterGrid && !grid.CurrPlantBase.CanCarryOtherPlant)
			{
				return false;
			}
			if (grid.CurrPlantBase.CanCarryOtherPlant && grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() != BasePlant)
			{
				return false;
			}
		}
		if (grid.CurrPlantBase != null && grid.CurrPlantBase.GetPlantType() == BasePlant)
		{
			num++;
		}
		else if (grid.CurrPlantBase != null && grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() == BasePlant)
		{
			num++;
		}
		if (nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.GetPlantType() == BasePlant)
		{
			num++;
		}
		else if (nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.CarryPlant != null && nextGrid.CurrPlantBase.CarryPlant.GetPlantType() == BasePlant)
		{
			num++;
		}
		int num2 = NeedSun + 200 - num * 100;
		if (num != 2)
		{
			num2 += SeedBank.Instance.noBasePlantExtra;
		}
		if (PlayerManager.Instance.GetSunNum(isSun: true, Player) < (float)num2)
		{
			return false;
		}
		return true;
	}

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "idel":
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			break;
		case "unarmedidel":
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1 && ChargeOver)
			{
				clipController.clip.sequence = "charge";
			}
			break;
		case "shoot":
			if (swfClip.currentFrame == 100)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.coblaunch, base.transform.position);
			}
			if (swfClip.currentFrame == 126)
			{
				CreateCannon();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				StartCoroutine(WaitCharge());
				clipController.clip.sequence = "unarmedidel";
			}
			break;
		case "charge":
			if (swfClip.currentFrame == 34)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.shoop, base.transform.position);
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "idel";
				base.transform.GetComponent<CapsuleCollider2D>().enabled = true;
			}
			break;
		}
	}

	private void OnMouseOver()
	{
		if (!MyTool.IsPointerOverGameObject() && !SpectatorList.Instance.LocalIsSpectator && (LV.Instance.CurrLVType != LVType.PvP || PvPSelector.Instance.IsSameTeam(PlacePlayer)) && currGrid != null && Input.GetMouseButtonDown(0) && !isSleeping && LVManager.Instance.GameIsStart)
		{
			CobCannonTarget.Instance.StartAim(this, delegate(Vector2 pos)
			{
				ShootCannon(pos);
			});
		}
	}

	private IEnumerator WaitCharge()
	{
		if (!GameManager.Instance.isClient)
		{
			yield return new WaitForSeconds(35f);
			ChargeOver = true;
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 1;
				synItem.SynCode[1] = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	private void CreateCannon()
	{
		if (currGrid != null)
		{
			Object.Instantiate(GameManager.Instance.GameConf.CannonCob).GetComponent<CannonCob>().CreateInit(base.transform.position + new Vector3(-0.1f, 3f), TargetPos, attackValue, isHypno);
		}
	}

	public void ShootCannon(Vector2 pos, bool synClient = false)
	{
		if (!ChargeOver)
		{
			return;
		}
		ChargeOver = false;
		if (GameManager.Instance.isClient && !synClient)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = 1;
			synItem.Twofloat = pos;
			SocketClient.Instance.SendSynBag(synItem);
			ChargeOver = true;
			base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
			return;
		}
		if (GameManager.Instance.isServer)
		{
			SynItem synItem2 = new SynItem();
			synItem2.OnlineId = OnlineId;
			synItem2.Type = 1;
			synItem2.Twofloat = pos;
			SocketServer.Instance.SendSynBag(synItem2);
		}
		clipController.clip.sequence = "shoot";
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
		TargetPos = pos;
	}

	public override void OnlineSynPlant(SynItem syn)
	{
		base.OnlineSynPlant(syn);
		if (syn.SynCode[0] != 0)
		{
			return;
		}
		if (syn.SynCode[1] == 0)
		{
			if (LV.Instance.CurrLVType == LVType.PvP)
			{
				if (GameManager.Instance.isClient && !PvPSelector.Instance.IsSameTeam(GameManager.Instance.HostName))
				{
					syn.Twofloat = MyTool.ReverseX(syn.Twofloat);
				}
				if (GameManager.Instance.isServer && !PvPSelector.Instance.IsSameTeam(PlacePlayer))
				{
					syn.Twofloat = MyTool.ReverseX(syn.Twofloat);
				}
			}
			ShootCannon(syn.Twofloat, synClient: true);
		}
		else if (syn.SynCode[1] == 1)
		{
			ChargeOver = true;
		}
	}

	protected override void DeadEvent()
	{
		if (nextGrid.CurrPlantBase == this)
		{
			nextGrid.CurrPlantBase = null;
		}
		else if (nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.CarryPlant != null && nextGrid.CurrPlantBase.CarryPlant == this)
		{
			nextGrid.CurrPlantBase.CarryPlant = null;
		}
	}

	protected override void GoAwakeSpecial()
	{
		if (ChargeOver)
		{
			base.transform.GetComponent<CapsuleCollider2D>().enabled = true;
		}
	}

	protected override void GoSleepSpecial()
	{
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
	}
}
