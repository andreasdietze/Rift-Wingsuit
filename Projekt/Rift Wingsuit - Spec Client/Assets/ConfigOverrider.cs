using UnityEngine;
using System.Collections;

public class ConfigOverrider : MonoBehaviour {

    public long detailDistance = 10000;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Terrain.activeTerrain.basemapDistance = detailDistance;
	}
}
