using UnityEngine;
using System.Collections;

public class KeyboardAndMouseController : Controller {

	public float flySpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override Vector3 GetDir()
    {
        Vector3 dir = new Vector3();			// create (0,0,0)

		if (Input.GetKey(KeyCode.W)) dir.z += flySpeed;
		if (Input.GetKey(KeyCode.S)) dir.z -= flySpeed;
		if (Input.GetKey(KeyCode.A)) dir.x -= flySpeed;
		if (Input.GetKey(KeyCode.D)) dir.x += flySpeed;
		if (Input.GetKey(KeyCode.Q)) dir.y -= flySpeed;
		if (Input.GetKey(KeyCode.E)) dir.y += flySpeed;
	
        dir.Normalize();

        return dir;
    }

    public override Vector3 CalculateViewport(bool inverted)
    {
        // Mouse Look
        lastViewport = Input.mousePosition - lastViewport;
        if (!inverted) lastViewport.y = -lastViewport.y;
        lastViewport *= sensitivity;
        lastViewport = new Vector3(transform.eulerAngles.x + lastViewport.y,
                                 transform.eulerAngles.y + lastViewport.x, 0);
        transform.eulerAngles = lastViewport;
        lastViewport = Input.mousePosition;

        return lastViewport;
    }
}
