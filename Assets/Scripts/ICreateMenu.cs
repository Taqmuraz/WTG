using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public enum CreateMenuState
{
	Race,
	Class,
	Params,
	Colors
}

[System.Serializable]
public class ChangeParameterSlot
{
	public string info = "";
	public Text text;
	public Button plus;
	public Button minus;
}

public class ICreateMenu : MonoBehaviour {

	public Text classInfo;
	public Text raceInfo;
	public Text paramsInfo;

	private CreateMenuState state_get;
	public CreateMenuState state
	{
		get {
			return state_get;
		}
		set {
			state_get = value;
			SetState ();
		}
	}

	public Status status = new Status (ClassType.Simple);

	public static int slotIndex = 0;

	public Button[] races;
	public Button[] classes;
	public GameObject[] state_objs;
	public TextAsset[] racesTexts;
	public TextAsset[] classesTexts;
	//public Scrollbar scroll;

	public Color[] colorArray;
	public Color[] skinColorsArray;
	public Color[] hairColorArray;

	public GameObject clothOneLine;
	public GameObject clothtwoLine;
	public GameObject hairLine;
	public GameObject skinLine;

	public Image clothShow;
	public Image clothTwoShow;
	public Image skinShow;
	public Image hairShow;

	public Button next;
	public Button back;

	public Button gender_male;
	public Button gender_female;
	public Status.Gender gender;
	public GameObject genderMenu;
	public GameObject nameMenu;
	public GameObject mainMenu;
	public GameObject raceMaleButtons;
	public GameObject raceFemaleButtons;

	public InputField field;

	public RawImage dollView;

	public GameObject StartGameButton;

	public ChangeParameterSlot[] slots;

	private void Start () {
		doll = IDoll.FindObjectOfType<IDoll> ();
		IFontSetter.SetFontForall ();
		PrepareForStart ();


		SetMaybeClasses ();

		DollUpdate ();

		SetState ();
	}
	private void Update () {
		if (status.iType == ClassType.Simple) {
			status.iType = ClassType.Monk;
		}
		string skillsInfo = "" + '\n' + '\n' + "Особые умения : ";
		Skill[] skills = status.skills.forMyClass;
		foreach (var item in skills) {
			skillsInfo = "" + '\n' + '\n' + skillsInfo + item.Info ();
		}
		IControl.SetTextWithScales (classInfo, classesTexts [(int)status.iType - 1].text + skillsInfo);
		Status n = new Status (ClassType.Simple);
		n.iRace = status.iRace;
		IControl.SetTextWithScales (raceInfo, racesTexts [(int)status.iRace].text + '\n' + '\n' + n.immunity.ToText ());

		dollView.texture = (Texture)Resources.Load ("Runtime/RenderTextures/SelectionCamera");

		string pinfo = "Доступные очки : " + paramsPoints + '\n';
		int[] ints = GetParamsInInts ();
		int[] maxs = GetMaximum (status.iType);
		for (int i = 0; i < slots.Length; i++) {
			//.pinfo = pinfo + slots[i].text.text + " : " + ints[i] + "[1..." + maxs[i] + "]" + '\n';

			slots [i].text.text = slots[i].info + " = " + ints [i] + " [ 1 . . . " + maxs [i] + " ]";
		}
		paramsInfo.text = pinfo;
		//float delta = Mathf.Abs((localSize - globalSize).y);
		//scroll.size = 1 - delta / localSize.y;
		//localField.anchoredPosition = Vector3.up * scroll.value * delta;

		clothShow.color = status.iPerson.cloth_color;
		hairShow.color = status.iPerson.hair_color;
		skinShow.color = status.iPerson.skin_color;
		clothTwoShow.color = status.iPerson.cloth_more_color;
	}
	private void SetState () {
		for (int i = 0; i < state_objs.Length; i++) {
			state_objs[i].SetActive((CreateMenuState)i == state);
		}
		IFontSetter.SetFontForall ();
	}
	public void SetStateGlobal (int st) {
		if (st < 0) {
			mainMenu.SetActive (false);
			nameMenu.SetActive (false);
			genderMenu.SetActive (true);
			return;
		}
		if (st < System.Enum.GetNames (typeof (CreateMenuState)).Length) {
			state = (CreateMenuState)st;
		} else {
			mainMenu.SetActive (false);
			nameMenu.SetActive (true);
		}
	}

