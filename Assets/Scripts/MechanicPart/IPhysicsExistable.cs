using System;
using UnityEngine;
using RPG_System;

namespace RPG_Mechanic
{
	public interface IPhysicsExistable
	{
		Rigidbody body { get; }
		Collider coll { get; }
	}
	
}
