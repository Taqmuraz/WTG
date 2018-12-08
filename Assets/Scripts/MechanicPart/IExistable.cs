using System;
using UnityEngine;
using RPG_System;

namespace RPG_Mechanic
{
	public interface IExistable
	{
		Transform trans { get; }
		GameObject gameObj { get; }
	}
	
}
