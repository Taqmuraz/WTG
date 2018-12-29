using System;
using UnityEngine;
using RPG_Data;
using System.Linq;
using System.Collections.Generic;

namespace RPG_System
{
	public class TextManager
	{
		public const string TextsPath = "Texts/";
		public const string DialogsPath = "Dialogs/";
		public const string ItemsPath = "Items/";

		static Dictionary<string, TextAsset> assets = new Dictionary<string, TextAsset> ();

		public static string Read (string path)
		{
			bool contains = assets.ContainsKey (path);
			TextAsset asset = contains ? assets [path] : ResourcesManager.LoadSource<TextAsset> (path);
			if (!contains) {
				assets.Add (path, asset);
			}
			return asset.text;
		}
	}
	
}
