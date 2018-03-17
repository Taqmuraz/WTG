using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum IItemType
{
	Amulet,
	Armor,
	Food,
	ScrollOfWater,
	ScrollOfEarth,
	ScrollOfFire,
	ScrollOfAir,
	ScrollOfGod,
	Weapon,
	EmptyScroll,
	None
}

[System.Serializable]
public class IItem
{
	public int sellCount = 1;
	public int value = 1;
	public string name = "unknown";
	public string intro = "nothing";
	public IItemType type;
	public IColor color = new IColor (1, 1, 1);
	public IClassType[] spetiality = new IClassType[0];

	public Immunities bonuses = new Immunities();

	public static bool IsSpell (int index) {
		IItem it = IItemAsset.items [index];
		return it.type == IItemType.ScrollOfAir ||
		it.type == IItemType.ScrollOfWater ||
		it.type == IItemType.ScrollOfFire ||
		it.type == IItemType.ScrollOfEarth ||
		it.type == IItemType.ScrollOfGod;
	}

	public static ISpellRune FromItemType (IItemType itemType) {
		ISpellRune r = ISpellRune.Fire;
		switch (itemType) {
		case IItemType.ScrollOfFire:
			r = ISpellRune.Fire;
			break;
		case IItemType.ScrollOfAir:
			r = ISpellRune.Air;
			break;
		case IItemType.ScrollOfEarth:
			r = ISpellRune.Earth;
			break;
		case IItemType.ScrollOfGod:
			r = ISpellRune.Gods;
			break;
		case IItemType.ScrollOfWater:
			r = ISpellRune.Water;
			break;
		}
		return r;
	}

	public string Info () {
		string param = "";
		switch (type) {
		case IItemType.Amulet:
			param = "Уровень защиты : ";
			break;
		case IItemType.Armor:
			param = "Уровень защиты : ";
			break;
		case IItemType.EmptyScroll:
			param = "";
			break;
		case IItemType.Food:
			param = "Минус к голоду :";
			break;
		case IItemType.ScrollOfAir:
			param = "Уровень заклинания : ";
			break;
		case IItemType.ScrollOfEarth:
			param = "Уровень заклинания : ";
			break;
		case IItemType.ScrollOfFire:
			param = "Уровень заклинания : ";
			break;
		case IItemType.ScrollOfGod:
			param = "Уровень заклинания : ";
			break;
		case IItemType.ScrollOfWater:
			param = "Уровень заклинания : ";
			break;
		case IItemType.Weapon:
			param = "Урон : ";
			break;
		}
		string t = "" + name + '\n' + "(" + type + ")" + '\n' +
		           param + value + '\n' +
		           "Цена предмета : " + sellCount + '\n' + "Описание предмета : " +
		           IItemAsset.GetIntro (IItemAsset.GetIDByName (name)) + '\n' +
		           "Бонусы : " + bonuses.ToText ();
		return t;
	}

	public IItem (int sellCount, int value, string name, IItemType type)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = new Immunities ();
		this.color = new IColor (1, 1, 1);
	}
	public IItem (int sellCount, int value, string name, IItemType type, IClassType[] spetial)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = new Immunities ();
		this.color = new IColor (1, 1, 1);
		this.spetiality = spetial;
	}
	public IItem (int sellCount, int value, string name, IItemType type, IColor clr)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = new Immunities ();
		this.color = clr;
	}
	public IItem (int sellCount, int value, string name, IItemType type, IColor clr, IClassType[] spetial)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = new Immunities ();
		this.color = clr;
		this.spetiality = spetial;
	}
	public IItem (int sellCount, int value, string name, IItemType type, Immunities bons)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = bons;
		this.color = new IColor (1, 1, 1);
	}
	public IItem (int sellCount, int value, string name, IItemType type, Immunities bons, IClassType[] spetial)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = bons;
		this.color = new IColor (1, 1, 1);
		this.spetiality = spetial;
	}
	public IItem (int sellCount, int value, string name, IItemType type, Immunities bons, IColor clr)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = bons;
		this.color = clr;
	}
	public IItem (int sellCount, int value, string name, IItemType type, Immunities bons, IColor clr, IClassType[] spetial)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = bons;
		this.color = clr;
		this.spetiality = spetial;
	}
}

[System.Serializable]
public class IItemAsset
{

