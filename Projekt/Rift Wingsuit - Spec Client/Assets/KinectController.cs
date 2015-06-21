using UnityEngine;
using System.Collections;

public class KinectController : Controller
{
    // Kincect control
    public HSDOutputText kinectOutput;
    private float kinectYaw;
    private float kinectPitch;
	public bool inverted = false;

    public bool enableYaw = true;
    public bool enablePitch = true;
	// Use this for initialization
	void Start () {
	
	}

    public override Vector3 GetDir()
    {
        Vector3 dir = new Vector3();			// create (0,0,0)

        if (Input.GetKey(KeyCode.W)) dir.z += 1.0f;
        if (Input.GetKey(KeyCode.S)) dir.z -= 1.0f;
        if (Input.GetKey(KeyCode.A)) dir.x -= 1.0f;
        if (Input.GetKey(KeyCode.D)) dir.x += 1.0f;
        if (Input.GetKey(KeyCode.Q)) dir.y -= 1.0f;
        if (Input.GetKey(KeyCode.E)) dir.y += 1.0f;

        dir.z += 1.0f;

        dir.Normalize();

        return dir;
    }

	public override Vector3 CalculateViewport(bool inverted)
    {
        // Kinect control
       /* if (enableYaw)
			kinectYaw = kinectOutput.ReturnDeltaY ();
        else
            kinectYaw = 0.0f;
        if (enablePitch)
            kinectPitch = kinectOutput.ReturnDeltaZ();
        else
            kinectPitch = 0.0f;
		print ("KinectOutput: " + kinectOutput.ToString ());*/
        // Mouse Look
        lastViewport = Input.mousePosition - lastViewport;
        if (!inverted) lastViewport.y = -lastViewport.y;
        lastViewport *= sensitivity;
        lastViewport = new Vector3(transform.eulerAngles.x + lastViewport.y + kinectPitch,
                                 transform.eulerAngles.y + lastViewport.x + kinectYaw, 0);
        transform.eulerAngles = lastViewport;
        lastViewport = Input.mousePosition;

		return transform.eulerAngles; //lastViewport;
    }
	
	// Update is called once per frame
	void Update () {
	}
}
