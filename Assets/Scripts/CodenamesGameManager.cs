using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CodenamesGameManager : NetworkBehaviour
{
    public static CodenamesGameManager Instance { get; private set; }

    private const int WORD_COUNT = 18;

    [SerializeField] private Button startGameButton;
    [SerializeField] private Button resetGameButton;
    [SerializeField] private Sprite blueAgentSprite;
    [SerializeField] private Sprite redAgentSprite;
    [SerializeField] private Sprite greyAgentSprite;
    [SerializeField] private Sprite blackAgentSprite;
    public EventHandler OnGameStarted;

    private Dictionary<Button, bool> redSideWords = new Dictionary<Button, bool>();
    private Dictionary<Button, bool> blueSideWords = new Dictionary<Button, bool>();
    private Button blackWord;

    SideColor startSide;
    public Color blue = new Color32(2, 144, 204, 255);
    public Color red = new Color32(209, 45, 45, 255);
    public Color grey = new Color32(224, 224, 224, 255);

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
        //this function will be executed by server side only
        startSide = (SideColor)UnityEngine.Random.Range(0, 2);
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
        for (int i = WORD_COUNT / 2 + 1; i < WORD_COUNT -1; i++)
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
        SetButtonServerRpc(words[WORD_COUNT - 1].transform.parent.GetComponentInChildren<NetworkObject>(), SideColor.Black);
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
        else if (sideColor == SideColor.Black)
        {
            if (canSeeColor)
            {
                button.image.color = Color.black; //red
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            blackWord = button;
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
        else if(blackWord == btn)
        {
            OperativeManager.Instance.OnSelectedBlackWord(playerSideColor);
            ShowButtonVisualServerRpc(btn.GetComponentInChildren<NetworkObject>(), SideColor.Black);
        }
        else
        {
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.Grey);
            ChangeGameState();
            ShowButtonVisualServerRpc(btn.GetComponentInChildren<NetworkObject>(), SideColor.Grey);
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
        Image agentImage = button.transform.GetChild(2).GetComponent<Image>();
        agentImage.enabled = true;
        if (btnColor == SideColor.Blue)
        {
            button.image.color = blue;
            agentImage.sprite = blueAgentSprite;
            blueSideWords[button] = true;
        }
        else if (btnColor == SideColor.Red)
        {
            button.image.color = red;
            agentImage.sprite = redAgentSprite;
            redSideWords[button] = true;
        }
        else if(btnColor == SideColor.Grey)
        {
            button.image.color = grey;
            agentImage.sprite = greyAgentSprite;
        }
        else if(btnColor == SideColor.Black)
        {
            button.image.color = Color.black;
            agentImage.sprite = blackAgentSprite;
        }
        //disable the button
        button.enabled = false;

        // add a new listener to agent button
        Button agentButton = agentImage.GetComponent<Button>();
        agentButton.enabled = true;
        agentButton.onClick.AddListener(() =>
        {
            var tempColor = agentButton.image.color;

            if (tempColor.a == 0f)
                tempColor.a = 1f;

            else tempColor.a = 0f;

            agentButton.image.color = tempColor;
        });
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
