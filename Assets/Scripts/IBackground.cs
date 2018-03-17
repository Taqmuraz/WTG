using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IBackground : MonoBehaviour {

	public RawImage raw;
	public float speed = 0.1f;
	public Vector2 direction = new Vector2(0.5f, -0.25f).normalized;
	public bool randomDirection = true;

	private void Update () {

		Vector2 random = Random.insideUnitCircle;

		direction += random / 100;

		direction.Normalize ();

		Rect r = raw.uvRect;

		r.position += direction.normalized * speed * Time.deltaTime;

		raw.uvRect = r;
	}
}
