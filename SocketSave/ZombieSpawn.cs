using System;
using UnityEngine;

namespace SocketSave;

[Serializable]
public class ZombieSpawn
{
	public int OnlineId;

	public string PlacePlayer;

	public ZombieType Type;

	public Vector2 SpawnPos;

	public int UpdateLine;

	public float DefSpeed;
}
