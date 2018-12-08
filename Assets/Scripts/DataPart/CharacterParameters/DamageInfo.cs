using System;
using RPG_System;

namespace RPG_Data
{
	[Serializable]
	public enum DamageType
	{
		Fire,
		Cold,
		Electric,
		Weapon
	}

	[Serializable]
	public class DamageInfo
	{
		public DamageType damageType { get; private set; }
		public float damage {get; private set; }

		public DamageInfo (DamageType _dType, float _damage)
		{
			damageType = _dType;
			damage = _damage;
		}
	}
	
}
