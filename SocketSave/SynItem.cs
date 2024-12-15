using System;
using UnityEngine;

namespace SocketSave;

[Serializable]
public class SynItem
{
	public int OnlineId;

	public int Type;

	public string name;

	public Vector2 Twofloat;

	public int[] SynCode = new int[4];
}
