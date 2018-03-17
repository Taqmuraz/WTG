using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

[System.Serializable]
public struct IColor
{
	public float r;
	public float g;
	public float b;

	public IColor (float r, float g, float b) {
		this.r = r;
		this.g = g;
		this.b = b;
	}
	public static implicit operator Color (IColor c) {
		return new Color (c.r, c.g, c.b, 1);
	}
	public static implicit operator IColor (Color c) {
		return new IColor (c.r, c.g, c.b);
	}
	public static implicit operator string (IColor c) {
		return "Color : " + c.r + ", " + c.g + ", " + c.b;
	}
	public string ToText () {
		return "r : " + r + ", g : " + g + ", b : " + b;
	}
}
[System.Serializable]
public struct IVector
{
	public float x;
	public float y;
	public float z;
	
	public IVector (float x, float y, float z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}
	public static implicit operator Vector3 (IVector v) {
		return new Vector3 (v.x, v.y, v.z);
	}
	public static implicit operator IVector (Vector3 v) {
		return new IVector (v.x, v.y, v.z);
	}
}
[System.Serializable]
public class ISaveble
{
	public string name = "unknown";
	public IVector position = new IVector (0, 0, 0);
	public float euler_y = 0;

	public string ToJSON () {

		if (this is IStatus) {
			return JsonUtility.ToJson ((IStatus)this);
		}
		if (this is IDoorSave) {
			return JsonUtility.ToJson ((IDoorSave)this);
		}
		if (this is ISavableItem) {
			return JsonUtility.ToJson ((ISavableItem)this);
		}

		return JsonUtility.ToJson (this);
	}
}
/*[System.Serializable]
public class SaveTexture
{
	public byte[] bytes = new byte[0];

	public Texture2D texture
	{
		get {
			Texture2D tex = new Texture2D (Screen.width, Screen.height);
			tex.LoadImage (bytes);
			return tex;
		}
	}
	public SaveTexture (Texture2D tex) {
		if (tex) {
			bytes = tex.EncodeToPNG ();
		}
	}
	public SaveTexture Load (int index) {
		string path = Application.persistentDataPath + "/Picture_" + index + ".tex";
		SaveTexture s = new SaveTexture (null);
		if (File.Exists(path)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);
			s = (SaveTexture)bf.Deserialize (file);
			file.Close ();
		}
		return s;
	}
	public void Save (int index, SaveTexture tex) {
		string path = Application.persistentDataPath + "/Picture_" + index + ".tex";
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (path);
		bf.Serialize (file, tex);
		file.Close ();
	}
}*/
[System.Serializable]
public class Location
{
	public string locationName = "unnamed";
	public ISaveble[] objects;
	public bool hasBeenHere = false;

	public Location (ISaveble[] objs, string name) {
		objects = objs;
		locationName = name;
	}
}
[System.Serializable]
public class Settings
{
	public int quality = 3;
	public int fontSize = 22;
}
[System.Serializable]
public class ISlot
{
	public int id;
	public int count;

	public ISlot (int ID) {
		id = ID;
		count = 1;
	}
	public void Add () {
		count++;
	}
}
[System.Serializable]
public class IGame
{
	public Dictionary <string, Location> locations = new Dictionary<string, Location> ();
	public IQuest[] progress;
	public List<DebugMessage> messages = new List<DebugMessage>();

	public string currentLocationName = "Arena";

	public DateTime date = new DateTime();

	public static bool isNew
	{
		get {
			return (!buffer.locations[buffer.currentLocationName].hasBeenHere);
		}
	}
	public static int currentProfile = 0;

	public static IGame buffer;

	public ISaveble FindByName (string name) {
		ISaveble[] svs = locations [currentLocationName].objects;

		ISaveble sav = new ISaveble ();

		for (int i = 0; i < svs.Length; i++) {
			if (svs[i].name == name) {
				sav = svs[i];
				break;
			}
		}

		return sav;
	}

	public IGame (ISaveble[] objects, IQuest[] progress) {
		this.locations.Add ("Arena", new Location(objects, "Arena"));
		this.progress = progress;
	}

	public Location currentLocation
	{
		get
		{
			return this.locations [currentLocationName];
		}
	}

	public void MoveToLocation (string locationName) {
		currentLocationName = locationName;
		if (this.locations[currentLocationName] == null) {
			this.locations.Add (currentLocationName, new Location (new ISaveble[0], currentLocationName));
		}
		currentLocation.hasBeenHere = true;
	}

