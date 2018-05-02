using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;
using System;

public class GameHandlerTry2 : MonoBehaviour {
	public string weString;
	public GameObject panel;
	public List<Text> letters;
	public Text letter;
	public Canvas renderCanvas;

	System.Threading.Timer timer;
	public float counter;
	public int counterInt;

	public float bpm;

	int timerUpdatePeriod = 41;

	public GameObject box;

	public string currentCharacter;

	public float score;
	bool keyAlreadyPressed;

	public Text scoreText;

	public StreamWriter writer;
	string path = "Assets/Resources/test.txt";


	public float mu = 3.658f;
	public float sigma = 2f; // 6.599f; 

	public Text affirmation;

	void Start () {
		TextAsset file = (TextAsset)Resources.Load("output");
		StringReader reader = new StringReader (file.text);
		string keylog = reader.ReadToEnd();
		reader.Close();

		keylog = new Regex ("\n+").Replace (keylog, "\n");
		string[] weLines = keylog.Split ('\n');

		int desiredSize = 1000;

		int startingPlace = (int)UnityEngine.Random.Range (0, weLines.Length - desiredSize);
		for (int i = startingPlace; i < startingPlace + desiredSize; i++) {
			weString += weLines [i];
		}

//		weString = "";
//		while (weString.Length < desiredSize) {
//			for (int i = 0; i < 3; i++) {
//				for (int j = 0; j < 7; j++) { weString += "j"; }
//				weString += "k";
//			}
//			for (int j = 0; j < 6; j++) {
//				weString += "j";
//			}
//			weString += "k ";
//		}


//		for (int i = 0; i < desiredSize; i++) {
//			weString += UnityEngine.Random.value < 0.5f ? "j" : "k";
//		}

		panel = Instantiate (panel, transform.position, panel.transform.rotation);
		panel.gameObject.transform.position = new Vector2 (300f, 200f);

		panel.transform.SetParent (renderCanvas.transform);

		for (int i = 0; i < desiredSize; i++) {
			letters.Add ((Text)Instantiate (letter, box.transform.position, transform.rotation));
			letters[i].text = weString [i] + "";
			letters[i].transform.SetParent (panel.transform, false);
		}


		timer = new System.Threading.Timer (UpdateProperty); 
		timer.Change (timerUpdatePeriod, timerUpdatePeriod);

	}

	void Update () {
		panel.gameObject.transform.position = new Vector2 (300f - counter * 27f, panel.transform.position.y);
		currentCharacter = getCurrentLetter();

		scoreText.text = "Score: " + (int)(score+0.5f) + ", current char: " + currentCharacter;

		if (counterInt != (int)counter) {
			if (!keyAlreadyPressed) {
				if (currentCharacter != " ")
					score--;
			}
			keyAlreadyPressed = false;
			counterInt = (int)counter;

			writer = new StreamWriter("Assets/Resources/socrelog.txt", true);
			writer.WriteLine(score);
			writer.Close();
		}
	}

	private void UpdateProperty(object state) {
		lock(this) {
			counter += (bpm / 60f)/(1000f / timerUpdatePeriod); 
		}
	}

	public int currentCharacterIndex() {
		float minDiff = 9001f;
		int indexOfMinDiff = -1;
		for (int i = 0; i < letters.Count; i++) {
			float diff = Mathf.Abs (letters [i].transform.position.x - box.transform.position.x);
			if (diff < minDiff) {
				minDiff = diff;
				indexOfMinDiff = i;
			}
		}
		return indexOfMinDiff;
	}

	public float getCrrentMinDiff() {
		float minDiff = 9001f;
		float minDiffNotAbs = 0f;
		int indexOfMinDiff = -1;
		for (int i = 0; i < letters.Count; i++) {
			float diff = letters [i].transform.position.x - box.transform.position.x;
			float absDiff = Mathf.Abs(diff);
			if (absDiff < minDiff) {
				minDiff = absDiff;
				minDiffNotAbs = diff;
				indexOfMinDiff = i;
			}
		}
//		return minDiff;
		return minDiffNotAbs;
	}

	public Text currentCharacterText() {
		return letters [currentCharacterIndex ()];
	}

	public string getCurrentLetter() {
		return currentCharacterText().text;
	}

	public void OnGUI() {
		Event e = Event.current;
		if (e.isKey && Input.anyKeyDown && e.keyCode != KeyCode.None) {

			float pts = calculatePoints (getCrrentMinDiff ());

			writer = new StreamWriter(path, true);
//			writer.WriteLine(getCrrentMinDiff ());
			writer.WriteLine(pts);
			writer.Close();



			Debug.Log ("Detected key code: " + e.keyCode.ToString ().ToLower ());
			string key = e.keyCode.ToString ().ToLower ();
			if (key == "space") {
				key = " ";
			}
			if (key == currentCharacter || e.keyCode.ToString () == currentCharacter) {
				if (keyAlreadyPressed) {
					score--;
					affirmation.text = "";
				} else {
					score += pts;
					if (pts > 0.961091f) { // above Q3
						affirmation.text = "Perfect!";
					} else if (pts > 0.83858085f) { // above median
						affirmation.text = "Awesome!";
					} else if (pts > 0.716573862548f) { // above mean
						affirmation.text = "Good!";
//					} else if (pts > 0.5258039f) { // above Q1
//						affirmation.text = "OK!";
					} else {
						affirmation.text = "";
					}
				}
			} else {
				score--;
				affirmation.text = "";
			}

//			if (pts 

			keyAlreadyPressed = true;
		}
	}

	public float calculatePoints(float diff) {
//		return e^-((x-mu)^2/(2sigma^2))
//		return (float) Math.Pow(Math.E, - Math.Pow((double)(diff - mu), 2.0) / (2.0 * sigma*sigma));
		return (float) Math.Pow(Math.E, - Math.Pow((double)(diff - mu), 2.0) / (2.0 * sigma*sigma));
	}
}
