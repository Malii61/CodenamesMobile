using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class InGameLobbyUI : NetworkBehaviour
{

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private Button leaveRoomButton;
    [SerializeField] private Transform areYouSurePanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    private void Awake()
    {
        leaveRoomButton.onClick.AddListener(() => { areYouSurePanel.gameObject.SetActive(true); });
        yesButton.onClick.AddListener(() => {
            LeftGame();
        });
        noButton.onClick.AddListener(() => { areYouSurePanel.gameObject.SetActive(false); });

        areYouSurePanel.gameObject.SetActive(false);
    }
    private void Start()
    {
        Lobby lobby = CodenamesGameLobby.Instance.GetLobby();
        lobbyNameText.text = "Lobi Adý: " + lobby.Name;
        lobbyCodeText.text = "Lobi Kodu: " + lobby.LobbyCode;
    }
    private void LeftGame()
    {
        if (IsServer)
            CodenamesGameLobby.Instance.DeleteLobby();
        else
            CodenamesGameLobby.Instance.LeaveLobby();

        NetworkManager.Singleton.Shutdown();
        Loader.LoadScene(Loader.Scene.MainMenuScene);
    }
}
