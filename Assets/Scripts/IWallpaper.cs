using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IWallpaper : MonoBehaviour {

	public Texture[] wallpapers;

	public RawImage image;

	private void Start () {
		SetRandom ();
	}

	private void SetRandom () {
		int random = Random.Range (0, wallpapers.Length);
		SetTexture (wallpapers[random]);
	}
	private void SetTexture (Texture texture) {
		float k = 0;
		bool byWidth = !(texture.height < texture.width);

		float offset = 0;



		k = (float)Screen.width / (float)texture.width;
		RectTransform trans = image.rectTransform;

		if (byWidth) {
			offset = (Screen.height - texture.height * k) / 2;
		}

		trans.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, texture.width * k);
		trans.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, texture.height * k);

		trans.anchoredPosition = Vector2.up * offset;
		image.texture = texture;
	}
}