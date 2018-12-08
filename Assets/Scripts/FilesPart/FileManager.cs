using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

namespace RPG_System
{
	public class FileManager
	{
		public static string rootPath
		{
			get {
				return Application.persistentDataPath + '/';
			}
		}

		public static void WriteFile (object obj, string name)
		{
			FileStream file = File.Create (rootPath + name);
			BinaryFormatter bf = new BinaryFormatter ();
			bf.Serialize (file, obj);
			file.Close ();
		}
		public static T ReadFile <T> (string name)
		{
			string path = rootPath + name;
			T resoult = default(T);
			if (File.Exists(path)) {
				FileStream file = File.Open (rootPath + name, FileMode.Open);
				BinaryFormatter bf = new BinaryFormatter ();
				resoult = (T)bf.Deserialize (file);
				file.Close ();
			}
			return resoult;
		}
	}
}

