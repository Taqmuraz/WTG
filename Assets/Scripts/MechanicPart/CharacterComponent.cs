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
		public const float CharacterHeight = 1f;


		public CharacterCollider (AliveOverlay basedOn) : base (basedOn)
		{
		}
		public override void Initialize ()
		{
			Transform trans = component.transform;
			component.radius = CharacterHeight * 0.25f;
			component.center = Vector3.up * CharacterHeight * 0.5f;
			component.height = CharacterHeight;
		}
	}
}

