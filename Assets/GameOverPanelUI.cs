using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerText;
    private Animator anim;
    [SerializeField] private Button playAgainButton;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        playAgainButton.onClick.AddListener(() => { OnClick_PlayAgainButton(); });
    }
    void Start()
    {
        ScoreManager.Instance.OnBlueTeamWon += ScoreManager_OnBlueTeamWon;
        ScoreManager.Instance.OnRedTeamWon += ScoreManager_OnRedTeamWon;
        gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
    }

    private void ScoreManager_OnRedTeamWon(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        GetComponent<Image>().color = CodenamesGameManager.Instance.red;
        winnerText.text = "Kýrmýzý takým casuslarý tüm kelimeleri tahmin etti !";
        anim.Play("GameOver");
        if (IsServer)
            playAgainButton.gameObject.SetActive(true);
    }

    private void ScoreManager_OnBlueTeamWon(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        GetComponent<Image>().color = CodenamesGameManager.Instance.blue;
        winnerText.text = "Mavi takým casuslarý tüm kelimeleri tahmin etti !";
        anim.Play("GameOver");
        if (IsServer)
            playAgainButton.gameObject.SetActive(true);
    }
    private void OnClick_PlayAgainButton()
    {
        Loader.LoadSceneOnNetwork(Loader.Scene.GameScene);
    }
    public override void OnDestroy()
    {
        ScoreManager.Instance.OnBlueTeamWon -= ScoreManager_OnBlueTeamWon;
        ScoreManager.Instance.OnRedTeamWon -= ScoreManager_OnRedTeamWon;
    }
}
