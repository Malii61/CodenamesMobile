using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class WordTableUI : NetworkBehaviour
{
    public static WordTableUI Instance { get; private set; }
    [SerializeField] List<TextMeshProUGUI> wordTexts;
    private List<string> wordList;
    private bool executeOnce;
    private void Awake()
    {
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        CodenamesGameManager.Instance.OnGameStarted += ShowWord;
    }

    private void ShowWord(object sender, EventArgs e)
    {
        Debug.Log("Kelimeler gösteriliyor");
    }
    public override void OnDestroy()
    {
        CodenamesGameManager.Instance.OnGameStarted -= ShowWord;
    }

    private void Update()
    {
        if (!IsServer || executeOnce)
            return;
        if (Input.GetKeyDown(KeyCode.T))
        {
            executeOnce = true;
            wordList = WordGenerator.GenerateRandomWordList(wordTexts.Count);
            for (int i = 0; i < wordList.Count; i++)
            {
                FixedString64Bytes word = wordList[i];
                GenerateRandomListClientRpc(i, word);
            }
        }
    }
    [ClientRpc]
    private void GenerateRandomListClientRpc(int order, FixedString64Bytes word)
    {
        wordTexts[order].text = word.ToString();
    }
    public void OnClick_OptionButton(TextMeshProUGUI word)
    {
        OnClick_WordServerRpc(word.text);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnClick_WordServerRpc(string word)
    {
        OnClick_WordClientRpc(word);
    }
    [ClientRpc]
    private void OnClick_WordClientRpc(string word)
    {
        Debug.Log("(CLIENT) þuna bastýn: " + word);
    }
    public List<TextMeshProUGUI> GetRandomWords(int wordCount)
    {
        System.Random rng = new System.Random();
        List<TextMeshProUGUI> shuffledList = wordTexts.OrderBy(a => rng.Next()).ToList();
        List<TextMeshProUGUI> res = shuffledList.GetRange(0, wordCount);
        return res;
    }
}