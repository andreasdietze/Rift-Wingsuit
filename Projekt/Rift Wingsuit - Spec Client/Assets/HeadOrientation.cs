﻿using UnityEngine;
using System.Collections;

public class HeadOrientation : MonoBehaviour {
	private NetworkManager nManager;
	private Player player;
	private Quaternion headOrientation;

	void Start () {
		// Find networkManager
		nManager = (NetworkManager)GameObject.FindGameObjectWithTag("Network").GetComponent("NetworkManager");
	}
	
	// Update is called once per frame
	void Update () {

		// Check if client has joined a server.
		// This is necessary because the playerprefab is automatically generated.
		if (nManager.serverJoined) {
			try {
				player = (Player)GameObject.FindGameObjectWithTag ("Player").GetComponent("Player");
				headOrientation = player.syncEndOVRRotation;//lerpedOVRRotation;
			} catch (UnityException e) {
				Debug.Log(e.Message);
			}
		}
		
		transform.rotation = headOrientation * Quaternion.Euler(new Vector3(0.0f, 0.0f, -90.0f));
	}

	bool showText = true;
	Rect textArea = new Rect(300,30,Screen.width, Screen.height);
	
	private void OnGUI()
	{
		if(nManager.serverJoined && showText)
			GUI.Label(textArea, headOrientation.ToString());
	}
}