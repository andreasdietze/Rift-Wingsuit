using UnityEngine;
using System.Collections;

public class RiftInfo : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool showText = true;
	Rect textArea = new Rect(Screen.width / 2, Screen.height / 2, Screen.width, Screen.height);

	// Test output on rift -> does not work
	private void OnGUI()
	{
		if(showText)
			GUI.Label(textArea, "HEEEEEEEELLLLLOOOOOOOO");
	}
}
