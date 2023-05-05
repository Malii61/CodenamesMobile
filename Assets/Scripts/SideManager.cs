using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public enum Side
{
    RedSideOperative,
    RedSideSpymaster,
    BlueSideOperative,
    BlueSideSpymaster,
}
public enum SideColor
{
    Blue,
    Red,
    None
}
public class SideManager : NetworkBehaviour
{
    [SerializeField] private Button RedSideOperativeButton;
    [SerializeField] private Button RedSideSpymasterButton;
    [SerializeField] private Button BlueSideOperativeButton;
    [SerializeField] private Button BlueSideSpymasterButton;

    [SerializeField] private Transform RedSideOperativeTransform;
    [SerializeField] private Transform RedSideSpymasterTransform;
    [SerializeField] private Transform BlueSideOperativeTransform;
    [SerializeField] private Transform BlueSideSpymasterTransform;

    [SerializeField] private Transform usernameLabelPrefab;

    private List<Vector3> firstPositions = new List<Vector3>();
    private void Awake()
    {
        firstPositions.Add(RedSideOperativeTransform.position);
        firstPositions.Add(BlueSideOperativeTransform.position);
        firstPositions.Add(RedSideSpymasterTransform.position);
        firstPositions.Add(BlueSideSpymasterTransform.position);
    }

    private Button lastSelectedButton;

    private Dictionary<ulong, ulong> usernamePrefabs = new Dictionary<ulong, ulong>();
    public override void OnNetworkSpawn()
    {
        RedSideOperativeButton.onClick.AddListener(() =>
        {
            OnClick_SideButton(Side.RedSideOperative, NetworkManager.Singleton.LocalClientId, CodenamesGameMultiplayer.Instance.GetPlayerName());
        });
        RedSideSpymasterButton.onClick.AddListener(() =>
        {
            OnClick_SideButton(Side.RedSideSpymaster, NetworkManager.Singleton.LocalClientId, CodenamesGameMultiplayer.Instance.GetPlayerName());
        });
        BlueSideOperativeButton.onClick.AddListener(() =>
        {
            OnClick_SideButton(Side.BlueSideOperative, NetworkManager.Singleton.LocalClientId, CodenamesGameMultiplayer.Instance.GetPlayerName());
        });
        BlueSideSpymasterButton.onClick.AddListener(() =>
        {
            OnClick_SideButton(Side.BlueSideSpymaster, NetworkManager.Singleton.LocalClientId, CodenamesGameMultiplayer.Instance.GetPlayerName());
        });

    }

    private void UpdateTransforms()
    {
        RedSideOperativeTransform.position = firstPositions[0];
        RedSideOperativeTransform.localScale = Vector3.one;

        BlueSideOperativeTransform.position = firstPositions[1];
        BlueSideOperativeTransform.localScale = Vector3.one;

        RedSideSpymasterTransform.position = firstPositions[2];
        RedSideSpymasterTransform.localScale = Vector3.one;

        BlueSideSpymasterTransform.position = firstPositions[3];
        BlueSideSpymasterTransform.localScale = Vector3.one;
    }
    private void LateUpdate()
    {
        if (RedSideOperativeTransform.position != firstPositions[0])
            UpdateTransforms();
    }
    public void OnClick_SideButton(Side _side, ulong clientId, FixedString64Bytes username)
    {
        //check last selected button
        if (lastSelectedButton != null)
            lastSelectedButton.gameObject.SetActive(true);

        //assign this button to last selected
        lastSelectedButton = GetButtonFromSide(_side);
        lastSelectedButton.gameObject.SetActive(false);

        //assign player side
        CodenamesGameMultiplayer.Instance.ChangePlayerSide(_side);

        //create or set username prefab to visual
        SetUsernamePrefabServerRpc(clientId, username, _side);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetUsernamePrefabServerRpc(ulong clientId, FixedString64Bytes username, Side side)
    {
        if (!usernamePrefabs.ContainsKey(clientId))
        {
            NetworkObject usernamePrefabNetworkObj = Instantiate(usernameLabelPrefab, GetTransformFromSide(side)).GetComponent<NetworkObject>();
            usernamePrefabNetworkObj.SpawnWithOwnership(NetworkManager.LocalClientId);
            ulong prefabId = usernamePrefabNetworkObj.NetworkObjectId;
            AddUsernameToDictionaryClientRpc(clientId, prefabId, username.ToString());
        }
        NetworkManager.SpawnManager.SpawnedObjects[usernamePrefabs[clientId]].transform.SetParent(GetTransformFromSide(side));
    }

    [ClientRpc]
    private void AddUsernameToDictionaryClientRpc(ulong clientId, ulong networkObjectId, string username)
    {
        usernamePrefabs.Add(clientId, networkObjectId);
        Transform usernamePrefab = NetworkManager.SpawnManager.SpawnedObjects[usernamePrefabs[clientId]].transform;
        usernamePrefab.GetChild(0).GetComponent<TextMeshProUGUI>().text = username;
    }
    private Transform GetTransformFromSide(Side side)
    {
        return side switch
        {
            Side.RedSideSpymaster => RedSideSpymasterTransform,
            Side.BlueSideOperative => BlueSideOperativeTransform,
            Side.BlueSideSpymaster => BlueSideSpymasterTransform,
            _ => RedSideOperativeTransform,
        };
    }
    private Button GetButtonFromSide(Side side)
    {
        return side switch
        {
            Side.RedSideSpymaster => RedSideSpymasterButton,
            Side.BlueSideOperative => BlueSideOperativeButton,
            Side.BlueSideSpymaster => BlueSideSpymasterButton,
            _ => RedSideOperativeButton,
        };
    }
}