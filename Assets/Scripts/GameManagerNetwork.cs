using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManagerNetwork : NetworkBehaviour
{
    [SerializeField] private WireController2D _hostPrefab; // Prefab for the host
    [SerializeField] private WireController2D _clientPrefab; // Prefab for the client
    [SerializeField] private CameraMovement _cameraController;
    private List<NetworkObject> _spawnedObjects = new List<NetworkObject>();
    [SerializeField] private Vector3 hostPosition; // Camera offset
    [SerializeField] private Vector3 clientPosition; // Camera offset
    private GameManager GM;

    public override void OnNetworkSpawn()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (IsServer)
        {
            // The host (server) also needs to spawn its player
            Debug.Log("Host is spawning its player.");
            // hostPosition = new Vector3(-58, -36, 0); // Replace with your desired position
            SpawnPlayer(NetworkManager.Singleton.LocalClientId, true, hostPosition);
        }
        else if (IsClient)
        {
            // Clients request to spawn their player
            Debug.Log("Client is requesting to spawn its player.");
            // clientPosition = new Vector3(38, -36, 0); // Replace with your desired position
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
        _spawnedObjects.Add(spawn.NetworkObject); // Add the spawned object to the list

        //GM.UpdateUI(spawn.NetworkObject.gameObject.GetComponent<WireController2D>());

        // Assign the camera to follow the newly spawned player on the client side
        AssignCameraClientRpc(spawn.NetworkObject.NetworkObjectId, clientId);
        Debug.Log($"Player spawned for client {clientId}. IsHost: {isHost}, Position: {position}");
    }

    [ClientRpc]
    private void AssignCameraClientRpc(ulong networkObjectId, ulong clientId)
    {
        // Only set the camera for the local client
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            Transform playerTransform = null;
            
            // Check if the networkObject has the expected child objects
            if (networkObject != null)
            {
                if (networkObject.OwnerClientId == 0)
                {
                    playerTransform = networkObject.transform.Find("Player1");
                }
                else if (networkObject.OwnerClientId == 1)
                {
                    playerTransform = networkObject.transform.Find("Player2");
                }

                if (playerTransform != null)
                {
                    _cameraController.target = playerTransform;
                    _cameraController.offset = playerTransform.position;
                }
                else
                {
                    Debug.LogError("Player child object not found.");
                }
            }
            else
            {
                Debug.LogError("NetworkObject not found.");
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        // Destroy all spawned network objects
        foreach (var networkObject in _spawnedObjects)
        {
            if (networkObject != null)
            {
                networkObject.Despawn();
                Destroy(networkObject.gameObject);
            }
        }
        _spawnedObjects.Clear();
    }
}