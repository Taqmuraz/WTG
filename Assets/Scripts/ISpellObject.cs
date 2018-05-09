using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISpellObject : MonoBehaviour {
	public ICharacter caster;
	public ICharacter target;
	public Vector3 destination;
	public ISpell spell;
	private Transform trans;

	public void Start () {
		trans = transform;
	}

	public bool canBeActivated
	{
		get {
			bool can = false;
			float dist = 0;
			if (caster) {
				if (spell.spellType == ISpellType.Target && target) {
					dist = (trans.position - target.trans.position).magnitude;
				}
				if (spell.spellType == ISpellType.Point || spell.spellType == ISpellType.Radian) {
					dist = (trans.position - destination).magnitude;
				}
			}
			can = dist < 0.5f;
			return can;
		}
	}

	public void Update () {
		if ((!target && spell.spellType == ISpellType.Target) || !caster) {
			DestroySpell ();
		}
		if (canBeActivated) {
			Activate ();
		}
		Vector3 d = destination;
		if (spell.spellType == ISpellType.Target && target) {
			d = target.trans.position;
		}
		trans.position += (d - trans.position).normalized * Time.deltaTime * 4;
	}

	private void DestroySpell () {
		GameObject effect = (GameObject)Resources.Load ("Prefabs/SpellEffects/" + spell.spellRune + spell.spellEffect);
		effect = Instantiate (effect, trans.position, trans.rotation);
		Destroy (effect, 5);
		Destroy (gameObject);
	}

	public void Activate () {

		DamageType dmgType = DamageType.Magic;

		switch (spell.spellRune) {
		case ISpellRune.Air:
			dmgType = DamageType.Air;
			break;
		case ISpellRune.Earth:
			dmgType = DamageType.Earth;
			break;
		case ISpellRune.Fire:
			dmgType = DamageType.Fire;
			break;
		case ISpellRune.Water:
			dmgType = DamageType.Water;
			break;
		}
		
		switch (spell.spellType) {
		case ISpellType.Target:
			if (target) {
				target.ApplyDamage (spell.value, dmgType);
			}
			break;
		case ISpellType.Radian:
			ICharacter[] chars = ICharacter.GetNearestFromPointAll (destination, null, spell.range);
			foreach (ICharacter ch in chars) {
				ch.ApplyDamage (spell.value, dmgType);
			}
			break;
		case ISpellType.Point:
			ICharacter c = ICharacter.GetNearestFromPoint (destination, null, 0.5f);
			if (c) {
				c.ApplyDamage (spell.value, dmgType);
			}
			break;
		}
		if (target && caster) {
			caster.status.SetDamageLawEvent (target.status);
		}
		//ISpell.SpellEffect (spell.spellRune, caster.status, trans.position);
		DestroySpell ();
	}
}
