using UnityEngine;
using System.Collections;

public class IBaker : MonoBehaviour {

	public Transform playerSpawn;

	private void Start () {
		IControl.control.cameraEuler = (Vector2)(Vector3)IGame.buffer.cameraEuler;
		if (IGame.isNew) {
			InstantiatePlayer((IStatus)IGame.buffer.FindByName ("Player"));
			SetIndexesForDoors ();
			IQuest.current = IQuest.GetStart ();
		} else {
			ICharacter[] chars = ICharacter.FindObjectsOfType<ICharacter> ();
			for (int i = 0; i < chars.Length; i++) {
				Destroy (chars[i].gameObject);
			}
			IItemObject[] ios = IItemObject.FindObjectsOfType<IItemObject> ();
			for (int i = 0; i < ios.Length; i++) {
				Destroy (ios[i].gameObject);
			}
			for (int i = 0; i < IGame.buffer.currentLocation.objects.Length; i++) {
				if (IGame.buffer.currentLocation.objects[i] is IStatus) {
					InstantiatePlayer((IStatus)IGame.buffer.currentLocation.objects[i]);
				}
				if (IGame.buffer.currentLocation.objects[i] is ISavableItem) {
					InstantiateItem((ISavableItem)IGame.buffer.currentLocation.objects[i]);
				}
				if (IGame.buffer.currentLocation.objects[i] is IDoorSave) {
					InitializeDoor ((IDoorSave)IGame.buffer.currentLocation.objects[i]);
				}
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
	public static void InitializeDoor (IDoorSave door) {
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
	public static void InstantiatePlayer (IStatus status) {
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Players_new/" + status.iRace);
		GameObject spawned = (GameObject)Instantiate (prefab, status.position, Quaternion.Euler(Vector3.up * status.euler_y));
		ICharacter ch = spawned.GetComponent<ICharacter> ();
		ch.status = status;
		ch.PrepareToGame();
	}
	public static void InstantiateItem (ISavableItem item) {
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/ItemObject");
		GameObject spawned = (GameObject)Instantiate (prefab);
		IItemObject io = spawned.GetComponent<IItemObject> ();
		io.indentification = item.id;
		io.SetWithID ();
		io.trans.position = item.position;
		io.trans.eulerAngles = Vector3.up * item.euler_y;
	}


	public static IStatus AirDemon (IStatus caster, int level, Vector3 position) {
		IStatus st = new IStatus ();
		st.position = position;
		st.characterName = "Air_demon_" + caster.name;
		st.iPerson = caster.iPerson;
		int[] it = {8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 0};
		st.items = it;
		st.iRace = IRace.Devil;
		st.iType = IClassType.Wonder;
		st.reputationType = IReputationType.Monster;
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