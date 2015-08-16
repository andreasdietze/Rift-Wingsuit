using UnityEngine;
using System.Collections;

// Syncronize playerpos with campos
public class PlayerCamSync : MonoBehaviour {

	// Find by tag
	private GameObject cam;

	// Set potision to players eyes
	private float playerSize = 1.8f;
	
	void Start () {
		// Get rift cam by tag
		cam = GameObject.FindGameObjectWithTag ("MainCamera");
	}

	void Update () {
		// Sync player position and player orientation with camera
		GetComponent<Rigidbody> ().transform.position = cam.transform.position +
			(cam.transform.rotation * (Vector3.down * playerSize));

		GetComponent<Rigidbody> ().transform.rotation = cam.transform.rotation;
	}
}
