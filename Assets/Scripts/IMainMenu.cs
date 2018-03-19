using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public enum MainMenuState
{
	Main,
	New,
	Load,
	Opts,
	Info
}

public class IMainMenu : MonoBehaviour {
	public Button[] stateControls;
	public Button[] backControls;
	public GameObject[] states;
	public MainMenuState state;
	public Button[] startNew;
	public Button[] loadGame;
	public Button[] removeGame;
	public Button removeButton;
	public Button cancelRemoveButton;
	public GameObject removeMenu;
	public Scrollbar fontSize;


	private void Start () {
		IFontSetter.SetFontForall ();
		PrepareToStart ();
		SetState ();
		LoadSettings ();
		SetQualitySettings (settings.quality);
		float v = (settings.fontSize - 11) / 15f;
		fontSize.value = v;
		fontSize.onValueChanged.Invoke (v);
	}

	private void LoadCreateScene (int index) {
		ICreateMenu.slotIndex = index;
		ISpace.LoadLevel (1);
	}
	private void LoadGameFromSlot (int index) {
		if (IGame.Exist(index)) {
			ISpace.LoadGameFromIndex (index);
		}
	}

	private void OpenRemoveMenu () {
		removeMenu.SetActive (true);
	}
	private void CloseRemoveMenu () {
		removeMenu.SetActive (false);
	}
	private void RemoveGame () {
		IGame.Remove (toRemoveIndex);
		toRemoveIndex = -1;
	}
	private int toRemoveIndex = -1;

	private void PrepareToStart () {

		fontSize.onValueChanged.RemoveAllListeners ();
		fontSize.onValueChanged.AddListener (delegate {
			int v = 11 + (int)(fontSize.value * 15);
			fontSize.GetComponentInChildren<Text>().text = "Размер шрифта : " + v;
			settings.fontSize = v;
			IFontSetter.SetFontForall();
		});

		removeButton.onClick.RemoveAllListeners ();
		removeButton.onClick.AddListener (delegate {
			RemoveGame();
			CloseRemoveMenu();
			Start();
		});

		cancelRemoveButton.onClick.RemoveAllListeners ();
		cancelRemoveButton.onClick.AddListener (delegate {
			CloseRemoveMenu();
		});

		for (int i = 0; i < removeGame.Length; i++) {
			int index = i;
			removeGame [i].onClick.RemoveAllListeners ();
			removeGame [i].onClick.AddListener (delegate {

				toRemoveIndex = index;
				OpenRemoveMenu();

			}
			);
			removeGame [i].gameObject.SetActive (IGame.Exist(index));
		}

		for (int i = 0; i < loadGame.Length; i++) {

			loadGame[i].onClick.RemoveAllListeners();
			int index = i;
			loadGame[i].onClick.AddListener(delegate {

				LoadGameFromSlot(index);

			});

			string st = "> Пусто <";

			if (IGame.Exist(i)) {
				IGame loaded = IGame.Load (i);
				st = ((IStatus)loaded.FindByName ("Player")).characterName + " : " + loaded.date.ToLongDateString() + " " + loaded.date.ToLongTimeString();
			}

			RawImage img = loadGame [i].GetComponentInChildren<RawImage> ();
			if (IGame.Exist (i)) {
				img.texture = IGame.LoadPicture (i);
			} else {
				img.texture = IItemAsset.LoadTexture (-1);
			}

			loadGame [i].GetComponentInChildren<Text> ().text = st;

		}

		for (int i = 0; i < stateControls.Length; i++) {
			stateControls[i].onClick.RemoveAllListeners();
			int index = i;
			stateControls[i].onClick.AddListener(delegate {

				state = (MainMenuState)index;
				SetState();

			});
		}
		for (int i = 0; i < startNew.Length; i++) {
			string text = "Новый персонаж";
			startNew[i].onClick.RemoveAllListeners();
			if (IGame.Exist (i)) {
				text = "Слот занят";
			} else {
				int index = i;
				startNew[i].onClick.AddListener(delegate {

					LoadCreateScene(index);

				});
			}
			startNew [i].GetComponentInChildren<Text> ().text = text;
		}
		for (int i = 0; i < backControls.Length; i++) {
			backControls[i].onClick.RemoveAllListeners();
			backControls[i].onClick.AddListener(delegate {

				if (state == MainMenuState.Opts) {
					SaveSettings();
				}
				state = MainMenuState.Main;
				SetState();
				
			});
		}
	}

	private void SetState () {
		for (int i = 0; i < states.Length; i++) {
			states[i].SetActive((MainMenuState)i == state);
		}
		IFontSetter.SetFontForall ();
	}

	public void Quit () {
		Application.Quit ();
	}

	public void SetQualitySettings (int index) {
		QualitySettings.SetQualityLevel (index);
		settings.quality = index;
		SaveSettings ();
	}
	public static Settings settings = new Settings();
	public void SaveSettings () {
		string path = Application.persistentDataPath + "/settings.set";
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (path);
		bf.Serialize (file, settings);
		file.Close ();
	}
	public Settings LoadSettings () {
		string path = Application.persistentDataPath + "/settings.set";
		Settings toReturn = new Settings ();
		if (File.Exists(path)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);
			toReturn = (Settings)bf.Deserialize (file);
			file.Close ();
		}
		settings = toReturn;
		return toReturn;
	}
}