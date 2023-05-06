using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    private void Awake()
    {
        createPublicButton.onClick.AddListener(() => { CodenamesGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false); });
        createPrivateButton.onClick.AddListener(() => { CodenamesGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true); });
        closeButton.onClick.AddListener(() => { gameObject.SetActive(false); });
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
