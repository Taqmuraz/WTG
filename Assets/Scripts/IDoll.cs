using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class IDoll : MonoBehaviour {

	public Renderer rend;

	public Transform parentPrefab;
	public GameObject doll;
	Transform doll_trans;

	float euler = 0;

	private void Update () {
		if (doll_trans) {
			doll_trans.localEulerAngles = Vector3.up * euler;
			euler += Time.deltaTime * 45;
		}
	}

	public void Set (Status status) {

		if (doll) {
			Destroy (doll.gameObject);
		}

		GameObject source = (GameObject)Resources.Load ("Prefabs/Players_new/" + status.iRace);
		doll = (GameObject)Instantiate (source, parentPrefab);

		doll_trans = doll.transform;

		doll_trans.position = Vector3.zero;
		doll_trans.rotation = Quaternion.Euler(Vector3.up * euler);

		Destroy (doll.GetComponent<ICharacter> ());
		Destroy (doll.GetComponent<NavMeshAgent> ());

		doll.GetComponentInChildren<RawImage> ().texture = null;

		rend = doll_trans.GetComponentInChildren<SkinnedMeshRenderer> ();

		Renderer[] rends = GetComponentsInChildren<SkinnedMeshRenderer> ();

		if (rends.Length > 2) {
			PersonView.SetToManyRenderer (status, rends);
		} else {
			PersonView.SetToRenderer (status, rend);
		}
	}
}