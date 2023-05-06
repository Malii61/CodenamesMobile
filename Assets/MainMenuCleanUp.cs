using System.Collections;
using System.Collections.Generic;
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
        Loader.LoadScene(Loader.Scene.LobbyScene);
    }

}