using UnityEngine;

[CreateAssetMenu(fileName = "GameConf", menuName = "GameConf")]
public class GameConf : ScriptableObject
{
	[Header("地图")]
	[Tooltip("前院草地")]
	public GameObject FrontYard;

	[Tooltip("后院泳池")]
	public GameObject BackYard;

	[Tooltip("屋顶")]
	public GameObject Roof;

	[Tooltip("前沼泽")]
	public GameObject SwampFront;

	[Tooltip("对战场地")]
	public GameObject PvPYard;

	[Header("植物")]
	[Tooltip("阳光")]
	public GameObject Sun;

	[Tooltip("向日葵")]
	public GameObject SunFlower;

	[Tooltip("豌豆射手")]
	public GameObject PeaShooter;

	[Tooltip("坚果墙")]
	public GameObject WallNut;

	[Tooltip("樱桃炸弹")]
	public GameObject Cherry;

	[Tooltip("双发射手")]
	public GameObject Repeater;

	[Tooltip("高坚果")]
	public GameObject Tallnut;

	[Tooltip("睡莲")]
	public GameObject Lilypad;

	[Tooltip("地刺")]
	public GameObject Spike;

	[Tooltip("火炬树桩")]
	public GameObject Torchwood;

	[Tooltip("火爆辣椒")]
	public GameObject Jalapeno;

	[Tooltip("大嘴花")]
	public GameObject Chomper;

	[Tooltip("寒冰射手")]
	public GameObject SnowPea;

	[Tooltip("土豆地雷")]
	public GameObject PotatoMine;

	[Tooltip("倭瓜")]
	public GameObject Squash;

	[Tooltip("缠绕海草")]
	public GameObject Tanglekelp;

	[Tooltip("三线射手")]
	public GameObject ThreePeater;

	[Tooltip("小喷菇")]
	public GameObject PuffShroom;

	[Tooltip("阳光菇")]
	public GameObject SunShroom;

	[Tooltip("大喷菇")]
	public GameObject FumeShroom;

	[Tooltip("墓碑吞噬者")]
	public GameObject Gravebuster;

	[Tooltip("魅惑菇")]
	public GameObject HypnoShroom;

	[Tooltip("胆小菇")]
	public GameObject ScaredyShroom;

	[Tooltip("寒冰菇")]
	public GameObject IceShroom;

	[Tooltip("毁灭菇")]
	public GameObject DoomShroom;

	[Tooltip("三叶草")]
	public GameObject Blover;

	[Tooltip("仙人掌")]
	public GameObject Cactus;

	[Tooltip("海兵菇")]
	public GameObject SeaShroom;

	[Tooltip("花盆")]
	public GameObject Pot;

	[Tooltip("路灯花")]
	public GameObject Plantern;

	[Tooltip("机枪豌豆")]
	public GameObject GatlingPea;

	[Tooltip("孪生向日葵")]
	public GameObject TwinSunflower;

	[Tooltip("地刺王")]
	public GameObject SpikeRock;

	[Tooltip("金盏花")]
	public GameObject Marigold;

	[Tooltip("裂夹豌豆")]
	public GameObject SplitPea;

	[Tooltip("卷心菜投手")]
	public GameObject Cabbagepult;

	[Tooltip("玉米投手")]
	public GameObject Cornpult;

	[Tooltip("西瓜投手")]
	public GameObject Melonpult;

	[Tooltip("冰瓜投手")]
	public GameObject Wintermelonpult;

	[Tooltip("忧郁菇")]
	public GameObject GloomShroom;

	[Tooltip("磁力菇")]
	public GameObject Magnetshroom;

	[Tooltip("杨桃")]
	public GameObject Starfruit;

	[Tooltip("吸金磁")]
	public GameObject GoldMagnet;

	[Tooltip("咖啡豆")]
	public GameObject Coffeebean;

	[Tooltip("叶子保护伞")]
	public GameObject Umbrellaleaf;

	[Tooltip("大蒜")]
	public GameObject Garlic;

	[Tooltip("南瓜头")]
	public GameObject Pumpkin;

	[Tooltip("香蒲")]
	public GameObject Cattail;

	[Tooltip("玉米大炮")]
	public GameObject CobCannon;

	[Tooltip("薄荷")]
	public GameObject Mint;

	[Tooltip("太阳花")]
	public GameObject Heronsbill;

	[Tooltip("双发寒冰射手")]
	public GameObject SnowRepeater;

	[Tooltip("雪割草")]
	public GameObject Hepatica;

	[Tooltip("模仿者")]
	public GameObject Imitater;

	[Tooltip("月光墓碑")]
	public GameObject MoonTombStone;

	[Header("子弹")]
	[Tooltip("豌豆")]
	public GameObject Pea;

	[Tooltip("火焰豌豆")]
	public GameObject FirePea1;

	[Tooltip("寒冰豌豆")]
	public GameObject FrozenPea1;

	[Tooltip("刺")]
	public GameObject Thron;

	[Tooltip("星星")]
	public GameObject Star;

	[Tooltip("辣椒爆炸")]
	public GameObject JalapenoBoom;

	[Tooltip("缠绕海草拖拽")]
	public GameObject Tanglekelpgrab;

	[Tooltip("蘑菇孢子")]
	public GameObject ShroomPuff;

	[Tooltip("卷心菜")]
	public GameObject Cabbage;

	[Tooltip("西瓜")]
	public GameObject Melon;

	[Tooltip("冰西瓜")]
	public GameObject Wintermelon;

	[Tooltip("玉米粒")]
	public GameObject Kernal;

	[Tooltip("黄油")]
	public GameObject Butter;

	[Tooltip("香蒲子弹")]
	public GameObject CattailBullet;

	[Tooltip("加农炮弹")]
	public GameObject CannonCob;

