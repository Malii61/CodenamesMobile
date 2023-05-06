using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CodenamesGameManager : NetworkBehaviour
{
    public static CodenamesGameManager Instance { get; private set; }

    private const int WORD_COUNT = 17;

    [SerializeField] private Button startGameButton;
    [SerializeField] private Button resetGameButton;

    public EventHandler OnGameStarted;

    private Dictionary<Button, bool> redSideWords = new Dictionary<Button, bool>();
    private Dictionary<Button, bool> blueSideWords = new Dictionary<Button, bool>();

    SideColor startSide;
    public Color blue = new Color32(2, 144, 204, 255);
    public Color red = new Color32(209, 45, 45, 255);
    private void Awake()
    {
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            startGameButton.gameObject.SetActive(false);
            resetGameButton.gameObject.SetActive(false);
        }
    }
    public void OnClick_StartButton()
    {
        startGameButton.gameObject.SetActive(false);
        OnGameStartedClientRpc();
        ChooseTheSideToStartFirst();
        SetWords();
    }
    public void OnClick_ResetGameButton()
    {
        Loader.LoadSceneOnNetwork(Loader.Scene.GameScene);
    }
    [ClientRpc]
    private void OnGameStartedClientRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    private void ChooseTheSideToStartFirst()
    {
        //this function is gonna be executed only at server side
        startSide = (SideColor)UnityEngine.Random.Range(0, 1);
        if (startSide == SideColor.Blue)
        {
            SetWordCountClientRpc(WORD_COUNT, SideColor.Blue);
            GameStateManager.Instance.SetState(State.BlueSpymasterGivesClue);
        }
        else if (startSide == SideColor.Red)
        {
            SetWordCountClientRpc(WORD_COUNT, SideColor.Red);
            GameStateManager.Instance.SetState(State.RedSpymasterGivesClue);
        }

    }
    [ClientRpc]
    private void SetWordCountClientRpc(int count, SideColor sideColor)
    {
        int wordCount = WORD_COUNT / 2;
        if (sideColor == SideColor.Blue)
        {
            ScoreManager.Instance.SetRemainingWordCount(WORD_COUNT - wordCount, SideColor.Blue);
            ScoreManager.Instance.SetRemainingWordCount(wordCount, SideColor.Red);
        }
        else
        {
            ScoreManager.Instance.SetRemainingWordCount(WORD_COUNT - wordCount, SideColor.Red);
            ScoreManager.Instance.SetRemainingWordCount(wordCount, SideColor.Blue);
        }
    }
    private void SetWords()
    {
        List<TextMeshProUGUI> words = WordTableUI.Instance.GetRandomWords(WORD_COUNT);

        for (int i = 0; i <= WORD_COUNT / 2; i++)
        {
            if (startSide == SideColor.Blue)
            {
                SetButtonServerRpc(words[i].transform.parent.GetComponentInChildren<NetworkObject>(), SideColor.Blue);
            }
            else if (startSide == SideColor.Red)
            {
                SetButtonServerRpc(words[i].transform.parent.GetComponentInChildren<NetworkObject>(), SideColor.Red);
            }
        }
        for (int i = WORD_COUNT / 2 + 1; i < WORD_COUNT; i++)
        {
            if (startSide == SideColor.Blue)
            {
                SetButtonServerRpc(words[i].transform.parent.GetComponentInChildren<NetworkObject>(), SideColor.Red);
            }
            else if (startSide == SideColor.Red)
            {
                SetButtonServerRpc(words[i].transform.parent.GetComponentInChildren<NetworkObject>(), SideColor.Blue);
            }
        }
    }
    [ServerRpc]
    private void SetButtonServerRpc(NetworkObjectReference reference, SideColor sideColor)
    {
        SetButtonClientRpc(reference, sideColor);
    }
    [ClientRpc]
    private void SetButtonClientRpc(NetworkObjectReference reference, SideColor sideColor)
    {
        var localSide = CodenamesGameMultiplayer.Instance.GetPlayerData().side;
        bool canSeeColor = true;
        //only spymasters can see the buttons color
        if (localSide == Side.BlueSideOperative || localSide == Side.RedSideOperative)
            canSeeColor = false;

        reference.TryGet(out NetworkObject obj);
        Button button = obj.GetComponentInParent<Button>();
        if (sideColor == SideColor.Blue)
        {
            if (canSeeColor)
                button.image.color = blue;
            blueSideWords.Add(button, false);
        }
        else if (sideColor == SideColor.Red)
        {
            if (canSeeColor)
                button.image.color = red; //red
            redSideWords.Add(button, false);
        }
    }

    public void OnPlayerGuessed(Button btn, SideColor playerSideColor)
    {
        if (blueSideWords.ContainsKey(btn))
        {
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.Blue);
            CheckIfPlayerGuessedRight(playerSideColor, SideColor.Blue);
            ShowButtonVisualServerRpc(btn.GetComponentInChildren<NetworkObject>(), SideColor.Blue);
        }
        else if (redSideWords.ContainsKey(btn))
        {
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.Red);
            CheckIfPlayerGuessedRight(playerSideColor, SideColor.Red);
            ShowButtonVisualServerRpc(btn.GetComponentInChildren<NetworkObject>(), SideColor.Red);
        }
        else
        {
            //guessed noncolor word
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.None);
            ChangeGameState();
        }

    }
    [ServerRpc(RequireOwnership = false)]
    private void ShowButtonVisualServerRpc(NetworkObjectReference reference, SideColor btnColor)
    {
        ShowButtonVisualClientRpc(reference, btnColor);
    }
    [ClientRpc]
    private void ShowButtonVisualClientRpc(NetworkObjectReference reference, SideColor btnColor)
    {

        reference.TryGet(out NetworkObject obj);
        Button button = obj.GetComponentInParent<Button>();
        if (btnColor == SideColor.Blue)
        {
            button.image.color = blue;
            blueSideWords[button] = true;
        }
        else if (btnColor == SideColor.Red)
        {
            button.image.color = red;
            redSideWords[button] = true;
        }
        // button will be not clickable anymore 
        button.enabled = false;
        button.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }

    private void CheckIfPlayerGuessedRight(SideColor playerSideColor, SideColor btnColor)
    {
        if (playerSideColor != btnColor)
        {
            //guessed wrong
            ChangeGameState();
        }
    }

    private void ChangeGameState()
    {
        GameStateManager.Instance.ChangeState();
    }
}
