using System;
using Unity.Netcode;
using UnityEngine;

public class CodenamesGameMultiplayer : NetworkBehaviour
{
    public static CodenamesGameMultiplayer Instance { get; private set; }

    public event EventHandler OnPlayerDataNetworkListChanged;
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler<ClientIdEventArgs> OnPlayerLeft;
    public event EventHandler<ClientIdEventArgs> OnPlayerJoined;
    public class ClientIdEventArgs: EventArgs
    {
        public ulong clientId;
    }
    private NetworkList<PlayerData> playerDataNetworkList;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        //host
        NetworkManager_OnClientConnectedCallback(0);
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
    }
    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= CodenamesGameLobby.MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            return;
        }

        connectionApprovalResponse.Approved = true;
    }
    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
    }
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                // Disconnected!
                OnPlayerLeft?.Invoke(this, new ClientIdEventArgs { clientId = clientId });
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        if (!IsServer)
        {
            return;
        }
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
        });
        OnPlayerJoined?.Invoke(this, new ClientIdEventArgs { clientId = clientId });
        SetPlayerNameServerRpc(GetPlayerName());
    }

 
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    public void ChangePlayerSide(Side side)
    {
        ChangePlayerSideServerRpc(side);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerSideServerRpc(Side side, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.side = side;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    public string GetPlayerName()
    {
        return UsernameUI.username;
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }
    public SideColor GetLocalPlayerSideColor()
    {
        Side side = GetPlayerData().side;
        if (side == Side.BlueSideSpymaster || side == Side.BlueSideOperative)
            return SideColor.Blue;
        else
            return SideColor.Red;
    }
}
