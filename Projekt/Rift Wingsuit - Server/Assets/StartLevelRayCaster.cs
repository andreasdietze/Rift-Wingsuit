using UnityEngine;
using System.Collections;

// Start game via look at an object (or press g)
public class StartLevelRayCaster : MonoBehaviour {

	// Origin of the rays
	public Camera cam;

	// Rays
	private Ray rayForward,
				rayDown;

	// Raycast collider 
	public Collider coll;

	// Ray distance
	public float rayDist = 25.0f;

	// Game starts ?
	public bool startGame = false;
	
	void Start () {
		rayForward = new Ray ();
		rayDown = new Ray ();
	}

	void Update () {

		// Set forward ray (player z)
		rayForward.origin = cam.transform.position;
		rayForward.direction = cam.transform.rotation * Vector3.forward;
		Debug.DrawRay (rayForward.origin, rayForward.direction * rayDist, Color.cyan);

		// Set down ray (player y)
		rayDown.origin = cam.transform.position;
		rayDown.direction = cam.transform.rotation * Vector3.down;
		Debug.DrawRay (rayDown.origin, rayDown.direction * rayDist, Color.cyan);

		// Launch via forward ray collision
		RaycastHit hit;
		if (coll.Raycast (rayForward, out hit, rayDist)) {
			startGame = true;
			//Debug.Log ("collision!");
		}

		// Launch via kb
		if (Input.GetKeyDown (KeyCode.G))
			startGame = true;
	}
}
