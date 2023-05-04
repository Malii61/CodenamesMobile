using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ClueDisplayPanelUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI clueText;
    [SerializeField] private TextMeshProUGUI clueCountText;

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += GameStateManager_OnStateChanged;
        gameObject.SetActive(false);
    }

    private void GameStateManager_OnStateChanged(object sender, System.EventArgs e)
    {
    }

    public void ShowClue(string _clueText, string _clueCount)
    {
        ShowClueServerRpc(_clueText, _clueCount);
    }
    [ServerRpc(RequireOwnership = false)]
    private void ShowClueServerRpc(string _clueText, string _clueCount)
    {
        ShowClueClientRpc(_clueText, _clueCount);
    }
    [ClientRpc]
    private void ShowClueClientRpc(string _clueText, string _clueCount)
    {
        gameObject.SetActive(true);
        //Animasyon oynatýlabilir
        clueText.text = _clueText;
        clueCountText.text = _clueCount;
    }
}
