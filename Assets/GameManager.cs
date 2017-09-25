using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public Orb dungeonOrbPrefab;

	public int orbsToGenerate;
	public float xOffset;
	public float yOffset;
	public float zOffset;
	public Vector3 orbCenter; // = new Vector3 (-1.0f, -0.5f, 0.0f);

	private List<Orb> orbs;

	// Use this for initialization
	void Start () {
		// If there an even number of orbs, split the first two from center
		orbs = new List<Orb>();

		if (orbsToGenerate % 2 == 0) {
			Vector3 leftOrb = new Vector3 (orbCenter.x - xOffset, orbCenter.y, orbCenter.z);
			Vector3 rightOrb = new Vector3 (orbCenter.x - xOffset, orbCenter.y, orbCenter.z);
			createOrb (leftOrb, 1);
			createOrb (rightOrb, 1);

			// Create half the remaining orbs on the left side
			Vector3 leftOrbCenter = new Vector3(orbCenter.x - (xOffset * 2), orbCenter.y + yOffset, orbCenter.z + zOffset);
			generateOrbs((orbsToGenerate - 2) / 2, -1, -1, leftOrbCenter);

			// Create the other half of remaining orbs on the right side
			Vector3 rightOrbCenter = new Vector3(orbCenter.x + (xOffset * 2), orbCenter.y + yOffset, orbCenter.z + zOffset);
			generateOrbs((orbsToGenerate - 2) / 2, 1, -1, rightOrbCenter);
		} else {
			Vector3 centerOrb = new Vector3 (orbCenter.x, orbCenter.y, orbCenter.z);
			Vector3 leftOrbCenter = new Vector3(orbCenter.x - (xOffset), orbCenter.y + yOffset, orbCenter.z + zOffset);
			Vector3 rightOrbCenter = new Vector3(orbCenter.x + (xOffset), orbCenter.y + yOffset, orbCenter.z + zOffset);

			createOrb (centerOrb, 1);
			generateOrbs ((orbsToGenerate - 1) / 2, -1, -1, leftOrbCenter);
			generateOrbs ((orbsToGenerate - 1) / 2, 1, -1, rightOrbCenter);
		}
	
	}

	// Recursively generate orbs
	private void generateOrbs (int numOrbs, int xDirection, int yDirection, Vector3 orbCenter) {
		if (numOrbs == 0)
			return;
		
		createOrb (orbCenter, yDirection);
		Vector3 newCenter = new Vector3 (orbCenter.x + (xOffset * xDirection), orbCenter.y + yOffset, orbCenter.z + zOffset);

		generateOrbs (numOrbs - 1, xDirection, yDirection * -1, newCenter);
	}


	private void createOrb(Vector3 orbPosition, int direction) {
		Orb orbInstance = Instantiate (dungeonOrbPrefab) as Orb;
		orbInstance.setStartingPosition(orbPosition, direction);
		orbs.Add (orbInstance);
	}


	// Update is called once per frame
	void Update () {

	}
}
