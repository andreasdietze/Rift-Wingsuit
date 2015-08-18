using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	private const string typeName = "RiftWingsuit";
	private const string gameName = "Wingsuit-Lobby";

	private bool isRefreshingHostList = false;
	private HostData[] hostList;

	public GameObject playerPrefab;
	public Transform camPos;

	public bool serverJoined = false;

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			//if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				//StartServer();
			
			if (GUI.Button(new Rect(100, 100, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}

	private void StartServer()
	{
		Network.InitializeServer(2, // Players
		                         25000, // Port
		                         !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		//SpawnPlayer();
	}

    void Update()
    {
        if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
        {
            isRefreshingHostList = false;
            hostList = MasterServer.PollHostList();
        }
    }

    private void RefreshHostList()
    {
        if (!isRefreshingHostList)
        {
            isRefreshingHostList = true;
            MasterServer.RequestHostList(typeName);
        }
    }

	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
		serverJoined = true;
		//SpawnPlayer();
		
	}
	
	private void SpawnPlayer()
    {
        Network.Instantiate(playerPrefab, camPos.position, Quaternion.identity, 0);   // Quaternion.identity
    }

}