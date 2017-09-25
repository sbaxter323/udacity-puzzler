using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {
	public GameObject player;
	public GameObject eventSystem;
	public GameObject startUI, restartUI;
	public GameObject startPoint, playPoint, restartPoint;
	public GameObject casualLengthText, challengeLengthText;

	public int puzzleLength = 1; //How many times we light up.  This is the difficulty factor.  The longer it is the more you have to memorize in-game.
	public float puzzleSpeed = 1f; //How many seconds between puzzle display pulses
	private int[] puzzleOrder; //For now let's have 5 orbs

	public Orb dungeonOrbPrefab;

	public float xOffset;
	public float yOffset;
	public float zOffset;
	public Vector3 orbCenter; // = new Vector3 (-1.0f, -0.5f, 0.0f);

	private List<Orb> puzzleSpheres; //A list to hold our dynamically generated puzzle spheres
	public int numSpheres = 5;
	private bool movementEnabled;


	private int currentDisplayIndex = 0; //Temporary variable for storing the index when displaying the pattern
	public bool currentlyDisplayingPattern = true;
	public bool playerWon = false;
	private int currentSolveIndex = 0; //Temporary variable for storing the index that the player is solving for in the pattern.

	public AudioClip correctClip;
	public AudioClip failureClip;
	public AudioClip successClip;


	// Use this for initialization
	void Start () {
		player.transform.position = startPoint.transform.position;
		createOrbUI();
		setStartUIText ();
		// Moving this step to startPuzzle to take puzzleLength input from user
		/*puzzleOrder = new int[puzzleLength]; //Set the size of our array to the declared puzzle length
		generatePuzzleSequence (); //Generate the puzzle sequence for this playthrough.*/  
	}


	// Update is called once per frame
	void Update () {

	}

	public void playerSelection(int selectedIndex) {
		if(playerWon != true) { //If the player hasn't won yet
			//int selectedIndex=0;
			//Get the index of the selected object
			/*for (int i = 0; i < puzzleSpheres.Count; i++) { //Go through the puzzlespheres array
				if(puzzleSpheres[i] == sphere) { //If the object we have matches this index, we're good
					Debug.Log("Looks like we hit sphere: " + i);
					selectedIndex = i;
				}
			}*/
			solutionCheck (selectedIndex);//Check if it's correct
		}
	}

	public void setStartUIText(){
		UnityEngine.UI.Text casualText = casualLengthText.GetComponent<Text> ();
		casualText.text = "Puzzle Length: " + puzzleLength;
		UnityEngine.UI.Text challengeText = challengeLengthText.GetComponent<Text> ();
		challengeText.text = "Puzzle Length: " + (puzzleLength+1);

	}

	public void solutionCheck(int playerSelectionIndex) { //We check whether or not the passed index matches the solution index
		if (playerSelectionIndex == puzzleOrder [currentSolveIndex]) { //Check if the index of the object the player passed is the same as the current solve index in our solution array
			currentSolveIndex++;
			Debug.Log ("Correct!  You've solved " + currentSolveIndex + " out of " + puzzleLength);
			this.GetComponent<AudioSource> ().clip = correctClip;
			this.GetComponent<AudioSource>().Play(); //Play the correct guess audio
			if (currentSolveIndex >= puzzleLength) {
				puzzleSuccess ();
			}

		} else {
			puzzleFailure ();
		}

	}

	public void startPuzzle() { //Begin the puzzle sequence
		startUI.SetActive (false);
		eventSystem.SetActive(false);
		iTween.MoveTo (player, playPoint.transform.position, 10f);
		CancelInvoke ("displayPattern");
		InvokeRepeating("displayPattern", 3, puzzleSpeed); //Start running through the displaypattern function
		setOrbMovement(movementEnabled);
		currentSolveIndex = 0; //Set our puzzle index at 0

	}

	public void setOrbMovement(bool b){
		for (int i = 0; i < puzzleSpheres.Count; i++) {
			puzzleSpheres [i].setMovementEnabled (b);
		}
	}

	public void startPuzzleCasual() {
		puzzleOrder = new int[puzzleLength]; //Set the size of our array to the declared puzzle length
		generatePuzzleSequence (); //Generate the puzzle sequence for this playthrough
		movementEnabled = false;
		startPuzzle ();
	}

	public void startPuzzleChallenge() {
		puzzleLength++;
		puzzleOrder = new int[puzzleLength]; //Set the size of our array to the declared puzzle length
		generatePuzzleSequence (); //Generate the puzzle sequence for this playthrough
		movementEnabled = true;
		startPuzzle ();
	}

	void displayPattern() { //Invoked repeating.
		currentlyDisplayingPattern = true; //Let us know were displaying the pattern
		eventSystem.SetActive(false); //Disable gaze input events while we are displaying the pattern.

		if (currentlyDisplayingPattern == true) { //If we are not finished displaying the pattern
			if (currentDisplayIndex < puzzleOrder.Length) { //If we haven't reached the end of the puzzle
				Debug.Log (puzzleOrder[currentDisplayIndex] + " at index: " + currentDisplayIndex); 
				puzzleSpheres [puzzleOrder [currentDisplayIndex]].GetComponent<lightUp> ().patternLightUp (puzzleSpeed); //Light up the sphere at the proper index.  For now we keep it lit up the same amount of time as our interval, but could adjust this to be less.
				currentDisplayIndex++; //Move one further up.
			} else {
				Debug.Log ("End of puzzle display");
				currentlyDisplayingPattern = false; //Let us know were done displaying the pattern
				currentDisplayIndex = 0;
				CancelInvoke(); //Stop the pattern display.  May be better to use coroutines for this but oh well
				eventSystem.SetActive(true); //Enable gaze input when we aren't displaying the pattern.
			}
		}
	}

	public void generatePuzzleSequence() {

		int tempReference;
		for (int i = 0; i < puzzleLength; i++) { //Do this as many times as necessary for puzzle length
			tempReference = Random.Range(0, puzzleSpheres.Count); //Generate a random reference number for our puzzle spheres
			puzzleOrder [i] = tempReference; //Set the current index to our randomly generated reference number
		}
	}

	public void createOrbUI() {
		puzzleSpheres = new List<Orb>();

		// If there an even number of orbs, split the first two from center, then create in pairs on left and right
		if (numSpheres % 2 == 0) {
			Vector3 leftOrb = new Vector3 (orbCenter.x - xOffset, orbCenter.y, orbCenter.z);
			Vector3 rightOrb = new Vector3 (orbCenter.x - xOffset, orbCenter.y, orbCenter.z);
			createOrb (leftOrb, 1);
			createOrb (rightOrb, 1);

			// Create half the remaining orbs on the left side
			Vector3 leftOrbCenter = new Vector3(orbCenter.x - (xOffset * 2), orbCenter.y + yOffset, orbCenter.z + zOffset);
			generateOrbs((numSpheres - 2) / 2, -1, -1, leftOrbCenter);

			// Create the other half of remaining orbs on the right side
			Vector3 rightOrbCenter = new Vector3(orbCenter.x + (xOffset * 2), orbCenter.y + yOffset, orbCenter.z + zOffset);
			generateOrbs((numSpheres - 2) / 2, 1, -1, rightOrbCenter);
		} else {
			Vector3 centerOrb = new Vector3 (orbCenter.x, orbCenter.y, orbCenter.z);
			Vector3 leftOrbCenter = new Vector3(orbCenter.x - (xOffset), orbCenter.y + yOffset, orbCenter.z + zOffset);
			Vector3 rightOrbCenter = new Vector3(orbCenter.x + (xOffset), orbCenter.y + yOffset, orbCenter.z + zOffset);

			createOrb (centerOrb, 1);
			generateOrbs ((numSpheres - 1) / 2, -1, -1, leftOrbCenter);
			generateOrbs ((numSpheres - 1) / 2, 1, -1, rightOrbCenter);
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
		puzzleSpheres.Add(orbInstance);
		orbInstance.GetComponent<lightUp> ().setIndex (puzzleSpheres.Count - 1);
	}

		
	public void resetPuzzle() { //Reset the puzzle sequence
		player.transform.position = startPoint.transform.position;
		//toggleUI ();
		/*iTween.MoveTo (player, 
			iTween.Hash (
				"position", startPoint.transform.position, 
				"time", 4, 
				"easetype", "linear",
				"oncomplete", "resetGame", 
				"oncompletetarget", this.gameObject
			)
		);*/
		resetGame ();
	}

	public void resetGame() {
		restartUI.SetActive (false);
		setStartUIText ();
		startUI.SetActive (true);
		playerWon = false;
		puzzleLength++;
		setOrbMovement (false);
		resetOrbPosition ();
		//generatePuzzleSequence (); //Generate the puzzle sequence for this playthrough.  
	}

	public void resetOrbPosition() {
		for (int i = 0; i < puzzleSpheres.Count; i++) {
			puzzleSpheres [i].resetPosition();
		}
	}

	public void puzzleFailure() { //Do this when the player gets it wrong
		Debug.Log("You've Failed, Resetting puzzle");
		this.GetComponent<AudioSource> ().clip = failureClip;
		this.GetComponent<AudioSource>().Play(); //Play the success audio

		currentSolveIndex = 0;

		setOrbMovement (false);
		resetOrbPosition ();
		startPuzzle ();

	}

	public void puzzleSuccess() { //Do this when the player gets it right
		restartUI.SetActive (true);
		iTween.MoveTo (player, 
			iTween.Hash (
				"position", restartPoint.transform.position, 
				"time", 3.38, 
				"easetype", "linear",
				"oncomplete", "finishingFlourish", 
				"oncompletetarget", this.gameObject
			)
		);
	}
		
	public void toggleUI() {
		startUI.SetActive (!startUI.activeSelf);
		restartUI.SetActive (!restartUI.activeSelf);
	}

	public void enterUpdateUIColor(GameObject panel) {
		Image img = panel.GetComponent<Image>();
		img.color = UnityEngine.Color.blue;
	}

	public void exitUpdateUIColor(GameObject panel) {
		Image img = panel.GetComponent<Image>();
		img.color = UnityEngine.Color.black;
	}

	public void finishingFlourish() { //A nice visual flourish when the player wins
		this.GetComponent<AudioSource> ().clip = successClip;
		this.GetComponent<AudioSource>().Play(); //Play the success audio
		restartUI.SetActive (true);
		playerWon = true;

	}

}