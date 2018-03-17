using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ISpellButton : MonoBehaviour, IDragHandler, IEndDragHandler {
	RectTransform trans;

	private void Start () {
		trans = GetComponent<RectTransform> ();
		startPosition = (Vector2)trans.position;
	}
	public Vector2 startPosition;

	public void OnEndDrag (PointerEventData data) {
		if ((data.position - startPosition).magnitude > trans.sizeDelta.y / 2) {
			CastSpell (data.position);
		}
		trans.position = (Vector3)startPosition;
		trans.localScale = Vector3.one;
	}
	public void OnDrag (PointerEventData data) {
		trans.position = data.position;
		trans.localScale = Vector3.one * 0.5f;
	}
	public void CastSpell (Vector3 screenPoint) {
		RaycastHit hit;
		Ray ray = IControl.camMain.ScreenPointToRay (screenPoint);
		if (Physics.Raycast(ray, out hit)) {
			IControl.character.CastSpell (hit.point);
		}
	}
}
