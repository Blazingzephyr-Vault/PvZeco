using System.Drawing;
using UnityEngine;

public class ShroomPuff : MonoBehaviour
{
    #region Major New Changes
    /// <summary>
    /// Whether this puff should spawn Puff-Shroom.
    /// </summary>
    private bool spawnsPuffshroom;

    /// <summary>
    /// Whether the Puff-shroom should be grayed out.
    /// </summary>
    private int spawnsImitated;

	/// <summary>
	/// Stall effect power.
	/// Wears off over time.
	/// </summary>
    private readonly float stallEffect = 0.5f;

	/// <summary>
	/// Stall effect duration.
	/// Wears off over time.
	/// </summary>
    private readonly float stallDuration = 10f;

    /// <summary>
    /// Spawns a Puff-Shroom in the specified grid.
    /// </summary>
    /// <param name="grid">Grid to spawn in</param>
    /// <returns>Whether the spawn was successful</returns>
    public bool SpawnPuffshroom(Grid grid)
    {
        SeedBank bank = SeedBank.Instance;
        PlantBase puff = PlantManager.Instance.GetNewPlant(PlantType.PuffShroom);

        if (bank.CheckPlant(puff, grid, -2, null))
        {
            bank.PlantConfirm(puff, grid, -2, spawnsImitated, null);
            puff.GoAwake();
            return true;
        }
        else
        {
            Object.Destroy(puff.gameObject);
            return false;
        }
    }
    #endregion

	private Rigidbody2D rigibody;

	private int attackValue;

	private Vector2 Dirction;

	private int CurrLine;

	private bool isHypno;

	private float createX;

	private bool needDis;

	public Transform particle;

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirct, bool NeedDis, bool isHyp, bool spwnPf = false, int isImt = 0)
	{
		isHypno = isHyp;
		needDis = NeedDis;
		createX = pos.x;
		CurrLine = currLine;
		Dirction = dirct;
		base.transform.position = pos;
		rigibody = GetComponent<Rigidbody2D>();
		rigibody.velocity = Vector2.zero;
		rigibody.velocity = dirct * 6f;
		this.attackValue = attackValue;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		particle.localScale = new Vector3(Mathf.Abs(particle.localScale.x), particle.localScale.y, particle.localScale.z);
		if (dirct.x < 0f)
		{
			particle.localScale = new Vector3(0f - particle.localScale.x, particle.localScale.y, particle.localScale.z);
		}

        // New parameters for the Scaredy-Shroom remake.
        spawnsPuffshroom = spwnPf;
        spawnsImitated = isImt;

        Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine);
        if (gridByWorldPos.CurrPlantBase.ProtectPlant.GetPlantType() == PlantType.Heronsbill)
        {
            spawnsPuffshroom = true;
			spawnsImitated = 0;
        }
	}

	private void Update()
	{
		if (needDis && Mathf.Abs(base.transform.position.x - createX) > 4.9f)
		{
			Destroy();
			return;
		}
		if (MapManager.Instance.GetCurrMap(base.transform.position) == null)
		{
			Destroy();
		}
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine);
		if (gridByWorldPos != null && gridByWorldPos.CurrPlantBase != null && ((!gridByWorldPos.CurrPlantBase.isHypno && isHypno) || (gridByWorldPos.CurrPlantBase.isHypno && !isHypno)) && Mathf.Abs(base.transform.position.x - gridByWorldPos.Position.x) < 0.2f)
        {
            bool isDead() => gridByWorldPos.CurrPlantBase == null;
            bool alreadyDead = isDead();

            gridByWorldPos.CurrPlantBase.Hurt(attackValue, null);
			HitEff();

            if (spawnsPuffshroom)
            {
                gridByWorldPos.CurrPlantBase.ApplyScaredyStall(stallEffect, stallDuration);
                if (!alreadyDead && isDead())
                    SpawnPuffshroom(gridByWorldPos);
            }

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

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			if (componentInParent.CurrLine == CurrLine && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
                bool isDead() => componentInParent.State == ZombieState.Dead;
                bool alreadyDead = isDead();

				componentInParent.Hurt(attackValue, Dirction);
				HitEff();

                if (spawnsPuffshroom)
                {
                    componentInParent.ApplyScaredyStall(stallEffect, stallDuration);
                    if (!alreadyDead && isDead())
                        SpawnPuffshroom(componentInParent.CurrGrid);
                }
			}
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
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
			HitEff();
		}
	}

	private void HitEff()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PuffParticle).transform.position = base.transform.position + new Vector3(rigibody.velocity.normalized.x * Random.Range(0.1f, 0.2f), rigibody.velocity.normalized.y * Random.Range(0.1f, 0.2f));
		Destroy();
	}

	private void Destroy()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.ShroomPuff, base.gameObject);
	}
}
