using System;
using RPG_System;

namespace RPG_Data
{
	[Serializable]
	public class InventoryItem
	{
		public virtual string name { get; private set; }
		public virtual string description { get; private set; }

		public InventoryItem (string _name, string _description)
		{
			name = _name;
			description = _description;
		}

		public virtual void OnAdd (InventoryCParameter inventory)
		{
		}
		public virtual void OnRemove (InventoryCParameter inventory)
		{
		}
	}
	[Serializable]
	public abstract class HandingItem : InventoryItem
	{
		public HandingItem (string _name, string _desc) : base (_name, _desc)
		{
		}
		public virtual void OnInhanded (InventoryCParameter inventory)
		{
		}
	}
	[Serializable]
	public class WeaponItem : HandingItem
	{
		public DamageInfo damageInfo { get; private set; }

		public WeaponItem (string _name, string _desc, DamageInfo _damageInfo) : base (_name, _desc)
		{
			damageInfo = _damageInfo;
		}
	}
}







