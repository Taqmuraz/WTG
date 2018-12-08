using System;
using UnityEngine;
using RPG_Data;
using System.Collections.Generic;
using System.Linq;
using RPG_System;

namespace RPG_Mechanic
{
	public class AliveOverlay : OverlayBehaviour<AliveBehaviour>
	{
		protected Container<IAliveComponent> components { get; private set; }

		public AliveOverlay (GameObject basedOn, ObjectData data) : base (basedOn, data)
		{
			components = new Container<IAliveComponent> ();
		}
		public IAliveComponent AddComponent (Type type)
		{
			object[] prms = new object[1] { (AliveOverlay)this };
			IAliveComponent comp = (IAliveComponent)type.GetConstructors ().FirstOrDefault().Invoke(prms);
			components.Add (comp);
			return comp;
		}
		public T AddComponent<T> () where T : IAliveComponent
		{
			return (T)AddComponent (typeof(T));
		}
		public IAliveComponent AddComponent (string typeName)
		{
			return AddComponent (Type.GetType (typeName));
		}
		public T GetComponent<T> () where T : IAliveComponent
		{
			return components.GetOfType<T> ();
		}
	}
	public class AliveBehaviour : PhysicsBehaviour
	{
		protected override ExtendBehaviour Init (IOwner owner)
		{
			return base.Init (owner);
		}
		protected override void Move (Vector3 direction)
		{
			float y = body.velocity.y;
			direction = new Vector3 (direction.x, y, direction.z);
			base.Move (direction);
		}
	}
}

