using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using System.Linq;

[System.Serializable]
public struct SColor
{
	public float r;
	public float g;
	public float b;

	public SColor (float r, float g, float b) {
		this.r = r;
		this.g = g;
		this.b = b;
	}
	public static implicit operator Color (SColor c) {
		return new Color (c.r, c.g, c.b, 1);
	}
	public static implicit operator SColor (Color c) {
		return new SColor (c.r, c.g, c.b);
	}
	public static implicit operator string (SColor c) {
		return "Color : " + c.r + ", " + c.g + ", " + c.b;
	}
	public string ToText () {
		return "r : " + r + ", g : " + g + ", b : " + b;
	}
}
[System.Serializable]
public struct SVector
{
	public float x;
	public float y;
	public float z;
	
	public SVector (float x, float y, float z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}
	public static implicit operator Vector3 (SVector v) {
		return new Vector3 (v.x, v.y, v.z);
	}
	public static implicit operator SVector (Vector3 v) {
		return new SVector (v.x, v.y, v.z);
	}
	public static SVector operator * (SVector a, float b) {
		return new SVector (a.x * b, a.y * b, a.z * b);
	}
	public static SVector operator / (SVector a, float b) {
		return new SVector (a.x / b, a.y / b, a.z / b);
	}
	public static SVector operator + (SVector a, SVector b) {
		return new SVector (a.x + b.x, a.y + b.y, a.z + b.z);
	}
	public static SVector operator - (SVector a, SVector b) {
		return new SVector (a.x - b.x, a.y - b.y, a.z - b.z);
	}
	public static SVector Flat (SVector origin) {
		return new SVector (origin.x, 0, origin.z);
	}
	public static SVector FlatAndNormallize (Vector3 origin) {
		return Flat (origin).normallized;
	}
	public float magnitude
	{
		get {
			return Mathf.Sqrt (x * x + y * y + z * z);
		}
	}
	public SVector normallized
	{
		get
		{
			return this / magnitude;
		}
	}
	public SVector flatten
	{
		get
		{
			return Flat (this);
		}
	}
}
[System.Serializable]
public class Saveble
{
	public string name = "unknown";
	public SVector position = new SVector (0, 0, 0);
	public float euler_y = 0;

	public string ToJSON () {

		if (this is Status) {
			return JsonUtility.ToJson ((Status)this);
		}
		if (this is SDoorSave) {
			return JsonUtility.ToJson ((SDoorSave)this);
		}
		if (this is SavebleItem) {
			return JsonUtility.ToJson ((SavebleItem)this);
		}

		return JsonUtility.ToJson (this);
	}
	public void MoveToLocation (GameObject obj, string nextLocation) {
		foreach (var l in SGame.buffer.locations) {
			l.Value.RemoveObject (this);
		}
		if (!SGame.buffer.HasLocation(nextLocation)) {
			SGame.buffer.InitializeLocation (nextLocation);
		}
		SGame.buffer.locations [nextLocation].AddObject (this);
		if (obj) {
			GameObject.Destroy (obj);
		}
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
	public Saveble[] objects;
	public bool hasBeenHere = false;

	public Location (Saveble[] objs, string name) {
		objects = objs;
		locationName = name;
	}
	public void AddObject (Saveble obj) {
		List<Saveble> objs = new List<Saveble> ();
		objs.AddRange (objects);
		objs.Add (obj);
		objects = objs.ToArray ();
	}
	public void RemoveObject (Saveble obj) {
		List<Saveble> objs = new List<Saveble> ();
		objs.AddRange (objects);
		objs.Remove (obj);
		objects = objs.ToArray ();
	}
}
[System.Serializable]
public class Settings
{
	public int quality = 3;
	public int fontSize = 22;
}
[System.Serializable]
public class Slot
{
	public int id
	{
		get {
			return (int)identificator;
		}
	}
	public Skill skill
	{
		get {
			return (Skill)identificator;
		}
	}
	public SkillEffect effect
	{
		get {
			return (SkillEffect)identificator;
		}
	}
	private object identificator;
	public int count;

	public Slot (object ID) {
		identificator = ID;
		count = 1;
	}
	public void Add () {
		count++;
	}
}
[System.Serializable]
public class SGame
{
	public Dictionary <string, Location> locations = new Dictionary<string, Location> ();
	public SQuest[] progress;
	public List<DebugMessage> messages = new List<DebugMessage>();

	public string currentLocationName = "Arena";

	public DateTime date = new DateTime();

