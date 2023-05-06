using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class InGameLobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    private void Start()
    {
        Lobby lobby = CodenamesGameLobby.Instance.GetLobby();
        lobbyNameText.text = "LobbyName: " + lobby.Name;
        lobbyCodeText.text = "LobbyCode: " + lobby.LobbyCode;
    }
}
