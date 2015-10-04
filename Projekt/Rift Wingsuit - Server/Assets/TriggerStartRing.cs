using UnityEngine;
using System.Collections;

public class TriggerStartRing : MonoBehaviour {
	Player player;
	AudioSource audio;
	NetworkManager nManager;
	private bool hasColAppeard = false;

	// Use this for initialization
	void Start () {
		this.name = "StartWP";

		// Access to NetworkManger script
		nManager = (NetworkManager)GameObject.FindGameObjectWithTag ("Network").GetComponent ("NetworkManager");

		// Access to audio instance
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		// Access to player script
		if(nManager.serverInitiated)
			player = (Player)GameObject.FindGameObjectWithTag ("Player").GetComponent ("Player");
	}

	void OnTriggerEnter(Collider other){
		//Debug.Log ("Collision detected with trigger object " + other.name);
		if (other.name == "Player(Clone)" && !hasColAppeard){
			// Play the sound
			audio.Play(44100);

			// Flag for player score
			player.collideWithWP = true;

			// Remove it (TODO)
			hasColAppeard = true;
		}
	}
}
