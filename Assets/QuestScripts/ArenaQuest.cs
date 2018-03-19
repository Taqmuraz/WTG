using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IQuest_Arena : IQuest
{
	public int enemy_index = 0;
	public int enemy_defeated_index = -1;
	public int cost_getted_index = -1;
}

public class ArenaQuest : MonoBehaviour {
	public IDoor[] vorota_mine;
	public IDoor[] vorota_enemy;

	public Transform enemyPoint;
	public Transform playerPoint;

	public Transform enemyStartPoint;
	public Transform playerStartPoint;

	public IQuest_Arena iquest
	{
		get {
			IQuest_Arena q = new IQuest_Arena ();
			q.available = false;
			for (int i = 0; i < IQuest.current.Length; i++) {
				if (IQuest.current[i] is IQuest_Arena && IQuest.current[i].available && !IQuest.current[i].did) {
					q = (IQuest_Arena)IQuest.current [i];
				}
			}
			return q;
		}
		set {
			for (int i = 0; i < IQuest.current.Length; i++) {
				if (IQuest.current[i] is IQuest_Arena) {
					IQuest.current [i] = value;
				}
			}
		}
	}

	public ICharacter[] enemies = new ICharacter[0];
	public string[] enemies_names;
	public int[] win_scores;

	public ICharacter main_character;
	public ICharacter director;
	public string directorName = "Daarch";

	private void Start () {
		if (iquest.did) {
			Destroy (this);
		}
	}
	private bool bothEntered = false;

	public void PrepareBattle () {
		main_character.agent.Warp (playerStartPoint.position);
		enemies[iquest.enemy_index].agent.Warp (enemyStartPoint.position);
		preparedToBattle = true;
	}

	public bool preparedToBattle = false;

	public void GetScores (int enemyIndex) {
		main_character.status.money += win_scores [enemyIndex];
		iquest.cost_getted_index = enemyIndex;
	}

	public void Win () {
		iquest.enemy_defeated_index = iquest.enemy_index;
		iquest.enemy_index++;
		preparedToBattle = false;
		main_character.AddMoney (win_scores [iquest.enemy_defeated_index]);
		iquest.cost_getted_index = iquest.enemy_defeated_index;
	}

	private void Update () {
		if (!main_character || !director) {
			main_character = ICharacter.GetPlayer ();
			director = ICharacter.GetByName (directorName);
			if (director) {
				director.status.dialogData.onAction = delegate {
					PrepareBattle();
				};
			}
		}
		if (main_character && iquest.available) {
			if (enemies.Length > 0) {
				if (iquest.enemy_index < enemies.Length) {
					if (preparedToBattle) {
						if (enemies [iquest.enemy_index]) {
							if ((enemies [iquest.enemy_index].trans.position - enemyPoint.position).magnitude > 10) {
								enemies [iquest.enemy_index].MoveTo (enemyPoint.position);
							} else {
								if ((main_character.trans.position - playerPoint.position).magnitude < 10) {
									enemies [iquest.enemy_index].status.reputationType = IReputationType.Monster;
									bothEntered = true;
								}
							}
						} else {
							if (iquest.cost_getted_index != iquest.enemy_index) {
								Win ();
							}
						}
					}
				}
			} else {
				ICharacter[] ch = new ICharacter[enemies_names.Length];
				for (int i = 0; i < ch.Length; i++) {
					ch [i] = ICharacter.GetByName (enemies_names [i]);
				}
				enemies = ch;
			}
		} else {
			main_character = ICharacter.GetPlayer ();
		}
		bool b = (main_character && iquest.enemy_index < enemies.Length && enemies[iquest.enemy_index]) && ((enemies [iquest.enemy_index].trans.position - enemyPoint.position).magnitude > 10 ||
			(main_character.trans.position - playerPoint.position).magnitude > 10);
		if (bothEntered) {
			if (!(iquest.enemy_index < enemies.Length) || !enemies[iquest.enemy_index] || b) {
				bothEntered = false;
			}
		}
		foreach (var v in vorota_mine) {
			v.data.locked = bothEntered;
			if (v.data.opened == bothEntered) {
				v.OpenOrClose ();
			}
		}
		foreach (var v in vorota_enemy) {
			v.data.locked = bothEntered;
			if (v.data.opened == bothEntered) {
				v.OpenOrClose ();
			}
		}
	}
}
