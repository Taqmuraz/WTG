using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public enum SkillTarget
{
	Enemy,
	Ally,
	Self,
	Usable,
	Point
}

[System.Serializable]
public class SkillSystem
{
	public SkillSystem (ClassType cl) {
		iType = cl;
	}

	public ClassType iType;
	public int pointsToUpdate = 0;

	public string[] quickSkills = new string[3] {"", "", ""};

	public List<string> upgraded = new List<string> ();

	public Skill[] currentSkills
	{
		get
		{
			return GetFromNames (upgraded.ToArray ());
		}
	}

	public static Skill[] GetFromNames (string[] names) {
		List<Skill> skills = new List<Skill> ();
		if (names != null) {
			foreach (var item in names) {
				skills.Add (GetByName (item));
			}
		}
		skills.RemoveAll ((Skill s) => !HasInDatabase (s.name));
		return skills.ToArray ();
	}

	public static bool HasInDatabase (string name) {
		bool h = false;
		foreach (var item in allSkills) {
			if (item.name == name) {
				h = true;
				break;
			}
		}
		return h;
	}

	public void AddSkill (Skill skill) {
		upgraded.Add (skill.name);
	}

	public Skill[] forMyClass
	{
		get {
			return allSkills.Where ((Skill arg) => arg.availableFor == iType).ToArray ();
		}
	}

	public static Skill GetByName (string name) {
		return allSkills.FirstOrDefault ((Skill s) => s.name == name);
	}

	public static Skill[] allSkills
	{
		get {

			// wonder

			List<Skill> skills = new List<Skill> ();
			Skill simpleSpell = new Skill ("SimpleSpell", ClassType.Wonder, delegate(SkillActionData data) {
				if (data.target) {
					data.skiller.CastSpell (data.target, ISpellType.Target, ISpellEffect.Ball);
				}
			}, SkillTarget.Enemy);
			skills.Add (simpleSpell);

			// monk

			Skill riseUp = new Skill ("RiseUp", ClassType.Monk, delegate(SkillActionData data) {
				data.skiller.status.AddEffect(SkillEffect.Unkillable(10 + data.skiller.status.level));
			}, SkillTarget.Enemy);
			skills.Add (riseUp);

			// thief

			Skill poisonWeapon = new Skill ("PoisonWeapon", ClassType.Thief, delegate(SkillActionData data) {
				data.skiller.status.AddEffect(SkillEffect.PoisonWeapon(data.skiller));
			}, SkillTarget.Enemy);
			skills.Add (poisonWeapon);

			return skills.ToArray ();
		}
	}
}
[System.Serializable]
public struct SkillActionData
{
	public ICharacter target;
	public ICharacter skiller;
	public IUsable use;
	public Vector3 point;

	public SkillActionData (ICharacter s, ICharacter t, Vector3 p) {
		target = t;
		skiller = s;
		point = p;
		use = null;
	}
	public SkillActionData (ICharacter s, ICharacter t) {
		target = t;
		skiller = s;
		point = Vector3.zero;
		use = null;
	}
	public SkillActionData (ICharacter s, IUsable u) {
		target = null;
		skiller = s;
		point = Vector3.zero;
		use = u;
	}
	public SkillActionData (ICharacter s, Vector3 p) {
		target = null;
		skiller = s;
		point = p;
		use = null;
	}
}
[System.Serializable]
public struct Skill
{
	public delegate void SkillAction (SkillActionData data);

	public SkillTarget targetType { get; private set; }

	public SkillAction action { get; private set; }

	public ClassType availableFor { get; private set; }

	public string name { get; private set; }

	public string Info () {
		TextAsset t = Resources.Load<TextAsset> ("Text/Skills/" + name);
		return t ? t.text : "Нет информации" + '\n' + "Не найдено информации по данному навыку";
	}
	public string LitraName () {
		string origin = Info ();
		int sym = 0;
		string n = "";
		while (sym < origin.Length && origin[sym] != '\n') {
			n = n + origin [sym];
			sym++;
		}
		return n;
	}

	public Texture image
	{
		get {
			return LoadImageByName (name);
		}
	}

	public static Texture LoadImageByName (string name) {
		Texture tex = Resources.Load<Texture> ("Sprites/Skills/" + name);
		return tex ? tex : ItemsAsset.LoadTexture (-1);
	}

	public Skill (string nam, ClassType ava, SkillAction act, SkillTarget t) {
		name = nam;
		action = act;
		availableFor = ava;
		targetType = t;
	}
}
[System.Serializable]
public class SkillEffect
{
	[System.Serializable]
	public delegate void EffectAction (ICharacter target);
	public string effectName { get; private set; }
	public EffectAction everySecUpdate { get; private set; }
	public EffectAction onDamaged { get; private set; }
	public EffectAction preAttack { get; private set; }
	public int lifeTime { get; private set; }
	public int startLifeTime { get; private set; }

	public SkillEffect (string name, EffectAction esu, EffectAction onD, EffectAction preA, int lt) {
		effectName = name;
		everySecUpdate = esu;
		onDamaged = onD;
		preAttack = preA;
		lifeTime = lt;
		startLifeTime = lt;
	}
	public SkillEffect (string name, EffectAction esu, int lt) {
		effectName = name;
		everySecUpdate = esu;
		onDamaged = delegate(ICharacter target) {
			return;
		};
		preAttack = delegate(ICharacter target) {
			return;
		};
		lifeTime = lt;
		startLifeTime = lt;
	}

	public string Info () {
		return "" + effectName + " - оставшееся время действия : " + lifeTime;
	}

	public static void EverySecUpdate () {
		foreach (var item in ICharacter.charactersAll) {
			for (int i = 0; i < item.status.effects.Count; i++) {
				SkillEffect e = item.status.effects [i];
				e.everySecUpdate (item);
				e.lifeTime -= 1;
				if (e.lifeTime < 1) {
					item.status.RemoveEffect (e);
				}
			}
		}
	}
	public static SkillEffect Poison (int time, int damage) {
		return new SkillEffect ("Poison", delegate(ICharacter target) {
			target.ApplyDamage(damage, DamageType.Just);
		}, time);
	}
	public static SkillEffect PoisonWeapon (ICharacter caster) {
		return new SkillEffect ("PoisonWeapon", delegate(ICharacter target) {
			return;
		},delegate(ICharacter target) {
			return;
		},delegate(ICharacter target) {
			target.status.AddEffect(Poison(5 + caster.status.level, caster.status.level));
		}, 10 + caster.status.level);
	}
	public static SkillEffect Unkillable (int time) {
		return new SkillEffect ("RiseUp", delegate(ICharacter target) {
			return;
		}, delegate(ICharacter target) {
			target.status.Heal(0);
		}, delegate(ICharacter target) {
			return;
		}, time);
	}
	public static SkillEffect Regen (int points, int time) {
		return new SkillEffect ("Regen", delegate(ICharacter target) {
			target.status.Heal(points);
		}, delegate(ICharacter target) {
			return;
		}, delegate(ICharacter target) {
			return;
		}, time);
	}
}