	public SVector cameraEuler;

	public static bool isNew
	{
		get {
			return (!buffer.locations[buffer.currentLocationName].hasBeenHere);
		}
	}
	public static int currentProfile = 0;

	public static SGame buffer;

	public Saveble FindByName (string name) {
		Saveble[] svs = locations [currentLocationName].objects;

		Saveble sav = new Saveble ();

		for (int i = 0; i < svs.Length; i++) {
			if (svs[i].name == name) {
				sav = svs[i];
				break;
			}
		}

		return sav;
	}

	public SGame (Saveble[] objects, SQuest[] progress) {
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
		CaptureGame ();
		Saveble player = FindByName ("Player");

		currentLocationName = locationName;

		player.MoveToLocation (null, locationName);

		currentLocation.hasBeenHere = true;
	}

	public bool HasLocation (string name) {
		return locations.ContainsKey (name);
	}

	public void InitializeLocation (string name) {
		locations.Add (name, new Location (new Saveble[0], name));
	}

	public static void Save (SGame game, int index) {
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
	public static SGame Load (int index) {
		string path = Application.persistentDataPath + "/Save_" + index + ".sav";
		SGame game = new SGame (new Saveble[0], new SQuest[0]);
		if (File.Exists(path)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);
			game = (SGame)bf.Deserialize (file);
			file.Close ();
		}
		SQuest.current = game.progress;
		buffer = game;
		return game;
	}
	public static void CaptureGame () {
		List<Saveble> savs = new List<Saveble> ();

		ICharacter[] chars = ICharacter.charactersAll.ToArray ();

		for (int i = 0; i < chars.Length; i++) {
			chars [i].status.dialogData.onAction = delegate() {
				return;	
			};
			savs.Add(chars[i].status);
		}

		IItemObject[] objs = IItemObject.itemObjectsAll.ToArray ();

		for (int i = 0; i < objs.Length; i++) {
			savs.Add (objs[i].CaptureItem());
		}

		IDoor[] doors = IDoor.doorsAll.ToArray ();

		for (int i = 0; i < doors.Length; i++) {
			savs.Add (doors [i].data);
		}
		SGame.buffer.locations [SGame.buffer.currentLocationName].objects = savs.ToArray ();
		SGame.buffer.date = DateTime.Now;
		SGame.buffer.progress = SQuest.current;
		SGame.buffer.cameraEuler = (Vector3)IControl.control.cameraEuler;
		SGame.buffer.currentLocation.hasBeenHere = true;
	}
}
[System.Serializable]
public class SQuest
{
	public bool available = true;
	public bool did = false;

	public static SQuest[] current = new SQuest[0];

	public static SQuest[] GetStart () {
		SQuest[] q = { new IQuest_Arena () };
		return q;
	}
}
[System.Serializable]
public class SavebleItem : Saveble
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
public class Status : Saveble
{
	public string characterName = "no name";
	public int health = 100;
	public int level = 1;
	public int money = 0;

	public List<SkillEffect> effects {get; private set;}

	public Status (ClassType iT) {
		skills = new SkillSystem (iT);
		effects = new List<SkillEffect> ();
	}

	public void SetEffect (string name, SkillEffect newValue) {
		effects [effects.FindIndex ((SkillEffect se) => se.effectName == name)] = newValue;
	}

	public bool HasEffect (string name) {
		return effects.FirstOrDefault ((SkillEffect arg) => arg.effectName == name) != null;
	}

	public void Heal (int points) {
		health += points;
		health = Mathf.Clamp(health, 1, maxHealth);
	}

	public SkillSystem skills;

	public void SetDamageLawEvent (Status to) {
		if (!(Reputation.GetEnemity(to, this) > 1)) {
			switch (reputationType) {
			case ReputationType.Bandit:
				reputationType = ReputationType.Monster;
				break;
			case ReputationType.People:
				reputationType = ReputationType.Bandit;
				break;
			case ReputationType.Cleric:
				reputationType = ReputationType.People;
				break;
			case ReputationType.Mage:
				reputationType = ReputationType.Monster;
				break;
			}
		}
	}

