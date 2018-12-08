using System;
using RPG_System;
using System.Linq;

namespace RPG_Data
{
	[Serializable]
	public class GameData : Container<ObjectData>
	{
		public static GameData activeGame { get; private set; }

		public ObjectData[] objects { get; private set; }

		public GameData (ObjectData[] objs) : base (objs)
		{
			objects = objs;
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
			return objects.FirstOrDefault ((ObjectData od) => od.id == id);
		}
		public static ObjectData GetActiveByID (string id)
		{
			return activeGame.GetByID (id);
		}
	}
}

