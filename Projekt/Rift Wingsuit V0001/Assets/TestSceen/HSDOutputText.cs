using UnityEngine;
using System.Collections;

public class HSDOutputText : MonoBehaviour {

	public Transform rightShoulder;
	public Transform rightHand;
	
	// Delta height between shoulder and hand
	public float deltaY;
	public float deltaZ;

	// Offet depth between shoulder and hand

	// GUI Style
	public GUIStyle labelFont;

	// Use this for initialization
	void Start () {
		labelFont = new GUIStyle ();
		labelFont.fontSize = 28;
	}
	
	// Update is called once per frame
	void Update () {
		// Compute delta
		deltaY = GetDeltaY (rightHand.position.y, rightShoulder.position.y);
		deltaZ = GetDeltaZ (rightHand.position.z, rightShoulder.position.z);
	}
 
	void OnGUI(){
		GUI.Label(new Rect(10, 10, 150, 150), "ShoulderY: " + rightShoulder.position.y.ToString(), labelFont);
		GUI.Label(new Rect(10, 40, 150, 150), "HandY: " + rightHand.position.y.ToString(), labelFont);
		GUI.Label(new Rect(10, 70, 150, 150), "OffsetY: " + deltaY.ToString(), labelFont);
		GUI.Label(new Rect(10, 100, 150, 150), "HandZ: " + rightHand.position.z.ToString(), labelFont);
		GUI.Label(new Rect(10, 130, 150, 150), "OffsetZ: " + deltaZ.ToString(), labelFont);
	}

	private float GetDeltaY(float shoulder, float hand){
		return hand - shoulder;
	}

	public float ReturnDeltaY()
	{
		return deltaY;
	}

	private float GetDeltaZ(float shoulder, float hand){
		return hand - shoulder;
	}

	public float ReturnDeltaZ(){
		return deltaZ;
	}




}
