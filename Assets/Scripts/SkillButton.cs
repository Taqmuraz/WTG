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
	public Image back;
	private GameObject obj;

	private bool active_getter = true;
	public void SetActive (bool active) {
		if (active_getter != active) {
			obj.SetActive (active);
			active_getter = active;
		}
	}

	private bool locked_getter;
	public bool locked
	{
		get {
			return locked_getter;
		}
		set {
			if (!img) {
				Start ();
			}
			locked_getter = value;
			img.color = !locked_getter ? Color.white : Color.gray;
		}
	}
	private bool targeted_getter;
	public bool targeted
	{
		get {
			return targeted_getter;
		}
		set {
			if (!back) {
				Start ();
			}
			targeted_getter = value;
			back.color = !targeted_getter ? Color.white : Color.green;
		}
	}

	public void Start () {
		obj = gameObject;

		img = GetComponent<RawImage> ();
		img = img ? img : GetComponentInChildren<RawImage> ();
		back = GetComponent<Image> ();
		back = back ? back : GetComponentInParent<Image> ();
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
