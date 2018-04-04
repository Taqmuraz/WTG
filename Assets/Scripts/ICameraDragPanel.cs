using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ICameraDragPanel : MonoBehaviour, IDragHandler {

	public static Vector2 dragVector;

	public void OnDrag (PointerEventData data) {
		dragVector = data.delta;
	}
}
