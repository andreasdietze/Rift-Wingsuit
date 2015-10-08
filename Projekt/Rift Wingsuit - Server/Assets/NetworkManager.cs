using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	// Lobby settings
	private const string typeName = "RiftWingsuit";
	private const string gameName = "Wingsuit-Game";
	
	// Host data for global unity master server
	// Note: global -> global unity master server
	private bool isRefreshingHostList = false;
	private HostData[] hostList;
	
	// Player prefab and spawn settings (by cam)
	public GameObject playerPrefab;
	public Transform camPos;
	
	public bool serverInitiated = false;
	
	// User for seperate master server
	// Note: need to set your own ip (local or net)
	public bool useOwnMasterServer = true;

	// 
	
	// Provide server network gui (todo: extend game settings)
	void OnGUI()
	{
		// Start new server
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
		}
	}
	
	// Setup own master server and connect to it or connect to 
	// global unity master server
	private void StartServer()
	{

		// Settings for own master server (test)
		if (useOwnMasterServer) {
			MasterServer.ipAddress = "192.168.0.172";
			MasterServer.port = 23466;
			Network.natFacilitatorIP = "192.168.0.172";
			Network.natFacilitatorPort = 50005;
		}
		
		// Init server
		Network.InitializeServer(2,	
		                         25000,
		                         !Network.HavePublicAddress());
		
		MasterServer.RegisterHost(typeName, gameName);
		serverInitiated = true;
	}
	
	void OnServerInitialized(){
		SpawnPlayer();
	}
	
	// void OnConnectedToServer(){
	//SpawnPlayer();
	// }
	
	// Start server by key input (if oculus vp overrides server gui)
	void Update(){
		if (Input.GetKeyDown(KeyCode.Space))
			StartServer();
	}
	
	// Init a new player prefab
	private void SpawnPlayer(){
		Network.Instantiate(playerPrefab,
		                    camPos.position,
		                    camPos.rotation, //  * Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f))
		                    0); 
	}
}
