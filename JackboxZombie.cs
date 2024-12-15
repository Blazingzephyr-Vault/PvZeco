using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class JackboxZombie : ZombieBase
{
	private int BoomNum;

	public Sprite arm;

	public Sprite lostArm;

	public SpriteRenderer jackBox;

	public SpriteRenderer jackBoxHandle;

	private EFAudio fAudio;

	protected override GameObject Prefab => GameManager.Instance.GameConf.JackboxZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => 1.8f;

	protected override float attackValue => 80f;

	public override int MaxHP => 500;

	protected override float DropHeadScale => 0.75f;

	public override void InitZombieHpState()
	{
		BoomNum = 0;
		Arm2Renderer.sprite = arm;
		jackBox.enabled = true;
		jackBoxHandle.enabled = true;
		if (LVManager.Instance.GameIsStart && !IsOVer)
		{
			fAudio = AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.jackinthebox, base.transform.position);
		}
	}

	private void PopClip()
	{
		if (!GameManager.Instance.isClient)
		{
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = 2;
				synItem.SynCode[0] = 2;
				SocketServer.Instance.SendSynBag(synItem);
			}
			anCanMove = false;
			animator.SetInteger("Change", 41);
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp < 180 && Arm2Renderer.sprite != lostArm)
		{
			DropArm(new Vector3(-0.25f, -0.14f), 0.38f);
			Arm2Renderer.sprite = lostArm;
		}
		if (HitSound)
		{
			if (Random.Range(0, 3) == 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat1, base.transform.position);
			}
			else if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat2, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat3, base.transform.position);
			}
		}
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().InitCharred(1, base.transform.position, new Vector3(-0.74f, 1.32f), Sorting.sortingOrder, base.IsFacingLeft);
		Dead(canDropItem: true, 0f);
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if (fAudio != null && fAudio.PlayClipName == GameManager.Instance.AudioConf.jackinthebox.name)
		{
			fAudio.Close();
		}
	}

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		BoomNum++;
	}

	public override void OnlineSynZombie(SynItem syn)
	{
		base.OnlineSynZombie(syn);
		if (syn.SynCode[0] == 2)
		{
			anCanMove = false;
			animator.SetInteger("Change", 41);
		}
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		SpriteRenderer result = null;
		if (base.Hp > 0 && jackBox.enabled)
		{
			result = jackBox;
			if (needClearEquip)
			{
				jackBox.enabled = false;
				jackBoxHandle.enabled = false;
				base.Hp = 270;
			}
		}
		return result;
	}

	public override void SpecialAnimEvent1()
	{
		if (jackBox.enabled && Random.Range(0, 8) == 1)
		{
			PopClip();
		}
	}

	public override void SpecialAnimEvent2()
	{
		if (PlacePlayer != null)
		{
			return;
		}
		if (jackBox.enabled && base.transform.position.x < 3f && base.transform.position.x >= -4f)
		{
			if (Random.Range(BoomNum, 18) > 16)
			{
				PopClip();
			}
		}
		else if (jackBox.enabled && base.transform.position.x < -4f && !isHypno)
		{
			PopClip();
		}
	}

	public override void SpecialAnimEvent3()
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.jack_surprise1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.jack_surprise2, base.transform.position);
		}
	}

	public override void SpecialAnimEvent4()
	{
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.25f, isHypno);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, needCapsule: false, !isHypno);
		if (PlacePlayer == null)
		{
			for (int i = 0; i < aroundPlant.Count; i++)
			{
				aroundPlant[i].Hurt(1800f, this);
			}
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(1800);
			}
		}
		else
		{
			for (int k = 0; k < aroundPlant.Count; k++)
			{
				if (aroundPlant[k].IsZombiePlant)
				{
					aroundPlant[k].Hurt(1800 / aroundPlant.Count, this);
				}
				else
				{
					aroundPlant[k].Hurt(1800f, this);
				}
			}
			for (int l = 0; l < zombies.Count; l++)
			{
				zombies[l].BoomHurt(1800 / zombies.Count);
			}
		}
		CameraControl.Instance.ShakeCamera(base.transform.position);
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CarParticle).transform.position = base.transform.position;
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.JackboxCloudParticle).transform.position = base.transform.position;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.explosion, base.transform.position);
		Dead(canDropItem: true, 0f);
	}

	protected override void ClickEvent(string clickPlayer)
	{
		if (clickPlayer == PlacePlayer)
		{
			PopClip();
		}
	}
}
