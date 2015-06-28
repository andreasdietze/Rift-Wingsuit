using UnityEngine;
using System.Collections;

public class LookAtCam : MonoBehaviour {

	private Transform cam; 
	private Transform target;
	private Vector3 oldCamPos;

	public bool follow = false; 

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


		try{
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			if(follow){
				cam.transform.position = oldCamPos;
				cam.transform.position = target.transform.position + new Vector3(10.0f, 0.0f, 0.0f);
			}
		} catch(UnityException e){};

		if(target)
			cam.LookAt (target.transform);	

		//Debug.Log ("lookAtTarget: " + target.transform);

	}
}