	public static void Save (IGame game, int index) {
		BinaryFormatter bf = new BinaryFormatter ();
		string path = Application.persistentDataPath + "/Save_" + index + ".sav";
		FileStream file = File.Create (path);
		bf.Serialize (file, game);
		file.Close ();

		/*string textPath = Application.persistentDataPath + "/Save_" + index + ".txt";
		FileStream text = File.Create (textPath);
		StreamWriter sw = new StreamWriter (text);
		string st = JsonUtility.ToJson (game, true);
		Debug.Log (st);
		sw.WriteLine (st);
		text.Close ();*/
	}
	public static void Remove (int index) {
		if (Exist(index)) {
			string path = Application.persistentDataPath + "/Save_" + index + ".sav";
			File.Delete (path);
			path = Application.persistentDataPath + "/Picture_" + index + ".png";
			File.Delete (path);
		}
	}
	public static Texture2D LoadPicture (int index) {
		string path = Application.persistentDataPath + "/Picture_" + index + ".png";
		Texture2D t = new Texture2D (8, 8);
		if (File.Exists(path)) {
			t.LoadImage(File.ReadAllBytes (path));
		}
		return t;
	}
	public static byte[] screenshot;
	public static void SavePicture (int index) {
		string path = Application.persistentDataPath + "/Picture_" + index + ".png";
		IScreenshotCamera.screenCamera.Screenshot ();
		File.WriteAllBytes (path, screenshot);
	}
	public static bool Exist (int index) {
		string path = Application.persistentDataPath + "/Save_" + index + ".sav";
		return File.Exists (path);
	}
	public static IGame Load (int index) {
		string path = Application.persistentDataPath + "/Save_" + index + ".sav";
		IGame game = new IGame (new ISaveble[0], new IQuest[0]);
		if (File.Exists(path)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);
			game = (IGame)bf.Deserialize (file);
			file.Close ();
		}
		IQuest.current = game.progress;
		MyDebug.messages = game.messages;
		buffer = game;
		return game;
	}
	public static void CaptureGame () {
		List<ISaveble> savs = new List<ISaveble> ();

		ICharacter[] chars = ICharacter.FindObjectsOfType<ICharacter> ();

		for (int i = 0; i < chars.Length; i++) {
			chars [i].status.dialogData.onAction = delegate() {
				return;	
			};
			savs.Add(chars[i].status);
		}

		IItemObject[] objs = IItemObject.FindObjectsOfType<IItemObject> ();

		for (int i = 0; i < objs.Length; i++) {
			savs.Add (objs[i].CaptureItem());
		}

		IDoor[] doors = IDoor.FindObjectsOfType<IDoor> ();

		for (int i = 0; i < doors.Length; i++) {
			savs.Add (doors [i].data);
		}

		IGame.buffer.locations [IGame.buffer.currentLocationName].objects = savs.ToArray ();
		IGame.buffer.date = DateTime.Now;
		IGame.buffer.progress = IQuest.current;
		IGame.buffer.currentLocation.hasBeenHere = true;
		IGame.buffer.messages = MyDebug.messages;
	}
}
[System.Serializable]
public class IQuest
{
	public bool available = true;
	public bool did = false;

	public static IQuest[] current = new IQuest[0];

	public static IQuest[] GetStart () {
		IQuest[] q = { new IQuest_Arena () };
		return q;
	}
}
[System.Serializable]
public class ISavableItem : ISaveble
{
	public int id = 0;
}
[System.Serializable]
public class Immunities
{
	public int melee = 0;
	public int fire = 0;
	public int water = 0;
	public int air = 0;
	public int earth = 0;
	public int magic = 0;

	public Immunities (int m, int f, int w, int a, int e, int mg) {
		melee = m;
		fire = f;
		water = w;
		air = a;
		earth = e;
		magic = mg;
	}
	public Immunities () {
		melee = 0;
		fire = 0;
		water = 0;
		air = 0;
		earth = 0;
		magic = 0;
	}
	public string ToText () {
		string f = "Сопротивление огню : " + fire + "%";
		string w = "Сопротивление воде : " + water + "%";
		string a = "Сопротивление воздуху : " + air + "%";
		string e = "Сопротивление земле : " + earth + "%";
		string m = "Сопротивление оружию : " + melee + "%";
		string mg = "Сопротивление чистой магии : " + magic + "%";

		string tr = "";

		if (fire != 0) {
			tr = tr + f + '\n';
		}
		if (water != 0) {
			tr = tr + w + '\n';
		}
		if (air != 0) {
			tr = tr + a + '\n';
		}
		if (earth != 0) {
			tr = tr + e + '\n';
		}
		if (melee != 0) {
			tr = tr + m + '\n';
		}
		if (magic != 0) {
			tr = tr + mg;
		}
		return tr;
	}
	public static Immunities operator + (Immunities a, Immunities b) {
		a.air += b.air;
		a.earth += b.earth;
		a.fire += b.fire;
		a.magic += b.magic;
		a.melee += b.melee;
		a.water += b.water;

		return a;
	}
}
[System.Serializable]
public class IStatus : ISaveble
{
	public string characterName = "no name";
	public int health = 100;
	public int level = 1;
	public int money = 0;
	public Immunities immunity
	{
		get {
			Immunities imm = new Immunities ();

			switch (iRace) {
			case IRace.Angel:
				imm = new Immunities (-50, 25, 25, 25, 25, 100);
				break;
			case IRace.Devil:
				imm = new Immunities (15, 90, -50, -100, 0, -100);
				break;
			case IRace.Elf:
				imm = new Immunities (-100, -25, 50, 90, -25, 100);
				break;
			case IRace.Human:
				imm = new Immunities (0, 0, 0, 0, 0, 0);
				break;
			case IRace.Orc:
				imm = new Immunities (75, -50, -50, 50, 90, -50);
				break;
			case IRace.Vampire:
				imm = new Immunities (100, -300, 0, 0, 0, -100);
				break;
			case IRace.Verwolf:
				imm = new Immunities (75, -25, -25, 100, 50, 0);
				break;
			case IRace.Witch:
				imm = new Immunities (-25, 100, -100, 100, -50, -100);
				break;
			}

			switch (iType) {
			case IClassType.Antimage:
				imm += new Immunities (0, 5 * level, 5 * level, 5 * level, 5 * level, 5 * level);
				break;
			case IClassType.Monk:
				imm += new Immunities (5 * level, 0, 0, 0, 0, 0);
				break;
			}

			Immunities ar = new Immunities ();
			if (armor > -1) {
				ar = IItemAsset.items [armor].bonuses;
			}
			Immunities we = new Immunities ();
			if (weapon > -1) {
				we = IItemAsset.items [weapon].bonuses;
			}
			Immunities am = new Immunities ();
			if (amulet > -1) {
				am = IItemAsset.items [amulet].bonuses;
			}

			return imm + ar + we + am;
		}
	}
	public bool CanUseItem (int itemID) {
		IItem item = IItemAsset.items [itemID];
		bool can = true;
		if (item.spetiality.Length > 0) {
			bool has = false;
			for (int i = 0; i < item.spetiality.Length; i++) {
				if (item.spetiality[i] == iType) {
					has = true;
					break;
				}
			}
			can = has;
		}
		return can;
	}
	[System.Serializable]
	public enum Gender
	{
		Male,
		Female
	}
	public Gender gender
	{
		get {
			Gender g = Gender.Male;
			if (iRace == IRace.Angel || iRace == IRace.Witch) {
				g = Gender.Female;
			}
			return g;
		}
	}
	public string GetGenderText ()
	{
		string t = "";
		switch (gender) {
		case Gender.Male:
			t = "Мужчина";
			break;
		case Gender.Female:
			t = "Женщина";
			break;
		}
		return t;
	}
	public string GetAllInfo () {
		string info = "" + GetGenderText() + '\n';
		info = info + '\n' + characterName;
		info = info + '\n' + iRace;
		info = info + '\n' + iType + '\n';
		info = info + '\n' + "Уровень : " + level;
		info = info + '\n' + "Сила : " + strongness;
		info = info + '\n' + "Ловкость : " + speedness;
		info = info + '\n' + "Мудрость : " + wisdom;
		info = info + '\n' + "Интеллект : " + intellect;
		info = info + '\n' + "Выносливость : " + invicibility;
		info = info + '\n' + "Харизма : " + personability;
		info = info + '\n' + '\n' + immunity.ToText();

		return info;
	}
	public int spellsToday
	{
		get {
			if (!canUseRunes) {
				sptd = 0;
			}
			return sptd;
		}
		set {
			if (canUseRunes) {
				sptd = Mathf.Clamp(value, 0, 10);
			}
		}
	}
	public void SetSpellsTodayBy () {
		int sp = (intellect - 1) / 2 + level / 2;
		if (iType != IClassType.Wonder) {
			sp /= 2;
		}
		spellsToday = sp;
	}
	private int sptd = 2;
	public IReputationType reputationType = IReputationType.People;
	public IReputation reputation
	{
		get
		{
			IReputation rep = new IReputation (0, 0, 0, 0, 0, 0);

			if (reputationType == IReputationType.Bandit) {
				rep = IReputation.standart_bandit;
			}
			if (reputationType == IReputationType.Cleric) {
				rep = IReputation.standart_cleric;
			}
			if (reputationType == IReputationType.Guard) {
				rep = IReputation.standart_guard;
			}
			if (reputationType == IReputationType.Mage) {
				rep = IReputation.standart_mage;
			}
			if (reputationType == IReputationType.Monster) {
				rep = IReputation.standart_monster;
			}
			if (reputationType == IReputationType.People) {
				rep = IReputation.standart_citizen;
			}

			return rep;
		}
	}

