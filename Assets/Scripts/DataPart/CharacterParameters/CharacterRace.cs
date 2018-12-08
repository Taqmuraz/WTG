using System;
using RPG_Mechanic;
using RPG_System;

namespace RPG_Data
{
	[Serializable]
	public enum CharacterRaceEnum
	{
		Human,
		Elf
	}
	[Serializable]
	public class CharacterRace : CharacterParameter
	{
		public CharacterRaceEnum race { get; private set; }

		public CharacterRace (CharacterRaceEnum _race, string _id) : base (_id)
		{
			race = _race;
		}
	}
}
