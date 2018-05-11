using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public enum ItemType
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
public struct Item
{
	public int sellCount;
	public int value;
	public string name;
	public string intro;
	public ItemType type;
	public SColor color;
	public ClassType[] spetiality;

	public Immunities bonuses;

	public static bool IsSpell (int index) {
		Item it = ItemsAsset.items [index];
		return it.type == ItemType.ScrollOfAir ||
		it.type == ItemType.ScrollOfWater ||
		it.type == ItemType.ScrollOfFire ||
		it.type == ItemType.ScrollOfEarth ||
		it.type == ItemType.ScrollOfGod;
	}

	public static ISpellRune FromItemType (ItemType itemType) {
		ISpellRune r = ISpellRune.Fire;
		switch (itemType) {
		case ItemType.ScrollOfFire:
			r = ISpellRune.Fire;
			break;
		case ItemType.ScrollOfAir:
			r = ISpellRune.Air;
			break;
		case ItemType.ScrollOfEarth:
			r = ISpellRune.Earth;
			break;
		case ItemType.ScrollOfGod:
			r = ISpellRune.Gods;
			break;
		case ItemType.ScrollOfWater:
			r = ISpellRune.Water;
			break;
		}
		return r;
	}

	public string Info () {
		string param = "";
		switch (type) {
		case ItemType.Amulet:
			param = "Уровень защиты : ";
			break;
		case ItemType.Armor:
			param = "Уровень защиты : ";
			break;
		case ItemType.EmptyScroll:
			param = "";
			break;
		case ItemType.Food:
			param = "Минус к голоду :";
			break;
		case ItemType.ScrollOfAir:
			param = "Уровень заклинания : ";
			break;
		case ItemType.ScrollOfEarth:
			param = "Уровень заклинания : ";
			break;
		case ItemType.ScrollOfFire:
			param = "Уровень заклинания : ";
			break;
		case ItemType.ScrollOfGod:
			param = "Уровень заклинания : ";
			break;
		case ItemType.ScrollOfWater:
			param = "Уровень заклинания : ";
			break;
		case ItemType.Weapon:
			param = "Урон : ";
			break;
		}
		string t = "" + name + '\n' + "(" + type + ")" + '\n' +
		           param + value + '\n' +
		           "Цена предмета : " + sellCount + '\n' + "Описание предмета : " +
		           ItemsAsset.GetIntro (ItemsAsset.GetIDByName (name)) + '\n' +
		           "Бонусы : " + bonuses.ToText ();
		return t;
	}

	public static ClassType[] Spetialities (params ClassType[] tps) {
		return tps;
	}

	public Item (int sellCount, int value, string name, ItemType type)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = new Immunities ();
		this.color = new SColor (1, 1, 1);
		spetiality = new ClassType[0];
	}
	public Item (int sellCount, int value, string name, ItemType type, ClassType[] spetial)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = new Immunities ();
		this.color = new SColor (1, 1, 1);
		this.spetiality = spetial;
	}
	public Item (int sellCount, int value, string name, ItemType type, SColor clr)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = new Immunities ();
		this.color = clr;
		spetiality = new ClassType[0];
	}
	public Item (int sellCount, int value, string name, ItemType type, SColor clr, ClassType[] spetial)
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
	public Item (int sellCount, int value, string name, ItemType type, Immunities bons)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = bons;
		this.color = new SColor (1, 1, 1);
		this.spetiality = new ClassType[0];
	}
	public Item (int sellCount, int value, string name, ItemType type, Immunities bons, ClassType[] spetial)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = bons;
		this.color = new SColor (1, 1, 1);
		this.spetiality = spetial;
	}
	public Item (int sellCount, int value, string name, ItemType type, Immunities bons, SColor clr)
	{
		this.name = name;
		this.sellCount = sellCount;
		this.type = type;
		this.value = value;
		this.intro = "";
		this.bonuses = bons;
		this.color = clr;
		this.spetiality = new ClassType[0];
	}
	public Item (int sellCount, int value, string name, ItemType type, Immunities bons, SColor clr, ClassType[] spetial)
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
public class ItemsAsset
{

	public static int[] GetStartKit (Status stats) {
		List<int> toAdd = new List<int> ();

		if (stats.canUseRunes) {
			toAdd.Add (Random.Range(5, 8));
		}
		switch (stats.iType) {
		case ClassType.Antimage:
			toAdd.Add (Random.Range(0, 2));
			toAdd.Add (16);
			break;
		case ClassType.Thief:
			toAdd.Add (Random.Range(0, 2));
			toAdd.Add (16);
			break;
		case ClassType.Cleric:
			toAdd.Add (13);
			toAdd.Add (14);
			toAdd.Add (3);
			break;
		case ClassType.DarknessSpirit:
			toAdd.Add (Random.Range (0, 2));
			toAdd.Add (16);
			break;
		case ClassType.Inquisitor:
			toAdd.Add (Random.Range (0, 2));
			toAdd.Add (17);
			break;
		case ClassType.Monk:
			toAdd.Add (4);
			break;
		case ClassType.Palladin:
			toAdd.Add (Random.Range (0, 2));
			toAdd.Add (17);
			break;
		case ClassType.Wonder:
			for (int i = 0; i < Random.Range (2, 5); i++) {
				toAdd.Add (Random.Range (5, 8));
			}
			toAdd.Add (13);
			toAdd.Add (15);
			break;
		}

		return toAdd.ToArray ();
	}

	public static string[] GetNames (Item[] items) {
		List<string> a = new List<string> ();
		a.Add ("None");
		a.AddRange (
			items.Select ((Item item) => item.name)
		);
		return a.ToArray ();
	}

