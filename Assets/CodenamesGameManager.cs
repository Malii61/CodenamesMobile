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
            startGameButton.gameObject.SetActive(false);
    }
    public void OnClick_StartButton()
    {
        startGameButton.gameObject.SetActive(false);
        OnGameStarted?.Invoke(this, EventArgs.Empty);
        ChooseTheSideToStartFirst();
        SetWords();
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
    [ServerRpc(RequireOwnership = false)]
    private void SetButtonServerRpc(NetworkObjectReference reference, SideColor sideColor, bool onlySpymastersCanSee = true, bool addItems = true)
    {
        SetButtonClientRpc(reference, sideColor, onlySpymastersCanSee, addItems);
    }
    [ClientRpc]
    private void SetButtonClientRpc(NetworkObjectReference reference, SideColor sideColor, bool onlySpymastersCanSee = true, bool addItems = true)
    {
        var localSide = CodenamesGameMultiplayer.Instance.GetPlayerData().side;
        bool canSeeColor = true;
        //only spymasters can see the buttons color
        if (onlySpymastersCanSee && (localSide == Side.BlueSideOperative || localSide == Side.RedSideOperative))
            canSeeColor = false;

        reference.TryGet(out NetworkObject obj);
        Button button = obj.GetComponentInParent<Button>();
        if (sideColor == SideColor.Blue)
        {
            if (canSeeColor)
                button.image.color = blue;
            if (addItems)
                blueSideWords.Add(button, false);
        }
        else if (sideColor == SideColor.Red)
        {
            if (canSeeColor)
                button.image.color = red; //red
            if (addItems)
                redSideWords.Add(button, false);
        }
    }

    public void OnPlayerGuessed(Button btn, SideColor playerSideColor)
    {
        if (blueSideWords.ContainsKey(btn))
        {
            Debug.Log("game manager: kelime mavide var");

            blueSideWords[btn] = true;
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.Blue);
            CheckIfPlayerGuessedRight(playerSideColor, SideColor.Blue);
            SetButtonServerRpc(btn.GetComponentInChildren<NetworkObject>(), SideColor.Blue, false, false);
        }
        else if (redSideWords.ContainsKey(btn))
        {
            Debug.Log("game manager: kelime kýrmýzýda var");
            redSideWords[btn] = true;
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.Red);
            CheckIfPlayerGuessedRight(playerSideColor, SideColor.Red);
            SetButtonServerRpc(btn.GetComponentInChildren<NetworkObject>(), SideColor.Red, false, false);
        }
        else
        {
            //guessed noncolor word
            Debug.Log("game manager: kelime yok");
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.None);
            ChangeGameState();
        }

    }

    private void CheckIfPlayerGuessedRight(SideColor playerSideColor, SideColor btnColor)
    {
        if (playerSideColor != btnColor)
        {
            Debug.Log("check if player guessed right: yanlýþ seçti state deðiþiyor." + playerSideColor + " " + btnColor);
            //guessed wrong
            ChangeGameState();
        }
    }

    private void ChangeGameState()
    {
        GameStateManager.Instance.ChangeState();
    }
}
