using System;
using RPG_Mechanic;
using RPG_System;

namespace RPG_Data
{
	[Serializable]
	public abstract class CharacterType : CharacterParameter
	{
		public CharacterType (string _id) : base (_id)
		{
			
		}
	}
}
