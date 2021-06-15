using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerTennis : NetworkManager
{
    public Transform SpawnA;
    public Transform SpawnB;
    NetworkManager manager;


    public void StartButton()
    {
        manager = GetComponent<NetworkManager>();
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                // Server + Client
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUILayout.Button("Host (Server + Client)"))
                    {
                        manager.StartHost();
                    }
                }

                // Client + IP
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Client"))
                {
                    manager.StartClient();
                }
                manager.networkAddress = GUILayout.TextField(manager.networkAddress);
                GUILayout.EndHorizontal();

                // Server Only
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // cant be a server in webgl build
                    GUILayout.Box("(  WebGL cannot be server  )");
                }
                else
                {
                    if (GUILayout.Button("Server Only")) manager.StartServer();
                }
            }
            else
            {
                // Connecting
                GUILayout.Label("Connecting to " + manager.networkAddress + "..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    manager.StopClient();
                }
            }
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = numPlayers == 0 ? SpawnA : SpawnB;
        GameObject playerGo = Instantiate(playerPrefab, start.position, start.rotation);
        Player player = playerGo.GetComponent<Player>();
        player.thisPlayer = numPlayers+1;
        player.canServe = numPlayers == 0 ? true : false;
        NetworkServer.AddPlayerForConnection(conn, playerGo);


    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {

        base.OnServerDisconnect(conn);
    }
}
