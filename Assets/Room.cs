using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject play;
    // Start is called before the first frame update
    private void OnEnable()
    {
        SimpleMatchmaking.LobbyPlayersUpdated += NetworkLobbyPlayersUpdated;
    }
    
    private void NetworkLobbyPlayersUpdated(Dictionary<ulong, bool> players)
    {
        foreach (var player in players)
        {
            if (player.Key == 0)
            {
                player1.SetActive(true);
                var textComponent = player1.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = NetworkManager.Singleton.IsHost ? "You" : "Player1";
                }
            }
            else
            {
                player2.SetActive(true);
                var textComponent = player2.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = !NetworkManager.Singleton.IsHost ? "You" : "Player2";
                }
            }
        }
        play.SetActive(NetworkManager.Singleton.IsHost && players.Count > 1);
    }
}