	[Header("僵尸")]
	[Tooltip("普通僵尸")]
	public GameObject Zombie;

	[Tooltip("撑杆僵尸")]
	public GameObject Polevaulter;

	[Tooltip("橄榄球僵尸")]
	public GameObject Zombie_Football;

	[Tooltip("黑橄榄球僵尸")]
	public GameObject Zombie_BlackFootball;

	[Tooltip("读报僵尸")]
	public GameObject PaperZombie;

	[Tooltip("潜水僵尸")]
	public GameObject SnorkleZombie;

	[Tooltip("海豚僵尸")]
	public GameObject DolphinriderZombie;

	[Tooltip("冰车僵尸")]
	public GameObject ZamboniZombie;

	[Tooltip("滑雪僵尸")]
	public GameObject BobsledZombie;

	[Tooltip("小丑僵尸")]
	public GameObject JackboxZombie;

	[Tooltip("舞王僵尸")]
	public GameObject JacksonZombie;

	[Tooltip("舞者僵尸")]
	public GameObject DancerZombie;

	[Tooltip("扶梯僵尸")]
	public GameObject LadderZombie;

	[Tooltip("矿工僵尸")]
	public GameObject DiggerZombie;

	[Tooltip("气球僵尸")]
	public GameObject BalloonZombie;

	[Tooltip("投石车僵尸")]
	public GameObject CatapultZombie;

	[Tooltip("雪人僵尸")]
	public GameObject Yeti;

	[Tooltip("蹦极僵尸")]
	public GameObject BungiZombie;

	[Tooltip("跳跳僵尸")]
	public GameObject PogoZombie;

	[Tooltip("巨人僵尸")]
	public GameObject Gargantuar;

	[Tooltip("小鬼僵尸")]
	public GameObject ImpZombie;

	[Tooltip("僵王博士")]
	public GameObject BossZombie;

	[Tooltip("沼泽僵尸")]
	public GameObject SwampZombie;

	[Tooltip("沼泽幽灵")]
	public GameObject GhostZombie;

	[Tooltip("对战目标")]
	public GameObject PvPTarget;

	[Header("装备")]
	[Tooltip("篮球")]
	public GameObject Basketball;

	[Header("物品")]
	[Tooltip("钻石")]
	public GameObject Diamond;

	[Tooltip("金币")]
	public GameObject Goldcoin;

	[Tooltip("银币")]
	public GameObject Silvercoin;

	[Tooltip("钱袋子")]
	public GameObject Moneybag;

	[Tooltip("图鉴卡槽")]
	public GameObject AlmanacCardSlot;

	[Tooltip("卡槽")]
	public GameObject CardSlot;

	[Tooltip("在线卡槽")]
	public GameObject UICardSlot;

	[Tooltip("卡槽动画")]
	public GameObject CardSlotAnimation;

	[Tooltip("关卡小旗帜")]
	public GameObject LVFlag;

	[Tooltip("小推车")]
	public GameObject LawnMover;

	[Tooltip("水池推车")]
	public GameObject PoolCleaner;

	[Tooltip("屋顶推车")]
	public GameObject RoofCleaner;

	[Tooltip("烧焦僵尸")]
	public GameObject CharredZombie;

	[Tooltip("爆炸效果")]
	public GameObject EFObj;

	[Tooltip("冰道")]
	public GameObject Iceroad;

	[Tooltip("雪橇车")]
	public GameObject Sled;

	[Tooltip("蹦极目标")]
	public GameObject BungeeTarget;

	[Tooltip("墓碑")]
	public GameObject GraveStone;

	[Tooltip("坑洞")]
	public GameObject Crater;

	[Tooltip("水花")]
	public GameObject WaterFlower;

	[Tooltip("雨溅射")]
	public GameObject Rain_splash;

	[Tooltip("雨圆形")]
	public GameObject Rain_circle;

	[Tooltip("雨水坑")]
	public GameObject Puddle;

	[Header("粒子等")]
	[Tooltip("豌豆粒子")]
	public GameObject PeaParticle;

	[Tooltip("冰豌豆粒子")]
	public GameObject SnowpeaParticle;

	[Tooltip("孢子粒子")]
	public GameObject PuffParticle;

	[Tooltip("星星粒子")]
	public GameObject StarParticle;

	[Tooltip("西瓜粒子")]
	public GameObject MelonParticle;

	[Tooltip("冰西瓜粒子")]
	public GameObject WintermelonParticle;

	[Tooltip("大喷菇雾")]
	public GameObject ShroomFumeParticle;

	[Tooltip("坚果碎片")]
	public GameObject NutParticle;

	[Tooltip("樱桃爆炸")]
	public GameObject CherryParticle;

	[Tooltip("土豆爆炸")]
	public GameObject PotatoParticle;

	[Tooltip("爆米花")]
	public GameObject PopcornParticle;

	[Tooltip("圆形喷雾")]
	public GameObject CircleFumeParticle;

	[Tooltip("寒冰溅射")]
	public GameObject IceParticle;

	[Tooltip("土溅射")]
	public GameObject SmallDirtParticle;

	[Tooltip("水溅射")]
	public GameObject SmallWaterParticle;

	[Tooltip("模仿者烟雾")]
	public GameObject ImitaterParticle;

	[Tooltip("车辆零件")]
	public GameObject CarParticle;

	[Tooltip("蓝色爆炸")]
	public GameObject CloudParticle;

	[Tooltip("紫色爆炸")]
	public GameObject JackboxCloudParticle;

	[Tooltip("僵尸头")]
	public GameObject ZombieHeadParticle;

	[Tooltip("僵尸胳膊")]
	public GameObject ArmDropParticle;

	[Tooltip("僵尸装备")]
	public GameObject ZombieEquipParticle;
}
