using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPointerClickHandler {
	public delegate void ToDo();
	public ToDo onClick = delegate() {
		return;
	};

	public RawImage img;

	public void Start () {
		img = GetComponent<RawImage> ();
	}

	public void OnPointerClick (PointerEventData data) {
		onClick ();
	}

	public void SetWithSkill (Skill skill) {
		img.texture = skill.image;
	}
}