	public Immunities immunity
	{
		get {
			Immunities imm = new Immunities ();

			switch (iRace) {
			case Race.Angel:
				imm = new Immunities (-50, 0, 0, 0, 0, 75);
				break;
			case Race.Devil:
				imm = new Immunities (0, 80, -100, 0, 0, -50);
				break;
			case Race.Elf:
				imm = new Immunities (-25, 0, 0, 0, 0, 45);
				break;
			case Race.Human:
				imm = new Immunities (0, 0, 0, 0, 0, 0);
				break;
			case Race.Orc:
				imm = new Immunities (35, 25, 0, 0, 25, -100);
				break;
			case Race.Vampire:
				imm = new Immunities (50, -100, 0, 0, 0, -100);
				break;
			case Race.Verwolf:
				imm = new Immunities (50, -50, -50, 0, 0, -50);
				break;
			case Race.Witch:
				imm = new Immunities (0, 0, 0, 0, 0, -50);
				break;
			}

			switch (iType) {
			case ClassType.Antimage:
				int m = 20 + 5 * level;
				imm += new Immunities (0, m, m, m, m, m);
				break;
			case ClassType.Monk:
				imm += new Immunities (5 * level, 0, 0, 0, 0, 0);
				break;
			}

			Immunities ar = new Immunities ();
			if (armor > -1) {
				ar = ItemsAsset.items [armor].bonuses;
			}
			Immunities we = new Immunities ();
			if (weapon > -1) {
				we = ItemsAsset.items [weapon].bonuses;
			}
			Immunities am = new Immunities ();
			if (amulet > -1) {
				am = ItemsAsset.items [amulet].bonuses;
			}

			return imm + ar + we + am;
		}
	}
	public bool CanUseItem (int itemID) {
		Item item = ItemsAsset.items [itemID];
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
			if (iRace == Race.Angel || iRace == Race.Witch) {
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

		string eff = "" + '\n';

		foreach (var item in effects) {
			eff += item.Info () + '\n';
		}

		info = info + eff;

		return info;
	}
	public int spellsToday
	{
		get {
			return sptd;
		}
		set {
			sptd = Mathf.Clamp(value, 0, 10);
		}
	}
	public void SetSpellsTodayBy () {
		int sp = 1;
		switch (iType) {
		case ClassType.Wonder:
			sp = (intellect) / 2 + level / 4;
			break;
		case ClassType.Thief:
			sp = (speedness + intellect) / 4 + level / 10;
			break;
		case ClassType.Monk:
			sp = (speedness + invicibility + strongness) / 6 + level / 5;
			break;
		}
		spellsToday = sp;
	}
	private int sptd = 2;
	public ReputationType reputationType = ReputationType.People;
	public Reputation reputation
	{
		get
		{
			Reputation rep = new Reputation (0, 0, 0, 0, 0, 0);

			if (reputationType == ReputationType.Bandit) {
				rep = Reputation.standart_bandit;
			}
			if (reputationType == ReputationType.Cleric) {
				rep = Reputation.standart_cleric;
			}
			if (reputationType == ReputationType.Guard) {
				rep = Reputation.standart_guard;
			}
			if (reputationType == ReputationType.Mage) {
				rep = Reputation.standart_mage;
			}
			if (reputationType == ReputationType.Monster) {
				rep = Reputation.standart_monster;
			}
			if (reputationType == ReputationType.People) {
				rep = Reputation.standart_citizen;
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
				Item it = ItemsAsset.items [items [i]];
				if (it.type == ItemType.Weapon) {
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
				damage = ItemsAsset.items [weapon].value + (strongness / 10) + level;
			} else {
				if (iType == ClassType.Monk) {
					damage = (strongness / 8) + level * 2;
				}
				if (iRace == Race.Verwolf) {
					damage = damage + level * 2;
				}
			}
			return damage;
		}
	}
	public bool canUseRunes
	{
		get {
			return iType == ClassType.Thief || iType == ClassType.Wonder;
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
				prot = ItemsAsset.items [armor].value;
			}
			if (amulet > -1) {
				prot += ItemsAsset.items [amulet].value;
			}
			prot = Mathf.Clamp (prot, 0, 10);
			return prot;
		}
	}
	public void ApplyDamage (int damage) {
		ApplyDamage (damage, DamageType.Just);
	}
	public int ApplyDamage (int damage, DamageType dmgType) {
		if (armor > -1) {
			damage = Mathf.Clamp (damage - armorProtection, 1, damage);
		}
		int imm = 0;
		switch (dmgType) {
		case DamageType.Air:
			imm = immunity.air;
			break;
		case DamageType.Water:
			imm = immunity.water;
			break;
		case DamageType.Fire:
			imm = immunity.fire;
			break;
		case DamageType.Earth:
			imm = immunity.earth;
			break;
		case DamageType.Magic:
			imm = immunity.magic;
			break;
		case DamageType.Melee:
			imm = immunity.melee;

			break;
		}
		imm = 100 - imm;
		damage = (damage * imm) / 100;
		health -= damage;
		return damage;
	}

	public void AddItem (int id) {
		List<int> its = items.ToList();
		its.Add(id);
		items = its.ToArray ();
	}
	public void RemoveItemAt (int index) {
		List<int> its = items.ToList();
		its.RemoveAt(index);
		items = its.ToArray ();
	}

	public void AddEffect (SkillEffect effect) {
		effects.Add (effect);
	}
	public void RemoveEffect (string name) {
		effects.Remove (effects.Find ((SkillEffect s) => s.effectName == name));
	}
	public void RemoveEffect (SkillEffect effect) {
		effects.Remove (effect);
	}

	public static ClassType[] MayBe (Race race) {

		ClassType[] types = new ClassType[0];

		if (race == Race.Angel) {
			ClassType[] n = {ClassType.Antimage, ClassType.Wonder, ClassType.Thief, ClassType.Monk, ClassType.Cleric, ClassType.Inquisitor, ClassType.Palladin};
			types = n;
		}
		if (race == Race.Devil) {
			ClassType[] n = {ClassType.DarknessSpirit, ClassType.Thief};
			types = n;
		}
		if (race == Race.Elf) {
			ClassType[] n = {ClassType.Antimage, ClassType.Wonder, ClassType.Thief, ClassType.Cleric, ClassType.Inquisitor};
			types = n;
		}
		if (race == Race.Human) {
			ClassType[] n = {ClassType.Antimage,
				ClassType.Wonder,
				ClassType.Thief,
				ClassType.Cleric,
				ClassType.DarknessSpirit,
				ClassType.Inquisitor,
				ClassType.Monk,
				ClassType.Palladin};
			types = n;
		}
		if (race == Race.Orc) {
			ClassType[] n = {ClassType.Antimage, ClassType.DarknessSpirit, ClassType.Thief, ClassType.Monk};
			types = n;
		}
		if (race == Race.Vampire) {
			ClassType[] n = {ClassType.DarknessSpirit, ClassType.Wonder, ClassType.Thief};
			types = n;
		}
		if (race == Race.Verwolf) {
			ClassType[] n = {ClassType.Antimage, ClassType.Monk, ClassType.Wonder, ClassType.Thief, ClassType.DarknessSpirit};
			types = n;
		}
		if (race == Race.Witch) {
			ClassType[] n = {ClassType.Wonder, ClassType.Thief, ClassType.Antimage, ClassType.DarknessSpirit, ClassType.Monk};
			types = n;
		}

		return types;
	}
	public static bool MayBeNow (Race race, ClassType type) {

		ClassType[] may = MayBe (race);

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
			ClassType ic = this.iType;
			int h = 1;
			if (ic == ClassType.Antimage) {
				h = 5 + 5 * level;
			}
			if (ic == ClassType.Thief) {
				h = 5 + 2 * level;
			}
			if (ic == ClassType.Cleric) {
				h = 5 + 4 * level;
			}
			if (ic == ClassType.DarknessSpirit) {
				h = 5 + 6 * level;
			}
			if (ic == ClassType.Inquisitor) {
				h = 5 + 6 * level;
			}
			if (ic == ClassType.Monk) {
				h = 5 + 7 * level;
			}
			if (ic == ClassType.Palladin) {
				h = 5 + 4 * level;
			}
			if (ic == ClassType.Wonder) {
				h = 5 + 1 * level;
			}

			return h;
		}
	}
	public bool HasItem (int id) {
		bool has = items.Contains<int> (id);
		return has;
	}
	public ClassType iType
	{
		get {
			return skills.iType;
		}
		set {
			skills.iType = value;
		}
	}
	public PersonView iPerson = new PersonView();
	public PersonView iPersonWithArmor
	{
		get
		{
			PersonView person = iPerson;
			if (armor > -1) {
				person.cloth_color = ItemsAsset.items [armor].color;
			}
			return person;
		}
	}
	public Race iRace = Race.Human;
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
public struct PersonView
{
	public SColor hair_color;
	public SColor cloth_color;
	public SColor cloth_more_color;
	public SColor skin_color;

