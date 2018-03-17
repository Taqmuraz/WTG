using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ISpace : MonoBehaviour {

	private static int level = 0;

	public static void LoadLevel (int index) {
		level = index;

		AsyncOperation op = SceneManager.LoadSceneAsync ("Space");

		GameObject loadIndicator = (GameObject)Resources.Load ("Prefabs/LoadStatus");
		ILoadStatus st = ((GameObject)Instantiate (loadIndicator)).GetComponent<ILoadStatus>();
		op.allowSceneActivation = true;
		op.priority = 15;

		st.loading = op;
	}
	public static void LoadMainMenu () {
		LoadLevel (0);
	}
	public static void LoadGameFromIndex (int gameIndex) {
		if (IGame.Exist(gameIndex)) {
			IGame.Load (gameIndex);
			IGame.currentProfile = gameIndex;
			ISpace.LoadLevel (3);
		}
	}

	private void Start () {
		Time.timeScale = 1;
	}

	private void Update () {
		time += Time.deltaTime;
		if (time > 2 && !loaded) {
			Load ();
			loaded = true;
		}
	}
	private bool loaded = false;
	private float time = 0;

	private void Load () {
		AsyncOperation op = SceneManager.LoadSceneAsync (level);
		GameObject loadIndicator = (GameObject)Resources.Load ("Prefabs/LoadStatus");
		ILoadStatus st = ((GameObject)Instantiate (loadIndicator)).GetComponent<ILoadStatus>();
		op.priority = 15;
		op.allowSceneActivation = true;
		st.loading = op;
	}
}