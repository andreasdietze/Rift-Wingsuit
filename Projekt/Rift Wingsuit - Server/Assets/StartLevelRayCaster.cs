using UnityEngine;
using System.Collections;

public class StartLevelRayCaster : MonoBehaviour {

	public Camera cam;
	public Collider coll;
	private Ray ray;
	private float width = 1920 / 2;
	private float height = 1080 / 2;
	public float rayDist = 25.0f;
	public bool startGame = false;
	
	void Start () {
		ray = new Ray ();
	}

	void Update () {
		// Get ray from screen center
		//ray = cam.ScreenPointToRay(new Vector3(width, height, 0.0f));

		// Set ray manual
		ray.origin = cam.transform.position;
		ray.direction = cam.transform.rotation * (Vector3.forward * rayDist);
		//ray.direction = cam.transform.TransformDirection;
		Debug.DrawRay (ray.origin, ray.direction * rayDist, Color.cyan);

		RaycastHit hit;
		if (coll.Raycast (ray, out hit, rayDist)) {
			startGame = true;
			//Debug.Log ("collision!");
		}

	}
}