	public int bestWeapon
	{
		get {
			int b = -1;
			int max = 0;
			for (int i = 0; i < items.Length; i++) {
				IItem it = IItemAsset.items [items [i]];
				if (it.type == IItemType.Weapon) {
					if (it.value > max) {
						b = i;
						max = it.value;
					}
				}
			}
			return b;
		}
	}

	public IDialog dialogData = new IDialog();

	public int damage_melee
	{
		get {
			int damage = (strongness / 10) + level;
			if (weapon > -1) {
				damage = IItemAsset.items [weapon].value + (strongness / 10) + level;
			} else {
				if (iType == IClassType.Monk) {
					damage = (strongness / 8) + level * 2;
				}
				if (iRace == IRace.Verwolf) {
					damage = damage + level * 2;
				}
			}
			return damage;
		}
	}
	public bool canUseRunes
	{
		get {
			return iType == IClassType.Bard || iType == IClassType.Wonder;
		}
	}
	public int damage_spell
	{
		get {
			int damage = 0;
			if (canUseRunes) {
				damage = ((intellect - 1) / 3) * level;
			}
			return damage;
		}
	}
	public int armorProtection
	{
		get
		{
			int prot = 0;
			if (armor > -1) {
				prot = IItemAsset.items [armor].value;
			}
			if (amulet > -1) {
				prot += IItemAsset.items [amulet].value;
			}
			prot = Mathf.Clamp (prot, 0, 10);
			return prot;
		}
	}
	public void ApplyDamage (int damage) {
		ApplyDamage (damage, IDamageType.Just);
	}
	public int ApplyDamage (int damage, IDamageType dmgType) {
		if (armor > -1) {
			damage = Mathf.Clamp (damage - armorProtection, 1, damage);
		}
		int imm = 0;
		switch (dmgType) {
		case IDamageType.Air:
			imm = immunity.air;
			break;
		case IDamageType.Water:
			imm = immunity.water;
			break;
		case IDamageType.Fire:
			imm = immunity.fire;
			break;
		case IDamageType.Earth:
			imm = immunity.earth;
			break;
		case IDamageType.Magic:
			imm = immunity.magic;
			break;
		case IDamageType.Melee:
			imm = immunity.melee;
			break;
		}
		imm = 100 - imm;
		damage = (damage * imm) / 100;
		health -= damage;
		return damage;
	}