	public void SaveAndPlay () {
		SGame.SetGameAsNew ();
		//status.items = IItemAsset.GetAllInventory ();
		status.euler_y = 180f;
		status.position = new SVector (10, 0, 10);
		status.items = ItemsAsset.GetStartKit (status);
		status.SetSpellsTodayBy ();
		SGame.buffer.currentLocation.objects [0] = status;
		SGame.currentProfile = slotIndex;
		ISpace.LoadLevel (3);
	}
	private void SetGender (Status.Gender g) {
		gender = g;
		raceFemaleButtons.SetActive (g == Status.Gender.Female);
		raceMaleButtons.SetActive (g == Status.Gender.Male);
		genderMenu.SetActive (false);
		mainMenu.SetActive (true);
		switch (gender) {
		case Status.Gender.Female:
			status.iRace = Race.Angel;
			break;
		case Status.Gender.Male:
			status.iRace = Race.Human;
			break;
		}
		DollUpdate ();
	}
	private void PrepareForStart () {

		status.name = "Player";

		next.onClick.RemoveAllListeners ();
		next.onClick.AddListener (delegate {
			SetStateGlobal(((int)state) + 1);
		});
		back.onClick.RemoveAllListeners ();
		back.onClick.AddListener (delegate {
			SetStateGlobal(((int)state) - 1);
		});

		field.onEndEdit.RemoveAllListeners ();
		field.onEndEdit.AddListener (delegate {

			if (field.text.Length > 0) {
				status.characterName = field.text;
				StartGameButton.SetActive(true);
				field.gameObject.SetActive(false);
			}

		});

		state_objs [2].SetActive (true);


		Button[] bts = clothOneLine.GetComponentsInChildren<Button>();

		for (int i = 0; i < bts.Length; i++) {
			bts[i].onClick.RemoveAllListeners();

			bts[i].image.color = colorArray[i];

			int index = i;

			bts[i].onClick.AddListener(delegate {


				status.iPerson.cloth_color = colorArray[index];

				DollUpdate();

			});
		}

		bts = clothtwoLine.GetComponentsInChildren<Button>();
		
		for (int i = 0; i < bts.Length; i++) {
			bts[i].onClick.RemoveAllListeners();
			
			bts[i].image.color = colorArray[i];
			
			int index = i;
			
			bts[i].onClick.AddListener(delegate {


				status.iPerson.cloth_more_color = colorArray[index];

				DollUpdate();
				
			});
		}

		gender_female.onClick.RemoveAllListeners ();
		gender_female.onClick.AddListener (delegate {

			SetGender(Status.Gender.Female);

		});

		gender_male.onClick.RemoveAllListeners ();
		gender_male.onClick.AddListener (delegate {

			SetGender(Status.Gender.Male);

		});

		bts = skinLine.GetComponentsInChildren<Button>();
		
		for (int i = 0; i < bts.Length; i++) {
			bts[i].onClick.RemoveAllListeners();
			
			bts[i].image.color = skinColorsArray[i];
			
			int index = i;
			
			bts[i].onClick.AddListener(delegate {


				status.iPerson.skin_color = skinColorsArray[index];

				DollUpdate();
				
			});
		}
		bts = hairLine.GetComponentsInChildren<Button>();
		
		for (int i = 0; i < bts.Length; i++) {
			bts[i].onClick.RemoveAllListeners();
			
			bts[i].image.color = hairColorArray[i];
			
			int index = i;
			
			bts[i].onClick.AddListener(delegate {


				status.iPerson.hair_color = hairColorArray[index];

				DollUpdate();
				
			});
		}


		state_objs [2].SetActive (false);

		for (int i = 0; i < races.Length; i++) {
			races[i].onClick.RemoveAllListeners();
			Race cur = (Race)i;
			races[i].onClick.AddListener(delegate {


				status.iRace = cur;
				SetMaybeClasses();
				DollUpdate();

			});
		}
		for (int i = 0; i < classes.Length; i++) {
			classes[i].onClick.RemoveAllListeners();
			ClassType cur = (ClassType)i + 1;
			classes[i].onClick.AddListener(delegate {
				
				status.iType = cur;
				ResetParams();
				
			});
		}

		for (int i = 0; i < slots.Length; i++) {
			slots[i].minus.onClick.RemoveAllListeners();
			slots[i].plus.onClick.RemoveAllListeners();
			int index = i;
			slots[i].minus.onClick.AddListener(delegate {
				
				ChangeParameter(index, -1);
				
			});
			slots[i].plus.onClick.AddListener(delegate {
				
				ChangeParameter(index, 1);
				
			});
		}

		status.iType = ClassType.Monk;

		status.iPerson.cloth_color = colorArray [0];
		status.iPerson.skin_color = skinColorsArray [0];
		status.iPerson.hair_color = hairColorArray [1];
		status.iPerson.cloth_more_color = colorArray [1];

		ResetParams ();

		SetMaybeClasses ();

		DollUpdate ();

		SetState ();
	}

	private IDoll doll;

