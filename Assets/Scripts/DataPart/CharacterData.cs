using System;
using RPG_Mechanic;
using RPG_System;

namespace RPG_Data
{
	[Serializable]
	public class CharacterData : AliveWorldData
	{
		public CharacterParameterContainer parameters { get; private set; }

		public CharacterData (string _id, string prefabName, params CharacterParameter[] prms)
			: base (_id, ResourcesManager.CharacterPrefabPath + prefabName,
				typeof(CharacterBehaviour).FullName,
				typeof(CharacterAnimator).FullName,
				typeof(CharacterRigidbody).FullName,
				typeof(CharacterCollider).FullName)
		{
			parameters = new CharacterParameterContainer (this, prms);
		}
	}
}

