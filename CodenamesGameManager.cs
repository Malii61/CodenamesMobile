using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class CodenamesGameManager : NetworkBehaviour
{
    public static CodenamesGameManager Instance { get; private set; }
    [SerializeField] private Button startGameButton;
    public EventHandler OnGameStarted;
    private Dictionary<Button, bool> redSideWords = new Dictionary<Button, bool>();
    private Dictionary<Button, bool> blueSideWords = new Dictionary<Button, bool>();
    private const int WORD_COUNT = 17;
    SideColor startSide;
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
        startSide = (SideColor) UnityEngine.Random.Range(-1, 1);
    }
    private void SetWords()
    {
        List<TextMeshProUGUI> words = WordTableUI.Instance.GetRandomWords(WORD_COUNT);
        for (int i = 0; i < WORD_COUNT + 1 / 2; i++)
        {
            if(startSide == SideColor.Blue)
            {
                blueSideWords.Add(words[i].transform.parent.GetComponent<Button>(), false);
            }
        }
    }
}
