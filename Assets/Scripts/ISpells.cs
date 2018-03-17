using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ISpellType
{
	Point,
	Target,
	Radian
}
[System.Serializable]
public enum ISpellEffect
{
	Ball,
	Area,
	Wall
}
[System.Serializable]
public enum ISpellRune
{
	Fire,
	Water,
	Air,
	Earth,
	Gods
}
[System.Serializable]
public struct ISpellData
{
	public delegate void CastSpell (ICharacter caster, ICharacter target, Vector3 destination, ISpell spell);
	public CastSpell onCastSpell;
	public static ISpellData[] spells
	{
		get {
			List<ISpellData> spells = new List<ISpellData> ();
			ISpellData simple = new ISpellData ();
			simple.onCastSpell = new CastSpell ((ICharacter caster, ICharacter target, Vector3 destination, ISpell spell) => {
				if (target != null) {
					ISpell.InitializeSpell(spell, caster, target);
				} else {
					ISpell.InitializeSpell(spell, caster, destination);
				}
			});
			spells.Add (simple);

			return spells.ToArray ();
		}
	}
}

[System.Serializable]
public class ISpell
{
	public ISpellType spellType = ISpellType.Point;
	public ISpellEffect spellEffect = ISpellEffect.Ball;
	public ISpellRune spellRune = ISpellRune.Fire;
	public int value;
	public float range;

	public ISpell (ISpellType st, ISpellEffect se, ISpellRune sr, int vl, float rg) {
		this.spellType = st;
		this.spellEffect = se;
		this.spellRune = sr;
		this.value = vl;
		this.range = rg;
	}

	public static void InitializeSpell (ISpell spell, ICharacter caster, ICharacter target) {
		GameObject effect = (GameObject)Resources.Load ("Prefabs/Spells/" + spell.spellRune);
		ISpellObject sp = ISpellObject.Instantiate (effect, caster.trans.position, Quaternion.identity).GetComponent<ISpellObject>();
		sp.Start ();
		sp.caster = caster;
		sp.target = target;
		sp.spell.spellType = ISpellType.Target;
		caster.status.rune = -1;
		caster.AddItem (12);
		sp.spell = spell;
	}

	public static void InitializeSpell (ISpell spell, ICharacter caster, Vector3 point) {
		GameObject effect = (GameObject)Resources.Load ("Prefabs/Spells/" + spell.spellRune);
		ISpellObject sp = ISpellObject.Instantiate (effect, caster.trans.position, Quaternion.identity).GetComponent<ISpellObject>();
		sp.Start ();
		sp.caster = caster;
		sp.target = null;
		sp.spell.spellType = ISpellType.Point;
		caster.status.rune = -1;
		caster.AddItem (12);
		sp.destination = point;
		sp.spell = spell;
	}
	public static void SpellEffect (ISpellRune spell, IStatus caster, Vector3 position) {
		float pr = Random.Range (0, 100);
		if (pr > 95) {
			switch (spell) {
			case ISpellRune.Air:
				IBaker.InstantiatePlayer (IBaker.AirDemon(caster, caster.level, position));
				break;
			}
		}
	}
}
