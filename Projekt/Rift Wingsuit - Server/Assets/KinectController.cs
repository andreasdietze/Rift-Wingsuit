using UnityEngine;
using System.Collections;

public class KinectController : Controller
{
    // Kincect control
    public HSDOutputText kinectOutput;
	public StartLevelRayCaster slrc;
    private float kinectYaw;
    private float kinectPitch;
	public bool inverted = false;

    public bool enableYaw = true;
    public bool enablePitch = true;

	private Vector3 mouse = new Vector3 ();
	public bool useMouse = true;
	public bool useKB = true;

	// Const flyspeed
	public bool hasAutoVelocity = false;
	public float flySpeed;

	private bool serverInitiated = false;
	private bool gameStart = false;

	private NetworkManager nManager;
	
	void Start(){
		nManager = (NetworkManager)GameObject.FindGameObjectWithTag("Network").GetComponent("NetworkManager");
	}

    public override Vector3 GetDir()
    {
        Vector3 dir = new Vector3();			// create (0,0,0)

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

		if (hasAutoVelocity) {
			//if(serverInitiated){
				if(gameStart)
					dir.z += flySpeed;
			//}
		}

        dir.Normalize();

        return dir;
    }

	public override Vector3 CalculateViewport(bool inverted)
    {
        // Kinect control
        if (enableYaw)
			kinectYaw = kinectOutput.ReturnDeltaY ();
        else
            kinectYaw = 0.0f;
        if (enablePitch)
            kinectPitch = kinectOutput.ReturnDeltaZ();
        else
            kinectPitch = 0.0f;
		//print ("KinectOutput: " + kinectOutput.ToString ());

        // Mouse Look
		if (useMouse) {
			mouse = Input.mousePosition;
			lastViewport = mouse - lastViewport;
			if (!inverted)
				lastViewport.y = -lastViewport.y;
		}
        lastViewport *= sensitivity;
        lastViewport = new Vector3(transform.eulerAngles.x + lastViewport.y + kinectPitch,
		                           transform.eulerAngles.y + lastViewport.x + kinectYaw, 0);// * sensitivity;

		if (useMouse) {
			transform.eulerAngles = lastViewport;
			lastViewport = Input.mousePosition;
		}

		return transform.eulerAngles; //lastViewport;
    }
	
	// Update is called once per frame
	void Update () {
		//this.serverInitiated = nManager.serverInitiated;  // works
		if (Input.GetKey(KeyCode.Return))
			this.serverInitiated = true;
	}
}
