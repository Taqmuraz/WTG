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

	public static List<IDoor> doorsAll = new List<IDoor>();

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
		doorsAll.Add (this);
		usablesAll.Add (this);
		NavMeshObstacle obs = GetComponent<NavMeshObstacle> ();
		BoxCollider coll = gameObject.AddComponent<BoxCollider> ();
		coll.center = obs.center;
		coll.size = obs.size;
		gameObject.layer = LayerMask.NameToLayer ("Door");
	}

	private bool stateStartSetted = false;

	private void OnDestroy () {
		usablesAll.Remove (this);
		doorsAll.Remove (this);
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
		IDoor[] doors = IDoor.doorsAll.ToArray ();
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
	}
	public void SetState () {
		if (!stateStartSetted) {
			SetStateNow ();
			stateStartSetted = true;
		}
		int a = 0;
		if (data.opened) {
			a = 1;
		}
		if (!vertical) {
			Quaternion rot = Quaternion.Euler ((startEuler + Vector3.forward * 90 * a));
			trans.localRotation = Quaternion.Slerp (trans.localRotation, rot, Time.fixedDeltaTime * 4);
		} else {
			Vector3 pos = startPosition - Vector3.up * 3 * a;
			trans.localPosition = Vector3.Slerp (trans.localPosition, pos, Time.fixedDeltaTime * 4);
		}
	}
	private void SetStateNow () {
		int a = 0;
		if (data.opened) {
			a = 1;
		}
		if (!vertical) {
			trans.localRotation = Quaternion.Euler ((startEuler + Vector3.forward * 90 * a));
		} else {
			Vector3 pos = startPosition - Vector3.up * 3 * a;
			trans.localPosition = pos;
		}
	}
	private void FixedUpdate () {
		if (!(this is IChest)) {
			SetState ();
		}
	}
}
