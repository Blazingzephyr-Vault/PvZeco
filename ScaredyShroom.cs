using System.Collections.Generic;
using FTRuntime;
using UnityEngine;

public class ScaredyShroom : PlantBase
{
	#region Major New Changes
	/// <summary>
	/// Pretty sure this doesn't work as intended.
	/// However, it is supposed to make this invincible.
	/// </summary>
	public override bool ZombieCanEat => !isSleeping;

    /// <summary>
	/// Equals to half of Conehead health.
	/// </summary>
    protected override int attackValue => 50;

	/// <summary>
	/// Shooting cooldown time.
	/// </summary>
	private float shootingCooldownTime = 1.35f;

    /// <summary>
    /// Counts idle repeat count so that shooting triggers after several sequenced idles.
    /// </summary>
    private int idlePass = 0;

	/// <summary>
	/// Scaredy Shroom's dying attack.
	/// </summary>
	/// <param name="target">The target we are trying to destroy</param>
	/// <returns>Whether the target was killed</returns>
    private bool Swallow(ZombieBase target)
    {
		// Skip these targets.
        if (target is Gargantuar || target is Zamboni || target is CatapultZombie) return false;
		if (target is BobsledZombie bz && (bz.Type == BobsledType.Sled || bz.Type == BobsledType.HelmetAndSled)) return false;

		// Lawnmower death animation.
        target.CleanerDead(true);
        Dead();
        return true;
    }

	/// <summary>
	/// Event that is activated when the Scaredy-Shroom loses health.
	/// </summary>
    protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
    {
		// Do nothing if sleeping.
        if (isSleeping) return;
        Swallow(zombie);
    }

    private void CheackScare()
    {
		// Edit: Distance --> 2
		// Works as 1x3.
        List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 2f, isHypno, needCapsule: true);
        List<PlantBase> list = null;

        if (zombies.Count == 0)
        {
            // Presumably the distance should be the same? Though they should not be destroyed by Swallow.
            list = MapManager.Instance.GetAroundPlant(base.transform.position, 2f, !isHypno);
        }

        if (zombies.Count > 0 || list.Count > 0)
        {
            clipController.clip.sequence = "scared";
            base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }

	private void CheackNoScare()
    {
        // Edit: Distance --> 2
        // Works as 1x3.
        List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 2f, isHypno, needCapsule: true);
        List<PlantBase> list = null;

        if (zombies.Count == 0)
        {
            // Presumably the distance should be the same? Though they should not be destroyed by Swallow.
            list = MapManager.Instance.GetAroundPlant(base.transform.position, 1f, !isHypno);
        }

        if (zombies.Count == 0 && list.Count == 0)
        {
            clipController.clip.sequence = "grow";

            // Edit: Re-enable the collider.
            // Reasoning: so that Scaredy-shroom will be vulnerable to long-range attacks, i.e. Catapult Zombie's balls.
            base.transform.GetComponent<CapsuleCollider2D>().enabled = true;
        }
        else
        {
            // Edit: Will trigger if there are still zombies around us.
            // Grabs the zombie and kills it, like Shadow Peashooter from PvZ 2.

            zombies = ZombieManager.Instance.GetZombies(currGrid.Point.y, base.transform.position, 0.6f, isHypno, needCapsule: true);
            foreach (ZombieBase target in zombies)
            {
				if (Swallow(target))
					break;
            }
        }
    }
    #endregion

    private Vector3 creatBulletOffsetPos = new Vector2(0.48f, -0.14f);

	public override float MaxHp => 300f;

	protected override PlantType plantType => PlantType.ScaredyShroom;

	protected override bool isShroom => true;

	protected override void FrameChangeEvent(SwfClip swfClip)
	{
		switch (swfClip.sequence)
		{
		case "idel":
			if ((swfClip.currentFrame == 0 || swfClip.currentFrame == 36) && currGrid != null)
			{
				CheackScare();

				if (idlePass >= (shootingCooldownTime / 1.35f)) CheckAttack(); 
				else if (swfClip.currentFrame == 36) idlePass++;
			}
			if (swfClip.currentFrame == closeEyeFrame)
			{
				StartCloseEyes();
			}
			break;
		case "shoot":
			if (swfClip.currentFrame == 42)
			{
				CreatePuff();
			}
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				idlePass = 0;
				clipController.clip.sequence = "idel";
			}
			break;
		case "scared":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				clipController.clip.sequence = "scaredidel";
			}
			break;
		case "scaredidel":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				CheackNoScare();
			}
			break;
		case "grow":
			if (swfClip.currentFrame == swfClip.frameCount - 1)
			{
				idlePass = 0;
				clipController.clip.sequence = "idel";
			}
			break;
		}
	}

	private void CheckAttack()
	{
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		PlantBase plantBase = null;
		if (zombieByLineMinDistance == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, currGrid.Point.y, base.IsFacingLeft, !isHypno);
		}
		if (zombieByLineMinDistance == null && plantBase == null)
		{
			if (clipController.clip.sequence != "scared")
            {
                idlePass = 0;
                clipController.clip.sequence = "idel";
            }
		}
		else if (clipController.clip.sequence != "scared")
		{
			clipController.clip.sequence = "shoot";
		}
	}

	private void CreatePuff()
	{
		if (currGrid != null)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Puff, base.transform.position);
			ShroomPuff component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ShroomPuff).GetComponent<ShroomPuff>();
			component.transform.SetParent(null);

			int isImitater = REnderer.material.GetInt("_OpenGray");
            if (base.IsFacingLeft)
			{
                component.Init(attackValue, base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), currGrid.Point.y, Vector2.left, NeedDis: false, isHypno, true, isImitater);
			}
			else
			{
				component.Init(attackValue, base.transform.position + creatBulletOffsetPos, currGrid.Point.y, Vector2.right, NeedDis: false, isHypno, true, isImitater);
            }
        }
	}

	protected override void GoSleepSpecial()
	{
		clipController.clip.sequence = "sleep";
	}
}