	public static Item[] GetOfType (params ItemType[] types) {
		return items.Where ((Item it) => types.Contains (it.type)).ToArray();
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
		Item[] its = items;
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

	public static Item[] items
	{
		get
		{
			List<Item> itemsList = new List<Item>();

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

	public static Item Item_0
	{
		get
		{
			ClassType[] clt = {ClassType.Antimage, ClassType.Thief, ClassType.DarknessSpirit, ClassType.Inquisitor, ClassType.Palladin};
			return new Item(5, 5, "Стальной меч", ItemType.Weapon, clt);
		}
	}
	public static Item Item_1
	{
		get
		{
			ClassType[] clt = {ClassType.Antimage, ClassType.Thief, ClassType.DarknessSpirit, ClassType.Inquisitor, ClassType.Palladin};
			return new Item(11, 6, "Стальная секира", ItemType.Weapon, clt);
		}
	}
	public static Item Item_2
	{
		get
		{
			return new Item(1, 5, "Яблоко", ItemType.Food);
		}
	}
	public static Item Item_3
	{
		get
		{
			return new Item(350, 8, "Божий оберег", ItemType.Amulet, new Immunities(25, 25, 25, 25, 25, 100));
		}
	}
	public static Item Item_4
	{
		get
		{
			ClassType[] clt = {ClassType.Monk};
			return new Item(25, 2, "Плащ монаха", ItemType.Armor, new SColor(0.1f, 0.1f, 0.1f), clt);
		}
	}
	public static Item Item_5
	{
		get
		{
			ClassType[] clt = { ClassType.Wonder, ClassType.Thief };
			return new Item(20, 1, "Руна огня (первый уровень)", ItemType.ScrollOfFire, clt);
		}
	}
	public static Item Item_6
	{
		get
		{
			ClassType[] clt = { ClassType.Wonder, ClassType.Thief };
			return new Item(20, 1, "Руна воды (первый уровень)", ItemType.ScrollOfWater, clt);
		}
	}
	public static Item Item_7
	{
		get
		{
			ClassType[] clt = { ClassType.Wonder, ClassType.Thief };
			return new Item(20, 1, "Руна земли (первый уровень)", ItemType.ScrollOfEarth, clt);
		}
	}
	public static Item Item_8
	{
		get
		{
			ClassType[] clt = { ClassType.Wonder, ClassType.Thief };
			return new Item(20, 1, "Руна ветра (первый уровень)", ItemType.ScrollOfAir, clt);
		}
	}
	public static Item Item_9
	{
		get
		{
			ClassType[] clt = { ClassType.Wonder, ClassType.Thief };
			return new Item(20, 1, "Руна бога (первый уровень)", ItemType.ScrollOfGod, clt);
		}
	}
	public static Item Item_10
	{
		get
		{
			ClassType[] clt = {ClassType.Antimage, ClassType.Thief, ClassType.DarknessSpirit, ClassType.Inquisitor, ClassType.Palladin};
			return new Item(150, 25, "Секира богов", ItemType.Weapon, new Immunities(0, 15, 15, -15, -15, 25), clt);
		}
	}
	public static Item Item_11
	{
		get
		{
			ClassType[] clt = {ClassType.Antimage, ClassType.Thief, ClassType.DarknessSpirit, ClassType.Inquisitor, ClassType.Palladin};
			return new Item(200, 27, "Клинок богов", ItemType.Weapon, new Immunities(10, 15, 15, 15, 15, 25), clt);
		}
	}
	public static Item Item_12
	{
		get
		{
			ClassType[] clt = { ClassType.Wonder, ClassType.Thief };
			return new Item(5, 0, "Незаряженная руна", ItemType.EmptyScroll, clt);
		}
	}
	public static Item Item_13
	{
		get
		{
			return new Item(3, 4, "Кинжал", ItemType.Weapon);
		}
	}
	public static Item Item_14
	{
		get
		{
			ClassType[] clt = { ClassType.Cleric };
			return new Item(65, 2, "Ряса священника", ItemType.Armor, new Immunities(10, 10, 10, 10, 10, 25), new SColor(0.1f, 0.1f, 0.5f), clt);
		}
	}
	public static Item Item_15
	{
		get
		{
			ClassType[] clt = { ClassType.Wonder, ClassType.Thief, ClassType.Cleric };
			return new Item(115, 1, "Ряса мага", ItemType.Armor, new Immunities(30, -5, -5, -5, -5, -15), new SColor(0.6f, 0.1f, 0.1f), clt);
		}
	}
	public static Item Item_16
	{
		get
		{
			ClassType[] clt = {ClassType.Antimage, ClassType.Thief, ClassType.DarknessSpirit, ClassType.Inquisitor, ClassType.Palladin};
			return new Item(250, 4, "Кираса солдата", ItemType.Armor, new Immunities(15, 0, 0, 0, 0, 0), new SColor(0.6f, 0.6f, 0.6f), clt);
		}
	}
	public static Item Item_17
	{
		get
		{
			ClassType[] clt = {ClassType.Thief, ClassType.DarknessSpirit, ClassType.Inquisitor, ClassType.Palladin};
			return new Item(450, 4, "Латы палладина", ItemType.Armor, new Immunities(15, 25, 25, 25, 25, 75), new SColor(0.8f, 0.8f, 0.8f), clt);
		}
	}
	public static Item Item_18
	{
		get
		{
			return new Item(0, 0, "Ключ от подземелья", ItemType.None);
		}
	}
}