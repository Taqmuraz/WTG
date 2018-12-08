using System;
using UnityEngine;
using RPG_System;

namespace RPG_Mechanic
{
	public class AnimatorParameter<T, TBehaviour> : NullBool, IAnimatorParameter<TBehaviour>
	{
		public delegate T GetValue (TBehaviour beh);
		public virtual string name { get; private set; }
		GetValue getValue { get; set; }

		public AnimatorParameter (string _name, GetValue _getValue)
		{
			name = _name;
			getValue = _getValue;
		}

		public virtual T GetParameterValue (TBehaviour beh)
		{
			return getValue (beh);
		}
		public virtual void Set (Animator animator, TBehaviour beh)
		{
			T value = GetParameterValue (beh);
			if (value is float) {
				float f = float.Parse (value.ToString ());
				animator.SetFloat (name, f);
				return;
			}
			if (value is bool) {
				bool b = bool.Parse (value.ToString ());
				animator.SetBool (name, b);
				return;
			}
			if (value is int) {
				int i = int.Parse (value.ToString ());
				animator.SetInteger (name, i);
				return;
			}
		}
	}
	
}
