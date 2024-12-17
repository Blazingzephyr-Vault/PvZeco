using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LV : MonoBehaviour
{
	public static LV Instance;

	public LVInfo levelInfo;

	public LVType CurrLVType;

	public SeedBankType CurrSeedBankType;

	public int LvNormalSunNum;

	public int CardNum;

	public bool SeedBankRat;

	private AudioClip dayBgm;

	private AudioClip nightBgm;

	public Vector2 SetupTime = new Vector2(10f, 12f);

	public List<List<int>> Weights = new List<List<int>>();

	public List<int> BigWaveNum = new List<int>();

	public List<List<ZombieType>> ZombieTypes = new List<List<ZombieType>>();

	public List<int> WeightForAction = new List<int>();

	public List<UnityAction> WeightAction = new List<UnityAction>();

	public Dictionary<int, UnityAction> TimeAction = new Dictionary<int, UnityAction>();

	private bool IsEasy;

	public AudioClip DayBgm
	{
		get
		{
			if (!(dayBgm == null))
			{
				return dayBgm;
			}
			return GameManager.Instance.AudioConf.GrassWalk;
		}
	}

	public AudioClip NightBgm
	{
		get
		{
			if (!(nightBgm == null))
			{
				return nightBgm;
			}
			return GameManager.Instance.AudioConf.MoonGrains;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	public void LoadLV(int LVNum, bool isEasy)
	{
		IsEasy = isEasy;
		ResetData();
		switch (LVNum)
		{
		case 0:
			LVPvP();
			break;
		case 1:
			LV1();
			break;
		case 2:
			LV2();
			break;
		case 3:
			LV3();
			break;
		case 4:
			LV4();
			break;
		case 5:
			LV5();
			break;
		case 6:
			LV6();
			break;
		case 7:
			LV7();
			break;
		case 8:
			LV8();
			break;
		case 9:
			LV9();
			break;
		case 10:
			LV10();
			break;
		case 11:
			LV11();
			break;
		case 12:
			LV12();
			break;
		case 13:
			LV13();
			break;
		case 14:
			LV14();
			break;
		case 15:
			LV15();
			break;
		case 16:
			LV16();
			break;
		case 17:
			LV17();
			break;
		case 101:
			LV101();
			break;
		case 102:
			LV102();
			break;
		case 103:
			LV103();
			break;
		case 104:
			LV104();
			break;
		case 105:
			LV105();
			break;
		case 106:
			LV106();
			break;
		default:
			LV1();
			break;
		}
	}

	private void ResetData()
	{
		Weights.Clear();
		BigWaveNum.Clear();
		ZombieTypes.Clear();
		WeightForAction.Clear();
		WeightAction.Clear();
		TimeAction.Clear();
		dayBgm = null;
		nightBgm = null;
		SeedBankRat = true;
		LvNormalSunNum = 50;
		CurrLVType = LVType.Normal;
		CurrSeedBankType = SeedBankType.SunBank;
	}

	private void LVTest()
	{
		CardNum = 10;
		SeedBankRat = false;
		LvNormalSunNum = 50;
		SetupTime = new Vector2(1000f, 2222f);
		CurrSeedBankType = SeedBankType.SunAndMoonBank;
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard
		});
		Weights = new List<List<int>>
		{
			new List<int> { 1, 1, 2, 2, 4, 12, 18, 28, 45 },
			new List<int> { 1, 1, 2, 4, 4, 15, 10, 18, 26 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType> { ZombieType.NormalZombie },
			new List<ZombieType> { ZombieType.NormalZombie }
		};
		BigWaveNum = new List<int> { 5, 8 };
		SkyManager.Instance.DirectSetTime(1140);
		TimeAction.Add(1142, delegate
		{
			MapManager.Instance.mapList[0].GraveStoneNum = 25;
			MapManager.Instance.mapList[0].GraveStoneLine = 6;
		});
	}

	private void LVPvP()
	{
		CurrLVType = LVType.PvP;
		CurrSeedBankType = PvPSelector.Instance.GetSeedBankType();
		MapManager.Instance.CreateMap(new List<GameObject> { GameManager.Instance.GameConf.PvPYard });
		if (!int.TryParse(PvPSelector.Instance.StartTime, out var result))
		{
			result = 480;
		}
		if (!int.TryParse(PvPSelector.Instance.StartSunNum, out var result2))
		{
			LvNormalSunNum = 50;
		}
		LvNormalSunNum = result2;
		SkyManager.Instance.DirectSetTime(result);
	}

	private void LV15()
	{
		CardNum = 15;
		LvNormalSunNum = 200;
		SetupTime = new Vector2(8f, 12f);
		dayBgm = GameManager.Instance.AudioConf.WateryGraves;
		nightBgm = GameManager.Instance.AudioConf.MoonGrains;
		CurrSeedBankType = SeedBankType.MoonBank;
		if (IsEasy)
		{
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 6, 14, 12, 24, 46, 30, 35, 35 },
				new List<int> { 1, 2, 4, 8, 15, 22, 31, 15, 24, 38 },
				new List<int> { 1, 2, 4, 12, 10, 18, 18, 15, 22, 38 }
			};
		}
		else
		{
			Weights = new List<List<int>>
			{
				new List<int> { 2, 2, 6, 14, 12, 24, 46, 30, 35, 55 },
				new List<int> { 2, 3, 4, 8, 15, 22, 31, 25, 34, 48 },
				new List<int> { 2, 3, 6, 12, 10, 18, 18, 25, 25, 48 }
			};
		}
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.BlackFootball,
				ZombieType.ConeZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.BungiZombie,
				ZombieType.PogoZombie,
				ZombieType.DiggerZombie,
				ZombieType.GargantuarHelmet
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.DiggerZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.GargantuarInjured,
				ZombieType.CatapultZombie,
				ZombieType.Zomboni
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.DoorAndCone,
				ZombieType.BungiZombie,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9 };
		SkyManager.Instance.DirectSetTime(972);
		TimeAction.Add(1140, delegate
		{
			SkyManager.Instance.RainScale = 5;
		});
		TimeAction.Add(1213, delegate
		{
			MapManager.Instance.mapList[0].GraveStoneNum = 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 4;
			MapManager.Instance.mapList[1].GraveStoneNum = 6;
			MapManager.Instance.mapList[1].GraveStoneLine = 4;
		});
	}

	private void LV16()
	{
		CardNum = 15;
		LvNormalSunNum = 200;
		SetupTime = new Vector2(8f, 12f);
		dayBgm = GameManager.Instance.AudioConf.WateryGraves;
		nightBgm = GameManager.Instance.AudioConf.MoonGrains;
		CurrSeedBankType = SeedBankType.SunAndMoonBank;
		if (IsEasy)
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 2, 6, 10, 10, 18, 36, 25, 28, 45,
					25, 35, 45
				},
				new List<int>
				{
					1, 3, 4, 8, 12, 18, 25, 25, 24, 38,
					22, 28, 40
				},
				new List<int>
				{
					1, 2, 6, 12, 8, 12, 18, 15, 25, 38,
					25, 35, 50
				}
			};
		}
		else
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 2, 6, 14, 12, 24, 46, 30, 35, 55,
					35, 45, 60
				},
				new List<int>
				{
					2, 3, 4, 8, 15, 22, 31, 25, 34, 48,
					35, 35, 50
				},
				new List<int>
				{
					2, 3, 6, 12, 10, 18, 18, 25, 25, 48,
					25, 35, 60
				}
			};
		}
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.BlackFootball,
				ZombieType.ConeZombie,
				ZombieType.BalloonZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.BungiZombie,
				ZombieType.PogoZombie,
				ZombieType.DiggerZombie,
				ZombieType.GargantuarHelmet
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.DiggerZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.GargantuarInjured,
				ZombieType.CatapultZombie,
				ZombieType.Zomboni,
				ZombieType.GargantuarHelmet
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.DoorAndCone,
				ZombieType.BungiZombie,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9, 12 };
		SkyManager.Instance.DirectSetTime(972);
	}

	private void LV1()
	{
        if (IsEasy)
        {
            CardNum = 15;
            LvNormalSunNum = 150;
            SetupTime = new Vector2(15f, 18f);
            Weights = new List<List<int>>
            {
                new List<int> { 1, 2, 8, 12, 18, 35 },
                new List<int> { 1, 1, 4, 6, 12, 22 }
            };
        }
        else
        {
            CardNum = 12;
            LvNormalSunNum = 100;
            SetupTime = new Vector2(8f, 12f);
            Weights = new List<List<int>>
            {
                new List<int> { 2, 4, 12, 18, 28, 55 },
                new List<int> { 1, 3, 5, 6, 12, 22 }
            };
        }
        SkyManager.Instance.DirectSetRainScale(5);
        MapManager.Instance.CreateMap(new List<GameObject>
        {
            GameManager.Instance.GameConf.FrontYard,
            GameManager.Instance.GameConf.Roof
        });
        ZombieTypes = new List<List<ZombieType>>
        {
            new List<ZombieType>
            {
                ZombieType.NormalZombie,
                ZombieType.ConeZombie,
                ZombieType.BucketZombie,
                ZombieType.Zomboni,
                ZombieType.FootballZombie,
                ZombieType.JackboxZombie,
                ZombieType.Gargantuar
            },
            new List<ZombieType>
            {
                ZombieType.NormalZombie,
                ZombieType.ConeZombie,
                ZombieType.BucketZombie,
                ZombieType.FootballZombie,
                ZombieType.CatapultZombie
            }
        };
        BigWaveNum = new List<int> { 5 };
        SkyManager.Instance.DirectSetTime(256);
    }

	private void LV2()
	{
		CardNum = 10;
		LvNormalSunNum = 75;
		dayBgm = GameManager.Instance.AudioConf.WateryGraves;
		nightBgm = GameManager.Instance.AudioConf.MoonGrains;
		if (IsEasy)
		{
			SetupTime = new Vector2(16f, 20f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 2, 2, 4, 12, 18, 28, 45 },
				new List<int> { 1, 1, 2, 4, 4, 15, 10, 18, 26 }
			};
		}
		else
		{
			SetupTime = new Vector2(8f, 12f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 4, 6, 8, 12, 25, 35, 80 },
				new List<int> { 2, 2, 2, 4, 4, 15, 10, 20, 36 }
			};
		}
		SkyManager.Instance.DirectSetRainScale(4);
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.Polevaulter,
				ZombieType.BucketZombie,
				ZombieType.Zomboni,
				ZombieType.FootballZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.PaperZombie,
				ZombieType.DoorAndCone
			}
		};
		BigWaveNum = new List<int> { 5, 8 };
		SkyManager.Instance.DirectSetTime(150);
	}

	private void LV3()
	{
		CardNum = 12;
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(20f, 24f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 4, 8, 10, 18, 22, 25, 35, 45 },
				new List<int> { 1, 2, 4, 8, 10, 12, 16, 10, 18, 28 }
			};
		}
		else
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(15f, 18f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 4, 6, 12, 10, 18, 30, 35, 40, 60 },
				new List<int> { 1, 2, 4, 8, 10, 12, 16, 10, 18, 28 }
			};
		}
		SkyManager.Instance.DirectSetRainScale(5);
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorAndCone,
				ZombieType.Zomboni,
				ZombieType.FootballZombie,
				ZombieType.BlackFootball,
				ZombieType.BalloonZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.DoorAndBucket
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9 };
		TimeAction.Add(1150, delegate
		{
			SkyManager.Instance.RainScale = 8;
			if (!MapManager.Instance.mapList[0].fog.IsOpen)
			{
				MapManager.Instance.mapList[0].fog.IsOpen = true;
				MapManager.Instance.mapList[0].fog.CreateFog(5);
				MapManager.Instance.mapList[0].fog.MoveBack();
			}
			MapManager.Instance.mapList[0].GraveStoneNum = 5;
			MapManager.Instance.mapList[0].GraveStoneLine = 7;
			MapManager.Instance.mapList[1].GraveStoneNum = 3;
			MapManager.Instance.mapList[1].GraveStoneLine = 4;
		});
		SkyManager.Instance.DirectSetTime(900);
	}

	private void LV4()
	{
		if (IsEasy)
		{
			CardNum = 15;
			LvNormalSunNum = 150;
			SetupTime = new Vector2(15f, 18f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 8, 12, 18, 35 },
				new List<int> { 1, 1, 4, 6, 12, 22 }
			};
		}
		else
		{
			CardNum = 12;
			LvNormalSunNum = 100;
			SetupTime = new Vector2(8f, 12f);
			Weights = new List<List<int>>
			{
				new List<int> { 2, 4, 12, 18, 28, 55 },
				new List<int> { 1, 3, 5, 6, 12, 22 }
			};
		}
		SkyManager.Instance.DirectSetRainScale(5);
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.Zomboni,
				ZombieType.FootballZombie,
				ZombieType.JackboxZombie,
				ZombieType.Gargantuar
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.FootballZombie,
				ZombieType.CatapultZombie
			}
		};
		BigWaveNum = new List<int> { 5 };
		SkyManager.Instance.DirectSetTime(256);
	}

	private void LV5()
	{
		CardNum = 15;
		SetupTime = new Vector2(15f, 18f);
		if (IsEasy)
		{
			LvNormalSunNum = 3000;
			Weights = new List<List<int>>
			{
				new List<int> { 1, 3, 8, 12, 35, 45, 20, 30, 65 },
				new List<int> { 1, 1, 5, 6, 8, 15, 12, 15, 25 }
			};
		}
		else
		{
			LvNormalSunNum = 2500;
			Weights = new List<List<int>>
			{
				new List<int> { 5, 5, 12, 20, 45, 65, 20, 50, 85 },
				new List<int> { 1, 2, 5, 6, 8, 15, 12, 15, 25 }
			};
		}
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.Zomboni,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar,
				ZombieType.BungiZombie,
				ZombieType.FootballZombie,
				ZombieType.BlackFootball,
				ZombieType.JackboxZombie,
				ZombieType.GargantuarHelmet
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.CatapultZombie,
				ZombieType.DiggerZombie
			}
		};
		BigWaveNum = new List<int> { 5, 8 };
		TimeAction.Add(262, delegate
		{
			if (!MapManager.Instance.mapList[0].fog.IsOpen)
			{
				MapManager.Instance.mapList[1].fog.IsOpen = true;
				MapManager.Instance.mapList[1].fog.CreateFog(3);
				MapManager.Instance.mapList[1].fog.MoveBack();
				MapManager.Instance.mapList[0].fog.IsOpen = true;
				MapManager.Instance.mapList[0].fog.CreateFog(1);
				MapManager.Instance.mapList[0].fog.MoveBack();
			}
			MapManager.Instance.mapList[0].GraveStoneNum = 20;
			MapManager.Instance.mapList[0].GraveStoneLine = 2;
			MapManager.Instance.mapList[1].GraveStoneNum = 3;
			MapManager.Instance.mapList[1].GraveStoneLine = 8;
			SkyManager.Instance.RainScale = 10;
		});
		SkyManager.Instance.DirectSetTime(213);
	}

	private void LV6()
	{
		CardNum = 15;
		LvNormalSunNum = 300;
		dayBgm = GameManager.Instance.AudioConf.WateryGraves;
		nightBgm = GameManager.Instance.AudioConf.RigorMormist;
		SetupTime = new Vector2(12f, 15f);
		if (IsEasy)
		{
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 4, 8, 8, 12, 22, 16, 20, 30 },
				new List<int> { 1, 2, 4, 8, 8, 10, 16, 8, 12, 25 }
			};
		}
		else
		{
			Weights = new List<List<int>>
			{
				new List<int> { 2, 2, 6, 10, 8, 16, 30, 20, 28, 40 },
				new List<int> { 1, 2, 4, 8, 10, 12, 18, 8, 12, 28 }
			};
		}
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.BackYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.BucketZombie,
				ZombieType.Gargantuar,
				ZombieType.BungiZombie,
				ZombieType.FootballZombie,
				ZombieType.BlackFootball,
				ZombieType.JackboxZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.GargantuarInjured,
				ZombieType.Polevaulter,
				ZombieType.PogoZombie
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9 };
		SkyManager.Instance.DirectSetTime(500);
	}

	private void LV7()
	{
		CardNum = 15;
		nightBgm = GameManager.Instance.AudioConf.RigorMormist;
		if (IsEasy)
		{
			LvNormalSunNum = 200;
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 3, 4, 12, 8, 12, 22 },
				new List<int> { 1, 1, 2, 4, 8, 10, 12, 16 }
			};
		}
		else
		{
			LvNormalSunNum = 50;
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 5, 6, 12, 12, 18, 22 },
				new List<int> { 1, 2, 2, 4, 12, 10, 18, 25 }
			};
		}
		SetupTime = new Vector2(10f, 12f);
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.DoorAndCone,
				ZombieType.DoorAndBucket,
				ZombieType.Polevaulter
			}
		};
		BigWaveNum = new List<int> { 4, 7 };
		TimeAction.Add(1050, delegate
		{
			SkyManager.Instance.DirectSetRainScale(10);
			SkyManager.Instance.IsThunder = true;
		});
		SkyManager.Instance.DirectSetTime(750);
	}

	private void LV8()
	{
		CardNum = 15;
		LvNormalSunNum = 400;
		nightBgm = GameManager.Instance.AudioConf.RigorMormist;
		if (IsEasy)
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 1, 3, 4, 12, 8, 10, 18, 10, 12,
					18, 35
				},
				new List<int>
				{
					1, 1, 2, 4, 8, 10, 12, 16, 12, 12,
					16, 25
				}
			};
		}
		else
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 3, 5, 6, 12, 8, 12, 22, 15, 18,
					20, 60
				},
				new List<int>
				{
					2, 2, 2, 4, 8, 10, 12, 16, 12, 12,
					20, 40
				}
			};
		}
		SkyManager.Instance.DirectSetRainScale(10);
		SetupTime = new Vector2(10f, 12f);
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.GargantuarInjured,
				ZombieType.FootballZombie,
				ZombieType.GargantuarHelmetRedeye
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.DoorAndCone,
				ZombieType.DoorAndBucket,
				ZombieType.GargantuarRedeye,
				ZombieType.Polevaulter
			}
		};
		BigWaveNum = new List<int> { 4, 7, 11 };
		TimeAction.Add(1050, delegate
		{
			SkyManager.Instance.IsThunder = true;
		});
		SkyManager.Instance.DirectSetTime(880);
	}

	private void LV9()
	{
		CardNum = 15;
		dayBgm = GameManager.Instance.AudioConf.GrazeTheRoof;
		nightBgm = GameManager.Instance.AudioConf.MoonGrains;
		CurrSeedBankType = SeedBankType.MoonBank;
		if (IsEasy)
		{
			LvNormalSunNum = 200;
			SetupTime = new Vector2(15f, 20f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 3, 12, 8, 12, 18, 15, 18, 30 },
				new List<int> { 1, 1, 4, 8, 10, 12, 16, 8, 12, 28 },
				new List<int> { 1, 1, 6, 12, 10, 18, 18, 15, 20, 30 }
			};
		}
		else
		{
			LvNormalSunNum = 50;
			SetupTime = new Vector2(8f, 12f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 6, 12, 8, 12, 22, 20, 25, 40 },
				new List<int> { 1, 1, 4, 8, 10, 12, 16, 8, 12, 28 },
				new List<int> { 1, 2, 8, 12, 10, 18, 18, 15, 20, 40 }
			};
		}
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.Polevaulter,
				ZombieType.Zomboni,
				ZombieType.ConeZombie,
				ZombieType.GargantuarInjured
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.DiggerZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.BucketZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.DoorAndCone,
				ZombieType.BungiZombie,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9 };
		SkyManager.Instance.DirectSetTime(960);
	}

	private void LV10()
	{
		CardNum = 15;
		if (IsEasy)
		{
			LvNormalSunNum = 300;
			BigWaveNum = new List<int> { 3, 6 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 6, 18, 10, 15, 22 },
				new List<int> { 1, 2, 6, 8, 10, 12, 18 }
			};
		}
		else
		{
			LvNormalSunNum = 100;
			BigWaveNum = new List<int> { 3, 6, 9 };
			Weights = new List<List<int>>
			{
				new List<int> { 2, 2, 8, 28, 12, 15, 22, 20, 25, 40 },
				new List<int> { 2, 2, 6, 8, 10, 12, 16, 8, 12, 58 }
			};
		}
		SetupTime = new Vector2(15f, 20f);
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.BlackFootball,
				ZombieType.BungiZombie,
				ZombieType.PogoZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.BlackFootball,
				ZombieType.PogoZombie,
				ZombieType.DoorAndCone,
				ZombieType.BucketZombie
			}
		};
		SkyManager.Instance.DirectSetTime(480);
	}

	private void LV11()
	{
		CardNum = 15;
		LvNormalSunNum = 350;
		dayBgm = GameManager.Instance.AudioConf.WateryGraves;
		nightBgm = GameManager.Instance.AudioConf.RigorMormist;
		SetupTime = new Vector2(12f, 15f);
		if (IsEasy)
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 1, 3, 8, 8, 8, 16, 20, 25, 30,
					20, 25, 40
				},
				new List<int>
				{
					1, 1, 3, 12, 10, 16, 30, 15, 18, 28,
					22, 25, 40
				},
				new List<int>
				{
					1, 1, 3, 12, 10, 15, 15, 15, 20, 28,
					25, 28, 40
				}
			};
		}
		else
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 2, 3, 12, 8, 12, 22, 26, 30, 40,
					30, 33, 50
				},
				new List<int>
				{
					2, 2, 3, 12, 10, 16, 30, 15, 18, 28,
					30, 33, 60
				},
				new List<int>
				{
					1, 2, 3, 12, 10, 18, 18, 15, 20, 30,
					30, 33, 50
				}
			};
		}
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.ConeZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.BungiZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.DiggerZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.Gargantuar,
				ZombieType.CatapultZombie,
				ZombieType.Zomboni
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.JackboxZombie,
				ZombieType.BungiZombie,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9, 12 };
		TimeAction.Add(1138, delegate
		{
			if (!MapManager.Instance.mapList[0].fog.IsOpen)
			{
				MapManager.Instance.mapList[0].fog.IsOpen = true;
				MapManager.Instance.mapList[0].fog.CreateFog(3);
				MapManager.Instance.mapList[0].fog.MoveBack();
				MapManager.Instance.mapList[1].fog.IsOpen = true;
				MapManager.Instance.mapList[1].fog.CreateFog(2);
				MapManager.Instance.mapList[1].fog.MoveBack();
				MapManager.Instance.mapList[2].fog.IsOpen = true;
				MapManager.Instance.mapList[2].fog.CreateFog(2);
				MapManager.Instance.mapList[2].fog.MoveBack();
			}
			MapManager.Instance.mapList[1].GraveStoneNum = 3;
			MapManager.Instance.mapList[1].GraveStoneLine = 4;
			SkyManager.Instance.RainScale = 4;
		});
		SkyManager.Instance.DirectSetTime(520);
	}

	private void LV12()
	{
		CardNum = 15;
		SetupTime = new Vector2(15f, 18f);
		if (IsEasy)
		{
			LvNormalSunNum = 500;
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 1, 3, 8, 8, 8, 12, 18, 20, 30,
					18, 20, 40
				},
				new List<int>
				{
					1, 1, 3, 12, 10, 16, 30, 15, 18, 28,
					20, 25, 35
				},
				new List<int>
				{
					1, 2, 3, 12, 10, 18, 18, 15, 20, 25,
					18, 20, 35
				}
			};
		}
		else
		{
			LvNormalSunNum = 300;
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 2, 3, 12, 8, 12, 22, 24, 26, 40,
					20, 28, 50
				},
				new List<int>
				{
					1, 2, 3, 12, 10, 16, 30, 15, 18, 28,
					20, 28, 40
				},
				new List<int>
				{
					1, 2, 3, 12, 10, 18, 18, 15, 20, 30,
					20, 26, 40
				}
			};
		}
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.ConeZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.DiggerZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.Gargantuar,
				ZombieType.CatapultZombie,
				ZombieType.Zomboni
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.JackboxZombie,
				ZombieType.BungiZombie,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9, 12 };
		TimeAction.Add(1140, delegate
		{
			SkyManager.Instance.RainScale = 10;
			SkyManager.Instance.IsThunder = true;
		});
		TimeAction.Add(1213, delegate
		{
			MapManager.Instance.mapList[0].GraveStoneNum = 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 4;
			MapManager.Instance.mapList[1].GraveStoneNum = 6;
			MapManager.Instance.mapList[1].GraveStoneLine = 4;
		});
		SkyManager.Instance.DirectSetTime(1021);
	}

	private void LV13()
	{
		CardNum = 15;
		SetupTime = new Vector2(15f, 18f);
		dayBgm = GameManager.Instance.AudioConf.GrazeTheRoof;
		nightBgm = GameManager.Instance.AudioConf.RigorMormist;
		if (IsEasy)
		{
			LvNormalSunNum = 800;
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 2, 2, 8, 6, 8, 18, 20, 20, 30,
					20, 28, 35
				},
				new List<int>
				{
					1, 2, 3, 4, 8, 10, 30, 10, 12, 25,
					20, 28, 40
				},
				new List<int>
				{
					1, 2, 3, 4, 6, 8, 12, 10, 15, 25,
					20, 25, 35
				}
			};
		}
		else
		{
			LvNormalSunNum = 600;
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 1, 3, 10, 8, 12, 22, 26, 30, 40,
					30, 33, 50
				},
				new List<int>
				{
					1, 2, 3, 8, 8, 10, 30, 15, 18, 28,
					30, 33, 60
				},
				new List<int>
				{
					1, 2, 3, 6, 8, 10, 18, 15, 20, 30,
					30, 33, 50
				}
			};
		}
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.BlackFootball,
				ZombieType.ConeZombie,
				ZombieType.BalloonZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.BungiZombie,
				ZombieType.PogoZombie,
				ZombieType.DiggerZombie,
				ZombieType.GargantuarHelmetRedeye
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.DiggerZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.Gargantuar,
				ZombieType.CatapultZombie,
				ZombieType.Zomboni,
				ZombieType.GargantuarRedeye,
				ZombieType.GargantuarHelmet
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.FootballZombie,
				ZombieType.BungiZombie,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.GargantuarRedeye
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9, 12 };
		TimeAction.Add(1213, delegate
		{
			if (!MapManager.Instance.mapList[0].fog.IsOpen)
			{
				MapManager.Instance.mapList[0].fog.IsOpen = true;
				MapManager.Instance.mapList[0].fog.CreateFog(2);
				MapManager.Instance.mapList[0].fog.MoveBack();
				MapManager.Instance.mapList[1].fog.IsOpen = true;
				MapManager.Instance.mapList[1].fog.CreateFog(2);
				MapManager.Instance.mapList[1].fog.MoveBack();
				MapManager.Instance.mapList[2].fog.IsOpen = true;
				MapManager.Instance.mapList[2].fog.CreateFog(2);
				MapManager.Instance.mapList[2].fog.MoveBack();
			}
			MapManager.Instance.mapList[0].GraveStoneNum = 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 4;
			MapManager.Instance.mapList[1].GraveStoneNum = 6;
			MapManager.Instance.mapList[1].GraveStoneLine = 4;
			SkyManager.Instance.RainScale = 6;
		});
		SkyManager.Instance.DirectSetTime(700);
	}

	private void LV14()
	{
		CardNum = 15;
		dayBgm = GameManager.Instance.AudioConf.GrazeTheRoof;
		nightBgm = GameManager.Instance.AudioConf.RigorMormist;
		if (IsEasy)
		{
			LvNormalSunNum = 1200;
			SetupTime = new Vector2(25f, 30f);
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 2, 3, 10, 6, 8, 12, 16, 20, 30,
					20, 23, 40, 25, 35, 45
				},
				new List<int>
				{
					1, 2, 3, 8, 10, 16, 30, 15, 18, 28,
					30, 33, 40, 30, 30, 45
				},
				new List<int>
				{
					1, 2, 3, 8, 10, 18, 10, 15, 15, 20,
					20, 23, 40, 30, 30, 45
				}
			};
		}
		else
		{
			LvNormalSunNum = 1000;
			SetupTime = new Vector2(20f, 25f);
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 2, 3, 16, 12, 12, 22, 26, 30, 40,
					30, 33, 50, 40, 50, 80
				},
				new List<int>
				{
					2, 2, 3, 12, 10, 20, 30, 15, 25, 28,
					30, 33, 60, 40, 50, 65
				},
				new List<int>
				{
					2, 2, 3, 8, 10, 18, 18, 15, 20, 30,
					30, 33, 50, 40, 50, 65
				}
			};
		}
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.BlackFootball,
				ZombieType.ConeZombie,
				ZombieType.BalloonZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.BungiZombie,
				ZombieType.PogoZombie,
				ZombieType.DiggerZombie,
				ZombieType.GargantuarHelmetRedeye
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.DiggerZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.Gargantuar,
				ZombieType.CatapultZombie,
				ZombieType.Zomboni,
				ZombieType.GargantuarRedeye,
				ZombieType.GargantuarHelmet
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.FootballZombie,
				ZombieType.BungiZombie,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.GargantuarRedeye,
				ZombieType.GargantuarHelmetRedeye
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9, 12, 15 };
		TimeAction.Add(1100, delegate
		{
			MapManager.Instance.mapList[0].GraveStoneNum = 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 4;
			MapManager.Instance.mapList[1].GraveStoneNum = 6;
			MapManager.Instance.mapList[1].GraveStoneLine = 4;
			SkyManager.Instance.RainScale = 10;
			SkyManager.Instance.IsThunder = true;
		});
		SkyManager.Instance.DirectSetTime(621);
	}

	private void LV17()
	{
		CardNum = 15;
		nightBgm = GameManager.Instance.AudioConf.RigorMormist;
		if (IsEasy)
		{
			LvNormalSunNum = 800;
			SetupTime = new Vector2(25f, 30f);
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 2, 3, 10, 6, 8, 12, 16, 20, 30,
					20, 23, 40, 25, 35, 55
				},
				new List<int>
				{
					2, 2, 3, 8, 10, 16, 30, 15, 18, 28,
					30, 33, 40, 30, 30, 55
				},
				new List<int>
				{
					2, 2, 5, 8, 10, 18, 10, 15, 15, 20,
					20, 23, 40, 30, 30, 55
				}
			};
		}
		else
		{
			LvNormalSunNum = 600;
			SetupTime = new Vector2(18f, 22f);
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 2, 3, 16, 12, 12, 22, 26, 30, 40,
					30, 43, 50, 50, 60, 80
				},
				new List<int>
				{
					2, 5, 8, 12, 10, 20, 30, 15, 25, 28,
					30, 33, 60, 50, 60, 75
				},
				new List<int>
				{
					1, 3, 6, 8, 10, 18, 18, 15, 20, 30,
					30, 33, 60, 50, 60, 75
				}
			};
		}
		CurrSeedBankType = SeedBankType.SunAndMoonBank;
		MapManager.Instance.CreateMap(new List<GameObject>
		{
			GameManager.Instance.GameConf.FrontYard,
			GameManager.Instance.GameConf.BackYard,
			GameManager.Instance.GameConf.Roof
		});
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.BlackFootball,
				ZombieType.ConeZombie,
				ZombieType.BalloonZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.BungiZombie,
				ZombieType.PogoZombie,
				ZombieType.DiggerZombie,
				ZombieType.GargantuarHelmetRedeye
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.DiggerZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.Gargantuar,
				ZombieType.CatapultZombie,
				ZombieType.Zomboni,
				ZombieType.GargantuarRedeye,
				ZombieType.GargantuarHelmet
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.DoorAndCone,
				ZombieType.JackboxZombie,
				ZombieType.FootballZombie,
				ZombieType.BungiZombie,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.GargantuarRedeye,
				ZombieType.GargantuarHelmetRedeye
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9, 12, 15 };
		TimeAction.Add(1100, delegate
		{
			MapManager.Instance.mapList[0].GraveStoneNum = 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 4;
			MapManager.Instance.mapList[1].GraveStoneNum = 6;
			MapManager.Instance.mapList[1].GraveStoneLine = 4;
			SkyManager.Instance.RainScale = 10;
			SkyManager.Instance.IsThunder = true;
		});
		SkyManager.Instance.DirectSetTime(821);
	}

	private void LV101()
	{
		dayBgm = GameManager.Instance.AudioConf.SwampDay;
		nightBgm = GameManager.Instance.AudioConf.SwampNight;
		if (IsEasy)
		{
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 3, 8, 12, 13, 20 }
			};
		}
		else
		{
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 4, 8, 22, 18, 28 }
			};
		}
		CardNum = 10;
		LvNormalSunNum = 100;
		SetupTime = new Vector2(15f, 18f);
		MapManager.Instance.CreateMap(new List<GameObject> { GameManager.Instance.GameConf.SwampFront });
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.SwampDoor
			}
		};
		BigWaveNum = new List<int> { 4, 6 };
		SkyManager.Instance.DirectSetTime(900);
	}

	private void LV102()
	{
		dayBgm = GameManager.Instance.AudioConf.SwampDay;
		nightBgm = GameManager.Instance.AudioConf.SwampNight;
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 2, 2, 6, 10, 6, 8, 12, 20, 18,
					25
				}
			};
		}
		else
		{
			LvNormalSunNum = 100;
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 2, 3, 6, 14, 8, 10, 18, 28, 20,
					35
				}
			};
		}
		CardNum = 12;
		SetupTime = new Vector2(13f, 15f);
		MapManager.Instance.CreateMap(new List<GameObject> { GameManager.Instance.GameConf.SwampFront });
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.SwampDoor,
				ZombieType.StoolZombie
			}
		};
		BigWaveNum = new List<int> { 4, 8, 10 };
		SkyManager.Instance.DirectSetTime(700);
	}

	private void LV103()
	{
		dayBgm = GameManager.Instance.AudioConf.SwampDay;
		nightBgm = GameManager.Instance.AudioConf.SwampNight;
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 4, 6, 18, 12, 22, 10, 16, 26 }
			};
		}
		else
		{
			LvNormalSunNum = 50;
			Weights = new List<List<int>>
			{
				new List<int> { 2, 3, 8, 8, 24, 18, 28, 14, 20, 30 }
			};
		}
		CardNum = 15;
		SetupTime = new Vector2(10f, 12f);
		MapManager.Instance.CreateMap(new List<GameObject> { GameManager.Instance.GameConf.SwampFront });
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.SwampDoor,
				ZombieType.StoolZombie
			}
		};
		BigWaveNum = new List<int> { 4, 6, 9 };
		TimeAction.Add(120, delegate
		{
			if (!MapManager.Instance.mapList[0].fog.IsOpen)
			{
				MapManager.Instance.mapList[0].fog.IsOpen = true;
				MapManager.Instance.mapList[0].fog.CreateFog(3);
				MapManager.Instance.mapList[0].fog.MoveBack();
			}
		});
		SkyManager.Instance.DirectSetTime(12);
	}

	private void LV104()
	{
		dayBgm = GameManager.Instance.AudioConf.SwampDay;
		nightBgm = GameManager.Instance.AudioConf.SwampNight;
		CardNum = 15;
		if (IsEasy)
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 2, 3, 6, 12, 8, 10, 12, 22, 25,
					35
				}
			};
		}
		else
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 2, 3, 6, 18, 10, 12, 18, 28, 35,
					48
				}
			};
		}
		LvNormalSunNum = 100;
		SetupTime = new Vector2(13f, 15f);
		MapManager.Instance.CreateMap(new List<GameObject> { GameManager.Instance.GameConf.SwampFront });
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.SwampDoor,
				ZombieType.StoolZombie,
				ZombieType.Ghost,
				ZombieType.Ghost
			}
		};
		BigWaveNum = new List<int> { 4, 8, 10 };
		SkyManager.Instance.DirectSetTime(1113);
	}

	private void LV105()
	{
		dayBgm = GameManager.Instance.AudioConf.SwampDay;
		nightBgm = GameManager.Instance.AudioConf.SwampNight;
		CardNum = 15;
		if (IsEasy)
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 2, 5, 8, 16, 16, 20, 14, 20, 20,
					25, 35
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie,
					ZombieType.Ghost
				}
			};
		}
		else
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 4, 6, 8, 24, 18, 28, 14, 20, 25,
					30, 45
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie,
					ZombieType.Ghost,
					ZombieType.Ghost
				}
			};
		}
		LvNormalSunNum = 150;
		SetupTime = new Vector2(10f, 12f);
		MapManager.Instance.CreateMap(new List<GameObject> { GameManager.Instance.GameConf.SwampFront });
		BigWaveNum = new List<int> { 4, 6, 11 };
		TimeAction.Add(198, delegate
		{
			if (!MapManager.Instance.mapList[0].fog.IsOpen)
			{
				MapManager.Instance.mapList[0].fog.IsOpen = true;
				MapManager.Instance.mapList[0].fog.CreateFog(4);
				MapManager.Instance.mapList[0].fog.MoveBack();
			}
		});
		SkyManager.Instance.DirectSetTime(142);
	}

	private void LV106()
	{
		dayBgm = GameManager.Instance.AudioConf.SwampDay;
		nightBgm = GameManager.Instance.AudioConf.SwampNight;
		CardNum = 15;
		if (IsEasy)
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 2, 4, 6, 18, 12, 24, 12, 16, 24,
					20, 25, 25, 30, 35
				}
			};
		}
		else
		{
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 2, 8, 8, 24, 18, 28, 14, 20, 30,
					20, 25, 45, 38, 55
				}
			};
		}
		LvNormalSunNum = 300;
		SetupTime = new Vector2(10f, 12f);
		MapManager.Instance.CreateMap(new List<GameObject> { GameManager.Instance.GameConf.SwampFront });
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.SwampDoor,
				ZombieType.StoolZombie,
				ZombieType.Ghost
			}
		};
		BigWaveNum = new List<int> { 4, 6, 9, 12, 14 };
		TimeAction.Add(1135, delegate
		{
			SkyManager.Instance.RainScale = 10;
			SkyManager.Instance.IsThunder = true;
		});
		SkyManager.Instance.DirectSetTime(820);
	}
}
