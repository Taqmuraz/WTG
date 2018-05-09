using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITextureDrawer {

	public static bool ColorsNearby (Color a, Color c) {
		float r = Mathf.Abs (a.r - c.r);
		float g = Mathf.Abs (a.g - c.g);
		float b = Mathf.Abs (a.b - c.b);
		float al = Mathf.Abs (a.a - c.a);
		float sum = (r + g + b + al);
		return sum < 1.25f;
	}

	public enum ColorType
	{
		Red,
		Green,
		White,
		Black,
		None
	}

	public static ColorType GetColorType (Color c) {
		bool white = c.r + c.g + c.b > 0.8f * 3;
		bool black = c.r + c.g + c.b < 0.6f;
		bool red = c.r > c.b && c.r > c.g;
		bool green = c.g > c.b && c.g > c.r;

		if (c.a > 0.1f) {
			if (white) {
				return ColorType.White;
			}
			if (black) {
				return ColorType.Black;
			}
			if (red) {
				return ColorType.Red;
			}
			if (green) {
				return ColorType.Green;
			}
		}

		return ColorType.None;
	}

	public static Texture2D GetFromPerson (Status status) {
		Texture2D tex = (Texture2D)Resources.Load ("Sprites/Icons/" + status.iRace);
		Texture2D toReturn = new Texture2D (tex.width, tex.height);
		Color[] colors = tex.GetPixels ();
		Color[] update = new Color[colors.Length];

		for (int i = 0; i < colors.Length; i++) {
			Color c = colors [i];

			update [i] = colors [i];

			ColorType t = GetColorType (c);

			switch (t) {
			case ColorType.Black:
				update [i] = status.iPerson.hair_color;
				break;
			case ColorType.White:
				update [i] = status.iPerson.skin_color;
				break;
			case ColorType.Green:
				update [i] = status.iPerson.cloth_color;
				break;
			case ColorType.Red:
				update [i] = status.iPerson.cloth_more_color;
				break;
			}

		}

		toReturn.filterMode = FilterMode.Point;

		toReturn.SetPixels (update);
		toReturn.Apply ();
		return toReturn;
	}
}
