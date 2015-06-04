using UnityEngine;
using System.Collections;

public class HSDOutputText : MonoBehaviour {

	public Transform rightShoulder;
	public Transform rightHand;
	
	// Delta height between shoulder and hand
	public float delta;

	// GUI Style
	public GUIStyle labelFont;

	// Use this for initialization
	void Start () {
		labelFont = new GUIStyle ();
		labelFont.fontSize = 32;
	}
	
	// Update is called once per frame
	void Update () {
		// Compute delta
		delta = GetDelta (rightHand.position.y, rightShoulder.position.y);
	}

	void OnGUI(){
		GUI.Label(new Rect(10, 10, 150, 150), "Shoulder: " + rightShoulder.position.y.ToString(), labelFont);
		GUI.Label(new Rect(10, 50, 150, 150), "Hand: " + rightHand.position.y.ToString(), labelFont);
		GUI.Label(new Rect(10, 90, 150, 150), "Offset: " + delta.ToString(), labelFont);
	}

	public float GetDelta(float shoulder, float hand){
		return hand - shoulder;
	}

	public float ReturnDetla()
	{
		return delta;
	}
}