	public static IClassType[] MayBe (IRace race) {

		IClassType[] types = new IClassType[0];

		if (race == IRace.Angel) {
			IClassType[] n = {IClassType.Antimage, IClassType.Wonder, IClassType.Bard, IClassType.Monk, IClassType.Cleric, IClassType.Inquisitor, IClassType.Palladin};
			types = n;
		}
		if (race == IRace.Devil) {
			IClassType[] n = {IClassType.DarknessSpirit, IClassType.Bard};
			types = n;
		}
		if (race == IRace.Elf) {
			IClassType[] n = {IClassType.Antimage, IClassType.Wonder, IClassType.Bard, IClassType.Cleric, IClassType.Inquisitor};
			types = n;
		}
		if (race == IRace.Human) {
			IClassType[] n = {IClassType.Antimage,
				IClassType.Wonder,
				IClassType.Bard,
				IClassType.Cleric,
				IClassType.DarknessSpirit,
				IClassType.Inquisitor,
				IClassType.Monk,
				IClassType.Palladin};
			types = n;
		}
		if (race == IRace.Orc) {
			IClassType[] n = {IClassType.Antimage, IClassType.DarknessSpirit, IClassType.Bard, IClassType.Monk};
			types = n;
		}
		if (race == IRace.Vampire) {
			IClassType[] n = {IClassType.DarknessSpirit, IClassType.Wonder, IClassType.Bard};
			types = n;
		}
		if (race == IRace.Verwolf) {
			IClassType[] n = {IClassType.Antimage, IClassType.Monk, IClassType.Wonder, IClassType.Bard, IClassType.DarknessSpirit};
			types = n;
		}
		if (race == IRace.Witch) {
			IClassType[] n = {IClassType.Wonder, IClassType.Bard, IClassType.Antimage, IClassType.DarknessSpirit, IClassType.Monk};
			types = n;
		}

		return types;
	}
	public static bool MayBeNow (IRace race, IClassType type) {

		IClassType[] may = MayBe (race);

		bool yet = false;

		for (int i = 0; i < may.Length; i++) {
			if (may[i] == type) {
				yet = true;
				break;
			}
		}

		return yet;
	}

	public int maxHealth
	{
		get 
		{
			IClassType ic = this.iType;
			int h = 1;
			if (ic == IClassType.Antimage) {
				h = 5 + 5 * level;
			}
			if (ic == IClassType.Bard) {
				h = 5 + 2 * level;
			}
			if (ic == IClassType.Cleric) {
				h = 5 + 4 * level;
			}
			if (ic == IClassType.DarknessSpirit) {
				h = 5 + 6 * level;
			}
			if (ic == IClassType.Inquisitor) {
				h = 5 + 6 * level;
			}
			if (ic == IClassType.Monk) {
				h = 5 + 7 * level;
			}
			if (ic == IClassType.Palladin) {
				h = 5 + 4 * level;
			}
			if (ic == IClassType.Wonder) {
				h = 5 + 1 * level;
			}

			return h;
		}
	}
	public bool HasItem (int id) {
		bool has = false;
		for (int i = 0; i < items.Length; i++) {
			if (items[i] == id) {
				has = true;
				break;
			}
		}
		return has;
	}
	public IClassType iType = IClassType.Simple;
	public IPersonView iPerson = new IPersonView();
	public IPersonView iPersonWithArmor
	{
		get
		{
			IPersonView person = iPerson;
			if (armor > -1) {
				person.cloth_color = IItemAsset.items [armor].color;
			}
			return person;
		}
	}
	public IRace iRace = IRace.Human;
	public int[] items = new int[0];
	public int strongness = 5;
	public int intellect = 5;
	public int wisdom = 5;
	public int speedness = 5;

	public int GetItemByID (int id) {
		int index = -1;
		for (int i = 0; i < items.Length; i++) {
			if (items[i] == id) {
				index = i;
				break;
			}
		}
		return index;
	}

	public float moveSpeed
	{
		get
		{
			return 3 + Mathf.Sqrt (speedness);
		}
	}
	public int invicibility = 5;
	public int personability = 5;

	public int armor = -1;
	public int weapon = -1;
	public int amulet = -1;
	public int rune = -1;
}
[System.Serializable]
public struct IPersonView
{
	public IColor hair_color;
	public IColor cloth_color;
	public IColor cloth_more_color;
	public IColor skin_color;

	public static void SetToRenderer (IStatus status, Renderer rend) {

		IPersonView person = status.iPersonWithArmor;

		for (int i = 0; i < rend.materials.Length; i++) {
			string cur = rend.materials[i].name;

			switch (cur) {
			case "hairmat (Instance)":
				rend.materials [i].color = person.hair_color;
				break;
			case "color_1mat (Instance)":
				rend.materials [i].color = person.cloth_color;
				break;
			case "color_2mat (Instance)":
				rend.materials [i].color = person.cloth_more_color;
				break;
			case "skinmat (Instance)":
				rend.materials [i].color = person.skin_color;
				break;
			}
		}
	}
	public static void SetToManyRenderer (IStatus status, Renderer[] rends) {
		IPersonView person = status.iPersonWithArmor;
		for (int i = 0; i < rends.Length; i++) {
			string cur = rends[i].material.name;

			switch (cur) {
			case "hairmat (Instance)":
				rends [i].material.color = person.hair_color;
				break;
			case "color_1mat (Instance)":
				rends [i].material.color = person.cloth_color;
				break;
			case "color_2mat (Instance)":
				rends [i].material.color = person.cloth_more_color;
				break;
			case "skinmat (Instance)":
				rends [i].material.color = person.skin_color;
				break;
			}
		}
	}
}
[System.Serializable]
public enum IClassType
{
	Simple,
	Monk,
	Cleric,
	Wonder,
	Palladin,
	Bard,
	Inquisitor,
	Antimage,
	DarknessSpirit
}
[System.Serializable]
public enum IRace
{
	Human,
	Elf,
	Orc,
	Verwolf,
	Vampire,
	Angel,
	Witch,
	Devil
}
[System.Serializable]
public enum IReputationType
{
	People,
	Bandit,
	Guard,
	Monster,
	Mage,
	Cleric
}
[System.Serializable]
public class IReputation
{
	public IReputation (int people, int bandit, int guard, int monster, int mages, int clerics) {
		this.people = people;
		this.bandit = bandit;
		this.guard = guard;
		this.monster = monster;
		this.mages = mages;
		this.clerics = clerics;
	}

