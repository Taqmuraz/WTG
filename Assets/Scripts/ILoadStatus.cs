using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ILoadStatus : MonoBehaviour {

	public Text text;
	public AsyncOperation loading;

	private void Start () {
		text.fontSize = IFontSetter.fontScale;
		text.font = IFontSetter.font;
	}

	private void Update () {
		text.text = "Загрузка..." + ((int)(loading.progress * 100)) + " %";
	}
}
