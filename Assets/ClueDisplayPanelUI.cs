using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections;
using System;
using System.Threading.Tasks;

public class ClueDisplayPanelUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI clueText;
    [SerializeField] private TextMeshProUGUI clueCountText;
    private Animator anim;
    private int changedStateCount;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += GameStateManager_OnStateChanged;
        gameObject.SetActive(false);
    }

    private void GameStateManager_OnStateChanged(object sender, EventArgs e)
    {
        changedStateCount++;
        if (changedStateCount == 3)
        {
            DisablePanelServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void DisablePanelServerRpc()
    {
        DisablePanelClientRpc();
    }

    [ClientRpc]
    private void DisablePanelClientRpc()
    {
        gameObject.SetActive(false);
    }

    public async void ShowClue(string _clueText, string _clueCount)
    {
        ShowClueServerRpc(_clueText, _clueCount);
        while (!gameObject.activeSelf)
        {
            await Task.Yield();
        }
        StartCoroutine(ChangeState());
    }
    private IEnumerator ChangeState()
    {
        yield return new WaitForSeconds(3);
        changedStateCount = 0;
        GameStateManager.Instance.ChangeState();
        HideClueServerRpc();
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
        anim.SetBool("Show", true);
        clueText.text = _clueText;
        clueCountText.text = _clueCount;
    }

    [ServerRpc(RequireOwnership = false)]
    private void HideClueServerRpc()
    {
        HideClueClientRpc();
    }

    [ClientRpc]
    private void HideClueClientRpc()
    {
        anim.SetBool("Show", false);
        anim.SetBool("Hide", true);
    }
}
