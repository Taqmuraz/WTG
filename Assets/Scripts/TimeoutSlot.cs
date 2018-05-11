using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class TimeoutSlot : MonoBehaviour
{
	public Image fill;
	public RawImage item;

	public float startValue;
	public float currentValue;

	private object setWith;

	private void Start () {
		fill = GetComponent<Image> ();
		item = GetComponentInChildren<RawImage> ();
	}

	public void Init (SkillEffect skill) {
		Start ();
		setWith = skill;
		item.texture = Skill.LoadImageByName (skill.effectName);
		Update ();
		fill.enabled = true;
		item.enabled = true;
	}

	private void Update () {
		if (setWith != null) {
			if (setWith is SkillEffect) {
				SetWithSkill ();
			} else {
				Debug.Log ("with is no effect!");
			}
		} else {
			Debug.Log ("with is null!");
		}
		fill.fillAmount = currentValue / startValue;
	}

	private void SetWithSkill () {
		SkillEffect e = (SkillEffect)setWith;
		currentValue = e.lifeTime;
		startValue = e.startLifeTime;
	}
}

