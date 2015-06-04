using UnityEngine;
using System.Collections;

public class FlyCam : MonoBehaviour {
	
	
	public float speed = 50.0f;		// max speed of camera
	public float sensitivity = 0.25f; 		// keep it from 0..1
	public bool inverted = false;
	
	
	private Vector3 lastMouse = new Vector3(255, 255, 255);
	
	
	// smoothing
	public bool smooth = true;
	public float acceleration = 0.1f;
	private float actSpeed = 0.0f;			// keep it from 0 to 1
	private Vector3 lastDir = new Vector3();

	// Kincect control
	//float kinectY;
	// Find Camera object
	//GameObject fly;
	// Access to Head/Shoulder/Delta-Script (HSDOutput)
	//HSDOutputText kinectOutput;

	public HSDOutputText kinectOutput;
	
	
	// Use this for initialization
	void Start () {
		//fly = GameObject.Find("Fly Cam");
		//kinectOutput = fly.GetComponent<HSDOutputText>(); 
		//kinectY = kinectOutput.ReturnDetla();
	}
	
	// Update is called once per frame
	void Update () {
		
		// Kinect control
		//fly = GameObject.Find("Fly Cam");
		//kinectOutput = fly.GetComponent<HSDOutputText>(); 
		//kinectY = kinectOutput.ReturnDetla();
		//print ("kinectY: " + kinectY.ToString());

		// Mouse Look
		lastMouse = Input.mousePosition - lastMouse;
		if ( ! inverted ) lastMouse.y = -lastMouse.y;
		lastMouse *= sensitivity;
		lastMouse = new Vector3( transform.eulerAngles.x + lastMouse.y, transform.eulerAngles.y + lastMouse.x + kinectOutput.ReturnDetla(), 0);
		transform.eulerAngles = lastMouse;
		lastMouse = Input.mousePosition;
		
	
		// Movement of the camera
		
		Vector3 dir = new Vector3();			// create (0,0,0)
		
		if (Input.GetKey(KeyCode.W)) dir.z += 1.0f;
		if (Input.GetKey(KeyCode.S)) dir.z -= 1.0f;
		if (Input.GetKey(KeyCode.A)) dir.x -= 1.0f;
		if (Input.GetKey(KeyCode.D)) dir.x += 1.0f;
		if (Input.GetKey(KeyCode.Q)) dir.y -= 1.0f;
		if (Input.GetKey(KeyCode.E)) dir.y += 1.0f;

		dir.z += 1.0f;

		dir.Normalize();
		
		
		if (dir != Vector3.zero) {
			// some movement 
			if (actSpeed < 1)
				actSpeed += acceleration * Time.deltaTime * 40;
			else 
				actSpeed = 1.0f;
			
			lastDir = dir;
		} else {
			// should stop
			if (actSpeed > 0)
				actSpeed -= acceleration * Time.deltaTime * 20;
			else 
				actSpeed = 0.0f;
		}

		
		if (smooth) 
			transform.Translate( lastDir * actSpeed * speed * Time.deltaTime );
		
		else 
			transform.Translate ( dir * speed * Time.deltaTime );
		
		
	}
	
	void OnGUI() {
		GUILayout.Box ("actSpeed: " + actSpeed.ToString());
		//GUI.Label(new Rect(10, 130, 150, 150), "Delta: " + kinectY.ToString(), kinectOutput.labelFont);
	}

	// RESET on Boden
	
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Boden"))
			Application.LoadLevel (0);  
	}
}
