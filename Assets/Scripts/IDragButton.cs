using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class IDragButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	private bool dragging = false;

	public void OnPointerDown (PointerEventData data) {
		onDrag.Invoke ();
		dragging = true;
	}
	public void OnPointerUp (PointerEventData data) {
		dragging = false;
	}

	private void Update () {
		if (dragging) {
			onDrag.Invoke ();
		}
	}

	public UnityEvent onDrag;
}
