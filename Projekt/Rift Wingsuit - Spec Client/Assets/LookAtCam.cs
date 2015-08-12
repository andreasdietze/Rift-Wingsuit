using UnityEngine;
using System.Collections;

public class LookAtCam : MonoBehaviour {
	
	// Camera transformation
	private Transform cam; 

	// Target (player) transformation
	private Transform target;

	// Tmp cam position
	private Vector3 oldCamPos;

	// Final value for rotation on y-axis
	private float rotationVelocity 		= 0.0f;

	// The value we increase the rotation every frame
	public float rotationSpeed 			= 0.0f;

	// Index for cam style via keyboard
	int index = 0;

	// Distance between cam and player in world units
	public float distanceToPlayer 		= 1.0f;

	// For distanceToPlayer lerp
	private float oldMinDistToPlayer 	= 0.0f;
	private float oldMaxDistToPlayer 	= 0.0f;
	public float minDistanceToPlayer	= 0.0f;
	public float maxDistanceToPlayer 	= 0.0f;
	public float lerpSpeed				= 0.0f;
	private float lerpedDistance		= 0.0f;
	private float lerpTimer				= 0.0f;
	
	// Camera styles for action cam:
	// - followLeft: 	look from the left to the player
	// - followAbove: 	look from above to the player
	// - followRight: 	look from the right to the player
	// - followBehind: 	look from behind to the player
	// - followHead: 	look through the eyes of the player
	// - circleAround: 	look at the player and circle around him
	// - circle: ...
	private enum ActionCam{followLeft, followRight, followAbove, followBehind, followFront, followHead,
		circleAroundY, circleAroundX};

	// Enum object of ActionCam
	private ActionCam actionCam = ActionCam.followLeft;
	
	// Update cam styles by unity menue interface
	public bool followLeft 		= false;
	public bool followRight 	= false;
	public bool followAbove 	= false;
	public bool followBehind 	= false;
	public bool followFront 	= false;
	public bool followHead 		= false;
	public bool circleAroundY	= false;
	public bool circleAroundX 	= false;
	
	
	// Instance to network managing. Verifies that client has joind a server.
	private NetworkManager nManager;
	
	// Use this for initialization
	void Start () {
		// Find main camera with editor properties
		cam = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		
		// Find networkManager
		nManager = (NetworkManager)GameObject.FindGameObjectWithTag("Network").GetComponent("NetworkManager");

		// Save properties by unity settings
		oldMinDistToPlayer = minDistanceToPlayer;
		oldMaxDistToPlayer = maxDistanceToPlayer;
		//Debug.Log ("oldMinDist: " + oldMinDistToPlayer + " oldMaxDist: " + oldMaxDistToPlayer);
	}
	
	// Update is called once per frame
	void Update () {

		// Keep from 0 - 359 (0 == 360)
		if (rotationVelocity % 359 == 0.0f)
			rotationVelocity = 0.0f;

		// Increase rotation value by rotationSpeed;
		rotationVelocity += rotationSpeed;

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
			case ActionCam.circleAroundY:

				// Add delta time each frame to lerpTimer
				lerpTimer += Time.deltaTime;

				// Reset the lerp if max value has been reached
				if(lerpedDistance >= oldMaxDistToPlayer - 0.2f){ // small offset if lerp doesnt reach 25.0f
					// Reset lerp parameter
					minDistanceToPlayer = oldMinDistToPlayer;
					maxDistanceToPlayer = oldMaxDistToPlayer;

					// Reset lerped distance
					lerpedDistance = oldMinDistToPlayer;

					// Reset the lerp timer
					lerpTimer = 0.0f;

					// And finally set another cam style ;)
					index = 7;
				}

				// Distance to player can be lerped now
				lerpedDistance = Mathf.Lerp(minDistanceToPlayer, maxDistanceToPlayer, lerpTimer * lerpSpeed);
				//Debug.Log("lerpDist: " + lerpedDistance);
				//Debug.Log ("time: " + lerpTimer);

				// Get target position and circle around it on y
				cam.transform.position = target.transform.position + 
					new Vector3(Mathf.Sin(rotationVelocity / 180 * Mathf.PI) * lerpedDistance,
					            0.0f,
					            Mathf.Cos(rotationVelocity / 180 * Mathf.PI) * lerpedDistance);
				cam.transform.rotation = Quaternion.LookRotation(target.transform.position - cam.transform.position); 
				break;
			case ActionCam.circleAroundX:

				// Add delta time each frame to lerpTimer
				lerpTimer += Time.deltaTime;
				
				// Reset the lerp if max value has been reached
				if(lerpedDistance >= oldMaxDistToPlayer - 0.2f){ // small offset if lerp doesnt reach 25.0f
					// Reset lerp parameter
					minDistanceToPlayer = oldMinDistToPlayer;
					maxDistanceToPlayer = oldMaxDistToPlayer;
					
					// Reset lerped distance
					lerpedDistance = oldMinDistToPlayer;
					
					// Reset the lerp timer
					lerpTimer = 0.0f;
					
					// And finally set another cam style ;)
					index = 6;
				}
				
				// Distance to player can be lerped now
				lerpedDistance = Mathf.Lerp(minDistanceToPlayer, maxDistanceToPlayer, lerpTimer * lerpSpeed);

				// Get target position and circle around it on y
				cam.transform.position = target.transform.position + 
					new Vector3(0.0f,
					            Mathf.Sin(rotationVelocity / 180 * Mathf.PI) * distanceToPlayer,
					            Mathf.Cos(rotationVelocity / 180 * Mathf.PI) * distanceToPlayer);
				cam.transform.rotation = Quaternion.LookRotation(target.transform.position - cam.transform.position); 
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
		else if(followFront)
			actionCam = ActionCam.followFront;
		else if(followHead)
			actionCam = ActionCam.followHead;
		else if(circleAroundY)
			actionCam = ActionCam.circleAroundY;
		else if(circleAroundX)
			actionCam = ActionCam.circleAroundX;
		else // Default actionCam
			actionCam = ActionCam.followBehind;
		
	}
	
	// Set actionCam by device input
	private void UpdateActionCamByKeyboard(){
		if (Input.GetKeyDown (KeyCode.C))
			index++;

		if (index == 8)
			index = 0;

		//Debug.Log ("Actual actionCam index: " + index);

		switch (index) {
			case 0: actionCam = ActionCam.followLeft; break;
			case 1: actionCam = ActionCam.followRight; break;
			case 2: actionCam = ActionCam.followAbove; break;
			case 3: actionCam = ActionCam.followBehind; break;
			case 4: actionCam = ActionCam.followFront; break;
			case 5: actionCam = ActionCam.followHead; break;
			case 6: actionCam = ActionCam.circleAroundY; break;
			case 7: actionCam = ActionCam.circleAroundX; break;
		}
	}

	
	// Set random actionCam by timer
	// TODO
	
	// Set actionCam by event
	// TODO 

	// TODO: Lerp distanceToPlayer 
}
