using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (CodenamesGameMultiplayer.Instance != null)
        {
            Destroy(CodenamesGameMultiplayer.Instance.gameObject);
        }

        if (CodenamesGameLobby.Instance != null)
        {
            Destroy(CodenamesGameLobby.Instance.gameObject);
        }
        StartCoroutine(LoadScene());
    }
    private IEnumerator LoadScene()
    {
        yield return CodenamesGameLobby.Instance == null;
        Loader.LoadScene(Loader.Scene.LobbyScene);

    }

}