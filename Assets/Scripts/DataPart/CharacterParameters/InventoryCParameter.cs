using System;
using RPG_System;

namespace RPG_Data
{
	[Serializable]
	public class InventoryItemContainer : Container<InventoryItem>
	{
		protected InventoryCParameter inventory { get; private set; }
		public InventoryItemContainer (InventoryCParameter _inventory, params InventoryItem[] _items) : base (_items)
		{
			inventory = _inventory;
		}
		public override void Add (InventoryItem element)
		{
			base.Add (element);
			element.OnAdd (inventory);
		}
		public override void Remove (InventoryItem element)
		{
			base.Remove (element);
			element.OnRemove (inventory);
		}
	}
	[Serializable]
	public class InventoryCParameter : CharacterParameter
	{
		protected InventoryItemContainer container { get; private set; }

		public InventoryCParameter (string _id) : base (_id)
		{
			container = new InventoryItemContainer (this);
		}

		public override void EverySecUpdate ()
		{
			base.EverySecUpdate ();
			Debugger.Log ("Inventory updating!");
		}
	}
}

