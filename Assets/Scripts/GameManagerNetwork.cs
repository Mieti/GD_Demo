using System;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameManagerNetwork : NetworkBehaviour
{
    [SerializeField] private WireController2D _hostPrefab; // Prefab for the host
    [SerializeField] private WireController2D _clientPrefab; // Prefab for the client

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // The host (server) also needs to spawn its player
            Debug.Log("Host is spawning its player.");
            Vector3 hostPosition = new Vector3(0, 0, 0); // Replace with your desired position
            SpawnPlayer(NetworkManager.Singleton.LocalClientId, true, hostPosition);
        }
        else if (IsClient)
        {
            // Clients request to spawn their player
            Debug.Log("Client is requesting to spawn its player.");
            Vector3 clientPosition = new Vector3(38, -36, 0); // Replace with your desired position
            RequestSpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, clientPosition);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ulong clientId, Vector3 position, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"Server received spawn request from client {clientId}.");
        // The server receives the request and spawns the player
        SpawnPlayer(clientId, false, position);
    }

    private void SpawnPlayer(ulong clientId, bool isHost, Vector3 position)
    {
        WireController2D spawnPrefab = isHost ? _hostPrefab : _clientPrefab;
        Debug.Log($"Spawning player for client {clientId} using prefab: {(isHost ? "HostPrefab" : "ClientPrefab")} at position {position}.");

        var spawn = Instantiate(spawnPrefab, position, Quaternion.identity);
        spawn.NetworkObject.SpawnWithOwnership(clientId);

        Debug.Log($"Player spawned for client {clientId}. IsHost: {isHost}, Position: {position}");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
    }
}