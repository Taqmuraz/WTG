using System;
using RPG_System;
using System.Linq;
using System.Collections.Generic;

namespace RPG_Data
{
	[Serializable]
	public class GameData : Container<ObjectData>
	{
		public static GameData activeGame { get; private set; }

		public GameData (ObjectData[] objs) : base (objs)
		{
		}

		public static GameData LoadGame (string name)
		{
			return FileManager.ReadFile<GameData> (name);
		}
		public void InstallGame ()
		{
			activeGame = this;
			DoForAll ((ObjectData obj) => obj.Install());
		}

		public ObjectData GetByID (string id)
		{
			return elements.FirstOrDefault ((ObjectData od) => od.id == id);
		}
		public static ObjectData GetActiveByID (string id)
		{
			return activeGame.GetByID (id);
		}
		public override void Add (ObjectData obj)
		{
			AddObjects (obj);
		}
		public void AddObjects (params ObjectData[] objs)
		{
			elements.AddRange (objs.Where ((ObjectData dat) => !GetByID (dat.id)));
		}
	}
}

