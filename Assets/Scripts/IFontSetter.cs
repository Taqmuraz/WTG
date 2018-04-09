using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DebugMessage
{
	public string message;
	public IColor color;

	public DebugMessage (string msg, Color clr) {
		message = msg;
		color = clr;
	}
}

public class MyDebug
{
	public static List<DebugMessage> messages = new List<DebugMessage> ();
	public static bool haveToUpdate = true;
	public static void Log (string log, Color color, ICharacter obj) {
		string a = "(" + obj.status.characterName + ") ";
		messages.Add (new DebugMessage(a + log, color));
		if (messages.Count > 3) {
			messages.RemoveAt (0);
		}
		haveToUpdate = true;
	}
}

public class IFontSetter : MonoBehaviour {

	public static Font font
	{
		get
		{
			return (Font)Resources.Load ("Font/font");
		}
	}
	public static int fontScale
	{
		get {
			return IMainMenu.settings.fontSize;
		}
	}

	private static IFontSetter setter;

	private void FontUpdate () {
		SetFontForall ();
	}

	public static void SetFont (Text t) {
		t.font = font;
		t.fontSize = fontScale;
	}

	public static void SetFontForall () {
		Text[] texts = Resources.FindObjectsOfTypeAll<Text> ();

		for (int i = 0; i < texts.Length; i++) {
			SetFont (texts [i]);
		}

		if (!setter) {
			GameObject sobj = new GameObject ();
			sobj.name = "FontSetter";
			sobj.AddComponent<IFontSetter> ();
			setter = sobj.GetComponent<IFontSetter> ();
		}
	}
}
