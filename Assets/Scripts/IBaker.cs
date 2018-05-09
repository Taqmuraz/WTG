using UnityEngine;
using System.Collections;

public class IBaker : MonoBehaviour {

	public Transform playerSpawn;

	private void Start () {
		IControl.control.cameraEuler = (Vector2)(Vector3)SGame.buffer.cameraEuler;
		if (SGame.isNew) {
			//InstantiatePlayer((IStatus)IGame.buffer.FindByName ("Player"));
			SetIndexesForDoors ();
			//IQuest.current = IQuest.GetStart ();
		} else {
			ICharacter[] chars = ICharacter.FindObjectsOfType<ICharacter> ();
			for (int i = 0; i < chars.Length; i++) {
				Destroy (chars[i].gameObject);
			}
			IItemObject[] ios = IItemObject.FindObjectsOfType<IItemObject> ();
			for (int i = 0; i < ios.Length; i++) {
				Destroy (ios[i].gameObject);
			}
		}
		for (int i = 0; i < SGame.buffer.currentLocation.objects.Length; i++) {
			if (SGame.buffer.currentLocation.objects[i] is Status) {
				InstantiatePlayer((Status)SGame.buffer.currentLocation.objects[i]);
			}
			if (SGame.buffer.currentLocation.objects[i] is SavebleItem) {
				InstantiateItem((SavebleItem)SGame.buffer.currentLocation.objects[i]);
			}
			if (SGame.buffer.currentLocation.objects[i] is SDoorSave) {
				InitializeDoor ((SDoorSave)SGame.buffer.currentLocation.objects[i]);
			}
		}
	}
	public static void SetIndexesForDoors () {
		IDoor[] doors = IDoor.FindObjectsOfType<IDoor> ();
		for (int i = 0; i < doors.Length; i++) {
			doors [i].data.doorIndex = i;
			doors [i].data.name = "door_" + i;
		}
	}
	public static void InitializeDoor (SDoorSave door) {
		IDoor[] doors = IDoor.FindObjectsOfType<IDoor> ();
		for (int i = 0; i < doors.Length; i++) {
			if (door.doorIndex == i) {
				doors [i].data = door;
				doors [i].DoorStart ();
				if (!(doors[i] is IChest)) {
					doors [i].SetState ();
				}
				break;
			}
		}
	}
	public static void InstantiatePlayer (Status status) {
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Players_new/" + status.iRace);
		GameObject spawned = (GameObject)Instantiate (prefab, status.position, Quaternion.Euler(Vector3.up * status.euler_y));
		ICharacter ch = spawned.GetComponent<ICharacter> ();
		ch.status = status;
		ch.PrepareToGame();
	}
	public static void InstantiateItem (SavebleItem item) {
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/ItemObject");
		GameObject spawned = (GameObject)Instantiate (prefab);
		IItemObject io = spawned.GetComponent<IItemObject> ();
		io.indentification = item.id;
		io.SetWithID ();
		io.trans.position = item.position;
		io.trans.eulerAngles = Vector3.up * item.euler_y;
	}


	public static Status AirDemon (Status caster, int level, Vector3 position) {
		Status st = new Status (ClassType.Wonder);
		st.position = position;
		st.characterName = "Air_demon_" + caster.name;
		st.iPerson = caster.iPerson;
		int[] it = {8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 0};
		st.items = it;
		st.iRace = Race.Devil;
		st.reputationType = ReputationType.Monster;
		st.strongness = 5;
		st.wisdom = 5;
		st.intellect = 5;
		st.invicibility = 10;
		st.level = level;
		st.name = st.characterName;
		st.euler_y = Random.Range (0, 360);

		return st;
	}
}