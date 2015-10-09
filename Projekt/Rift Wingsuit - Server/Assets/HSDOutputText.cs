using UnityEngine;
using System.Collections;

// Kinect input transformation:
// During tracking the right shoulder and right hand joint
// an offset between this two vertices can be computed
// and used as user input.
// There are two different offsets. 
// One for height (arm y-movement) and 
// another for the depth (arm z-movement).
public class HSDOutputText : MonoBehaviour {

	// Joint transformation
	public Transform rightShoulder;
	public Transform rightHand;
	
	// Delta height between shoulder and hand
	public float deltaY;

	// Delta depth between shoulder and hand
	public float deltaZ;

	// GUI Style
	public GUIStyle labelFont;

	// Use this for initialization
	void Start () {
		// Debug font
		labelFont = new GUIStyle ();
		labelFont.fontSize = 28;
	}
	
	// Update is called once per frame
	void Update () {
		// Compute delta y
		deltaY = GetDeltaY (rightHand.position.y,
		                    rightShoulder.position.y);

		// Compute delta z
		deltaZ = GetDeltaZ (rightHand.position.z,
		                    rightShoulder.position.z);
	}
 
	// Compute delta y
	private float GetDeltaY(float shoulder, float hand){
		return hand - shoulder;
	}

	// Compute delta z
	private float GetDeltaZ(float shoulder, float hand){
		return hand - shoulder;
	}

	// Getter
	public float ReturnDeltaY(){
		return deltaY;
	}

	public float ReturnDeltaZ(){
		return deltaZ;
	}

	// Update debug info
	void OnGUI(){
		GUI.Label(new Rect(10, 10, 150, 150), "ShoulderY: " 
		          + rightShoulder.position.y.ToString(), labelFont);
		GUI.Label(new Rect(10, 40, 150, 150), "HandY: " 
		          + rightHand.position.y.ToString(), labelFont);
		GUI.Label(new Rect(10, 70, 150, 150), "OffsetY: " 
		          + deltaY.ToString(), labelFont);
		GUI.Label(new Rect(10, 100, 150, 150), "HandZ: " 
		          + rightHand.position.z.ToString(), labelFont);
		GUI.Label(new Rect(10, 130, 150, 150), "OffsetZ: " 
		          + deltaZ.ToString(), labelFont);
	}
}
