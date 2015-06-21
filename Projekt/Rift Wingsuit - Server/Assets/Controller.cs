using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

    protected Vector3 lastViewport = new Vector3(255, 255, 255);
    protected float sensitivity = 0.25f; 		// keep it from 0..1

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    virtual public Vector3 CalculateViewport(bool inverted)
    {
        return Vector3.zero;
    }

    virtual public Vector3 GetDir()
    {
        return Vector3.zero;
    }
}
