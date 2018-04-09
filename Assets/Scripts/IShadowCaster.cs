using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IShadowCaster : MonoBehaviour {

	public Projector projector;

	private IRenderCamera renderCamera;

	private void Start () {

		projector = Instantiate ((GameObject)Resources.Load ("Prefabs/Shadow")).GetComponent<Projector> ();
		GameObject cam = new GameObject ("CamRend<" + name + ">", typeof(Camera), typeof(IRenderCamera));
		renderCamera = cam.GetComponent<IRenderCamera> ();
		renderCamera.Start ();
		renderCamera.cam.depth = -1;
		renderCamera.cam.orthographic = true;
		renderCamera.cam.orthographicSize = 1;
		renderCamera.cam.clearFlags = CameraClearFlags.SolidColor;
		renderCamera.cam.backgroundColor = Color.white;
		renderCamera.target = transform;
		renderCamera.cam.farClipPlane = 4;
		RenderTexture t = new RenderTexture (128, 128, 16);
		t.name = "RenderTexture<" + name + ">";
		renderCamera.targetTexture = t;
		renderCamera.cam.targetTexture = t;
		renderCamera.cam.nearClipPlane = 0.1f;
		renderCamera.cam.cullingMask = LayerMask.GetMask ("Char");
		renderCamera.targetProjector = projector.transform;

		InvokeRepeating ("ShadowUpdate", 0, 0.1f);
	}

	private void ShadowUpdate () {
		if (renderCamera.texture) {
			Texture tx = ToShadow (renderCamera.texture);
			projector.material.SetTexture (280, tx);
		}
	}

	public static Transform sunGetter;
	public static Transform sun
	{
		get
		{
			if (!sunGetter) {
				sunGetter = FindObjectsOfType<Light> ().Where (((Light arg) => arg.type == LightType.Directional)).ToArray () [0].transform;
			}
			return sunGetter;
		}
	}
	public static Vector3 sunEuler
	{
		get {
			Vector3 euler = Vector3.zero;
			if (sun) {
				euler = sun.eulerAngles;
			}
			return euler;
		}
	}

	public static Texture2D ToShadow (Texture2D origin) {
		Color[] cls = origin.GetPixels ();
		for (int i = 0; i < cls.Length; i++) {
			if (cls [i].Equals (Color.white)) {
				cls [i] = Color.white;
			} else {
				cls [i] = new Color (0.4f, 0.4f, 0.4f, 1);
			}
		}
		Texture2D tex = new Texture2D (origin.width, origin.height);
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.SetPixels (cls);
		Color a = Color.white;
		tex.Apply ();
		return tex;
	}
}
