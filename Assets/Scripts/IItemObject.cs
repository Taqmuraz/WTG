using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IItemObject : IUsable {

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
	}

	public void SetWithID () {
		trans = transform;
		RawImage image = GetComponentInChildren<RawImage>();
		image.texture = IItemAsset.LoadTexture (indentification);
	}
}