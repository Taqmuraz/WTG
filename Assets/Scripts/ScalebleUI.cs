using UnityEngine;
using System.Collections;

public class ScalebleUI : MonoBehaviour
{

	public static Vector2 startDiagonal = new Vector2 (854f, 480f);

	public static float resolutionSize
	{
		get {
			Vector2 sc = new Vector2 (Screen.width, Screen.height);
			float k = sc.magnitude / startDiagonal.magnitude;
			return k;
		}
	}

	public RectTransform trans { get; private set; }
	public RectTransform canvas { get; private set; }

	[SerializeField]
	private bool x = true;
	[SerializeField]
	private bool y = true;

	private void Start () {
		SetWith ();
	}
	public void SetWith () {
		trans = GetComponent<RectTransform> ();
		canvas = GetComponentInParent<Canvas> ().GetComponent<RectTransform> ();
		float w = trans.rect.width;
		float h = trans.rect.height;
		float k = resolutionSize;
		if (x) {
			trans.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, w * k);
		}
		if (y) {
			trans.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, h * k);
		}
	}
}

