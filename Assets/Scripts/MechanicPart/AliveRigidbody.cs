using System;
using UnityEngine;
using RPG_System;

namespace RPG_Mechanic
{
	
	public class AliveRigidbody : AliveComponent<Rigidbody>
	{
		public AliveRigidbody (AliveOverlay basedOn) : base (basedOn)
		{
		}

		public Vector3 localVelocity
		{
			get
			{
				return trans.InverseTransformDirection (body.velocity);
			}
			set 
			{
				body.velocity = trans.TransformDirection (value);
			}
		}

		public override void Initialize ()
		{
			component.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		}
	}
	
}
