using UnityEngine;
using System.Collections;
using System.Linq;

public class FlyCam : MonoBehaviour
{


    // smoothing
    public bool smooth = true;
    public float acceleration = 0.2f;
    protected float actSpeed = 0.0f;	// keep it from 0 to 1
    public float speed = 50.0f;			// max speed of camera
    public bool inverted = false;
	private Vector3 lastMouse = new Vector3(255, 255, 255);
	public float sensitivity = 0.25f;	// keep it from 0..1

	// Direction
    protected Vector3 lastDir = new Vector3();
    protected Vector3 lastViewport = new Vector3();
	Quaternion startingRot;
	Vector3 startingPos;

	// Global controller settings
	public bool useKinect = false;
	public bool useMouseKB = false;
	public bool usePad = false;
	private Controller controller;
	public Rigidbody playerRidgid;

	// Fly physics
	Vector3 fallVelocity = new Vector3();
	public bool startFly = false;
	Vector3 addToZ = Vector3.zero;

	// Script access
	NetworkManager nManager;
	StartLevelRayCaster slrc;

	// Debug font
	GUIStyle font;

    void Start()
    {
		// Set preferred controller
		if (useKinect) controller = (Controller)GameObject.Find ("RiftCam").GetComponent ("KinectController"); //gameObject.AddComponent<KinectController> ();
		else if (useMouseKB) controller = (Controller)GameObject.Find ("RiftCam").GetComponent ("KeyboardAndMouseController");
		else if (usePad) controller = (Controller)GameObject.Find ("RiftCam").GetComponent ("XBoxController");	

		// Get NetworkManager-Script
		nManager = (NetworkManager)GameObject.FindGameObjectWithTag("Network").GetComponent("NetworkManager");

		// Debug font
		font = new GUIStyle ();
		font.fontSize = 28;

		// Tmp start transformation for reset
        startingPos = playerRidgid.position;
        startingRot = playerRidgid.rotation;
    }

    void Update()
    {
		// Setup direction vector
		Vector3 dir = new Vector3();// create (0,0,0)

		// Compute direction by active controller
		lastViewport = controller.CalculateViewport (inverted);
		dir = controller.GetDir ();

		// -------------------------------- Verschiedene Ansätze der Flugphysik --------------------------------

		// Compute free fall velocity
		// 1 m/s = 3,6 km/h   -> fall speed m/s * 3.6f
		// http://www.frustfrei-lernen.de/mechanik/freier-fall.html
		// http://de.wikihow.com/Die-maximale-Fallgeschwindigkeit-berechnen
		// Maximale Fallgeschwindigkeit in X-Position ~ 198 km/h
		// Maximale Fallgeschwindigkeit kopfüber ~ 500 km/h
		float s = Mathf.Pow (Time.time, 2); // s²
		float g = 9.81f / s; // 9.81m/s²
		float h = (g * s) / 2; // Fallstrecke = (g * t²) / 2
		float v = (g * Time.time); // Mathf.Sqrt (s) Fallgeschwindigkeit

		slrc = (StartLevelRayCaster)GameObject.FindGameObjectWithTag ("MainCamera").GetComponent ("StartLevelRayCaster");
		if (slrc.startGame) {
			playerRidgid.useGravity = true;
		}

		//Debug.Log (v);
		fallVelocity = playerRidgid.velocity;
		
		//fallVelocity.y -= 9.81f * Time.deltaTime; // h * Time.deltaTime;
		//Debug.Log (fallVelocity);
		// Set maximal fall speed in x-position
		// TODO: - get playerorientation x-axis (test first with cam)
		// 		 - set the max fall speed in dependence of the angle between x0 and x90
		
		// http://wingsuit.de/wingsuit-lernen/fur-fallschirmspringer/aerodynamische-grundlagen/
		// The easy way:
		// - Speed ca 130-250 kmh bei fallrate von 40-50 kmh
		// Math way:
		// Formel: av = gvy * sin(playerRotX) - dämpfung * V²

		// Set max fall speed
		// TODO: - intervall could be from 1 * 198 in X pos to 2.5 * 198 in arrowPos (angle about 90?)
		// 		 - otherwise player fly against up vector (left part of unit cirlce)
		// 		 - so we need to set map x0-x90 to x0 = 1.0 to x90 = 2.5
		// 		 - alternative get cos via dotprodukt between players forward and down vector

		// Dampfungsfaktor abhängig von der Fläche A 
		// -> A abhängig von Winkel des Spielers zwischen forward und down.
		// -------------------------------- Verschiedene Ansätze der Flugphysik --------------------------------


		// Fallgeschwindigkeit während des Flugs
		float ms = fallVelocity.y;  // meter per second
		if (Mathf.Abs(MStoKMH (ms)) > 50.0f) { // && x-position -> playerorientationX
			fallVelocity.y = -50.0f  / 3.6f;
			ms = -50.0f / 3.6f;
			startFly = true;
		}

		//Debug.Log ("Fall time" + (int)Time.time);
		//Debug.Log ("Fall speed ms/s: " +  Mathf.Abs(ms));  //Mathf.Abs(playerRidgid.velocity.y)); 
		//Debug.Log ("Fall speed km/h: " +  Mathf.Abs(MStoKMH (ms))); //Mathf.Abs(playerRidgid.velocity.y * 3.6f));
	
		// Set computed fall velocity 
		playerRidgid.velocity = fallVelocity;

        // Movement of the camera
        if (dir != Vector3.zero)
        {
            // some movement 
            if (actSpeed < 1)
                actSpeed += acceleration * Time.deltaTime * 40;
            else
                actSpeed = 1.0f;

            lastDir = dir;
        }
        else
        {
            // should stop
            if (actSpeed > 0)
                actSpeed -= acceleration * Time.deltaTime * 20;
            else
                actSpeed = 0.0f;
        }

		// Use orientation accleration
		if(startFly)
			addToZ = UpdateSpeedSimple ();
		
        if (smooth)
            //transform.Translate(lastDir * actSpeed * speed * Time.deltaTime);
			transform.Translate((lastDir + addToZ) * speed * actSpeed * Time.deltaTime);
        else
            transform.Translate(dir * speed * Time.deltaTime); 
		
    }
	
