using System;
using UnityEngine;
using RPG_Data;

namespace RPG_Mechanic
{
	public interface IOwner
	{
		IThing GetThing ();
	}

	public class OverlayBehaviour<T> : NullBool, IOwner, IDataContainer where T : ExtendBehaviour
	{
		public T activeBehaviour { get; protected set; }
		protected GameObject gameObj { get; private set; }
		protected ObjectData objectData { get; private set; }

		public static string GetExtendBehaviourTypeName ()
		{
			return typeof(T).Name;
		}

		public OverlayBehaviour (GameObject basedOn, ObjectData data)
		{
			gameObj = basedOn;
			objectData = data;
			activeBehaviour = ExtendBehaviour.GetDefault<T> (this);
			Debug.Log ("Behaviour inited");
		}

		public ObjectData GetData ()
		{
			return objectData.Refresh (this);
		}

		public virtual IThing GetThing ()
		{
			return (IThing)activeBehaviour;
		}
		public virtual GameObject GetObject ()
		{
			return gameObj;
		}
	}
}