	public static int[] GetStartKit (IStatus stats) {
		List<int> toAdd = new List<int> ();

		if (stats.canUseRunes) {
			toAdd.Add (Random.Range(5, 8));
		}
		switch (stats.iType) {
		case IClassType.Antimage:
			toAdd.Add (Random.Range(0, 2));
			toAdd.Add (16);
			break;
		case IClassType.Bard:
			toAdd.Add (Random.Range(0, 2));
			toAdd.Add (16);
			break;
		case IClassType.Cleric:
			toAdd.Add (13);
			toAdd.Add (14);
			toAdd.Add (3);
			break;
		case IClassType.DarknessSpirit:
			toAdd.Add (Random.Range (0, 2));
			toAdd.Add (16);
			break;
		case IClassType.Inquisitor:
			toAdd.Add (Random.Range (0, 2));
			toAdd.Add (17);
			break;
		case IClassType.Monk:
			toAdd.Add (4);
			break;
		case IClassType.Palladin:
			toAdd.Add (Random.Range (0, 2));
			toAdd.Add (17);
			break;
		case IClassType.Wonder:
			for (int i = 0; i < Random.Range (2, 5); i++) {
				toAdd.Add (Random.Range (5, 8));
			}
			toAdd.Add (13);
			toAdd.Add (15);
			break;
		}

		return toAdd.ToArray ();
	}

	public static int[] GetAllInventory ()
	{
		int[] ints = new int[items.Length];
		for (int i = 0; i < ints.Length; i++) {
			ints[i] = i;
		}
		return ints;
	}

	public static int GetIDByName (string name) {
		int id = -1;
		IItem[] its = items;
		for (int i = 0; i < its.Length; i++) {
			if (its[i].name == name) {
				id = i;
			}
		}
		return id;
	}

	public static Texture LoadTexture (int id) {
		return (Texture)Resources.Load ("Sprites/Inventory/" + id);
	}

	public static string GetIntro (int index) {
		TextAsset asset = (TextAsset)Resources.Load ("Text/Items/Items");
		string all = asset.text;
		int symbol = 0;
		int part = 0;

		while (part < index) {

			while (all[symbol] != '*') {
				symbol++;
			}
			symbol++;
			part++;
		}

		string st = "";

		while (symbol < all.Length && all[symbol] != '*') {
			st = "" + st + all [symbol];
			symbol++;
		}
		return st;
	}

	public static IItem[] items
	{
		get
		{
			List<IItem> itemsList = new List<IItem>();

			itemsList.Add(Item_0);
			itemsList.Add(Item_1);
			itemsList.Add(Item_2);
			itemsList.Add(Item_3);
			itemsList.Add(Item_4);
			itemsList.Add(Item_5);
			itemsList.Add(Item_6);
			itemsList.Add(Item_7);
			itemsList.Add(Item_8);
			itemsList.Add(Item_9);
			itemsList.Add(Item_10);
			itemsList.Add(Item_11);
			itemsList.Add(Item_12);
			itemsList.Add(Item_13);
			itemsList.Add(Item_14);
			itemsList.Add(Item_15);
			itemsList.Add(Item_16);
			itemsList.Add(Item_17);
			itemsList.Add(Item_18);

			return itemsList.ToArray();
		}
	}

