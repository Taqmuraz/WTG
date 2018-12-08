using System;
using UnityEngine;
using RPG_Data;

namespace RPG_Mechanic
{
	public class PhysicsOverlay : OverlayBehaviour<PhysicsBehaviour>
	{
		public PhysicsOverlay (GameObject basedOn, ObjectData data) : base (basedOn, data)
		{
		}
	}
	public class PhysicsBehaviour : ExtendBehaviour, IPhysicsExistable
	{
		public Rigidbody body { get; private set; }
		public Collider coll { get; private set; }

		protected override ExtendBehaviour Init (IOwner owner)
		{
			body = GetComponent<Rigidbody> ();
			coll = GetComponent<Collider> ();

			return base.Init (owner);
		}
		protected virtual void Move (Vector3 direction)
		{
			body.velocity = direction;
		}
	}
}

