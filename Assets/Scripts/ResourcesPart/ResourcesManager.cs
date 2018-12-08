using System;
using UnityEngine;
using RPG_Data;
using System.Linq;

namespace RPG_System
{
	public class ResourcesManager
	{
		public const string PrefabsPath = "Prefabs/";
		public const string CharacterPrefabPath = "Characters/0_";
		public const string AnimatorControllersPath = "Controllers/";

		public static GameObject LoadPrefab (string path)
		{
			return LoadSource<GameObject> (PrefabsPath + path);
		}
		public static TR LoadSource<TR> (string path) where TR : UnityEngine.Object
		{
			TR resoult = (TR)Resources.Load (path);
			Debug.Log (path + " = " + ((bool)resoult).ToString());
			return resoult;
		}
		public static TR HardLoad <TR> (string name) where TR : UnityEngine.Object
		{
			TR[] all = Resources.FindObjectsOfTypeAll<TR> ();
			TR r = all.FirstOrDefault ((TR first) => ((UnityEngine.Object)first).name == name);
			Debug.Log ("At " + all.Length + " objects | " + name + " = " + ((bool)r).ToString());
			return r;
		}
	}
}

