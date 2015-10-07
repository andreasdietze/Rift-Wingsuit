using UnityEngine;
using System.Collections;

public class StartLevelRayCaster : MonoBehaviour {

	public Camera cam;
	public Collider coll;
	private Ray rayForward, rayDown;
	private float width = 1920 / 2;
	private float height = 1080 / 2;
	public float rayDist = 25.0f;
	public bool startGame = false;
	
	void Start () {
		rayForward = new Ray ();
		rayDown = new Ray ();
	}

	void Update () {
		// Get ray from screen center
		//ray = cam.ScreenPointToRay(new Vector3(width, height, 0.0f));

		// Set ray manual
		rayForward.origin = cam.transform.position;
		rayForward.direction = cam.transform.rotation * Vector3.forward; // * (Vector3.forward * rayDist);
		//ray.direction = cam.transform.TransformDirection;
		Debug.DrawRay (rayForward.origin, rayForward.direction * rayDist, Color.cyan);

		rayDown.origin = cam.transform.position;
		rayDown.direction = cam.transform.rotation * Vector3.down;
		Debug.DrawRay (rayDown.origin, rayDown.direction * rayDist, Color.cyan);

		RaycastHit hit;
		if (coll.Raycast (rayForward, out hit, rayDist)) {
			startGame = true;
			//Debug.Log ("collision!");
		}

		if (Input.GetKeyDown (KeyCode.G))
			startGame = true;

	}
}
