using UnityEngine;
using System.Collections;

// Syncronize playerpos with campos
public class PlayerCamSync : MonoBehaviour {

	// Set by prefab
	public GameObject player;

	// Find by tag
	private GameObject cam;

	// Offet 
	private Vector3 offsetPos;
	private Vector3 oldPlayerPos;
	
	void Start () {
		// Get rift cam by tag
		cam = GameObject.FindGameObjectWithTag ("MainCamera");
		oldPlayerPos = new Vector3 (19110, 3836, 7570);


		// Set offset
		offsetPos = new Vector3 (5.0f, -2.0f, 0.0f);
	}

	void Update () {
		// Sync playerpos 
		player.transform.position = oldPlayerPos;
		player.transform.position = cam.transform.position;// + offsetPos;

	}
}
