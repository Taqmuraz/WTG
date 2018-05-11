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
	private GameObject obj;

	public void SetActive (bool active) {
		obj.SetActive (active);
	}

	private bool locked_getter;
	public bool locked
	{
		get {
			return locked_getter;
		}
		set {
			locked_getter = value;
			img.color = !locked_getter ? Color.white : Color.gray;
		}
	}

	public void Start () {
		obj = gameObject;
		img = GetComponent<RawImage> ();
		img = img ? img : GetComponentInChildren<RawImage> ();
	}

	public void OnPointerClick (PointerEventData data) {
		if (!locked) {
			onClick ();
		}
	}

	public void SetWithSkill (Skill skill) {
		img.texture = skill.image;
	}
}
