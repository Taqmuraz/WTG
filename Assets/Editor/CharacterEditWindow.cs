using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class CharacterEditWindow : EditorWindow
{
	[MenuItem ("Window/CharacterEdit")]
	private static void ShowWindow () {
		EditorWindow.GetWindow<CharacterEditWindow> ();
	}

	public Vector2 scroll;
	private Vector2 itemsScroll;

	public ICharacter editing;
	public Status status;
	private string loadName = "";
	private int itemToCreate = 0;

	private void OnGUI () {
		if (status == null) {
			editing = (ICharacter)EditorGUILayout.ObjectField (editing, typeof(ICharacter), true);
			status = editing ? editing.status : null;
			loadName = EditorGUILayout.TextField ("Find character by name : ", loadName);
			if (GUILayout.Button("Load : " + loadName)) {
				status = CharacterDataManager.LoadByName (loadName);
				if (status == null) {
					Debug.Log ("Has no character in database!");
				}
			}
		}
		using (EditorGUILayout.ScrollViewScope s = new EditorGUILayout.ScrollViewScope (scroll)) {
			scroll = s.scrollPosition;
			if (status != null) {
				DrawStatus ();
			}
		}
		if (GUILayout.Button("Save changes")) {
			if (CharacterDataManager.LoadByName (status.name) == null) {
				CharacterDataManager.characters.Add (status);
			} else {
				int i = CharacterDataManager.characters.IndexOf(CharacterDataManager.characters.First((Status s) => s.name == status.name));
				CharacterDataManager.characters [i] = status;
			}
			CharacterDataManager.Save ();
		}
	}
	public void DrawStatus () {
		status.name = EditorGUILayout.TextField ("Code name : ", status.name);
		status.characterName = EditorGUILayout.TextField ("Character name : ", status.characterName);
		status.iRace = (Race)EditorGUILayout.EnumPopup ("Race : ", status.iRace);
		status.euler_y = EditorGUILayout.FloatField ("Euler Y",status.euler_y);
		DrawItems (status);
	}
	public int DrawItemSelect (string label, int id, params ItemType[] types) {
		string[] names = ItemsAsset.GetNames (ItemsAsset.GetOfType (types));
		string cur = id > -1 ? ItemsAsset.items [id].name : "None";
		int val = names.ToList().IndexOf (cur);
		int i = EditorGUILayout.Popup (label, val, names);
		return ItemsAsset.GetIDByName (names.ElementAtOrDefault (i));
	}
	public void DrawItems (Status st) {
		GUILayout.Label ("Equipment : ");
		st.armor = DrawItemSelect ("Armor : ", st.armor, ItemType.Armor);
		st.weapon = DrawItemSelect ("Weapon : ", st.weapon, ItemType.Weapon);
		st.amulet = DrawItemSelect ("Amulet : ", st.amulet, ItemType.Amulet);
		st.rune = DrawItemSelect ("Rune : ", st.rune, ItemType.ScrollOfAir,
			ItemType.ScrollOfEarth,
			ItemType.ScrollOfFire,
			ItemType.ScrollOfGod,
			ItemType.ScrollOfWater);
		GUILayout.Label ("Items : ");
		using (EditorGUILayout.ScrollViewScope s = new EditorGUILayout.ScrollViewScope (itemsScroll)) {
			itemsScroll = s.scrollPosition;
			for (int i = 0; i < status.items.Length; i++) {
				status.items [i] = DrawItemSelect("Item_" + status.items[i], status.items[i], System.Enum.GetValues (typeof (ItemType)).Cast<ItemType> ().ToArray());
				if (GUILayout.Button("Remove")) {
					status.RemoveItemAt (i);
				}
			}
			itemToCreate = DrawItemSelect ("Item to create_" + itemToCreate, itemToCreate, System.Enum.GetValues (typeof (ItemType)).Cast<ItemType> ().ToArray());
			if (GUILayout.Button("Add item")) {
				status.AddItem (itemToCreate);
			}
		}
	}
}