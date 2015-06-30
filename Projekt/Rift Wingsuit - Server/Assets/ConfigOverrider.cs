using UnityEngine;
using System.Collections;

public class ConfigOverrider : MonoBehaviour {

    public long detailDistance = 10000;
    public bool enableOverrideForStartPosition = true;
    private Vector3 startPosition = new Vector3(15168.0f, 3522.0f, 8784.0f);
	// Use this for initialization
	void Start () {

        if (!enableOverrideForStartPosition)
        {
            Camera.main.transform.localPosition = startPosition;
        }
	}
	
	// Update is called once per frame
	void Update () {
		Terrain.activeTerrain.basemapDistance = detailDistance;
	}
}
