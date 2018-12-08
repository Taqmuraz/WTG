using System;
using RPG_Mechanic;
using System.Linq;
using UnityEngine;

namespace RPG_Data
{
	public interface IDataContainer
	{
		ObjectData GetData ();
	}

	[Serializable]
	public abstract class ObjectData : NullBool
	{
		public string id { get; private set; }
		public string prefab { get; private set; }
		public ExtendEvent<AliveOverlay> OnInstalled = new ExtendEvent<AliveOverlay> ();
		[NonSerialized]
		IDataContainer _installed;
		public IDataContainer installed { get { return _installed; } protected set { _installed = value; }}

		public ObjectData (string _id, string _prefab)
		{
			id = _id;
			prefab = _prefab;
		}

		public virtual ObjectData Refresh (IDataContainer container)
		{
			return this;
		}

		public abstract IDataContainer Install ();
		public IDataContainer Install (IDataContainer container)
		{
			installed = container;
			OnInstalled.Invoke ((AliveOverlay)container);
			return installed;
		}
		public static object CreateByConstructor (Type type, Type[] paramTypes, object[] paramObjs)
		{
			Debug.Log ("Behaviour : " + " | " + type == null ? "null" : "exist");

			object beh = type.GetConstructor (paramTypes).Invoke(paramObjs);
			return beh;
		}
	}
}

