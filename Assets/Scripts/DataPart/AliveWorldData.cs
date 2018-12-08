using System;
using RPG_Mechanic;
using RPG_System;
using UnityEngine;
using System.Linq;

namespace RPG_Data
{
	[Serializable]
	public class AliveWorldData : ObjectData
	{
		public WorldPoint position { get; set; }
		public WorldPoint euler { get; set; }
		public string behaviour { get; private set; }
		public string[] components { get; private set; }

		public AliveWorldData (string _id, string _prefab, string _behaviour, params string[] _components) : base (_id, _prefab)
		{
			behaviour = _behaviour;
			components = _components;
		}

		public override ObjectData Refresh (IDataContainer container)
		{
			ExtendBehaviour beh = ((AliveOverlay)container).activeBehaviour;
			position = beh.trans.position;
			euler = beh.trans.eulerAngles;
			return base.Refresh (container);
		}
		public override IDataContainer Install ()
		{
			GameObject prefabObj = ResourcesManager.LoadPrefab (prefab);
			GameObject gameObject = GameObject.Instantiate (prefabObj);

			Type[] types = { typeof(GameObject), typeof(ObjectData) };
			object[] prms = { gameObject, this };

			Type type = Type.GetType (behaviour);

			IDataContainer beh = (IDataContainer)CreateByConstructor (type, types, prms);
			AliveOverlay aOver = (AliveOverlay)beh;

			foreach (string cmp in components) {
				aOver.AddComponent (cmp);
			}

			return base.Install (beh);
		}
	}
}

