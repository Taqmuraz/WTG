using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IScreenshotCamera : MonoBehaviour {

	public static bool captureShot = false;
	public Camera cam;
	public static IScreenshotCamera screenCamera;

	private void Start () {
		cam = Camera.main;
		screenCamera = this;
	}

	private void OnPostRender () {
		if (captureShot) {
			int w = Screen.currentResolution.width;
			int h = Screen.currentResolution.height;
			Rect r = new Rect (0, 0, w, h);
			Texture2D tex = new Texture2D((int)r.width, (int)r.height);
			try {
				tex.ReadPixels (r, 0, 0);
				tex.Apply ();
			} catch (System.Exception ex) {
				Debug.Log (ex.Message + '\n' + "At resolution : " + w + ", " + h + ";");
			}
			SGame.screenshot = tex.EncodeToPNG ();
			captureShot = false;
		}
	}
	private void OnPreRender () {
		IControl.control.CameraMotor ();
	}

	public void Screenshot () {
		captureShot = true;
		cam.Render ();
	}
}