	public static int GetEnemity (IStatus _target, IStatus _attacker) {
		int delta = 0;

		if (_target.reputationType == IReputationType.Bandit) {
			delta = Mathf.Abs(1 - _attacker.reputation.bandit);
		}
		if (_target.reputationType == IReputationType.Cleric) {
			delta = Mathf.Abs(1 - _attacker.reputation.clerics);
		}
		if (_target.reputationType == IReputationType.Guard) {
			delta = Mathf.Abs(1 - _attacker.reputation.guard);
		}
		if (_target.reputationType == IReputationType.Mage) {
			delta = Mathf.Abs(1 - _attacker.reputation.mages);
		}
		if (_target.reputationType == IReputationType.Monster) {
			delta = Mathf.Abs(1 - _attacker.reputation.monster);
		}
		if (_target.reputationType == IReputationType.People) {
			delta = Mathf.Abs(1 - _attacker.reputation.people);
		}

		return delta;
	}

	public int people = 0;
	public int bandit = 0;
	public int guard = 0;
	public int monster = 0;
	public int mages = 0;
	public int clerics = 0;

	public static IReputation standart_citizen
	{
		get
		{
			return new IReputation (1, -1, 1, -1, 0, 1);
		}
	}
	public static IReputation standart_bandit
	{
		get
		{
			return new IReputation (-1, 1, -1, -1, 0, 0);
		}
	}
	public static IReputation standart_mage
	{
		get
		{
			return new IReputation (0, 0, 0, -1, 1, 0);
		}
	}
	public static IReputation standart_cleric
	{
		get
		{
			return new IReputation (1, 0, 1, -1, 0, 1);
		}
	}
	public static IReputation standart_monster
	{
		get
		{
			return new IReputation (-1, -1, -1, 1, -1, -1);
		}
	}
	public static IReputation standart_guard
	{
		get
		{
			return new IReputation (1, -1, 1, -1, 0, 1);
		}
	}
}
[System.Serializable]
public enum IDamageType
{
	Fire,
	Water,
	Air,
	Earth,
	Magic,
	Melee,
	Just
}

public class ICharacter : MonoBehaviour
{
	public IStatus status = new IStatus();
	public Renderer main_render;
	public Transform trans;
	public Animator anims;
	public NavMeshAgent agent;

	[SerializeField]
	private int lastArmor = -1;

	private RawImage weaponImage;

