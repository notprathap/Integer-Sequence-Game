using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public float speed;
	private int count;
	public GUIText countText;
	public GUIText winText;
	public GUIText livesText;
	public Transform sphere;
	public Transform twoSidedCube;
	public Transform threeSidedShape;
	public Transform fourSidedCube;
	private List<IntegerSequence> sequences;
	private List<Color> colourMap;
	private IntegerSequence currentSequence;
	private int expectedIndex;
	private int levelToLoad = 0;
	private bool levelComplete;
	private int noOfLivesLeft = 20;

	void Start()
	{
		count = 0;
		levelComplete = false;
		countText.text = "Sequence Collected: ";
		livesText.text = "Lives left: " + noOfLivesLeft.ToString();
		winText.text = "";
		hydrateSequences();
		initColourMap ();
		currentSequence = this.sequences [levelToLoad];
		expectedIndex = 0;
		int randomNumber = 0;
		for (int i = 0; i < currentSequence.Elements.Length; i++) {
			randomNumber = currentSequence.Elements[i];
			//Debug.Log(randomNumber);
			if ((randomNumber >= 0) && (randomNumber < 10) ){
				Transform singleDigitObject = (Transform)Instantiate (sphere, new Vector3 (Random.Range(-9.5f,9.5f), 0.5f, Random.Range(-9.5f,9.5f)), Quaternion.identity);
				//Debug.Log(decimalPointFinder(4, randomNumber)[0][1]);
				singleDigitObject.gameObject.renderer.material.color = colourMap[randomNumber];
				singleDigitObject.gameObject.GetComponent<CubeRotator>().number = randomNumber;
			} else if ((randomNumber >= 10) && (randomNumber < 99) ){
				Transform doubleDigitObject = (Transform)Instantiate (twoSidedCube, new Vector3 (Random.Range(-9.5f,9.5f), 0.5f, Random.Range(-9.5f,9.5f)), Quaternion.identity);
				int noAtUnit = decimalPointFinder(2, randomNumber)[0][1];
				int noAtTens = decimalPointFinder(2, randomNumber)[1][1];
				doubleDigitObject.FindChild("Back").renderer.material.color = colourMap[noAtUnit];
				doubleDigitObject.FindChild("Front").renderer.material.color = colourMap[noAtTens];
				doubleDigitObject.gameObject.GetComponent<CubeRotator>().number = randomNumber;
			} else if ((randomNumber >= 100) && (randomNumber < 999) ){
				Transform threeDigitObject = (Transform)Instantiate (threeSidedShape, new Vector3 (Random.Range(-9.5f,9.5f), 0.5f, Random.Range(-9.5f,9.5f)), Quaternion.identity);
				int noAtUnit = decimalPointFinder(3, randomNumber)[0][1];
				int noAtTens = decimalPointFinder(3, randomNumber)[1][1];
				int noAtHundreds = decimalPointFinder(3, randomNumber)[2][1];
				threeDigitObject.FindChild("Horizontal").FindChild("Front").renderer.material.color = colourMap[noAtUnit];
				threeDigitObject.FindChild("Horizontal").FindChild("Back").renderer.material.color = colourMap[noAtUnit];
				threeDigitObject.FindChild("Lateral Left").FindChild("Front").renderer.material.color = colourMap[noAtTens];
				threeDigitObject.FindChild("Lateral Left").FindChild("Back").renderer.material.color = colourMap[noAtTens];
				threeDigitObject.FindChild("Lateral Right").FindChild("Front").renderer.material.color = colourMap[noAtHundreds];
				threeDigitObject.FindChild("Lateral Right").FindChild("Back").renderer.material.color = colourMap[noAtHundreds];
				threeDigitObject.gameObject.GetComponent<CubeRotator>().number = randomNumber;
			} else {
				Transform fourDigitObject = (Transform)Instantiate (fourSidedCube, new Vector3 (Random.Range(-9.5f,9.5f), 0.5f, Random.Range(-9.5f,9.5f)), Quaternion.identity);
				int noAtUnit = decimalPointFinder(4, randomNumber)[0][1];
				int noAtTens = decimalPointFinder(4, randomNumber)[1][1];
				int noAtHundreds = decimalPointFinder(4, randomNumber)[2][1];
				int noAtThousands = decimalPointFinder(4, randomNumber)[3][1];
				fourDigitObject.FindChild("Back").renderer.material.color = colourMap[noAtUnit];
				fourDigitObject.FindChild("Front").renderer.material.color = colourMap[noAtTens];
				fourDigitObject.FindChild("Right").renderer.material.color = colourMap[noAtHundreds];
				fourDigitObject.FindChild("Left").renderer.material.color = colourMap[noAtThousands];
				fourDigitObject.gameObject.GetComponent<CubeRotator>().number = randomNumber;
			}

		}
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rigidbody.AddForce (movement * speed * Time.deltaTime);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.GetComponent<CubeRotator> () != null) {
			int collectedNumber = other.gameObject.GetComponent<CubeRotator>().number;
			if (collectedNumber != currentSequence.Elements[expectedIndex]) {
				// wrong order of collection
				reset();
			} else {
				// right order of collection; check for level complete or update expected number
				Destroy(other.gameObject);
				count++;
				SetCountText(collectedNumber);
				expectedIndex++;
				if (expectedIndex >= currentSequence.Elements.Length) {
					winText.text = "Congratulations! You have mastered the " + currentSequence.Name;
					//loadNextLevel();
					levelComplete = true;
				}
			}
		}
	}

	void reset()
	{
		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag ("PickUp");
		foreach (GameObject thisObject in gameObjects) {
			if (thisObject.activeInHierarchy){
				Destroy(thisObject);
			}
		}
		noOfLivesLeft--;
		if (noOfLivesLeft > 0) {
			Start ();
		} else {
			// Game Over
			winText.text = "Game Over";
			countText.text = "";
			livesText.text = "";
		}
	}

	void OnGUI()
	{
		if (levelComplete) {
			if (GUI.Button(new Rect((Screen.width/2) - 50,(Screen.height/2) - 25,100,50), "Continue")){
				loadNextLevel();
			}
		}
	}

	void loadNextLevel()
	{
		this.levelToLoad++;
		Start ();
	}

	void SetCountText(int collectedNumber)
	{
		countText.text += collectedNumber.ToString() + ", ";
		if (count >= 15) {
			winText.text = "You Win!";
		}
	}

	List<int[]> decimalPointFinder(int noOfDecimalPlaces, int number)
	{
		// the number at each decimal point
		int placeNumber;

		// the result map containing digit at each decimal point
		List<int[]> decimalPointMap = new List<int[]>();

		for (int i = 1; i <= noOfDecimalPlaces; i++) {
			placeNumber = number % (int)Mathf.Pow( 10, i);
			decimalPointMap.Add(new int[] {i, placeNumber / (int)Mathf.Pow( 10, i-1)});
			number -= placeNumber; // getting rid of the unit altogether to faciliate further iterations
		}

		return decimalPointMap;
	}

	void initColourMap()
	{
		this.colourMap = new List<Color>();
		colourMap.Add (Color.white);
		colourMap.Add (Color.black);
		colourMap.Add (Color.red);
		colourMap.Add (Color.blue);
		colourMap.Add (Color.yellow);
		colourMap.Add (Color.cyan);
		colourMap.Add (Color.grey);
		colourMap.Add (Color.magenta);
		colourMap.Add (Color.green);
		colourMap.Add (new Color(0.5f, 0.3f, 0.2f));
	}

	void hydrateSequences()
	{
		sequences = new List<IntegerSequence>();
		sequences.Add(new IntegerSequence("Natural Number", new int[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10}));
		sequences.Add(new IntegerSequence("Lucas Number", new int[]{2, 1, 3, 4, 7, 11, 18, 29, 47, 76}));
		sequences.Add(new IntegerSequence("Prime Number", new int[]{2, 3, 5, 7, 11, 13, 17, 19, 23, 29}));
		sequences.Add(new IntegerSequence("Fibonacci Number", new int[]{0, 1, 1, 2, 3, 5, 8, 13, 21, 34}));
		sequences.Add(new IntegerSequence("Sylvester's sequence", new int[]{2, 3, 7, 43, 1807}));
		sequences.Add(new IntegerSequence("Catalan Number", new int[]{1, 1, 2, 5, 14, 42, 132, 429, 1430, 4862}));
		sequences.Add(new IntegerSequence("Bell Number", new int[]{1, 1, 2, 5, 15, 52, 203, 877, 4140}));
		sequences.Add(new IntegerSequence("Lazy caterer's sequence", new int[]{1, 2, 4, 7, 11, 16, 22, 29, 37, 46}));
		sequences.Add(new IntegerSequence("Pell Number", new int[]{0, 1, 2, 5, 12, 29, 70, 169, 408, 985}));
		sequences.Add(new IntegerSequence("Factorial", new int[]{1, 1, 2, 6, 24, 120, 720, 5040}));
		sequences.Add(new IntegerSequence("Triangular number", new int[]{0, 1, 3, 6, 10, 15, 21, 28, 36, 45}));
		sequences.Add(new IntegerSequence("Tetrahedral number", new int[]{0, 1, 4, 10, 20, 35, 56, 84, 120, 165}));
		sequences.Add(new IntegerSequence("Square pyramidal number", new int[]{0, 1, 5, 14, 30, 55, 91, 140, 204, 285}));
		sequences.Add(new IntegerSequence("Perfect number", new int[]{6, 28, 496, 8128}));
		sequences.Add(new IntegerSequence("Mersenne prime", new int[]{3, 7, 31, 127, 8191}));
		sequences.Add(new IntegerSequence("Landau's function", new int[]{1, 1, 2, 3, 4, 6, 6, 12, 15, 20}));
		sequences.Add(new IntegerSequence("Decimal expansion of Pi", new int[]{3, 1, 4, 1, 5, 9, 2, 6, 5, 3}));
		sequences.Add(new IntegerSequence("Padovan sequence", new int[]{1, 1, 1, 2, 2, 3, 4, 5, 7, 9}));
		sequences.Add(new IntegerSequence("Euclid–Mullin sequence", new int[]{2, 3, 7, 43, 13, 53, 5}));
		sequences.Add(new IntegerSequence("Lucky number", new int[]{1, 3, 7, 9, 13, 15, 21, 25, 31, 33}));
		sequences.Add(new IntegerSequence("Motzkin number", new int[]{1, 1, 2, 4, 9, 21, 51, 127, 323, 835}));
		sequences.Add(new IntegerSequence("Jacobsthal number", new int[]{0, 1, 1, 3, 5, 11, 21, 43, 85, 171, 341}));
		sequences.Add(new IntegerSequence("Decimal expansion of e (mathematical constant)", new int[]{2, 7, 1, 8, 2, 8, 1, 8, 2, 8}));
		sequences.Add(new IntegerSequence("Wedderburn–Etherington number", new int[]{0, 1, 1, 1, 2, 3, 6, 11, 23, 46}));
		sequences.Add(new IntegerSequence("Semiprime", new int[]{4, 6, 9, 10, 14, 15, 21, 22, 25, 26}));
		sequences.Add(new IntegerSequence("Golomb sequence", new int[]{1, 2, 2, 3, 3, 4, 4, 4, 5, 5}));
		sequences.Add(new IntegerSequence("Perrin number", new int[]{3, 0, 2, 3, 2, 5, 5, 7, 10, 12}));
		sequences.Add(new IntegerSequence("Euler–Mascheroni constant", new int[]{5, 7, 7, 2, 1, 5, 6, 6, 4, 9}));
		sequences.Add(new IntegerSequence("Decimal expansion of the golden ratio", new int[]{1, 6, 1, 8, 0, 3, 3, 9, 8, 8}));
		sequences.Add(new IntegerSequence("Primorial", new int[]{1, 2, 6, 30, 210, 2310}));
		sequences.Add(new IntegerSequence("Highly composite number", new int[]{1, 2, 4, 6, 12, 24, 36, 48, 60, 120}));
		sequences.Add(new IntegerSequence("Decimal expansion of square root of 2", new int[]{1, 4, 1, 4, 2, 1, 3, 5, 6, 2}));
		sequences.Add(new IntegerSequence("Superior highly composite number", new int[]{2, 6, 12, 60, 120, 360, 2520, 5040}));
		sequences.Add(new IntegerSequence("Pronic number", new int[]{0, 2, 6, 12, 20, 30, 42, 56, 72, 90}));
		sequences.Add(new IntegerSequence("Composite number", new int[]{4, 6, 8, 9, 10, 12, 14, 15, 16, 18}));
		sequences.Add(new IntegerSequence("Ulam number", new int[]{1, 2, 3, 4, 6, 8, 11, 13, 16, 18}));
		sequences.Add(new IntegerSequence("Carmichael number", new int[]{561, 1105, 1729, 2465, 2821, 6601, 8911}));
		sequences.Add(new IntegerSequence("Permutable prime", new int[]{2, 3, 5, 7, 11, 13, 17, 31, 37, 71}));
		sequences.Add(new IntegerSequence("Alcuin's sequence", new int[]{0, 0, 0, 1, 0, 1, 1, 2, 1, 3, 2, 4, 3, 5, 4, 7, 5, 8, 7, 10, 8, 12, 10, 14}));
		sequences.Add(new IntegerSequence("Deficient number", new int[]{1, 2, 3, 4, 5, 7, 8, 9, 10, 11}));
		sequences.Add(new IntegerSequence("Abundant number", new int[]{12, 18, 20, 24, 30, 36, 40, 42, 48, 54}));
		sequences.Add(new IntegerSequence("Look-and-say sequence", new int[]{1, 11, 21, 1211}));
		sequences.Add(new IntegerSequence("Aronson's sequence", new int[]{1, 4, 11, 16, 24, 29, 33, 35, 39, 45}));
		sequences.Add(new IntegerSequence("Fortunate number", new int[]{3, 5, 7, 13, 23, 17, 19, 23, 37, 61}));
		sequences.Add(new IntegerSequence("Sophie Germain prime", new int[]{2, 3, 5, 11, 23, 29, 41, 53, 83, 89}));
		sequences.Add(new IntegerSequence("Semiperfect number", new int[]{6, 12, 18, 20, 24, 28, 30, 36, 40, 42}));
		sequences.Add(new IntegerSequence("Weird number", new int[]{70, 836, 4030, 5830, 7192, 7912, 9272}));
		sequences.Add(new IntegerSequence("Farey sequence numerators", new int[]{0, 1, 0, 1, 1, 0, 1, 1, 2, 1}));
		sequences.Add(new IntegerSequence("Farey sequence denominators", new int[]{1, 1, 1, 2, 1, 1, 3, 2, 3, 1}));
		sequences.Add(new IntegerSequence("Euclid number", new int[]{2, 3, 7, 31, 211, 2311}));
		sequences.Add(new IntegerSequence("Kaprekar number", new int[]{1, 9, 45, 55, 99, 297, 703, 999, 2223, 2728}));
		sequences.Add(new IntegerSequence("Sphenic number", new int[]{30, 42, 66, 70, 78, 102, 105, 110, 114, 130}));
		sequences.Add(new IntegerSequence("Pascal's triangle", new int[]{1, 1, 1, 1, 2, 1, 1, 3, 3, 1}));
		sequences.Add(new IntegerSequence("Happy number", new int[]{1, 7, 10, 13, 19, 23, 28, 31, 32, 44}));
		sequences.Add(new IntegerSequence("Prouhet–Thue–Morse constant", new int[]{0, 1, 1, 0, 1, 0, 0, 1, 1, 0}));
		sequences.Add(new IntegerSequence("Factorion", new int[]{1, 2, 145}));
		sequences.Add(new IntegerSequence("Regular paperfolding sequence", new int[]{1, 1, 0, 1, 1, 0, 0, 1, 1, 1}));
		sequences.Add(new IntegerSequence("Circular prime", new int[]{2, 3, 5, 7, 11, 13, 17, 37, 79, 113}));
		sequences.Add(new IntegerSequence("Superperfect number", new int[]{2, 4, 16, 64, 4096}));
		sequences.Add(new IntegerSequence("Decimal expansion of Champernowne constant", new int[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 1}));
		sequences.Add(new IntegerSequence("Wythoff array", new int[]{1, 2, 4, 3, 7, 6, 5, 11, 10, 9}));
		sequences.Add(new IntegerSequence("Gilbreath's conjecture", new int[]{2, 1, 3, 1, 2, 5, 1, 0, 2, 7}));
		sequences.Add(new IntegerSequence("Home prime", new int[]{1, 2, 3, 211, 5, 23, 7}));
		sequences.Add(new IntegerSequence("Undulating number", new int[]{101, 121, 131, 141, 151, 161, 171, 181, 191, 202}));
		sequences.Add(new IntegerSequence("Achilles number", new int[]{72, 108, 200, 288, 392, 432, 500, 648, 675, 800}));
		sequences.Add(new IntegerSequence("Decimal expansion of Pisot–Vijayaraghavan number", new int[]{1, 3, 2, 4, 7, 1, 7, 9, 5, 7}));
		sequences.Add(new IntegerSequence("Baum–Sweet sequence", new int[]{1, 1, 0, 1, 1, 0, 0, 1, 0, 1}));
		sequences.Add(new IntegerSequence("Juggler sequence", new int[]{0, 1, 1, 5, 2, 11, 2, 18, 2, 27}));
		sequences.Add(new IntegerSequence("Highly totient number", new int[]{1, 2, 4, 8, 12, 24, 48, 72, 144, 240}));
		sequences.Add(new IntegerSequence("Decimal expansion of Chaitin's constant", new int[]{0, 0, 7, 8, 7, 4, 9, 9, 6, 9}));
		sequences.Add(new IntegerSequence("Magic number (physics)e", new int[]{2, 8, 20, 28, 50, 82, 126}));
		sequences.Add(new IntegerSequence("Ramanujan prime", new int[]{2, 11, 17, 29, 41, 47, 59, 67}));
	}

	// Don't know how to import classes yet - putting it here for the time being
	class IntegerSequence
	{
		protected string name;
		protected int[] elements;

		public IntegerSequence(string name, int[] elements)
		{
			this.name = name;
			this.elements = elements;
		}

		public int[] Elements
		{
			get { return this.elements;}
		}

		public string Name
		{
			get { return this.name;}
		}
	}

}
