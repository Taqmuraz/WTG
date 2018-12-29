using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPG_System;
using RPG_Data;
using RPG_Editor;

public class GameDirector : MonoBehaviour
{
	public virtual void Start ()
	{
		GameData game = new GameData (new ObjectData[0]);
		GenerateForGame (game);
		game.InstallGame ();
	}
	protected virtual void GenerateForGame (GameData game)
	{
		ObjectDataEditor[] editPoints = ObjectDataEditor.FindObjectsOfType<ObjectDataEditor> ();
		ObjectData[] datas = editPoints.Select ((ObjectDataEditor ep) => ep.GenerateData ()).ToArray();
		game.AddObjects (datas);
	}
}
