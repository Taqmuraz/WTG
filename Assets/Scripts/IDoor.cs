using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class IDoorSave : ISaveble
{
	public bool opened = false;
	public bool locked = false;
	public int keyItemIndex = -1;
	public int doorIndex = -1;
	public int[] items = new int[0];
}

public class IDoor : IUsable {

	public IDoorSave data;

	private Vector3 startEuler;
	private Vector3 startPosition;

	public bool isChest = false;

	public bool vertical = false;

	private bool started = false;

	public void DoorStart () {
		if (!started) {
			trans = transform;
			startEuler = trans.localEulerAngles;
			startPosition = trans.localPosition;
			started = true;
		}
	}
	private void Start () {
		DoorStart ();
	}

	public void Use (ICharacter whom) {
		if (!data.locked) {
			OpenOrClose ();
		} else {
			if (whom.status.HasItem (data.keyItemIndex)) {
				data.locked = false;
				OpenOrClose ();
				string keyName = IItemAsset.items [data.keyItemIndex].name;
				MyDebug.Log ("Дверь открыта, использован " + keyName, Color.green, whom);
			} else {
				if (data.keyItemIndex > -1) {
					string keyName = IItemAsset.items [data.keyItemIndex].name;
					MyDebug.Log ("Заперто, нужно найти " + keyName, Color.white, whom);
				}
			}
		}
	}
	public static IDoor GetNearestClosedDoor (Vector3 point) {
		IDoor d = null;
		float min = 100;
		IDoor[] doors = FindObjectsOfType<IDoor> ();
		for (int i = 0; i < doors.Length; i++) {
			if (doors[i] is IChest) {
				continue;
			}
			float m = (doors [i].trans.position - point).magnitude;
			if (m < min && !doors[i].data.opened) {
				d = doors [i];
				min = m;
			}
		}
		return d;
	}
	public void OpenOrClose () {
		data.opened = !data.opened;
		SetState ();
	}
	public void SetState () {
		int a = 0;
		if (data.opened) {
			a = 1;
		}
		if (!vertical) {
			trans.localEulerAngles = startEuler + Vector3.forward * 90 * a;
		} else {
			trans.localPosition = startPosition - Vector3.up * 3 * a;
		}
	}
}