	public static void SetToRenderer (Status status, Renderer rend) {

		PersonView person = status.iPersonWithArmor;

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
	public static void SetToManyRenderer (Status status, Renderer[] rends) {
		PersonView person = status.iPersonWithArmor;
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
public enum ClassType
{
	Simple,
	Monk,
	Cleric,
	Wonder,
	Palladin,
	Thief,
	Inquisitor,
	Antimage,
	DarknessSpirit
}
[System.Serializable]
public enum Race
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
public enum ReputationType
{
	People,
	Bandit,
	Guard,
	Monster,
	Mage,
	Cleric
}
[System.Serializable]
public class Reputation
{
	public Reputation (int people, int bandit, int guard, int monster, int mages, int clerics) {
		this.people = people;
		this.bandit = bandit;
		this.guard = guard;
		this.monster = monster;
		this.mages = mages;
		this.clerics = clerics;
	}

	public static int GetEnemity (Status _target, Status _attacker) {
		int delta = 0;

		if (_target.reputationType == ReputationType.Bandit) {
			delta = Mathf.Abs(1 - _attacker.reputation.bandit);
		}
		if (_target.reputationType == ReputationType.Cleric) {
			delta = Mathf.Abs(1 - _attacker.reputation.clerics);
		}
		if (_target.reputationType == ReputationType.Guard) {
			delta = Mathf.Abs(1 - _attacker.reputation.guard);
		}
		if (_target.reputationType == ReputationType.Mage) {
			delta = Mathf.Abs(1 - _attacker.reputation.mages);
		}
		if (_target.reputationType == ReputationType.Monster) {
			delta = Mathf.Abs(1 - _attacker.reputation.monster);
		}
		if (_target.reputationType == ReputationType.People) {
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

	public static Reputation standart_citizen
	{
		get
		{
			return new Reputation (1, -1, 1, -1, 0, 1);
		}
	}
	public static Reputation standart_bandit
	{
		get
		{
			return new Reputation (-1, 1, -1, -1, 0, 0);
		}
	}
	public static Reputation standart_mage
	{
		get
		{
			return new Reputation (0, 0, 0, -1, 1, 0);
		}
	}
	public static Reputation standart_cleric
	{
		get
		{
			return new Reputation (1, 0, 1, -1, 0, 1);
		}
	}
	public static Reputation standart_monster
	{
		get
		{
			return new Reputation (-1, -1, -1, -1, -1, -1);
		}
	}
	public static Reputation standart_guard
	{
		get
		{
			return new Reputation (1, -1, 1, -1, 0, 1);
		}
	}
}
[System.Serializable]
public enum DamageType
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
	public Status status = new Status(ClassType.Simple);
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
	private void Awake () {
		trans = transform;
		anims = GetComponent<Animator> ();
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		main_render = GetComponentInChildren<SkinnedMeshRenderer> ();
		charactersAll.Add (this);
	}
	public static List<ICharacter> charactersAll = new List<ICharacter>();
	public void AI_Update () {

		if (status.canUseRunes && status.rune < 0) {
			int rune = -1;
			for (int i = 0; i < status.items.Length; i++) {
				if (Item.IsSpell (status.items [i])) {
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
					//CastSpell (enemyNearby.trans.position);
				} else {
					MoveTo (enemyNearby.trans.position - trans.forward);
				}
			} else {
				Attack (enemyNearby);
			}
		}
	}
	public void CommitActionNow (Skill action) {
		if (SkillSystem.HasInDatabase(action.name)) {
			Vector3 dir = this == IControl.character ? IControl.cam.forward : trans.forward;
			switch (action.targetType) {
			case SkillTarget.Enemy:
				action.action (new SkillActionData (this, GetBest(dir)));
				break;
			case SkillTarget.Ally:
				action.action (new SkillActionData (this, GetBest(dir)));
				break;
			case SkillTarget.Usable:
				action.action (new SkillActionData (this, canUse));
				break;
			case SkillTarget.Self:
				action.action (new SkillActionData (this, this));
				break;
			}
			status.spellsToday -= 1;
		}
	}
	public void PrepareToGame () {

		weaponImage = GetComponentInChildren<RawImage> ();
		//weaponImage.rectTransform.parent.localEulerAngles = new Vector3(45, 180, -135);

		weaponImage.raycastTarget = false;

		PrepareRend ();

		if (!SGame.isNew || status.characterName == "Player") {
			agent.Warp (status.position);
			trans.eulerAngles = Vector3.up * status.euler_y;
		} else {
			status.health = status.maxHealth;
		}
		/*if (!GetComponent<IShadowCaster> ()) {
			gameObject.AddComponent<IShadowCaster> ();
		}*/
	}
	public void PrepareRend () {
		Renderer[] rends = GetComponentsInChildren<SkinnedMeshRenderer> ();

		if (rends.Length > 1) {
			PersonView.SetToManyRenderer (status, rends);
		} else {
			PersonView.SetToRenderer (status, main_render);
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
	public static ICharacter GetPlayer () {
		ICharacter finded = null;
		ICharacter[] chars = ICharacter.charactersAll.ToArray();
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
		ICharacter[] chars = charactersAll.ToArray();
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
		if (dead) {
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
			if (isPlayer) {
				return (IUsable.GetNearestToPositionAndWithDirection (trans.position, IControl.cam.forward));
			} else {
				return (IUsable.GetNearestToPosition (trans.position));
			}
		}
	}
	public ICharacter canTalk {
		get
		{
			ICharacter[] chars = charactersAll.ToArray();
			ICharacter finded = null;
			float dist = 1.5f;

			for (int i = 0; i < chars.Length; i++) {
				float dist_cur = (chars [i].trans.position - trans.position).magnitude;
				if (dist_cur < dist && chars[i].status.name != "Player" && !(Reputation.GetEnemity(chars[i].status, status) > 1)) {
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
			if (whom) {
				lookVector = whom.trans.position;
			}
			Invoke ("Attack_Damage_Set", 0.5f);
			float asp = (10f / (status.speedness + status.strongness));
			Invoke ("Attack_End", 2.5f + asp);
			Animate ();
			if (!dead) {
				anims.Play ("Combat");
			}
			Stop ();

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
			if (!inCombat) {
				l = value;
				if (IsInvoking("LookToSettedDirection")) {
					CancelInvoke ("LookToSettedDirection");
				}
				LookToSettedDirection ();
			}
		}
	}
	private void LookToSettedDirection () {
		Vector3 v = lookVector;
		v = new Vector3 (v.x, trans.position.y, v.z);
		v = v - trans.position;
		if (v.magnitude > 0) {
			Quaternion next = Quaternion.LookRotation (v);
			trans.rotation = Quaternion.Slerp (trans.rotation, next, 0.4f);
			if (Vector3.Angle (trans.forward, v) < 5) {
				CancelInvoke ("LookToSettedDirection");
			} else {
				Invoke ("LookToSettedDirection", 0.1f);
			}
		}
	}
	private void Attack_End () {
		return;
	}
	public bool inCombat
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
		int procent = 0;
		int rnd = 0;
		int dmg = 0;
		switch (status.iType) {
		case ClassType.Monk:
			procent = 20 + 5 * status.level;
			rnd = UnityEngine.Random.Range (0, 100);
			if (rnd < procent) {
				DamageType type = (DamageType)UnityEngine.Random.Range (0, 5);
				dmg = status.damage_melee;
				whom.ApplyDamage (dmg, type);
			}
			break;
		case ClassType.Inquisitor:
			dmg = 3 * status.level;
			procent = 25;
			rnd = UnityEngine.Random.Range (0, 100);
			if (rnd < procent) {
				whom.ApplyDamage (dmg, DamageType.Fire);
			}
			break;
		case ClassType.Palladin:
			dmg = 5 * status.level;
			procent = 15;
			rnd = UnityEngine.Random.Range (0, 100);
			if (rnd < procent) {
				whom.ApplyDamage (dmg, DamageType.Magic);
			}
			break;
		}
		foreach (var item in status.effects) {
			item.preAttack (whom);
		}
	}
	private void Attack_Damage_Set () {
		if (!inReact) {
			NavMeshHit hit;
			if (tacked && !agent.Raycast(tacked.trans.position, out hit)) {
				Vector3 delta = tacked.trans.position - trans.position;
				if (Vector3.Angle(trans.forward, delta.normalized) < 20 && delta.magnitude < 1.75f) {
					tacked.ApplyDamage (status.damage_melee, DamageType.Melee);
					Attack_SpetialDamage_Set (tacked);
					status.SetDamageLawEvent (tacked.status);
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
		ApplyDamage (damage, DamageType.Just);
	}
	public void ApplyDamage (int damage, DamageType dmgType) {
		react_clip_num = UnityEngine.Random.Range (0.0f, 1.0f);
		combatState = 5;
		if (this) {
			CancelInvoke ("Attack_Damage_Set");
			CancelInvoke ("Attack_End");
			Invoke ("EndReact", 1);
		}
		Animate ();
		if (!dead) {
			anims.Play ("Combat");
		}
		int app = status.ApplyDamage (damage, dmgType);
		Color clr = Color.yellow;

		if (dmgType == DamageType.Melee && app > 0) {
			DropBlood ();
		}

		switch (dmgType) {
		case DamageType.Air:
			clr = Color.gray;
			break;
		case DamageType.Earth:
			clr = new Color (0.8f, 0.2f, 0.2f);
			break;
		case DamageType.Fire:
			clr = new Color (0.9f, 0.5f, 0.5f);
			break;
		case DamageType.Magic:
			clr = Color.cyan;
			break;
		case DamageType.Melee:
			clr = Color.red;
			break;
		case DamageType.Water:
			clr = Color.blue;
			break;
		}
		if (app > 0) {
			MyDebug.Log ("Получен урон (" + app + ")", clr, this);
		}
		foreach (var item in status.effects) {
			item.onDamaged (this);
		}
	}
	private void DropBlood () {
		GameObject pref = Resources.Load<GameObject> ("Prefabs/Blood");
		GameObject blood = Instantiate (pref, trans.position, Quaternion.identity);
		Destroy (blood, 6f);
	}
	public static float GetSpellClipFloat (ISpellType type) {
		float clip = 0;
		switch (type) {
		case ISpellType.Target:
			clip = 1;
			break;
		case ISpellType.Radian:
			clip = 2;
			break;
		}
		return clip / 12f;
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
		weaponImage.texture = ItemsAsset.LoadTexture (status.weapon);
	}

	public bool dead
	{
		get {
			return !(status.health > 0);
		}
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
			if (usable is ILocationTransition) {
				((ILocationTransition)usable).Use (this);
			}
		}
	}
	public void OpenDoor (IDoor door) {
		door.Use (this);
	}
	public void OnDestroy () {
		charactersAll.Remove (this);
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
		ICharacter[] all = charactersAll.ToArray();
		float dist = 15;
		finded = all.Where ((ICharacter ch) => (ch.trans.position - trans.position).magnitude < dist &&
		ch != this).
		Where ((ICharacter ch) => Reputation.GetEnemity (ch.status, status) > 1).
		Where ((ICharacter ch) => !Physics.Linecast (trans.position, ch.trans.position)).
		OrderBy ((ICharacter ch) => (ch.trans.position - trans.position).magnitude).FirstOrDefault ();
		return finded;
	}
	public Vector3 position
	{
		get {
			return trans.position + IControl.headHeight;
		}
	}
	public ICharacter GetBest (Vector3 direction) {
		ICharacter[] chars = ICharacter.charactersAll.ToArray ();
		ICharacter f = null;

		float dist = 2;
		float angle = 60;

		Vector3 pos = position;

		f = chars.Where ((ICharacter arg) => arg != this && ((arg.position - pos).magnitude) < dist &&
			Vector3.Angle ((arg.position - pos), direction) < angle
			&&
			!Physics.Linecast (pos, arg.position, LayerMask.GetMask ("Default")))
			.OrderBy ((ICharacter arg) => ((pos - arg.position).magnitude)).OrderBy (
				(ICharacter arg) => Vector3.Angle ((arg.position - pos), direction)).FirstOrDefault();

		return f;
	}
	public ICharacter GetNearestAlly () {
		ICharacter finded = null;
		ICharacter[] all = charactersAll.ToArray();
		float dist = 15;
		finded = all.Where ((ICharacter ch) => (ch.trans.position - trans.position).magnitude < dist &&
			ch != this).
			Where ((ICharacter ch) => !(Reputation.GetEnemity (ch.status, status) > 1)).
			Where ((ICharacter ch) => !Physics.Linecast (trans.position, ch.trans.position)).
			OrderBy ((ICharacter ch) => (ch.trans.position - trans.position).magnitude).FirstOrDefault ();
		return finded;
	}
	public static ICharacter GetNearestFromPoint (Vector3 point, ICharacter mask, float maxDist) {
		ICharacter finded = null;
		ICharacter[] all = charactersAll.ToArray();
		float dist = maxDist;
		finded = all.Where ((ICharacter ch) => (ch.trans.position - point).magnitude < dist && ch != mask).
			OrderBy ((ICharacter ch) => (ch.trans.position - point).magnitude).FirstOrDefault ();
		return finded;
	}
	public static ICharacter[] GetNearestFromPointAll (Vector3 point, ICharacter mask, float maxDist) {
		ICharacter[] all = charactersAll.ToArray();
		float dist = maxDist;
		all = all.Where ((ICharacter ch) => (ch.trans.position - point).magnitude < dist && ch != mask).ToArray ();
		return all;
	}
	public void AttackNearest (Vector3 point) {
		ICharacter nearest = GetNearestFromPoint(point, this, 1.25f);
		if (!nearest) {
			lookVector = point;
		}
		Attack (nearest);
	}
	private void Spell_End () {
		return;
	}
	public void CastSpell (ICharacter ch, ISpellType type, ISpellEffect eff) {
		if (!inCombat) {
			if (status.rune > -1) {
				Vector3 point = ch ? ch.trans.position : trans.position;
				Item it = ItemsAsset.items [status.rune];
				ISpellRune r = Item.FromItemType (it.type);
				ISpell sp = new ISpell (type, eff, r, status.damage_spell, 5);
				ISpellData.spells [it.value - 1].onCastSpell.Invoke (this, ch, point, sp);
				lookVector = point;
				Invoke ("Spell_End", 1.5f);
				Animate ();
				if (!dead) {
					anims.Play ("Combat");
				}
				spell_clip_num = GetSpellClipFloat (sp.spellType);
			}
		}
	}
	public void AttackFromDirection () {
		AttackNearest (trans.position + trans.forward);
	}
	public void AttackFromDirection (Vector3 direction) {
		AttackNearest (trans.position + direction);
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
	public void CreateRune (ItemType itemType) {
		if (status.spellsToday > 0) {
			int itemIndex = 0;
			switch (itemType) {
			case ItemType.ScrollOfAir:
				itemIndex = 8;
				break;
			case ItemType.ScrollOfEarth:
				itemIndex = 7;
				break;
			case ItemType.ScrollOfFire:
				itemIndex = 5;
				break;
			case ItemType.ScrollOfWater:
				itemIndex = 6;
				break;
			case ItemType.ScrollOfGod:
				itemIndex = 9;
				break;
			}
			AddItem (itemIndex);
		}
	}
	public void UseItem (int index) {

		Item item = ItemsAsset.items [status.items [index]];

		switch (item.type) {
		case ItemType.Amulet :
			status.amulet = status.items[index];
			break;
		case ItemType.Food :

			break;
		case ItemType.Weapon :
			if (status.weapon > -1) {
				AddItem (status.weapon);
			}
			status.weapon = status.items[index];
			break;
		case ItemType.Armor :
			if (status.armor > -1) {
				AddItem (status.armor);
			}
			status.armor = status.items[index];
			break;
		case ItemType.ScrollOfAir :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		case ItemType.ScrollOfEarth :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		case ItemType.ScrollOfFire :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		case ItemType.ScrollOfGod :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		case ItemType.ScrollOfWater :
			if (status.rune > -1) {
				AddItem (status.rune);
			}
			status.rune = status.items[index];
			break;
		}
		if (item.type != ItemType.None) {
			RemoveItem (index);
		}
	}
	public void RemoveItem (int index) {
		status.RemoveItemAt (index);
	}
	public void DropItem (int index) {

		GameObject prefab = (GameObject)Resources.Load ("Prefabs/ItemObject");

		Vector3 rp = UnityEngine.Random.insideUnitSphere;
		Vector3 rr = UnityEngine.Random.onUnitSphere;

		rp = new Vector3 (rp.x, 0, rp.z);
		rr = new Vector3 (rr.x, 0, rr.z).normalized / 2;

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
			status.AddItem (id);
		}
	}
	public void BackToInv (ItemType type) {
		switch (type) {
		case ItemType.Amulet:
			AddItem (status.amulet);
			status.amulet = -1;
			break;
		case ItemType.Armor:
			AddItem (status.armor);
			status.armor = -1;
			break;
		case ItemType.Weapon:
			AddItem (status.weapon);
			status.weapon = -1;
			break;
		case ItemType.ScrollOfAir:
			AddItem (status.rune);
			status.rune = -1;
			break;
		case ItemType.ScrollOfEarth:
			AddItem (status.rune);
			status.rune = -1;
			break;
		case ItemType.ScrollOfFire:
			AddItem (status.rune);
			status.rune = -1;
			break;
		case ItemType.ScrollOfGod:
			AddItem (status.rune);
			status.rune = -1;
			break;
		case ItemType.ScrollOfWater:
			AddItem (status.rune);
			status.rune = -1;
			break;
		}
	}
}