	private void SetMaybeClasses () {
		for (int i = 0; i < classes.Length; i++) {
			if (!Status.MayBeNow (status.iRace, (ClassType)i + 1)) {
				classes [i].enabled = false;
				classes [i].image.color = new Color(0.3f, 0.3f, 0.3f, 1);
			} else {
				classes [i].enabled = true;
				classes [i].image.color = new Color(0.5f, 0.5f, 0.5f, 1);
			}
		}

		ClassType[] may = Status.MayBe (status.iRace);

		if (!Status.MayBeNow(status.iRace, status.iType)) {
			status.iType = may [0];
			ResetParams ();
		}
	}

	private void DollUpdate () {
		if (!doll) {
			doll = IDoll.FindObjectOfType<IDoll> ();
		}

		doll.Set (status);
	}

	public void ChangeParameter (int index, int count) {
		int[] ints = GetParamsInInts ();

		count = Mathf.Clamp (count, -ints[index], paramsPoints);
		int result = ints [index] + count;
		int[] maxs = GetMaximum (status.iType);
		if (result > 0 && !(result > GetMaximum(status.iType)[index])) {
			ints [index] += count;
			status.strongness = Mathf.Clamp(ints [0], 1, maxs[0]);
			status.wisdom = Mathf.Clamp(ints [1], 1, maxs[1]);
			status.intellect = Mathf.Clamp(ints [2], 1, maxs[2]);
			status.invicibility = Mathf.Clamp(ints [3], 1, maxs[3]);
			status.personability = Mathf.Clamp(ints [4], 1, maxs[4]);
			status.speedness = Mathf.Clamp(ints [5], 1, maxs[5]);
			
			paramsPoints -= count;
		}
	}
	public int[] GetParamsInInts () {
		int[] ints = {status.strongness,
			status.wisdom,
			status.intellect,
			status.invicibility,
			status.personability,
			status.speedness};
		return ints;
	}
	public int paramsPoints = 5;
	private void ResetParams () {
		status.strongness = 5;
		status.wisdom = 5;
		status.intellect = 5;
		status.invicibility = 5;
		status.personability = 5;
		status.speedness = 5;

		int[] ints = GetParamsInInts ();
		int[] maxs = GetMaximum (status.iType);

		status.strongness = Mathf.Clamp(ints [0], 1, maxs[0]);
		status.wisdom = Mathf.Clamp(ints [1], 1, maxs[1]);
		status.intellect = Mathf.Clamp(ints [2], 1, maxs[2]);
		status.invicibility = Mathf.Clamp(ints [3], 1, maxs[3]);
		status.personability = Mathf.Clamp(ints [4], 1, maxs[4]);
		status.speedness = Mathf.Clamp(ints [5], 1, maxs[5]);

		paramsPoints = 5;
	}
	public static int[] GetMaximum (ClassType r) {
		int[] ints = new int[6];

		for (int i = 0; i < ints.Length; i++) {
			ints[i] = 5;
		}

		switch (r) {
		case ClassType.Antimage :
			ints[0] = 7;
			ints[1] = 4;
			ints[2] = 6;
			ints[3] = 7;
			ints[4] = 4;
			ints[5] = 7;
			break;
		case ClassType.Thief :
			ints[0] = 4;
			ints[1] = 3;
			ints[2] = 7;
			ints[3] = 4;
			ints[4] = 9;
			ints[5] = 8;
			break;
		case ClassType.Cleric :
			ints[0] = 5;
			ints[1] = 9;
			ints[2] = 6;
			ints[3] = 6;
			ints[4] = 9;
			ints[5] = 5;
			break;
		case ClassType.DarknessSpirit :
			ints[0] = 8;
			ints[1] = 1;
			ints[2] = 8;
			ints[3] = 8;
			ints[4] = 1;
			ints[5] = 7;
			break;
		case ClassType.Inquisitor :
			ints[0] = 8;
			ints[1] = 8;
			ints[2] = 2;
			ints[3] = 8;
			ints[4] = 8;
			ints[5] = 7;
			break;
		case ClassType.Monk :
			ints[0] = 7;
			ints[1] = 6;
			ints[2] = 1;
			ints[3] = 9;
			ints[4] = 3;
			ints[5] = 9;
			break;
		case ClassType.Palladin :
			ints[0] = 9;
			ints[1] = 6;
			ints[2] = 8;
			ints[3] = 6;
			ints[4] = 9;
			ints[5] = 9;
			break;
		case ClassType.Wonder :
			ints[0] = 3;
			ints[1] = 4;
			ints[2] = 13;
			ints[3] = 3;
			ints[4] = 3;
			ints[5] = 8;
			break;
		}

		return ints;
	}
}