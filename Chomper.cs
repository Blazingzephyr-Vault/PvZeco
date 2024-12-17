using FTRuntime;
using SocketSave;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chomper : PlantBase
{
	private ZombieBase zombie;

	private bool canSwallow;

	private bool isHit;

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.Chomper;

	protected override int attackValue => 40;

	private bool shouldEat;

    protected override void OnInitForPlace()
	{
		clipController.clip.sequence = "idel";
		canSwallow = false;
        shouldEat = false;
    }

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.sequence == "bite")
		{
			if (swfClip.currentFrame == 80)
			{
				if (!GameManager.Instance.isClient)
				{
					Attack();
				}
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.BigChomp, base.transform.position);
				if (isHit && GameManager.Instance.isServer)
				{
					SynItem synItem = new SynItem();
					synItem.OnlineId = OnlineId;
					synItem.Type = 1;
					synItem.SynCode[1] = 2;
					SocketServer.Instance.SendSynBag(synItem);
				}
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.rateScale = 1f;
				if (isHit)
				{
					swfClip.sequence = "chew";
				}
				else
				{
					swfClip.sequence = "idel";
                    base.transform.GetComponent<CapsuleCollider2D>().enabled = true;
                }
			}
		}
		if (swfClip.sequence == "chew" && swfClip.currentFrame == swfClip.frameCount - 1 && canSwallow)
		{
			canSwallow = false;
			swfClip.sequence = "swallow";
		}
		if (swfClip.sequence == "swallow" && swfClip.currentFrame == swfClip.frameCount - 1)
		{
			swfClip.sequence = "idel";
            base.transform.GetComponent<CapsuleCollider2D>().enabled = true;
        }
		if (swfClip.sequence == "idel" && swfClip.currentFrame == 3 && currGrid != null)
		{
			CheckAttack();
		}
	}

	private void CheckAttack()
	{
		if (isSleeping || currGrid == null || GameManager.Instance.isClient)
		{
			return;
		}
		zombie = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (!(zombie == null) && Mathf.Abs(zombie.transform.position.x - base.transform.position.x) < 2.2f)
		{
			clipController.rateScale = 2f;
			clipController.clip.sequence = "bite";
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

    public void OnMouseDown()
    {
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			shouldEat = !shouldEat;
			if (shouldEat)
			{
				REnderer.material.SetColor("_Color", Color.red);
			}
		}
    }

    private bool Attack()
	{
		isHit = false;
		zombie = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (zombie == null)
		{
			return false;
		}
		if (Mathf.Abs(zombie.transform.position.x - base.transform.position.x) < 2.2f)
		{
			if (shouldEat && zombie.CanEatByChomper)
			{
				shouldEat = false;
                int num = zombie.Hp / 100 + 1;
                Invoke("ChewEnd", num * 3);
                zombie.DirectDead(canDropItem: true, 0f);
                isHit = true;
            }
			else
			{
                zombie.Hurt(attackValue, Vector2.zero);
            }
		}
		return isHit;
	}

	private void ChewEnd()
	{
		canSwallow = true;
		if (GameManager.Instance.isServer)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = 1;
			synItem.SynCode[1] = 0;
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	public override void OnlineSynPlant(SynItem syn)
	{
		base.OnlineSynPlant(syn);
		if (syn.SynCode[0] == 0)
		{
			if (syn.SynCode[1] == 0)
			{
				ChewEnd();
			}
			else if (syn.SynCode[1] == 1)
			{
				clipController.rateScale = 2f;
				clipController.clip.sequence = "bite";
			}
			else if (syn.SynCode[1] == 2)
			{
				isHit = true;
			}
		}
	}

	protected override void GoAwakeSpecial()
	{
	}

	protected override void GoSleepSpecial()
	{
		GoAwake();
	}
}
