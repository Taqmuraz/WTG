using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRenderCamera : MonoBehaviour {

	public Camera cam;

	public Texture2D texture;

	public Transform target;

	public RenderTexture targetTexture;

	private Transform trans;

	public Transform targetProjector;

	public void Start () {
		cam = GetComponent<Camera> ();
		trans = transform;
		trans.eulerAngles = IShadowCaster.sunEuler;
	}

	private void OnPostRender () {
		/*if (captureShot) {
			int w = cam.pixelWidth;
			int h = cam.pixelHeight;
			Rect r = new Rect (0, 0, w, h);
			Texture2D tex = new Texture2D((int)r.width, (int)r.height);
			tex.ReadPixels (r, 0, 0);
			tex.Apply ();
			texture = tex;
			texture.Apply ();
			captureShot = false;
			*/
		if (target) {
			trans.eulerAngles = IShadowCaster.sunEuler;
			trans.position = target.position + Vector3.up - trans.forward * 2;
			targetProjector.position = target.position + Vector3.up;
			targetProjector.eulerAngles = IShadowCaster.sunEuler;
		}
		if (!texture) {
			texture = new Texture2D (targetTexture.width, targetTexture.height);
		}
		int w = cam.pixelWidth;
		int h = cam.pixelHeight;
		Rect r = new Rect (0, 0, w, h);
		texture.ReadPixels (r, 0, 0);
		texture.Apply ();
	}
}
