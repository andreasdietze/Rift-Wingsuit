using UnityEngine;
using System.Collections;

public class LookAtCam : MonoBehaviour {
	
	// Camera transformation
	private Transform cam; 
	// Target (player) transformation
	private Transform target;
	// Tmp cam position
	private Vector3 oldCamPos;
	private float rot = 0.0f;
	// Index for cam style via keyboard
	int index = 0;
	// Distance between cam and player in world units
	public float distanceToPlayer = 1.0f;
	// For distanceToPlayer lerp
	private float oldDistanceToPlayer = 0.0f;
	
	// Camera styles for action cam:
	// - followLeft: 	look from the left to the player
	// - followAbove: 	look from above to the player
	// - followRight: 	look from the right to the player
	// - followBehind: 	look from behind to the player
	// - followHead: 	look through the eyes of the player
	// - circleAround: 	look at the player and circle around him
	// - circle: ...
	private enum ActionCam{followLeft, followRight, followAbove, followBehind, followFront, followHead,
		circleAround, circle};
	private ActionCam actionCam = ActionCam.followLeft;
	
	// Update cam styles by unity menue interface
	public bool followLeft 		= false;
	public bool followRight 	= false;
	public bool followAbove 	= false;
	public bool followBehind 	= false;
	public bool followFront 	= false;
	public bool followHead 		= false;
	public bool circleAround	= false;
	public bool circle 			= false;
	
	
	// Instance to network managing. Verifies that client has joind a server.
	private NetworkManager nManager;
	
	// Use this for initialization
	void Start () {
		// Find main camera with editor properties
		cam = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		
		// Find networkManager
		nManager = (NetworkManager)GameObject.FindGameObjectWithTag("Network").GetComponent("NetworkManager");
	}
	
	// Update is called once per frame
	void Update () {
		rot += 1.0f;

		// Check if client has joined a server.
		// This is necessary because the playerprefab is automatically generated.
		if (nManager.serverJoined) {
			try {
				target = GameObject.FindGameObjectWithTag ("Player").transform;
			} catch (UnityException e) {
				Debug.Log(e.Message);
			}
		}

		// If a player has been generated we now can setup any of the provided camera styles.
		if(target){
			switch(actionCam){
			case ActionCam.followLeft: 
				cam.transform.position = target.transform.position +
					(target.transform.rotation * (Vector3.left * distanceToPlayer));
				cam.LookAt(target.transform);
				break;				
			case ActionCam.followRight: 
				cam.transform.position = target.transform.position +
					(target.transform.rotation * (Vector3.right * distanceToPlayer));
				cam.LookAt(target.transform);
				break;				
			case ActionCam.followAbove:
				cam.transform.position = target.transform.position +
					(target.transform.rotation * (Vector3.up * distanceToPlayer));
				cam.LookAt(target.transform);
				break;
			case ActionCam.followBehind: 
				cam.transform.position = target.transform.position +
					(target.transform.rotation * (Vector3.up * distanceToPlayer)) +
					(target.transform.rotation * (Vector3.back * distanceToPlayer));
				cam.LookAt(target.transform);
				break;
			case ActionCam.followFront: 
				cam.transform.position = target.transform.position +
					(target.transform.rotation * (Vector3.up * distanceToPlayer / 10)) +
					(target.transform.rotation * (Vector3.forward * distanceToPlayer));
				cam.LookAt(target.transform);
				break;
			case ActionCam.followHead: // TODO: add rift orientation
				cam.transform.eulerAngles = target.transform.eulerAngles;
				cam.transform.position = target.transform.position;
				break;
			case ActionCam.circleAround: 
				Vector3 pos = target.transform.position;
				cam.transform.position = pos;// + new Vector3(10.0f, 0.0f, 0.0f);//target.transform.position;
				
				//cam.transform.Rotate(new Vector3(0.0f, rot, 0.0f));
				cam.transform.rotation = Quaternion.Euler(0, 90 + rot, 0);
				cam.transform.Translate(new Vector3(10.0f, 0.0f, 0.0f));
				
				//cam.LookAt(target.transform);
				//cam.rotation *= Quaternion.Euler(0, 90, 0);
				
				//cam.transform.Translate(new Vector3(10.0f, 0.0f, 0.0f));
				
				//cam.transform.Rotate(new Vector3(0.0f, 10.1f, 0.0f));
				//cam.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 0.1f, Space.World);
				//cam.transform.Translate(new Vector3(-10.0f, 0.0f, 0.0f));
				break;
			case ActionCam.circle: 
				cam.LookAt(target.transform);
				cam.RotateAround(target.transform.position, new Vector3(0.0f, 1.0f, 0.0f), 1.0f);
				
				//Vector3 CmT = target.transform.position - cam.position;
				//float dist = Mathf.Sqrt(CmT.x * CmT.x + CmT.y * CmT.y + CmT.z * CmT.z);
				//Debug.Log("dist: " + dist);
				//if(dist >= 10.0f)
				//cam.position = target.transform.position;//+= new Vector3(0.0f, 0.0f, dist);
				//cam.transform.position = Quaternion.AngleAxis(rot, Vector3.up) * cam.transform.position; //Rotate(new Vector3(0.0f, rot, 0.0f));
				break;
			}
		}
		UpdateActionCamByMenue();
		UpdateActionCamByKeyboard();
	}
	
	// Set actionCam by unity menue. Bit ugly implementation but
	// needed for realtime cam style manipulation in the unity menue.
	private void UpdateActionCamByMenue(){
		if(followLeft)
			actionCam = ActionCam.followLeft;
		else if(followRight)
			actionCam = ActionCam.followRight;
		else if(followAbove)
			actionCam = ActionCam.followAbove;
		else if(followBehind)
			actionCam = ActionCam.followBehind;
		else if(followHead)
			actionCam = ActionCam.followHead;
		else if(circleAround)
			actionCam = ActionCam.circleAround;
		else if(circle)
			actionCam = ActionCam.circle;
		else // Default actionCam
			actionCam = ActionCam.followBehind;
		
	}
	
	// Set actionCam by device input
	private void UpdateActionCamByKeyboard(){
		if (Input.GetKeyDown (KeyCode.C))
			index++;

		if (index == 6)
			index = 0;

		Debug.Log ("Actual actionCam index: " + index);

		switch (index) {
			case 0: actionCam = ActionCam.followLeft; break;
			case 1: actionCam = ActionCam.followRight; break;
			case 2: actionCam = ActionCam.followAbove; break;
			case 3: actionCam = ActionCam.followBehind; break;
			case 4: actionCam = ActionCam.followFront; break;
			case 5: actionCam = ActionCam.followHead; break;
		}
	}

	
	// Set random actionCam by timer
	// TODO
	
	// Set actionCam by event
	// TODO 

	// TODO: Lerp distanceToPlayer 
}
