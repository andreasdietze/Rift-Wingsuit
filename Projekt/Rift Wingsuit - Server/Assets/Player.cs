using UnityEngine;
using System.Collections;

// Players class job is to provide data for network transmission
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

	// Player status
	private int score = 0;
	public bool collideWithWP = false;

	// Only send date
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){

		// Oclus rift transformation 
		oculusTransform = GameObject.FindGameObjectWithTag ("RiftCenter").transform;

		// Sync transformation
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		Quaternion syncOVRRotation = Quaternion.identity;

		// Playerscore
		int syncScore = 0;

		if (stream.isWriting){ // Send data
			// Final Position
			syncPosition = GetComponent<Rigidbody>().position;
			stream.Serialize(ref syncPosition);

			// Final velocity
			syncVelocity = GetComponent<Rigidbody>().velocity;
			stream.Serialize(ref syncVelocity);

			// Final player rotation
			syncRotation = GetComponent<Rigidbody>().rotation;
			stream.Serialize(ref syncRotation);

			// Send OVR cam (seperate oculus head rotation)
			syncOVRRotation = oculusTransform.transform.rotation;
			stream.Serialize(ref syncOVRRotation);

			// Send score
			syncScore = score;
			stream.Serialize(ref syncScore);
			//Debug.Log(syncScore);

		}
		else {// Receive data
			/*stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncRotation);
			
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncStartPosition = GetComponent<Rigidbody>().position;
			
			syncEndRotation = syncRotation;
			syncStartRotation = GetComponent<Rigidbody>().rotation;*/
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
			//SyncedMovement();

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
