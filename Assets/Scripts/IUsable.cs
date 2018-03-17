using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IUsable : MonoBehaviour {

	public Transform trans;

	private void Start () {
		trans = transform;
	}

	public static IUsable GetNearestToPosition (Vector3 point) {
		IUsable[] usables = IUsable.FindObjectsOfType<IUsable> ();
		IUsable usable = null;

		float dist = 2;

		int index = -1;

		for (int i = 0; i < usables.Length; i++) {
			float d = (usables[i].trans.position - point).magnitude;
			if (d < dist) {
				dist = d;
				index = i;
			}
		}
		if (index > -1) {
			usable = usables [index];
		}

		return usable;
	}
}