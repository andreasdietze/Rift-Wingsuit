using UnityEngine;
using System.Collections;

public class LookAtCam : MonoBehaviour {

	public Transform cam; 
	public GameObject target;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		cam.LookAt(target.transform);	
	}
	
	void OnGUI()
    {
		//GUI.Label(new Rect(0, 50, 100, 50), "target: " + target.transform.ToString());
    }
}
