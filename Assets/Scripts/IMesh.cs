using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IMesh : MonoBehaviour {

	public Texture2D texture;
	public RawImage rend;

	void Update () {
		if (!texture) {
			texture = new Texture2D (Screen.currentResolution.width, Screen.currentResolution.height);
			texture.ReadPixels (new Rect(0, 0, Screen.currentResolution.width,
				Screen.height), 0, 0);
			texture.Apply ();

			rend.texture = texture;
		}
	}
}