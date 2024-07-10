using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private WireController2D _hostPrefab; // Prefab for the host
    [SerializeField] private WireController2D _clientPrefab; // Prefab for the client

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // The host (server) also needs to spawn its player
            Debug.Log("Host is spawning its player.");
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, true);
        }
        else if (IsClient)
        {
            // Clients request to spawn their player
            Debug.Log("Client is requesting to spawn its player.");
            RequestSpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ulong clientId, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"Server received spawn request from client {clientId}.");
        // The server receives the request and spawns the player
        SpawnPlayerServerRpc(clientId, false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId, bool isHost)
    {
        WireController2D spawnPrefab = isHost ? _hostPrefab : _clientPrefab;
        Debug.Log($"Spawning player for client {clientId} using prefab: {(isHost ? "HostPrefab" : "ClientPrefab")}.");

        var spawn = Instantiate(spawnPrefab);
        spawn.NetworkObject.SpawnWithOwnership(clientId);

        Debug.Log($"Player spawned for client {clientId}. IsHost: {isHost}");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
    }
}