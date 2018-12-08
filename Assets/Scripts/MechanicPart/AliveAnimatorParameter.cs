using System;
using UnityEngine;
using RPG_System;

namespace RPG_Mechanic
{

	public class AliveAnimatorParameter<T> : AnimatorParameter<T, AliveOverlay>
	{
		public AliveAnimatorParameter (string _name, GetValue _getValue) : base (_name, _getValue)
		{
		}
		public override void Set (Animator animator, AliveOverlay beh)
		{
			base.Set (animator, beh);
		}
	}
	
}
