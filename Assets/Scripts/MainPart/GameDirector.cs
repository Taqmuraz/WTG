using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG_System;
using RPG_Data;

public class GameDirector : MonoBehaviour
{
	public virtual void Start ()
	{
		/*
		string id = "Player";
		CharacterRace race = new CharacterRace (CharacterRaceEnum.Elf, id);
		CharacterParameter[] prms = { new InventoryCParameter(id), race };
		CharacterData ch = new CharacterData ("Player", race.race.ToString (), prms);
		GameData game = new GameData (new ObjectData[1] { ch });

		FileManager.WriteFile (game, "Experiment.exp");
		*/

		GameData game = FileManager.ReadFile<GameData> ("Experiment.exp");

		game.InstallGame ();
	}
}
