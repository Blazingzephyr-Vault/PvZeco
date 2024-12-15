using UnityEngine;

public class Goldcoin : Allcoin
{
	protected override AudioClip sound => GameManager.Instance.AudioConf.ClickCoin;

	protected override int money => 50;

	protected override AudioClip dropSound => GameManager.Instance.AudioConf.DropCoin;
}
