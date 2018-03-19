using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IChest : IDoor {

	private void Start () {
		isChest = true;
		trans = transform;
		IDoor.doorsAll.Add (this);
		usablesAll.Add (this);
	}
	private void OnDestroy () {
		usablesAll.Remove (this);
		doorsAll.Remove (this);
	}
	public int GetItemByID (int id) {
		int index = -1;
		for (int i = 0; i < data.items.Length; i++) {
			if (data.items[i] == id) {
				index = i;
				break;
			}
		}
		return index;
	}
	public void RemoveItem (int index) {
		List<int> ints = new List<int> ();
		ints.AddRange (data.items);
		ints.RemoveAt (index);
		data.items = ints.ToArray ();
	}
	public void AddItem (int id) {
		if (id > -1) {
			List<int> ints = new List<int> ();
			ints.AddRange (data.items);
			ints.Add (id);
			data.items = ints.ToArray ();
		}
	}
	public void CastTo (ICharacter to, int index) {
		to.AddItem (data.items [index]);
		RemoveItem (index);
	}
	public void AddFrom (ICharacter whom, int index) {
		AddItem (whom.status.items[index]);
		whom.RemoveItem (index);
	}
}
