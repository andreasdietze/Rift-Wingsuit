using UnityEngine;
using System.Collections;

public class FlyCam : MonoBehaviour
{
    public Controller controller;
    // smoothing
    public bool smooth = true;
    public float acceleration = 0.1f;
    protected float actSpeed = 0.0f;			// keep it from 0 to 1
    public float speed = 50.0f;		// max speed of camera
    public bool inverted = false;

    protected Vector3 lastDir = new Vector3();
    protected Vector3 lastViewport = new Vector3();

	public bool useKinect = false;
	private Vector3 lastMouse = new Vector3(255, 255, 255);
	public float sensitivity = 0.25f; 		// keep it from 0..1
	// Kincect control
	//public HSDOutputText kinectOutput;
	private float kinectYaw;
	private float kinectPitch;
	
	// Cfg
	public bool enableYaw = true;
	public bool enablePitch = true;
    void Start()
    {
		if(!useKinect)
        	controller = gameObject.AddComponent<KinectController>();
    }

    void Update()
    {
		Vector3 dir = new Vector3();// create (0,0,0)
		if (useKinect) {
			// Kinect control
			/*if (enableYaw)
				kinectYaw = kinectOutput.ReturnDeltaY ();
			else
				kinectYaw = 0.0f;
			
			if (enablePitch)
				kinectPitch = kinectOutput.ReturnDeltaZ ();
			else
				kinectPitch = 0.0f;*/
			
			// Mouse Look
			lastMouse = Input.mousePosition - lastMouse;
			if ( ! inverted ) lastMouse.y = -lastMouse.y;
			lastMouse *= sensitivity;
			lastMouse = new Vector3( transform.eulerAngles.x + lastMouse.y + kinectPitch,
			                        transform.eulerAngles.y + lastMouse.x + kinectYaw, 0);
			transform.eulerAngles = lastMouse;
			lastMouse = Input.mousePosition;
			
			
			// Movement of the camera
			if (Input.GetKey(KeyCode.W)) dir.z += 1.0f;
			if (Input.GetKey(KeyCode.S)) dir.z -= 1.0f;
			if (Input.GetKey(KeyCode.A)) dir.x -= 1.0f;
			if (Input.GetKey(KeyCode.D)) dir.x += 1.0f;
			if (Input.GetKey(KeyCode.Q)) dir.y -= 1.0f;
			if (Input.GetKey(KeyCode.E)) dir.y += 1.0f;
			
			//dir.z += 1.0f;
			
			dir.Normalize();
		} else {
			//Things to do here...
			lastViewport = controller.CalculateViewport (inverted);
			dir = controller.GetDir ();
		}

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

    // RESET on Boden

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boden"))
            Application.LoadLevel(0);
    }

    void OnGUI()
    {
        //GUILayout.Box("Vector: " + lastViewport.ToString(), GUILayout.Width(150), GUILayout.Height(25));
		//GUI.Label(new Rect(0, 50, 100, 50), "fooo");
        //GUI.Label(new Rect(10, 130, 150, 150), "Delta: " + kinectY.ToString(), kinectOutput.labelFont);
    }
}
