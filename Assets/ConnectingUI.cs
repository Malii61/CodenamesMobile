using UnityEngine;

public class ConnectingUI : MonoBehaviour
{

    private void Start()
    {
        CodenamesGameMultiplayer.Instance.OnTryingToJoinGame += CodenamesGameMultiplayer_OnTryingToJoinGame;
        CodenamesGameMultiplayer.Instance.OnFailedToJoinGame += CodenamesGameMultiplayer_OnFailedToJoinGame;

        Hide();
    }

    private void CodenamesGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void CodenamesGameMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
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
        CodenamesGameMultiplayer.Instance.OnTryingToJoinGame -= CodenamesGameMultiplayer_OnTryingToJoinGame;
        CodenamesGameMultiplayer.Instance.OnFailedToJoinGame -= CodenamesGameMultiplayer_OnFailedToJoinGame;
    }
}
