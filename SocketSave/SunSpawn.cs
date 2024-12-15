using System;
using UnityEngine;

namespace SocketSave;

[Serializable]
public class SunSpawn
{
	public int OnlineId;

	public string Player;

	public Vector2 Pos;

	public bool isSun;

	public bool isSkySun;

	public float Afloat;
}
