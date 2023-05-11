using UnityEngine;
using TMPro;
using System;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI remainingWordCountTextOnRedTeam;
    [SerializeField] private TextMeshProUGUI remainingWordCountTextOnBlueTeam;

    public event EventHandler<OnRedTeamWonEventArgs> OnRedTeamWon;
    public class OnRedTeamWonEventArgs : EventArgs
    {
        public string redSideGameOverText;
        public string blueSideGameOverText;
    }
    public event EventHandler<OnBlueTeamWonEventArgs> OnBlueTeamWon;
    public class OnBlueTeamWonEventArgs : EventArgs
    {
        public string redSideGameOverText;
        public string blueSideGameOverText;
    }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        OperativeManager.Instance.OnGuessedBlueWord += OperativeManager_OnGuessedBlueWord;
        OperativeManager.Instance.OnGuessedRedWord += OperativeManager_OnGuessedRedWord;
        OperativeManager.Instance.OnGuessedBlackWord += OperativeManager_OnGuessedBlackWord;
    }

    private void OperativeManager_OnGuessedBlackWord(object sender, OperativeManager.OnSelectedBlackWordEventArgs e)
    {
        OnGuessedBlackWordServerRpc(e.sideClr);
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnGuessedBlackWordServerRpc(SideColor sideColor)
    {
        OnGuessedBlackWordClientRpc(sideColor);
    }
    [ClientRpc]
    private void OnGuessedBlackWordClientRpc(SideColor sideColor)
    {
        if (sideColor == SideColor.Blue)
        {
            OnRedTeamWon?.Invoke(this, new OnRedTeamWonEventArgs
            {
                blueSideGameOverText = "Kaybettiniz.. Suikast�iyi buldunuz",
                redSideGameOverText = "Kazand�n�z!! Mavi tak�m�n casuslar� suikast�iyi buldu."
            });
        }
        else if (sideColor == SideColor.Red)
        {
            OnBlueTeamWon?.Invoke(this, new OnBlueTeamWonEventArgs
            {
                blueSideGameOverText = "Kazand�n�z!! Mavi tak�m�n casuslar� suikast�iyi buldu.",
                redSideGameOverText = "Kaybettiniz.. Suikast�iyi buldunuz"
            });
        }
    }
    public void SetRemainingWordCount(int count, SideColor sideColor)
    {
        if (sideColor == SideColor.Blue)
            remainingWordCountTextOnBlueTeam.text = count.ToString();
        else if (sideColor == SideColor.Red)
            remainingWordCountTextOnRedTeam.text = count.ToString();
    }
    private void OperativeManager_OnGuessedRedWord(object sender, EventArgs e)
    {
        OnGuessedRedWordServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnGuessedRedWordServerRpc()
    {
        OnGuessedRedWordClientRpc();
    }
    [ClientRpc]
    private void OnGuessedRedWordClientRpc()
    {
        int remainingWordCount = int.Parse(remainingWordCountTextOnRedTeam.text) - 1;
        remainingWordCountTextOnRedTeam.text = remainingWordCount.ToString();
        if (remainingWordCount == 0)
        {
            OnRedTeamWon?.Invoke(this, new OnRedTeamWonEventArgs
            {
                blueSideGameOverText = "Kaybettiniz.. K�rm�z� tak�m�n casuslar� t�m kelimeleri buldu.",
                redSideGameOverText = "Kazand�n�z!! Casuslar t�m kelimeleri tahmin etti."
            });
        }

    }

    private void OperativeManager_OnGuessedBlueWord(object sender, EventArgs e)
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
        remainingWordCountTextOnBlueTeam.text = remainingWordCount.ToString();
        if (remainingWordCount == 0)
        {
            OnBlueTeamWon?.Invoke(this, new OnBlueTeamWonEventArgs
            {
                blueSideGameOverText = "Kazand�n�z!! Casuslar t�m kelimeleri tahmin etti.",
                redSideGameOverText = "Kaybettiniz.. Mavi tak�m�n casuslar� t�m kelimeleri buldu."
            });
        }
    }
}
