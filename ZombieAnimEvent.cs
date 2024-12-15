using UnityEngine;

public class ZombieAnimEvent : MonoBehaviour
{
	public ZombieBase zombie;

	public void AnCanMove()
	{
		zombie.anCanMove = true;
	}

	public void AnDontMove()
	{
		zombie.anCanMove = false;
	}

	public void Attack()
	{
		zombie.AnimAttack();
	}

	public void FlatAttack()
	{
		zombie.AnimAttack(isFlat: true);
	}

	public void Dead()
	{
		zombie.DirectDead(canDropItem: true, 1f, synClient: true);
	}

	public void DropHead()
	{
		zombie.DropHead();
	}

	public void AnimFailSound()
	{
		zombie.AnimFailSound();
	}

	public void SpecialEvent()
	{
		zombie.SpecialAnimEvent1();
	}

	public void SpecialEvent2()
	{
		zombie.SpecialAnimEvent2();
	}

	public void SpecialEvent3()
	{
		zombie.SpecialAnimEvent3();
	}

	public void SpecialEvent4()
	{
		zombie.SpecialAnimEvent4();
	}

	public void SpecialEvent5()
	{
		zombie.SpecialAnimEvent5();
	}

	public void SpecialEvent6()
	{
		zombie.SpecialAnimEvent6();
	}
}
