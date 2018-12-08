using System;
using UnityEngine;
using RPG_System;
using RPG_Data;

namespace RPG_Mechanic
{
	public class CharacterRigidbody : AliveRigidbody
	{
		public CharacterRigidbody (AliveOverlay basedOn) : base(basedOn)
		{
			
		}
		public override void Initialize ()
		{
			base.Initialize ();
			component.mass = 80f;
			component.constraints = RigidbodyConstraints.FreezeRotation;
		}
	}
	public class CharacterAnimator : AliveAnimator
	{
		public CharacterAnimator (AliveOverlay basedOn)
			: base (basedOn,
				"Controller_male",
				"Undefined")
		{
		}
	}
	public class CharacterCollider : AliveComponent<CapsuleCollider>
	{
		public CharacterCollider (AliveOverlay basedOn) : base (basedOn)
		{
		}
		public override void Initialize ()
		{
			Transform trans = component.transform;
			component.center = trans.position + trans.up;
			component.height = 2f;
			component.radius = 0.5f;
		}
	}
}

