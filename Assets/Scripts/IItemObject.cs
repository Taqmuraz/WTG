using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IItemObject : IUsable {

	public static List<IItemObject> itemObjectsAll = new List<IItemObject> ();

	public int indentification;

	private Rigidbody body;

	public SavebleItem CaptureItem () {
		SavebleItem s = new SavebleItem ();
		s.euler_y = trans.eulerAngles.y;
		s.name = "item_" + indentification;
		s.position = trans.position;
		s.id = indentification;
		return s;
	}

	private void Start () {
		body = GetComponent<Rigidbody> ();
		body.angularVelocity = Random.insideUnitSphere * Random.Range (5, 15);
		trans = transform;
		itemObjectsAll.Add (this);
		usablesAll.Add (this);
	}

	public void OnDestroy () {
		itemObjectsAll.Remove (this);
		IUsable.usablesAll.Remove (this);
	}

	public void SetWithID () {
		trans = transform;
		RawImage image = GetComponentInChildren<RawImage>();
		GameObject pref = (GameObject)Resources.Load ("Prefabs/Items/Item_" + indentification);
		GameObject obj = null;
		BoxCollider coll = GetComponent<BoxCollider> ();
		if (pref) {
			obj = GameObject.Instantiate (pref, trans);
			obj.transform.localEulerAngles -= Vector3.right * 90;
			image.enabled = false;
			coll.enabled = false;
			Renderer[] rends = obj.GetComponentsInChildren<Renderer> ();
			foreach (var r in rends) {
				r.gameObject.AddComponent<BoxCollider> ();
				r.gameObject.layer = Physics.IgnoreRaycastLayer;
			}
		} else {
			image.enabled = true;
			image.texture = ItemsAsset.LoadTexture (indentification);
		}
	}
}