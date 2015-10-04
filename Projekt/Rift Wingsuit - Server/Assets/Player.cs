using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public float speed = 10f;
	
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	
	// Start- and endposition for lerp
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	
	// Start- and endrotation for lerp
	private Quaternion syncEndRotation = Quaternion.identity;
	private Quaternion syncStartRotation = Quaternion.identity;

	// Get ovr cam rig
	//private Component oculus;
	private Transform oculusTransform;
	//private OVRCameraRig oculus;

	// Player status
	private int score = 0;
	public bool collideWithWP = false;


	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
		//oculus = (Component)GameObject.FindGameObjectWithTag ("MainCamera").GetComponent("OVRCameraRig");
		//oculusTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent ("Tracker").GetComponent ("RiftCenter").transform;
		//oculus = (OVRCameraRig)GameObject.FindGameObjectWithTag ("MainCamera").GetComponent("OVRCameraRig");
		oculusTransform = GameObject.FindGameObjectWithTag ("RiftCenter").transform;
		//Debug.Log("Rift: " + oculus);
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		Quaternion syncOVRRotation = Quaternion.identity;

		if (stream.isWriting){ // Send data
			syncPosition = GetComponent<Rigidbody>().position;
			stream.Serialize(ref syncPosition);
			
			syncVelocity = GetComponent<Rigidbody>().velocity;
			stream.Serialize(ref syncVelocity);
			
			syncRotation = GetComponent<Rigidbody>().rotation;
			stream.Serialize(ref syncRotation);

			// OVR cam view has only to be sent
			syncOVRRotation = oculusTransform.transform.rotation;// oculusTransform.rotation; //oculus.transform.rotation;
			stream.Serialize(ref syncOVRRotation);
		}
		else {// Receive data
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncRotation);
			
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncStartPosition = GetComponent<Rigidbody>().position;
			
			syncEndRotation = syncRotation;
			syncStartRotation = GetComponent<Rigidbody>().rotation;
		}
	}
	
	void Awake(){
		lastSynchronizationTime = Time.time;
	}
	
	void Update(){
		if (GetComponent<NetworkView>().isMine){
			;//InputColorChange();
		}
		else{
			SyncedMovement();
		}

		UpdatePlayerStatus ();
	}
	
	private void SyncedMovement(){
		syncTime += Time.deltaTime;
		GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
		GetComponent<Rigidbody>().rotation =  Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay); 
	}

	// TODO: Send via network
	private void UpdatePlayerStatus(){
		if (collideWithWP) {
			score += 10;
			Debug.Log("Player score: " + score);
		}
		collideWithWP = false;
	}
	
	
	// RPC-Beispiel bitte drinne lassen
	/* private void InputColorChange()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }

    [RPC] void ChangeColorTo(Vector3 color)
    {
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);

        if (GetComponent<NetworkView>().isMine)
            GetComponent<NetworkView>().RPC("ChangeColorTo", RPCMode.OthersBuffered, color);
    }*/
}
