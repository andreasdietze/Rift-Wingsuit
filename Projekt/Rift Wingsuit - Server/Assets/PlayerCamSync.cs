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

		// Lock player rotation form 0-90° on X
		Vector3 rot = cam.transform.rotation.eulerAngles;
		if (rot.x >= 90.0f) 
			rot.x = 0.0f;

		// Sync player position and player orientation with camera
		GetComponent<Rigidbody> ().transform.position = (cam.transform.position + new Vector3 (0.0f, 0.0f, 0.0f)) +
			(Quaternion.Euler(rot) * (Vector3.back * playerSize));// +
		//(cam.transform.rotation * (Vector3.down * playerSize)) + 
		//(cam.transform.rotation * (Vector3.up * playerSize));

		GetComponent<Rigidbody> ().transform.rotation =  Quaternion.Euler(rot) *  //     cam.transform.rotation *
			Quaternion.AngleAxis (90.0f, new Vector3 (1.0f, 0.0f, 0.0f));

	}
}
