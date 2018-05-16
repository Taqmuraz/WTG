using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class CharacterDataManager : MonoBehaviour
{
	public static string path {
		get {
			return Application.streamingAssetsPath + "/CharactersDatabase/CharactersData.cfg";
		}
	}

	public static bool Exist () {
		return File.Exists (path);
	}

	public static List<Status> characters = new List<Status>();

	public static Status LoadByName (string name) {
		characters = LoadSaved ();
		return characters.FirstOrDefault ((Status st) => st.name == name);
	}

	public static List<Status> LoadSaved () {
		List<Status> its = new List<Status> ();
		if (Exist()) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);
			its = (List<Status>)bf.Deserialize (file);
			file.Close ();
		}
		return its;
	}

	public static void Save () {
		Directory.CreateDirectory (Application.streamingAssetsPath + "/CharactersDatabase");
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (path);
		bf.Serialize (file, characters);
		file.Close ();
	}
}

