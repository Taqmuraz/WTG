using System;
using UnityEngine;
using RPG_System;

namespace RPG_Mechanic
{

	public interface IAnimatorParameter<TBeh>
	{
		string name { get; }
		void Set (Animator animator, TBeh beh);
	}
	
}