	public void Start () {
		PrepareToGame ();
	}
	public void AI_Update () {

		if (status.canUseRunes && status.rune < 0) {
			int rune = -1;
			for (int i = 0; i < status.items.Length; i++) {
				if (IItem.IsSpell (status.items [i])) {
					rune = i;
					break;
				}
			}
			if (rune > -1) {
				UseItem (rune);
			}
		} else {
			if (status.weapon < 0) {
				if (status.bestWeapon > -1) {
					UseItem (status.bestWeapon);
				}
			}
		}
		ICharacter enemyNearby = GetNearestEnemy ();
		if (enemyNearby) {
			if (!((enemyNearby.trans.position - trans.position).magnitude < 1.25)) {
				if (status.canUseRunes && status.rune > -1) {
					CastSpell (enemyNearby.trans.position);
				} else {
					MoveTo (enemyNearby.trans.position - trans.forward);
				}
			} else {
				Attack (enemyNearby);
			}
		}
	}
	public void PrepareToGame () {
		trans = transform;
		anims = GetComponent<Animator> ();
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		main_render = GetComponentInChildren<SkinnedMeshRenderer> ();

		weaponImage = GetComponentInChildren<RawImage> ();
		//weaponImage.rectTransform.parent.localEulerAngles = new Vector3(45, 180, -135);

		PrepareRend ();

		if (!IGame.isNew || status.characterName == "Player") {
			agent.Warp (status.position);
			trans.eulerAngles = Vector3.up * status.euler_y;
		} else {
			status.health = status.maxHealth;
		}
	}
	public void PrepareRend () {
		Renderer[] rends = GetComponentsInChildren<SkinnedMeshRenderer> ();

		if (rends.Length > 1) {
			IPersonView.SetToManyRenderer (status, rends);
		} else {
			IPersonView.SetToRenderer (status, main_render);
		}
	}
	public bool isPlayer
	{
		get
		{
			return status.name == "Player";
		}
	}
	public void AddMoney (int m) {
		status.money += m;
		MyDebug.Log ("Получено золото : " + m, Color.yellow, this);
	}
	public void MoveAt (Vector3 direction) {
		if (canMove) {
			agent.SetDestination (trans.position + direction);

			agent.speed = direction.magnitude * status.moveSpeed;
		} else {
			agent.SetDestination (trans.position);
		}
	}
	public void MoveTo (Vector3 point) {
		if (canMove) {
			if (agent.pathStatus == NavMeshPathStatus.PathPartial) {
				IDoor door = IDoor.GetNearestClosedDoor (trans.position);
				if (door && !door.data.opened) {
					door.Use (this);
				}
			}
			agent.SetDestination (point);
			agent.speed = status.moveSpeed;
		} else {
			agent.SetDestination (trans.position);
		}
	}
	public bool canMove
	{
		get {
			return !inCombat && !inReact;
		}
	}
	private void Update () {
		try {
			IUpdate ();
			Animate ();
			IStatusSinhro ();
		} catch (Exception ex) {
			MyDebug.Log (ex.Message, Color.red, this);
		}
	}
	private void DrawStats () {
		string health = "Без ранений";
		if (status.health < status.maxHealth * 0.75f) {
			health = "Слабые ранения";
		}
		if (status.health < status.maxHealth / 2) {
			health = "Сильные ранения";
		}
		if (status.health < status.maxHealth / 4) {
			health = "При смерти";
		}
		Vector3 pos = IControl.camMain.WorldToScreenPoint (trans.position + Vector3.up * 4);
		Rect rect = new Rect (pos.x - 50, Screen.height - (pos.y - 25), 100, 50);
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		Color c = Color.green;
		if (IControl.character) {
			int en = IReputation.GetEnemity (status, IControl.character.status);
			if (en > 0) {
				c = Color.yellow;
			}
			if (en > 1) {
				c = Color.red;
			}
		}
		GUI.color = c;
		GUI.Label (rect, status.characterName + '\n' + health);
	}
	private void OnGUI () {
		if (IControl.control.state == IControl.IGameMenuState.Runtime) {
			DrawStats ();
		}
	}
	public static ICharacter GetPlayer () {
		ICharacter finded = null;
		ICharacter[] chars = ICharacter.FindObjectsOfType<ICharacter>();
		for (int i = 0; i < chars.Length; i++) {
			if (chars[i].isPlayer) {
				finded = chars[i];
				break;
			}
		}
		return finded;
	}
	public static ICharacter GetByName (string name) {
		ICharacter finded = null;
		ICharacter[] chars = ICharacter.FindObjectsOfType<ICharacter>();
		for (int i = 0; i < chars.Length; i++) {
			if (chars[i].status.name == name) {
				finded = chars[i];
				break;
			}
		}
		return finded;
	}
	private void IUpdate () {
		if (isPlayer) {
			MoveFromControls ();
		} else {
			AI_Update ();
		}
		if (!(status.health > 0)) {
			Die();
		}
	}
	public bool inMove
	{
		get
		{
			return (agent.destination - trans.position).magnitude > 0.1f ||
				agent.velocity.magnitude > 0.2f;
		}
	}
	public IUsable canUse
	{
		get
		{
			return (IUsable.GetNearestToPosition (trans.position));
		}
	}
	public ICharacter canTalk {
		get
		{
			ICharacter[] chars = ICharacter.FindObjectsOfType<ICharacter> ();
			ICharacter finded = null;
			float dist = 1.5f;

			for (int i = 0; i < chars.Length; i++) {
				float dist_cur = (chars [i].trans.position - trans.position).magnitude;
				if (dist_cur < dist && chars[i].status.name != "Player" && !(IReputation.GetEnemity(chars[i].status, status) > 1)) {
					finded = chars [i];
					dist = dist_cur;
				}
			}

			return finded;
		}
	}
	public float attack_clip_num = 0;
	public float spell_clip_num = 0;
	public float react_clip_num = 0;
	private float combatState = 0;
	private float combasStateLerp = 0;
	public int attack_clips_count = 7;
	public ICharacter tacked;
	public void Stop () {
		if (agent && agent.isOnNavMesh) {
			agent.destination = trans.position;
		}
	}
	public void Attack (ICharacter whom) {
		if (!inCombat && !inReact) {
			int num = UnityEngine.Random.Range (0, attack_clips_count - 1);

			attack_clip_num = (float)num / (float)(attack_clips_count);

			tacked = whom;
			combatState = 5;
			Invoke ("Attack_Damage_Set", 0.5f);
			float asp = (10f / (status.speedness + status.strongness));
			Invoke ("Attack_End", 2.5f + asp);
			Animate ();
			anims.Play ("Combat");
			Stop ();
			if (whom) {
				lookVector = whom.trans.position;
			}

			//anims.Play ("Combat");
		}
	}
	Vector3 l;
	public Vector3 lookVector {
		get
		{
			return l;
		}
		set
		{
			l = value;
			if (IsInvoking("LookToSettedDirection")) {
				CancelInvoke ("LookToSettedDirection");
			}
			LookToSettedDirection ();
		}
	}
	private void LookToSettedDirection () {
		Vector3 v = lookVector;
		v = new Vector3 (v.x, trans.position.y, v.z);
		v = v - trans.position;
		Quaternion next = Quaternion.LookRotation (v);
		trans.rotation = Quaternion.Slerp (trans.rotation, next, 0.2f);
		if (Vector3.Angle (trans.forward, v) < 5) {
			CancelInvoke ("LookToSettedDirection");
		} else {
			Invoke ("LookToSettedDirection", 0.1f);
		}
	}
	private void Attack_End () {
		return;
	}
	private bool inCombat
	{
		get {
			return attacking || spelling;
		}
	}
	private bool attacking
	{
		get {
			return IsInvoking ("Attack_End");
		}
	}
	private bool spelling
	{
		get {
			return IsInvoking("Spell_End");
		}
	}
	private void Attack_SpetialDamage_Set (ICharacter whom) {
		switch (status.iType) {
		case IClassType.Monk:
			int procent = 5 * status.level;
			int rnd = UnityEngine.Random.Range (0, 100);
			if (rnd < procent) {
				IDamageType type = (IDamageType)UnityEngine.Random.Range (0, 5);
				int dmg = status.damage_melee;
				whom.ApplyDamage (dmg, type);
			}
			break;
		}
	}
	private void Attack_Damage_Set () {
		if (!inReact) {
			NavMeshHit hit;
			if (tacked && !agent.Raycast(tacked.trans.position, out hit)) {
				Vector3 delta = tacked.trans.position - trans.position;
				if (Vector3.Angle(trans.forward, delta.normalized) < 20 && delta.magnitude < 1.75f) {
					tacked.ApplyDamage (status.damage_melee, IDamageType.Melee);
					Attack_SpetialDamage_Set (tacked);
					if (!(IReputation.GetEnemity(tacked.status, status) > 1)) {
						if (status.reputationType != IReputationType.Bandit &&
							status.reputationType != IReputationType.Monster) {
							status.reputationType = IReputationType.Bandit;
						}
					}
				}
			}
		}
	}
	private bool inReact
	{
		get {
			return IsInvoking ("EndReact");
		}
	}
	private void EndReact () {
		return;
	}
	public void ApplyDamage (int damage) {
		ApplyDamage (damage, IDamageType.Just);
	}
	public void ApplyDamage (int damage, IDamageType dmgType) {
		react_clip_num = UnityEngine.Random.Range (0.0f, 1.0f);
		combatState = 5;
		if (this) {
			CancelInvoke ("Attack_Damage_Set");
			CancelInvoke ("Attack_End");
			Invoke ("EndReact", 1);
		}
		Animate ();
		anims.Play ("Combat");
		int app = status.ApplyDamage (damage, dmgType);
		Color clr = Color.yellow;
		switch (dmgType) {
		case IDamageType.Air:
			clr = Color.gray;
			break;
		case IDamageType.Earth:
			clr = new Color (0.8f, 0.2f, 0.2f);
			break;
		case IDamageType.Fire:
			clr = new Color (0.9f, 0.5f, 0.5f);
			break;
		case IDamageType.Magic:
			clr = Color.cyan;
			break;
		case IDamageType.Melee:
			clr = Color.red;
			break;
		case IDamageType.Water:
			clr = Color.blue;
			break;
		}
		if (app > 0) {
			MyDebug.Log ("Получен урон (" + app + ")", clr, this);
		}
	}
	private void Animate () {
		combatState -= Time.deltaTime;
		combatState = Mathf.Clamp (combatState, 0, combatState);
		combasStateLerp = Mathf.Lerp (combasStateLerp, combatState, Time.deltaTime * 2);
		if (agent) {
			anims.SetFloat ("MoveK", Mathf.Sqrt((agent.destination - trans.position).magnitude));
		}
		float asp = 1.5f;
		anims.SetFloat ("Attack", attack_clip_num);
		anims.SetFloat ("Spell", spell_clip_num);
		anims.SetFloat ("React", react_clip_num);
		anims.SetFloat ("Idle", combasStateLerp);
		float combat = -1;
		if (attacking) {
			asp = 1.5f + ((status.speedness + status.strongness) / 40f);
			combat = 0;
		}
		if (inReact) {
			combat = 2;
		}
		if (spelling) {
			combat = 1;
		}
		anims.SetFloat ("ASP", asp);
		anims.SetFloat ("Combat", combat);
		weaponImage.texture = IItemAsset.LoadTexture (status.weapon);
	}
	public void UseGameObject (IUsable usable) {
		if (usable) {
			if (usable is IItemObject) {
				PickItem ((IItemObject)usable);
			}
			if (usable is IDoor && !(usable is IChest)) {
				OpenDoor ((IDoor)usable);
			}
			if (usable is IChest) {
				// do someshing with chest
			}
		}
	}
	public void OpenDoor (IDoor door) {
		door.Use (this);
	}
	public void Die () {
		anims.Play ("Death");
		Destroy (agent);
		Destroy (gameObject, 15);
		CancelInvoke ("EndReact");

		if (status.rune > -1) {
			AddItem (status.rune);
			status.rune = -1;
		}
		if (status.weapon > -1) {
			AddItem (status.weapon);
			status.weapon = -1;
		}
		if (status.armor > -1) {
			AddItem (status.armor);
			status.armor = -1;
		}
		if (status.amulet > -1) {
			AddItem (status.amulet);
			status.amulet = -1;
		}
		while (status.items.Length > 0) {
			DropItem (0);
		}

		Animate ();
		Destroy (this);
	}
	public ICharacter GetNearestEnemy () {
		ICharacter finded = null;
		ICharacter[] all = ICharacter.FindObjectsOfType<ICharacter> ();
		float dist = 15;
		for (int i = 0; i < all.Length; i++) {
			float dist_cur = (all [i].trans.position - trans.position).magnitude;
			if (dist_cur < dist && all[i] != this && IReputation.GetEnemity(all[i].status, status) > 1
				&& !Physics.Linecast(trans.position, all[i].trans.position)) {
				finded = all [i];
				dist = dist_cur;
			}
		}
		return finded;
	}
	public static ICharacter GetNearestFromPoint (Vector3 point, ICharacter mask, float maxDist) {
		ICharacter finded = null;
		ICharacter[] all = ICharacter.FindObjectsOfType<ICharacter> ();
		float dist = maxDist;
		for (int i = 0; i < all.Length; i++) {
			float dist_cur = (all [i].trans.position - point).magnitude;
			if (dist_cur < dist && all[i] != mask) {
				finded = all [i];
				dist = dist_cur;
			}
		}
		return finded;
	}
	public static ICharacter[] GetNearestFromPointAll (Vector3 point, ICharacter mask, float maxDist) {
		List<ICharacter> chars = new List<ICharacter> ();
		ICharacter[] all = ICharacter.FindObjectsOfType<ICharacter> ();
		float dist = maxDist;
		for (int i = 0; i < all.Length; i++) {
			float dist_cur = (all [i].trans.position - point).magnitude;
			if (dist_cur < dist && all[i] != mask) {
				chars.Add (all[i]);
			}
		}
		return chars.ToArray();
	}
	public void AttackNearest (Vector3 point) {
		ICharacter nearest = GetNearestFromPoint(point, this, 1.25f);

		Attack (nearest);
	}
	private void Spell_End () {
		return;
	}
	public void CastSpell (Vector3 point) {
		if (!inCombat) {
			ICharacter ch = GetNearestFromPoint (point, this, 0.5f);
			if (status.rune > -1) {
				IItem it = IItemAsset.items [status.rune];
				ISpellRune r = IItem.FromItemType (it.type);
				ISpell sp = new ISpell (ISpellType.Target, ISpellEffect.Ball, r, status.damage_spell, 5);
				ISpellData.spells [it.value - 1].onCastSpell.Invoke (this, ch, point, sp);
				lookVector = point;
				Invoke ("Spell_End", 1.5f);
				Animate ();
				anims.Play ("Combat");
			}
		}
	}
	public void AttackFromDirection () {
		AttackNearest (trans.position + trans.forward);
	}
	public void IStatusSinhro () {
		if (lastArmor != status.armor) {
			PrepareRend ();
			lastArmor = status.armor;
		}
		status.euler_y = trans.eulerAngles.y;
		status.position = trans.position;
	}
	public void MoveFromControls () {
		Vector3 direction = IControl.control.joypad;
		direction = new Vector3 (direction.x, 0, direction.y);
		direction = IControl.FromCameraAxes (direction);
		MoveAt (direction);
	}
	public void CreateRune (IItemType itemType) {
		if (status.spellsToday > 0) {
			int itemIndex = 0;
			switch (itemType) {
			case IItemType.ScrollOfAir:
				itemIndex = 8;
				break;
			case IItemType.ScrollOfEarth:
				itemIndex = 7;
				break;
			case IItemType.ScrollOfFire:
				itemIndex = 5;
				break;
			case IItemType.ScrollOfWater:
				itemIndex = 6;
				break;
			case IItemType.ScrollOfGod:
				itemIndex = 9;
				break;
			}
			AddItem (itemIndex);
			status.spellsToday -= 1;
		}
	}
	public void UseItem (int index) {

		IItem item = IItemAsset.items [status.items [index]];

		switch (item.type) {
		case IItemType.Amulet :
			status.amulet = status.items[index];
			break;
		case IItemType.Food :

			break;
		case IItemType.Weapon :
			if (status.weapon > -1) {
				AddItem (status.weapon);
			}
			status.weapon = status.items[index];
			break;
		case IItemType.Armor :
			if (status.armor > -1) {
				AddItem (status.armor);
			}
			status.armor = status.items[index];
			break;
		case IItemType.ScrollOfAir :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		case IItemType.ScrollOfEarth :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		case IItemType.ScrollOfFire :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		case IItemType.ScrollOfGod :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		case IItemType.ScrollOfWater :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		}
		if (item.type != IItemType.None) {
			RemoveItem (index);
		}
	}
	public void RemoveItem (int index) {
		List<int> ints = new List<int> ();
		ints.AddRange (status.items);
		ints.RemoveAt (index);
		status.items = ints.ToArray ();
	}
	public void DropItem (int index) {

		GameObject prefab = (GameObject)Resources.Load ("Prefabs/ItemObject");

		Vector3 rp = UnityEngine.Random.insideUnitSphere;
		Vector3 rr = UnityEngine.Random.onUnitSphere;

		rp = new Vector3 (rp.x, 0, rp.z);
		rr = new Vector3 (rr.x, 0, rr.z).normalized;

		prefab = Instantiate (prefab, trans.position + rp, Quaternion.LookRotation(rr));

		IItemObject o = prefab.GetComponent<IItemObject> ();

		int id = status.items [index];

		o.indentification = id;

		o.SetWithID ();

		RemoveItem (index);
	}
	public void PickItem (IItemObject pick) {

		AddItem (pick.indentification);

		Destroy (pick.gameObject);
	}
	public void AddItem (int id) {
		if (id > -1) {
			List<int> ints = new List<int> ();
			ints.AddRange (status.items);
			ints.Add (id);
			status.items = ints.ToArray ();
		}
	}
	public void BackToInv (IItemType type) {
		switch (type) {
		case IItemType.Amulet:
			AddItem (status.amulet);
			status.amulet = -1;
			break;
		case IItemType.Armor:
			AddItem (status.armor);
			status.armor = -1;
			break;
		case IItemType.Weapon:
			AddItem (status.weapon);
			status.weapon = -1;
			break;
		case IItemType.ScrollOfAir:
			AddItem (status.rune);
			status.rune = -1;
			break;
		case IItemType.ScrollOfEarth:
			AddItem (status.rune);
			status.rune = -1;
			break;
		case IItemType.ScrollOfFire:
			AddItem (status.rune);
			status.rune = -1;
			break;
		case IItemType.ScrollOfGod:
			AddItem (status.rune);
			status.rune = -1;
			break;
		case IItemType.ScrollOfWater:
			AddItem (status.rune);
			status.rune = -1;
			break;
		}
	}
}