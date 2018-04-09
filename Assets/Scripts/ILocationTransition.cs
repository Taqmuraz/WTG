using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ILocationTransition : IUsable {

	public static void TransitLocation (string name) {
		IGame.buffer.MoveToLocation (name);
		int index = SceneUtility.GetBuildIndexByScenePath ("Assets/Scenes/Locations/" + name + ".unity");
		ISpace.LoadLevel (index);
	}


	private void OnDestroy () {
		IUsable.usablesAll.Remove (this);
	}

	public string nextLevel = "Arena";

	public void Use (ICharacter ch) {
		if (ch.isPlayer) {
			TransitLocation (nextLevel);
		} else {
			ch.status.MoveToLocation (ch.gameObject, nextLevel);
		}
	}

	private void Start () {
		trans = transform;
		usablesAll.Add (this);
	}
}
