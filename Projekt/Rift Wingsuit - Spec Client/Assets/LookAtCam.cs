using UnityEngine;
using System.Collections;

public class LookAtCam : MonoBehaviour {

	private Transform cam; 
	private Transform target;
	private Vector3 oldCamPos;
	private float rot = 0.0f;

	// Demo types for action cam
	public bool follow = false; 
	public bool circle = false;

	// Use this for initialization
	void Start () {
		//target = this.transform;
		//target.position = new Vector3 (0.0f, 0.0f, 0.0f);
		cam = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		oldCamPos = cam.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		rot += 0.1f;


		try{
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			if(follow){
				cam.transform.position = oldCamPos;
				cam.transform.position = target.transform.position + new Vector3(10.0f, 0.0f, 0.0f);
			}
			if(circle){
				cam.transform.position = oldCamPos;
				cam.transform.position = target.transform.position + new Vector3(0.0f, 0.0f, 0.0f);
			}
		} catch(UnityException e){};

		if (target) {
			if(circle) {
				cam.RotateAround(target.transform.position, new Vector3(0.0f, 1.0f, 0.0f), 1.0f);
				//cam.transform.position += new Vector3(10.0f, 0.0f, 0.0f);
				//cam.transform.position = Quaternion.AngleAxis(rot, Vector3.up) * cam.transform.position; //Rotate(new Vector3(0.0f, rot, 0.0f));
			}
			else cam.LookAt (target.transform);
		}

		//Debug.Log ("lookAtTarget: " + target.transform);

	}
}
