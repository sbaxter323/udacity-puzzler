using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour {

	public float maxOffset;
	public float animationSpeed;
	private bool movementEnabled;
	private float angle;
	private float speed;
	private float radius;

	private Vector3 startingPosition;
	private int transformDirection;

	// Use this for initialization
	void Start () {
		angle = 0;
		speed = .5f * Random.value * Mathf.PI;
		radius = .0006f;
	}

	public void setStartingPosition(Vector3 pos, int dir) {
		startingPosition = pos;
		transform.localPosition = startingPosition;
		transformDirection = dir;
	}

	// Update is called once per frame
	void Update () {
		/*if (transform.localPosition.y - startingPosition.y >= maxOffset) {
			transformDirection = -1;
		} else if (startingPosition.y - transform.localPosition.y >= maxOffset) {
			transformDirection = 1;
		}
		Vector3 transformVector = new Vector3 (0.0f, animationSpeed * transformDirection * Time.deltaTime, 0.0f);
		transform.Translate (transformVector, Space.World);*/
		if (movementEnabled) {
			angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
			float x = Mathf.Cos (angle) * radius;
			float y = Mathf.Sin (angle) * radius;
			Vector3 transformVector = new Vector3 (x, y, 0.0f);
			transform.Translate (transformVector, Space.World);
		}
	}

	public void setMovementEnabled (bool movement) {
		movementEnabled = movement;
	}

	public void resetPosition(){
		transform.localPosition = startingPosition;
	}
}
