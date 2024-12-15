using System;
using System.Collections.Generic;

namespace SocketSave;

[Serializable]
public class LoadLVBag
{
	public int LoadType;

	public int LvNum;

	public int LvSeed;

	public List<int> CardNumList;

	public List<string> NameList;

	public string Name;

	public bool isEasy;
}
