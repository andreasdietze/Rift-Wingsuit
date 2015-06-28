using UnityEngine;
using System.Collections;

public class FlyCam : MonoBehaviour
{
    public Controller controller;
    // smoothing
    public bool smooth = true;
    public float acceleration = 0.1f;
    protected float actSpeed = 0.0f;			// keep it from 0 to 1
    public float speed = 50.0f;		// max speed of camera
    public bool inverted = false;

    protected Vector3 lastDir = new Vector3();
    protected Vector3 lastViewport = new Vector3();

    void Start()
    {
        	//controller = gameObject.AddComponent<KeyboardAndMouseController>();
    }

    void Update()
    {
		Vector3 dir = new Vector3();// create (0,0,0)

		//Things to do here...
		//lastViewport = controller.CalculateViewport (inverted);
		//dir = controller.GetDir ();
		

        // Movement of the camera
		/*
        if (dir != Vector3.zero)
        {
            // some movement 
            if (actSpeed < 1)
                actSpeed += acceleration * Time.deltaTime * 40;
            else
                actSpeed = 1.0f;

            lastDir = dir;
        }
        else
        {
            // should stop
            if (actSpeed > 0)
                actSpeed -= acceleration * Time.deltaTime * 20;
            else
                actSpeed = 0.0f;
        }


        if (smooth)
            transform.Translate(lastDir * actSpeed * speed * Time.deltaTime);

        else
            transform.Translate(dir * speed * Time.deltaTime); */
    }

    // RESET on Boden
    //void OnTriggerEnter(Collider other)
    //{
        //if (other.CompareTag("Boden"))
           // Application.LoadLevel(0);
    //}

}
