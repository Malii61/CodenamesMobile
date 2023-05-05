using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

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
        startSide = (SideColor)UnityEngine.Random.Range(0, 1);
        if (startSide == SideColor.Blue)
            GameStateManager.Instance.SetState(State.BlueSpymasterGivesClue);
        else if (startSide == SideColor.Red)
            GameStateManager.Instance.SetState(State.RedSpymasterGivesClue);

    }
    private void SetWords()
    {
        List<TextMeshProUGUI> words = WordTableUI.Instance.GetRandomWords(WORD_COUNT);

        for (int i = 0; i <= WORD_COUNT / 2; i++)
        {
            if (startSide == SideColor.Blue)
            {
                SetButton(words[i].transform.GetComponentInParent<Button>(), SideColor.Blue);
            }
            else if (startSide == SideColor.Red)
            {
                SetButton(words[i].transform.GetComponentInParent<Button>(), SideColor.Red);
            }
        }
        for (int i = WORD_COUNT / 2 + 1; i < WORD_COUNT; i++)
        {
            if (startSide == SideColor.Blue)
            {
                SetButton(words[i].transform.GetComponentInParent<Button>(), SideColor.Red);
            }
            else if (startSide == SideColor.Red)
            {
                SetButton(words[i].transform.GetComponentInParent<Button>(), SideColor.Blue);
            }
        }
    }
    private void SetButton(Button button, SideColor sideColor)
    {
        SetButtonClientRpc(button.GetComponentInChildren<NetworkObject>(), sideColor);
    }
    [ClientRpc]
    private void SetButtonClientRpc(NetworkObjectReference reference, SideColor sideColor)
    {
        var localSide = CodenamesGameMultiplayer.Instance.GetPlayerData().side;
        //only spymasters can see the buttons color
        if (localSide == Side.BlueSideOperative || localSide == Side.RedSideOperative)
            return;
        reference.TryGet(out NetworkObject obj);
        Button button = obj.GetComponentInParent<Button>();
        if (sideColor == SideColor.Blue)
        {
            button.image.color = blue; 
            blueSideWords.Add(button, false);
        }
        else if (sideColor == SideColor.Red)
        {
            button.image.color = red; //red
            redSideWords.Add(button, false);
        }
    }
 
    public void OnPlayerGuessed(Button btn ,SideColor playerSideColor)
    {
        if (blueSideWords.ContainsKey(btn))
        {
            blueSideWords[btn] = true;
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.Blue);
            CheckIfPlayerGuessedRight(playerSideColor, SideColor.Red);
        }
        else if (redSideWords.ContainsKey(btn))
        {
            redSideWords[btn] = true;
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.Red);
            CheckIfPlayerGuessedRight(playerSideColor, SideColor.Red);
        }
        else
        {
            //guessed noncolor word
            OperativeManager.Instance.CheckGuessedButtonColor(SideColor.None);
            ChangeGameState();
        }

    }

    private void CheckIfPlayerGuessedRight(SideColor playerSideColor, SideColor btnColor)
    {
        if(playerSideColor != btnColor)
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
