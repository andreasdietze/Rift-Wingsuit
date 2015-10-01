using UnityEngine;
using System.Collections;

public class ConfigOverrider : MonoBehaviour {

    public long detailDistance = 3000;
    private float detailObjectDistance;
    public bool overrideStartPos = false;
    public Vector3 startPosition = new Vector3(5000, 5000, 5000);

    private Vector3 forestPosition = new Vector3(26500, 1, 16100);

	// Use this for initialization
    void Start()
    {
        detailObjectDistance = detailDistance / 2;
        Terrain.activeTerrain.basemapDistance = detailDistance;
        Terrain.activeTerrain.detailObjectDistance = detailObjectDistance;

        if (overrideStartPos)
        {
            Camera.main.transform.position = startPosition;
        }
	}
	
	// Update is called once per frame
    void Update()
    {
        Terrain.activeTerrain.basemapDistance = detailDistance;
        float distX = Camera.main.transform.position.x - forestPosition.x;
        float distY = Camera.main.transform.position.z - forestPosition.z;

        double dist = Mathf.Sqrt(distX * distX + distY * distY);
        if (dist <= (detailObjectDistance / 4))
        {
            Terrain.activeTerrain.detailObjectDistance = detailObjectDistance*2;
        }
	}
}
