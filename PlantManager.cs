using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
	public static PlantManager Instance;

	public List<PlantBase> plants = new List<PlantBase>();

	public bool PlantInvincible;

	public bool PlantDontSleep;

	public void PlantDeadRemove(PlantBase plant)
	{
		plants.Remove(plant);
		PoolManager.Instance.PushObj(GetPlantByType(plant.GetPlantType()), plant.gameObject);
	}

	public void GameOverPause()
	{
		for (int i = 0; i < plants.Count; i++)
		{
			plants[i].GameOverFakeDeath();
		}
	}

	public void ClearAllPlant(bool synClient = true)
	{
		List<PlantBase> list = new List<PlantBase>(plants);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Dead(isFlat: false, 0f, synClient, deadRattle: false);
		}
		plants.Clear();
	}

	public int ClearMapPlant(MapBase map)
	{
		int num = 0;
		List<PlantBase> list = new List<PlantBase>(plants);
		for (int i = 0; i < list.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(list[i].transform.position) == map)
			{
				num++;
				list[i].Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
			}
		}
		return num;
	}

	private void Awake()
	{
		Instance = this;
	}

	public PlantBase GetNewPlant(PlantType type)
	{
		PlantBase component = PoolManager.Instance.GetObj(GetPlantByType(type)).GetComponent<PlantBase>();
		component.PlacePlayer = null;
		return component;
	}

	private GameObject GetPlantByType(PlantType type)
	{
		return type switch
		{
			PlantType.SunFlower => GameManager.Instance.GameConf.SunFlower, 
			PlantType.PeaShooter => GameManager.Instance.GameConf.PeaShooter, 
			PlantType.Cherry => GameManager.Instance.GameConf.Cherry, 
			PlantType.WallNut => GameManager.Instance.GameConf.WallNut, 
			PlantType.Tallnut => GameManager.Instance.GameConf.Tallnut, 
			PlantType.Lilypad => GameManager.Instance.GameConf.Lilypad, 
			PlantType.Spike => GameManager.Instance.GameConf.Spike, 
			PlantType.Repeater => GameManager.Instance.GameConf.Repeater, 
			PlantType.Torchwood => GameManager.Instance.GameConf.Torchwood, 
			PlantType.Jalapeno => GameManager.Instance.GameConf.Jalapeno, 
			PlantType.Chomper => GameManager.Instance.GameConf.Chomper, 
			PlantType.SnowPea => GameManager.Instance.GameConf.SnowPea, 
			PlantType.PotatoMine => GameManager.Instance.GameConf.PotatoMine, 
			PlantType.Squash => GameManager.Instance.GameConf.Squash, 
			PlantType.Tanglekelp => GameManager.Instance.GameConf.Tanglekelp, 
			PlantType.ThreePeater => GameManager.Instance.GameConf.ThreePeater, 
			PlantType.PuffShroom => GameManager.Instance.GameConf.PuffShroom, 
			PlantType.SunShroom => GameManager.Instance.GameConf.SunShroom, 
			PlantType.FumeShroom => GameManager.Instance.GameConf.FumeShroom, 
			PlantType.Gravebuster => GameManager.Instance.GameConf.Gravebuster, 
			PlantType.HypnoShroom => GameManager.Instance.GameConf.HypnoShroom, 
			PlantType.ScaredyShroom => GameManager.Instance.GameConf.ScaredyShroom, 
			PlantType.IceShroom => GameManager.Instance.GameConf.IceShroom, 
			PlantType.DoomShroom => GameManager.Instance.GameConf.DoomShroom, 
			PlantType.Blover => GameManager.Instance.GameConf.Blover, 
			PlantType.SeaShroom => GameManager.Instance.GameConf.SeaShroom, 
			PlantType.Pot => GameManager.Instance.GameConf.Pot, 
			PlantType.Plantern => GameManager.Instance.GameConf.Plantern, 
			PlantType.GatlingPea => GameManager.Instance.GameConf.GatlingPea, 
			PlantType.TwinSunflower => GameManager.Instance.GameConf.TwinSunflower, 
			PlantType.SpikeRock => GameManager.Instance.GameConf.SpikeRock, 
			PlantType.Marigold => GameManager.Instance.GameConf.Marigold, 
			PlantType.SplitPea => GameManager.Instance.GameConf.SplitPea, 
			PlantType.Cabbagepult => GameManager.Instance.GameConf.Cabbagepult, 
			PlantType.Cornpult => GameManager.Instance.GameConf.Cornpult, 
			PlantType.Melonpult => GameManager.Instance.GameConf.Melonpult, 
			PlantType.Wintermelonpult => GameManager.Instance.GameConf.Wintermelonpult, 
			PlantType.GloomShroom => GameManager.Instance.GameConf.GloomShroom, 
			PlantType.Magnetshroom => GameManager.Instance.GameConf.Magnetshroom, 
			PlantType.GoldMagnet => GameManager.Instance.GameConf.GoldMagnet, 
			PlantType.Coffeebean => GameManager.Instance.GameConf.Coffeebean, 
			PlantType.Pumpkin => GameManager.Instance.GameConf.Pumpkin, 
			PlantType.Cactus => GameManager.Instance.GameConf.Cactus, 
			PlantType.Starfruit => GameManager.Instance.GameConf.Starfruit, 
			PlantType.Umbrellaleaf => GameManager.Instance.GameConf.Umbrellaleaf, 
			PlantType.Garlic => GameManager.Instance.GameConf.Garlic, 
			PlantType.Cattail => GameManager.Instance.GameConf.Cattail, 
			PlantType.CobCannon => GameManager.Instance.GameConf.CobCannon, 
			PlantType.Mint => GameManager.Instance.GameConf.Mint, 
			PlantType.Heronsbill => GameManager.Instance.GameConf.Heronsbill, 
			PlantType.SnowRepeater => GameManager.Instance.GameConf.SnowRepeater, 
			PlantType.Hepatica => GameManager.Instance.GameConf.Hepatica, 
			PlantType.Imitater => GameManager.Instance.GameConf.Imitater, 
			PlantType.MoonTombStone => GameManager.Instance.GameConf.MoonTombStone, 
			_ => null, 
		};
	}
}