	// 1 m/s = 3,6 km/h   -> fall speed m/s * 3.6f
	private float MStoKMH(float ms){
			return ms * 3.6f;
	}

	// Nach Formel
	Vector3 UpdateSpeed(){
		// Winkel zwischen z- und y-Achse: ---------
		// 0° x -> vec3.back			   | \
		// 90°  x -> vec3.down			   |    \
		// alpha -> x rotation   		   | alpha \
		float alpha = transform.rotation.eulerAngles.x;
		if(alpha >= 90.0f)
			alpha = 90.0f; // -alpha; 
		
		// Geschindigkeit toAdd (z):
		// toAdd = gravityVec * sin(alpha) - (attenuation) * V²
		// Dämpfung abhängig von x rotation, 90° -> A = 1.0, 0° -> A = 0.0
		// att   =  xrot / 90
		Vector3 gravity = Vector3.zero;
		if (nManager.serverInitiated)
			gravity =  new Vector3(0.0f, playerRidgid.velocity.y, 0.0f);   // Vector3.down;  playerRidgid.velocity
		float att = alpha / 90.0f; 
		Vector3 toAdd = (gravity * Mathf.Sin (alpha)); //  - ((1.0f * att) / 2); 
		return toAdd - (Vector3.forward * ((1.0f * att) / 2)); 
	}

	// Einfache, derzeit verwendete Flugphysik
	Vector3 UpdateSpeedSimple(){
		float alpha = transform.rotation.eulerAngles.x;
		if(alpha >= 90.0f)
			alpha = 0.0f;

		float att = alpha / 90.0f;

		return  Vector3.forward * att;
	}

    private GameObject GetParent(Transform trans)
    {
        GameObject result = null;
        if (trans != null)
        {
            result = trans.parent.gameObject;
        }
        return result;
    }

    // Reload level if player collides with terrain
    void OnTriggerEnter(Collider other)
    {
		if (other.CompareTag ("Boden")) {
			Application.LoadLevel(0);
            playerRidgid.rotation= startingRot;
            playerRidgid.position= startingPos;
        }
        else if(other.name.Equals("StartWP"))
        {
            //"StartWP" is the name of every ring, which means a parent object is
            //existent. Check for the Finisher tag, if existent, start fading.
            GameObject actualWaypointRing = GetParent(other.transform);
            if (actualWaypointRing.CompareTag("FinisherRing"))
            {
                //Fade out in around 5 seconds and restarts the level
                Fader.StartFade(Color.white);
            }
        }
    }

    void OnGUI()
    {
        //GUILayout.Box("Vector: " + lastViewport.ToString());
        //GUI.Label(new Rect(10, 130, 150, 150), "Delta: " + kinectY.ToString(), kinectOutput.labelFont);
		Vector3 finSpeed = (lastDir + addToZ) * speed * actSpeed * Time.deltaTime;
		GUI.Label(new Rect(10, 250, 150, 150), "Speed : " + finSpeed.z, font);
    }
}
