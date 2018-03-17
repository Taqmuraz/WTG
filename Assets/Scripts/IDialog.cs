using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class IDialogTransition
{
	public int words_index = 0;
	public int nextNode = 0;
}

[System.Serializable]
public class IDialogNode
{
	public int npc_words = 0;
	public IDialogTransition[] answersTr;

	public string[] answers
	{
		get {

			string[] a = new string[answersTr.Length];

			for (int i = 0; i < answersTr.Length; i++) {
				a [i] = GetAnswerFromIndex (answersTr [i].words_index);
			}

			return a;
		}
	}
	public string npc_text
	{
		get {
			return GetQuestionFromIndex (npc_words);
		}
	}

	public static string GetAnswerFromIndex (int index) {
		TextAsset tas = (TextAsset)Resources.LoadAll ("Text/Dialogs/") [0];


		string text = tas.text;

		int currentLine = 0;

		int currentSym = 0;

		while (currentLine < index) {

			while (text[currentSym] != '\n') {
				currentSym++;
			}

			currentLine++;
			currentSym++;
		}
		string toReturn = "";

		while (currentSym < text.Length && text[currentSym] != '\n') {
			toReturn = toReturn + text [currentSym];
			currentSym++;
		}

		return toReturn;
	}
	public static string GetQuestionFromIndex (int index) {
		TextAsset tas = (TextAsset)Resources.LoadAll ("Text/Dialogs/") [1];

		string text = tas.text;

		int currentLine = 0;

		int currentSym = 0;

		while (currentLine < index) {

			while (text[currentSym] != '\n') {
				currentSym++;
			}

			currentLine++;
			currentSym++;
		}

		string toReturn = "";

		while (currentSym < text.Length && text[currentSym] != '\n') {
			toReturn = toReturn + text [currentSym];
			currentSym++;
		}
		return toReturn;
	}
}

[System.Serializable]
public class IDialog
{
	public string start_text = "";
	public IDialogNode[] nodes;
	public int current_index = 0;
	public IDialogNode currentNode
	{
		get {
			return nodes [current_index];
		}
	}

	public delegate void SpetialAction ();
	public SpetialAction onAction;

	public bool has_ended = false;

	public string CompareToString () {
		string st = "";

		return st;
	}
}
[System.Serializable]
public class IDialogWindow
{
	public IDialog dialog
	{
		get {
			return with.status.dialogData;
		}
	}

	public ICharacter with;

	public RectTransform parent;

	public IControl control;

	public Text for_answer;

	public void InitializeNode () {
		if (dialog.current_index < 0 || dialog.nodes.Length < 1) {
			dialog.has_ended = true;
			control.OutDialog ();
			switch (dialog.current_index) {
			case -2:
				// to trade menu
				break;
			case -3:
				//spetialAction
				dialog.onAction.Invoke ();
				break;
			}
			dialog.current_index = 0;
		} else {
			Button[] bts = parent.GetComponentsInChildren<Button> ();
			for (int i = 0; i < bts.Length; i++) {
				GameObject.Destroy (bts [i].gameObject);
			}
			float end_Scale = 0;
			Button[] bN = new Button[dialog.currentNode.answers.Length];
			for (int i = 0; i < dialog.currentNode.answers.Length; i++) {
				Button bt = null;
				GameObject bPref = (GameObject)Resources.Load ("Prefabs/DialogSlot");
				bt = Button.Instantiate (bPref, parent).GetComponent<Button> ();
				RectTransform trans = bt.GetComponent<RectTransform> ();
				trans.anchoredPosition = -Vector2.up * end_Scale + new Vector2(10, -10);
				Text t = bt.GetComponent<Text> ();
				IFontSetter.SetFont (t);
				IControl.SetTextWithScales (t, dialog.currentNode.answers [i]);
				float scale = trans.rect.height;
				end_Scale += scale + 15;
				bt.onClick.RemoveAllListeners ();
				int node_next = dialog.currentNode.answersTr[i].nextNode;
				bt.onClick.AddListener (delegate {

					dialog.current_index = node_next;
					InitializeNode();

				});
				bN [i] = bt;
			}
			parent.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, end_Scale + 10);
			IFontSetter.SetFont (for_answer);
			IControl.SetTextWithScales (for_answer, dialog.currentNode.npc_text);
		}
	}
}