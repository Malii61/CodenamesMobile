using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        CodenamesGameMultiplayer.Instance.OnFailedToJoinGame += CodenamesGameMultiplayer_OnFailedToJoinGame;
        CodenamesGameLobby.Instance.OnCreateLobbyStarted += CodenamesGameLobby_OnCreateLobbyStarted;
        CodenamesGameLobby.Instance.OnCreateLobbyFailed += CodenamesGameLobby_OnCreateLobbyFailed;
        CodenamesGameLobby.Instance.OnJoinStarted += CodenamesGameLobby_OnJoinStarted;
        CodenamesGameLobby.Instance.OnJoinFailed += CodenamesGameLobby_OnJoinFailed;
        CodenamesGameLobby.Instance.OnQuickJoinFailed += CodenamesGameLobby_OnQuickJoinFailed;

        Hide();
    }

    private void CodenamesGameLobby_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Could not find a Lobby to Quick Join!");
    }

    private void CodenamesGameLobby_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join Lobby!");
    }

    private void CodenamesGameLobby_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining Lobby...");
    }

    private void CodenamesGameLobby_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create Lobby!");
    }

    private void CodenamesGameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }

    private void CodenamesGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to connect");
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        CodenamesGameMultiplayer.Instance.OnFailedToJoinGame -= CodenamesGameMultiplayer_OnFailedToJoinGame;
        CodenamesGameLobby.Instance.OnCreateLobbyStarted -= CodenamesGameLobby_OnCreateLobbyStarted;
        CodenamesGameLobby.Instance.OnCreateLobbyFailed -= CodenamesGameLobby_OnCreateLobbyFailed;
        CodenamesGameLobby.Instance.OnJoinStarted -= CodenamesGameLobby_OnJoinStarted;
        CodenamesGameLobby.Instance.OnJoinFailed -= CodenamesGameLobby_OnJoinFailed;
        CodenamesGameLobby.Instance.OnQuickJoinFailed -= CodenamesGameLobby_OnQuickJoinFailed;
    }

}