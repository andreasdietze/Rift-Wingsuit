using UnityEngine;
using System.Collections;

public class KinectController : Controller
{
    // Access to kincect input transformation script
    public HSDOutputText kinectOutput;
	private float kinectSensitivity = 2.75f;

	// Kinect output
    private float kinectYaw;
	public bool enableYaw = true;

    private float kinectPitch;
	public bool enablePitch = true;

	// Script access 
	public StartLevelRayCaster slrc;
	private NetworkManager nManager;
	private FlyCam flyCam;

	// Debug-controlls
	private Vector3 mouse = new Vector3 ();
	public bool useMouse = true;
	public bool useKB = true;
	public bool inverted = false;

	// Const flyspeed
	public bool hasAutoVelocity = false;	
	public float flySpeed = 0.0f;			// between 0 and 1;

	// Network and gameStatus flag
	private bool serverInitiated = false;
	private bool gameStart = false;

	//Physic Global Variables
	public Rigidbody rb;

	// Debug font
	private GUIStyle font;

	void Start(){
		// Get NetworkManager-Script
		nManager = (NetworkManager)GameObject.FindGameObjectWithTag("Network").GetComponent("NetworkManager");

		// Get FlyCam-Script
		flyCam = (FlyCam)GameObject.FindGameObjectWithTag ("MainCamera").GetComponent ("FlyCam");

		// Debug font
		font = new GUIStyle ();
		font.fontSize = 28;
	}

	// Debug kb controls and auto velocity
    public override Vector3 GetDir()
    {
		// Speed
    	Vector3 dir = new Vector3();			

		// Debug kb controls
		if (useKB) {
			if (Input.GetKey (KeyCode.W))
				dir.z += 1.0f;
			if (Input.GetKey (KeyCode.S))
				dir.z -= 1.0f;
			if (Input.GetKey (KeyCode.A))
				dir.x -= 1.0f;
			if (Input.GetKey (KeyCode.D))
				dir.x += 1.0f;
			if (Input.GetKey (KeyCode.Q))
				dir.y -= 1.0f;
			if (Input.GetKey (KeyCode.E))
				dir.y += 1.0f;
		}

		gameStart = slrc.startGame;
		//Debug.Log (gameStart);

		// If all flags are true, add to z
		if (hasAutoVelocity) {				// by unity gui (debug)
			if(serverInitiated){  			// by network	(works)
				if(gameStart){				// by raycast	(works)
					if(flyCam.startFly){	// by fly physics (works)
						flySpeed = 1.0f;
						dir.z += flySpeed;
					}
				}
			}
		}

		dir.Normalize ();
		return dir;
    }

	// Get kinect user input and use it as rotation
	public override Vector3 CalculateViewport(bool inverted)
    {
        // Kinect user input
        if (enableYaw)
			kinectYaw = kinectOutput.ReturnDeltaY () * kinectSensitivity;
        else
            kinectYaw = 0.0f;
        if (enablePitch)
			kinectPitch = kinectOutput.ReturnDeltaZ() * kinectSensitivity;
        else
            kinectPitch = 0.0f;
		//print ("KinectOutput: " + kinectOutput.ToString ());

        // Mouse Look (Debug)
		if (useMouse) {
			mouse = Input.mousePosition;
			lastViewport = mouse - lastViewport;
			if (!inverted)
				lastViewport.y = -lastViewport.y;
		}

		// Actual angel + mouse + kinect
        lastViewport *= sensitivity;
        lastViewport = new Vector3(transform.eulerAngles.x + lastViewport.y + kinectPitch,
		                           transform.eulerAngles.y + lastViewport.x + kinectYaw, 0);// * sensitivity;


		// Lock cam rotation from 0-90 on x
		if (useMouse) {
			transform.eulerAngles = lastViewport;
			Vector3 rot = transform.eulerAngles;
			if(rot.x >= 90.0f)
				rot.x = 0.0f;

			transform.eulerAngles = rot;
			lastViewport = Input.mousePosition;
		}

		//Debug.Log ("Fall speed: " + rb.velocity.y);
		return  lastViewport; //FlyPhysic(kinectYaw, kinectPitch, lastViewport);  // transform.eulerAngles; // lastViewport
    }

	// Update is called once per frame
	void Update () {
		this.serverInitiated = nManager.serverInitiated;  	// works
		//if (Input.GetKey(KeyCode.Return))					// test
			//this.serverInitiated = true;
	}

	void OnGUI(){
		GUI.Label(new Rect(10, 280, 150, 150), "Viewport: " + lastViewport.ToString(), font);
	}





	// Michael 
	Vector3 FlyPhysic(float kinectYaw, float kinectPitch, Vector3 lastViewport){
		// Update position based on the user height from the groud.
		float xachse=transform.position.x; //go left and right
		float yachse=transform.position.y; //high
		if (transform.eulerAngles.y>0.0f){
			;//yachse+=Mathf.Sqrt(yachse*9.8f)-rb.drag/60.0f;
		}else{
			;//yachse-=Mathf.Sqrt(yachse*9.8f)+rb.drag/60.0f;
		}			
		float zachse=transform.position.z; // +flySpeed/60.0f; //forward
		//transform.position = new Vector3(xachse,yachse,zachse);
		lastViewport = new Vector3 (xachse, yachse, zachse);
		// The Drag Force changeable based on the user wide
		// value of kinectPitch:
		//		between 0.0f and 0.009f => increase the drag force and decrease the Y-velocity of player
		//		less than 0.009f 		=> decrease the drag force and increase the Y-velocity of player
		// 		other					=> no change, because invalid movement 

		if (kinectPitch>0.0f) {
			if(kinectPitch<=0.009f){
				rb.drag=20.0f+kinectPitch*1000.0f;
			}else{
				rb.drag=20.0f+kinectPitch*100.0f;
			}
			lastViewport.z -= kinectPitch;
			//transform.eulerAngles = lastViewport;
		}
		return lastViewport;
	}
}
