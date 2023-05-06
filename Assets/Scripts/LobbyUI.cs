using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;
    private void Awake()
    {
        createLobbyButton.onClick.AddListener(() => { lobbyCreateUI.gameObject.SetActive(true); });
        quickJoinButton.onClick.AddListener(() => { CodenamesGameLobby.Instance.QuickJoin(); });
        joinCodeButton.onClick.AddListener(() => { CodenamesGameLobby.Instance.JoinWithCode(joinCodeInputField.text); });
        lobbyTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
        CodenamesGameLobby.Instance.OnLobbyListChanged += CodenamesGameLobby_OnLobbyListChanged;
    }

    private void CodenamesGameLobby_OnLobbyListChanged(object sender, CodenamesGameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
    private void OnDestroy()
    {
        CodenamesGameLobby.Instance.OnLobbyListChanged -= CodenamesGameLobby_OnLobbyListChanged;
    }
}
