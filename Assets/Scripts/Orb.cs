using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour {

	public float maxOffset;
	public float animationSpeed;
	private bool movementEnabled;
	private float angle;
	private float baseSpeed;
	private float xSpeed, ySpeed;
	//private float radius;
	private float minX, maxX, minY, maxY;

	private Vector3 startingPosition;
	private int transformDirection;

	// Use this for initialization
	void Start () {
		angle = 0;
		xSpeed = 0;
		ySpeed = 0;
		//radius = .004f;
	}

	public void setStartingPosition(Vector3 pos, int dir) {
		startingPosition = pos;
		//transform.localPosition = startingPosition;
		transform.position = startingPosition;
		transformDirection = dir;
	}

	public void setMaxMovementValues (float a, float b, float c, float d) {
		minX = a;
		maxX = b;
		minY = c;
		maxY = d;
	}

	// Update is called once per frame
	void Update () {
		/* Vertical Movement
		 * if (transform.localPosition.y - startingPosition.y >= maxOffset) {
			transformDirection = -1;
		} else if (startingPosition.y - transform.localPosition.y >= maxOffset) {
			transformDirection = 1;
		}
		Vector3 transformVector = new Vector3 (0.0f, animationSpeed * transformDirection * Time.deltaTime, 0.0f);
		transform.Translate (transformVector, Space.World);*/

		/* Circule movement
		if (movementEnabled) {
			angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
			float x = Mathf.Cos (angle) * radius;
			float y = Mathf.Sin (angle) * radius;
			Vector3 transformVector = new Vector3 (x, y, 0.0f);
			transform.Translate (transformVector, Space.World);
		}*/
		if (movementEnabled) {
			if (transform.position.y < minY) {
				ySpeed = baseSpeed + .3f;// + (Random.value / 3);
				this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
			}
			if (transform.position.y > maxY) {
				ySpeed = (-1 * (baseSpeed + .3f));// - (Random.value / 3);
				this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
			}
			if (transform.position.x < minX) {
				xSpeed = baseSpeed + .3f;// + (Random.value / 3);
				this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
			}
			if (transform.position.x > maxX) {
				xSpeed = (-1 * (baseSpeed + .3f));// - (Random.value / 3);
				this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
			}
				
			Vector3 transformVector = new Vector3 (xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0.0f);
			transform.Translate (transformVector, Space.World);
		}
	}

	public void setMovementEnabled (bool movement, float bs) {
		movementEnabled = movement;
		baseSpeed = bs;
		if (movement == false) {
			xSpeed = 0;
			ySpeed = 0;
			this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
		}
		//xSpeed = bs + (Random.value / 3);
		//ySpeed = bs + (Random.value / 3);
	}

	public void resetPosition(){
		transform.localPosition = startingPosition;
	}
}
