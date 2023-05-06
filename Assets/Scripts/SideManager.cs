using System;
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

    [SerializeField] private RectTransform RedSideRectTransform;
    [SerializeField] private RectTransform BlueSideRectTransform;

    [SerializeField] private Transform usernameLabelPrefab;

    private List<Vector2> firstPositions = new List<Vector2>();
    private Button lastSelectedButton;

    private Dictionary<ulong, ulong> usernamePrefabs = new Dictionary<ulong, ulong>();
    private void Awake()
    {
        firstPositions.Add(RedSideOperativeTransform.position);
        firstPositions.Add(BlueSideOperativeTransform.position);
        firstPositions.Add(RedSideSpymasterTransform.position);
        firstPositions.Add(BlueSideSpymasterTransform.position);
    }
    private void Start()
    {
        CodenamesGameManager.Instance.OnGameStarted += CodenamesGameManager_OnGameStarted;
    }

    private void CodenamesGameManager_OnGameStarted(object sender, EventArgs e)
    {
        //disable the buttons

        RedSideOperativeButton.gameObject.SetActive(false);
        RedSideSpymasterButton.gameObject.SetActive(false);
        BlueSideOperativeButton.gameObject.SetActive(false);
        BlueSideSpymasterButton.gameObject.SetActive(false);

        // set the side positions
        BlueSideRectTransform.sizeDelta = new Vector2(BlueSideRectTransform.sizeDelta.x, BlueSideRectTransform.sizeDelta.y - 100);
        BlueSideRectTransform.Translate(new Vector2(0, 50));
        RedSideRectTransform.sizeDelta = new Vector2(RedSideRectTransform.sizeDelta.x, RedSideRectTransform.sizeDelta.y - 100);
        RedSideRectTransform.Translate(new Vector2(0, 50));
        UpdateTransforms(true);
    }
    public override void OnDestroy()
    {
        CodenamesGameManager.Instance.OnGameStarted -= CodenamesGameManager_OnGameStarted;
    }
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
        UpdateTransforms();
    }
    private void LateUpdate()
    {
        if (new Vector2(RedSideOperativeTransform.position.x,RedSideOperativeTransform.position.y) != firstPositions[0])
            UpdateTransforms();
    }
    private void UpdateTransforms(bool rectPosChanged = false)
    {
        if (rectPosChanged)
        {
            for (int i = 0; i < firstPositions.Count; i++)
            {
                if (i < firstPositions.Count / 2)
                {
                    firstPositions[i] = new Vector2(firstPositions[i].x, firstPositions[i].y + 30);
                }
                else
                {
                    firstPositions[i] = new Vector2(firstPositions[i].x, firstPositions[i].y + 48);
                }
            }
        }
        RedSideOperativeTransform.position = firstPositions[0];
        RedSideOperativeTransform.localScale = Vector3.one;

        BlueSideOperativeTransform.position = firstPositions[1];
        BlueSideOperativeTransform.localScale = Vector3.one;

        RedSideSpymasterTransform.position = firstPositions[2];
        RedSideSpymasterTransform.localScale = Vector3.one;

        BlueSideSpymasterTransform.position = firstPositions[3];
        BlueSideSpymasterTransform.localScale = Vector3.one;
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