using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.UI; 

public class GameHandler:MonoBehaviour {

	public string textContents; 
	public Text textObject; 
	public int maxTextLength; 
	public float keyPressPeriod; 
	public float bpm; 

	int timerUpdatePeriod = 41; 
	System.Threading.Timer timer; 
	public float counter; 

	int previousCounterInt; 

	Font font; 

	public int currentCharWidth;

	public float defaultTextX;

	public string currentCharacter;
	public int charIndex;

	public int score;
	bool keyAlreadyPressed;

	public Text scoreText;

	void Start () {
		font = Font.CreateDynamicFontFromOSFont("Menlo", 45);

		textContents = ""; 
		timer = new System.Threading.Timer (UpdateProperty); 
		timer.Change (timerUpdatePeriod, timerUpdatePeriod);
	}

	void Update () {
		if ((int)counter != previousCounterInt) {
			if (textContents.Length > 0)
				currentCharWidth = GetSize (font, textContents.Substring (0, 1)) + 2;
			updateText();
			textObject.text = textContents; 
			previousCounterInt = (int)counter;

			defaultTextX -= currentCharWidth;

			if (defaultTextX <= 125) {
				charIndex++;
			}
			keyAlreadyPressed = false;
		}

		string temp = textContents.Substring (charIndex);
		if (temp.Length > 0)
			currentCharacter = temp.Substring (0, 1);


		if (textContents.Length < maxTextLength)
			textObject.rectTransform.position = new Vector2 (defaultTextX - (currentCharWidth * (counter % 1)), textObject.rectTransform.position.y);

		scoreText.text = "Score: " + score;
	}

	public void OnGUI() {
		Event e = Event.current;
		if (e.isKey && Input.anyKeyDown && e.keyCode != KeyCode.None) {
			
			Debug.Log ("Detected key code: " + e.keyCode);

			if (e.keyCode.ToString ().ToLower () == currentCharacter) {
				if (keyAlreadyPressed)
					score--;
				else
					score++;
				
			} else {
				score--;
			}

			keyAlreadyPressed = true;
		}
	}

	void updateText() {
		if (textContents.Length < maxTextLength) {
			char letter = (char)Random.Range (97, 123);
			textContents += letter + " ";
		}
	}

	private void UpdateProperty(object state) {
		lock(this) {
			counter += (bpm / 60f)/(1000f / timerUpdatePeriod); 
		}
	}


	public int GetSize(Font font, string text) {
		int maxX = 0;
		font.RequestCharactersInTexture(text); 

		foreach (char c in text) {
			CharacterInfo ci; 
			if (font.GetCharacterInfo(c, out ci)) {
				maxX += ci.maxX; 
			}
		}

		return maxX; 
	}
}
