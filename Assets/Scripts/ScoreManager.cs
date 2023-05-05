using UnityEngine;
using TMPro;
using System;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI remainingWordCountTextOnRedTeam;
    [SerializeField] private TextMeshProUGUI remainingWordCountTextOnBlueTeam;

    public event EventHandler OnRedTeamWon;
    public event EventHandler OnBlueTeamWon;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        OperativeManager.Instance.OnGuessedBlueWord += OperativeManager_OnGuessedBlueWord;
        OperativeManager.Instance.OnGuessedRedWord += OperativeManager_OnGuessedRedWord;
    }
    public void SetRemainingWordCount(int count, SideColor sideColor)
    {
        if (sideColor == SideColor.Blue)
            remainingWordCountTextOnBlueTeam.text = count.ToString();
        else if (sideColor == SideColor.Red)
            remainingWordCountTextOnRedTeam.text = count.ToString();
    }
    private void OperativeManager_OnGuessedRedWord(object sender, OperativeManager.OnGuessedWordEventArgs e)
    {
        OnGuessedRedWordServerRpc();
    }
    [ServerRpc(RequireOwnership =false)]
    private void OnGuessedRedWordServerRpc()
    {
        OnGuessedRedWordClientRpc();
    }
    [ClientRpc]
    private void OnGuessedRedWordClientRpc()
    {
        int remainingWordCount = int.Parse(remainingWordCountTextOnRedTeam.text) - 1;
        if (remainingWordCount == 0)
        {
            OnRedTeamWon?.Invoke(this, EventArgs.Empty);
            Debug.Log("red kazandý");
        }
        else
            remainingWordCountTextOnRedTeam.text = remainingWordCount.ToString();
    }

    private void OperativeManager_OnGuessedBlueWord(object sender, OperativeManager.OnGuessedWordEventArgs e)
    {
        OnGuessedBlueWordServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnGuessedBlueWordServerRpc()
    {
        OnGuessedBlueWordClientRpc();
    }
    [ClientRpc]
    private void OnGuessedBlueWordClientRpc()
    {
        int remainingWordCount = int.Parse(remainingWordCountTextOnBlueTeam.text) - 1;
        if (remainingWordCount == 0)
        {
            OnBlueTeamWon?.Invoke(this, EventArgs.Empty);
            Debug.Log("blue kazandý");
        }
        else
            remainingWordCountTextOnBlueTeam.text = remainingWordCount.ToString();
    }
}
