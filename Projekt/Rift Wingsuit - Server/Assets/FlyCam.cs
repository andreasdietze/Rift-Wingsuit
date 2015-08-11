using UnityEngine;
using System.Collections;

public class FlyCam : MonoBehaviour
{
    
    // smoothing
    public bool smooth = true;
    public float acceleration = 0.2f;
    protected float actSpeed = 0.0f;	// keep it from 0 to 1
    public float speed = 50.0f;			// max speed of camera
    public bool inverted = false;

    protected Vector3 lastDir = new Vector3();
    protected Vector3 lastViewport = new Vector3();

	private Vector3 lastMouse = new Vector3(255, 255, 255);
	public float sensitivity = 0.25f;	// keep it from 0..1

	// Global controller settings
	public bool useKinect = false;
	public bool useMouseKB = false;
	public bool usePad = false;
	private Controller controller;
	public Rigidbody playerRidgid;

	// Fly physics
	Vector3 fallVelocity = new Vector3();

    void Start()
    {
		if (useKinect) controller = (Controller)GameObject.Find ("RiftCam").GetComponent ("KinectController"); //gameObject.AddComponent<KinectController> ();
		else if (useMouseKB) controller = (Controller)GameObject.Find ("RiftCam").GetComponent ("KeyboardAndMouseController");
		else if (usePad) controller = (Controller)GameObject.Find ("RiftCam").GetComponent ("XBoxController");	
    }

    void Update()
    {
		// Setup direction vector
		Vector3 dir = new Vector3();// create (0,0,0)

		// Compute direction by active controller
		lastViewport = controller.CalculateViewport (inverted);
		dir = controller.GetDir ();

		// Compute free fall velocity
		// 1 m/s = 3,6 km/h   -> fall speed m/s * 3.6f
		// http://www.frustfrei-lernen.de/mechanik/freier-fall.html
		// http://de.wikihow.com/Die-maximale-Fallgeschwindigkeit-berechnen
		// Maximale Fallgeschwindigkeit in X-Position ~ 198 km/h
		// Maximale Fallgeschwindigkeit kopfüber ~ 500 km/h
		float s = Mathf.Pow (Time.time, 2); // s²
		float g = 9.81f / s; // 9.81m/s²
		float h = (g * s) / 2; // Fallstrecke = (g * t²) / 2
		float v = (g * s); // Mathf.Sqrt (s) Fallgeschwindigkeit
		
		fallVelocity = playerRidgid.velocity;
		fallVelocity.y -= v * Time.deltaTime; // h * Time.deltaTime;
	
		// Set maximal fall speed in x-position
		// TODO: - get playerorientation x-axis (test first with cam)
		// 		 - set the max fall speed in dependence of the angle between x0 and x90
		float playerXR = GameObject.Find ("RiftCam").transform.rotation.eulerAngles.x;
		//Debug.Log ("Player rotation x: " + playerXR);
		
		// Set max fall speed
		// TODO: - intervall could be from 1 * 198 in X pos to 2.5 * 198 in arrowPos (angle about 90?)
		// 		 - otherwise player fly against up vector (left part of unit cirlce)
		// 		 - so we need to set map x0-x90 to x0 = 1.0 to x90 = 2.5
		// 		 - alternative get cos via dotprodukt between players forward and down vector

		float ms = fallVelocity.y;  // meter per second

		if (MStoKMH (ms) > 500.0f) { // && x-position -> playerorientationX
			fallVelocity.y = 500.0f;
			ms = 500.0f;
		}

		//Debug.Log ("Fall time" + (int)Time.time);
		//Debug.Log ("Fall speed ms/s: " +  Mathf.Abs(ms));  //Mathf.Abs(playerRidgid.velocity.y)); 
		//Debug.Log ("Fall speed km/h: " +  Mathf.Abs(MStoKMH (ms))); //Mathf.Abs(playerRidgid.velocity.y * 3.6f));
	
		// Set computed fall velocity 
		//playerRidgid.velocity = fallVelocity;

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
		
        if (smooth)
            transform.Translate(lastDir * actSpeed * speed * Time.deltaTime);
        else
            transform.Translate(dir * speed * Time.deltaTime); 
    }
	
	// 1 m/s = 3,6 km/h   -> fall speed m/s * 3.6f
	private float MStoKMH(float ms){
			return ms * 3.6f;
	}

    // Reload level if player collides with terrain
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boden"))
            Application.LoadLevel(0);

		// gamelogics
		// TODO: -> set player to spezified possition after colliding with the terrain
		// 		 -> set player orientation
		// 		 -> handle server/client-sync and connections
    }

    void OnGUI()
    {
        GUILayout.Box("Vector: " + lastViewport.ToString());
        //GUI.Label(new Rect(10, 130, 150, 150), "Delta: " + kinectY.ToString(), kinectOutput.labelFont);
    }
}
