using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IItemObject : IUsable {

	public static List<IItemObject> itemObjectsAll = new List<IItemObject> ();

	public int indentification;

	public ISavableItem CaptureItem () {
		ISavableItem s = new ISavableItem ();
		s.euler_y = trans.eulerAngles.y;
		s.name = "item_" + indentification;
		s.position = trans.position;
		s.id = indentification;
		return s;
	}

	private void Start () {
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
		image.texture = IItemAsset.LoadTexture (indentification);
	}
}