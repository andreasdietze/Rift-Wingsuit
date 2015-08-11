using UnityEngine;
using System.Collections;

// Syncronize playerpos with campos
public class PlayerCamSync : MonoBehaviour {

	// Find by tag
	private GameObject cam;
	
	void Start () {
		// Get rift cam by tag
		cam = GameObject.FindGameObjectWithTag ("MainCamera");
	}

	void Update () {
		// Sync player position and player orientation with camera
		GetComponent<Rigidbody> ().transform.position = cam.transform.position;
		GetComponent<Rigidbody> ().transform.rotation = cam.transform.rotation;
	}
}