	public static IItem Item_0
	{
		get
		{
			IClassType[] clt = {IClassType.Antimage, IClassType.Bard, IClassType.DarknessSpirit, IClassType.Inquisitor, IClassType.Palladin};
			return new IItem(5, 5, "Стальной меч", IItemType.Weapon, clt);
		}
	}
	public static IItem Item_1
	{
		get
		{
			IClassType[] clt = {IClassType.Antimage, IClassType.Bard, IClassType.DarknessSpirit, IClassType.Inquisitor, IClassType.Palladin};
			return new IItem(11, 6, "Стальная секира", IItemType.Weapon, clt);
		}
	}
	public static IItem Item_2
	{
		get
		{
			return new IItem(1, 5, "Яблоко", IItemType.Food);
		}
	}
	public static IItem Item_3
	{
		get
		{
			return new IItem(350, 8, "Божий оберег", IItemType.Amulet, new Immunities(25, 25, 25, 25, 25, 100));
		}
	}
	public static IItem Item_4
	{
		get
		{
			IClassType[] clt = {IClassType.Monk};
			return new IItem(25, 2, "Плащ монаха", IItemType.Armor, new IColor(0.1f, 0.1f, 0.1f), clt);
		}
	}
	public static IItem Item_5
	{
		get
		{
			IClassType[] clt = { IClassType.Wonder, IClassType.Bard };
			return new IItem(20, 1, "Руна огня (первый уровень)", IItemType.ScrollOfFire, clt);
		}
	}
	public static IItem Item_6
	{
		get
		{
			IClassType[] clt = { IClassType.Wonder, IClassType.Bard };
			return new IItem(20, 1, "Руна воды (первый уровень)", IItemType.ScrollOfWater, clt);
		}
	}
	public static IItem Item_7
	{
		get
		{
			IClassType[] clt = { IClassType.Wonder, IClassType.Bard };
			return new IItem(20, 1, "Руна земли (первый уровень)", IItemType.ScrollOfEarth, clt);
		}
	}
	public static IItem Item_8
	{
		get
		{
			IClassType[] clt = { IClassType.Wonder, IClassType.Bard };
			return new IItem(20, 1, "Руна ветра (первый уровень)", IItemType.ScrollOfAir, clt);
		}
	}
	public static IItem Item_9
	{
		get
		{
			IClassType[] clt = { IClassType.Wonder, IClassType.Bard };
			return new IItem(20, 1, "Руна бога (первый уровень)", IItemType.ScrollOfGod, clt);
		}
	}
	public static IItem Item_10
	{
		get
		{
			IClassType[] clt = {IClassType.Antimage, IClassType.Bard, IClassType.DarknessSpirit, IClassType.Inquisitor, IClassType.Palladin};
			return new IItem(150, 25, "Секира богов", IItemType.Weapon, new Immunities(0, 15, 15, -15, -15, 25), clt);
		}
	}
	public static IItem Item_11
	{
		get
		{
			IClassType[] clt = {IClassType.Antimage, IClassType.Bard, IClassType.DarknessSpirit, IClassType.Inquisitor, IClassType.Palladin};
			return new IItem(200, 27, "Клинок богов", IItemType.Weapon, new Immunities(10, 15, 15, 15, 15, 25), clt);
		}
	}
	public static IItem Item_12
	{
		get
		{
			IClassType[] clt = { IClassType.Wonder, IClassType.Bard };
			return new IItem(5, 0, "Незаряженная руна", IItemType.EmptyScroll, clt);
		}
	}
	public static IItem Item_13
	{
		get
		{
			return new IItem(3, 4, "Кинжал", IItemType.Weapon);
		}
	}
	public static IItem Item_14
	{
		get
		{
			IClassType[] clt = { IClassType.Cleric };
			return new IItem(65, 2, "Ряса священника", IItemType.Armor, new Immunities(10, 10, 10, 10, 10, 25), new IColor(0.1f, 0.1f, 0.5f), clt);
		}
	}
	public static IItem Item_15
	{
		get
		{
			IClassType[] clt = { IClassType.Wonder, IClassType.Bard, IClassType.Cleric };
			return new IItem(115, 1, "Ряса мага", IItemType.Armor, new Immunities(30, -5, -5, -5, -5, -15), new IColor(0.6f, 0.1f, 0.1f), clt);
		}
	}
	public static IItem Item_16
	{
		get
		{
			IClassType[] clt = {IClassType.Antimage, IClassType.Bard, IClassType.DarknessSpirit, IClassType.Inquisitor, IClassType.Palladin};
			return new IItem(250, 4, "Кираса солдата", IItemType.Armor, new Immunities(15, 0, 0, 0, 0, 0), new IColor(0.6f, 0.6f, 0.6f), clt);
		}
	}
	public static IItem Item_17
	{
		get
		{
			IClassType[] clt = {IClassType.Bard, IClassType.DarknessSpirit, IClassType.Inquisitor, IClassType.Palladin};
			return new IItem(450, 4, "Латы палладина", IItemType.Armor, new Immunities(15, 25, 25, 25, 25, 75), new IColor(0.8f, 0.8f, 0.8f), clt);
		}
	}
	public static IItem Item_18
	{
		get
		{
			return new IItem(0, 0, "Ключ от подземелья", IItemType.None);
		}
	}
}