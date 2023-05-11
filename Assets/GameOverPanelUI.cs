using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
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

    private void ScoreManager_OnRedTeamWon(object sender, ScoreManager.OnRedTeamWonEventArgs e)
    {
        gameObject.SetActive(true);
        SideColor sidecolor = CodenamesGameMultiplayer.Instance.GetLocalPlayerSideColor();
        if (sidecolor == SideColor.Blue)
        {
            gameOverText.text = e.blueSideGameOverText;
        }
        else
        {
            gameOverText.text = e.redSideGameOverText;
            anim.Play("GameOver");
        }
        if (IsServer)
            playAgainButton.gameObject.SetActive(true);
        GameStateManager.Instance.SetState(State.GameOver);
    }

    private void ScoreManager_OnBlueTeamWon(object sender, ScoreManager.OnBlueTeamWonEventArgs e)
    {
        gameObject.SetActive(true);
        SideColor sidecolor = CodenamesGameMultiplayer.Instance.GetLocalPlayerSideColor();
        if (sidecolor == SideColor.Blue)
        {
            gameOverText.text = e.blueSideGameOverText;
            anim.Play("GameOver");
        }
        else
        {
            gameOverText.text = e.redSideGameOverText;
        }
        if (IsServer)
            playAgainButton.gameObject.SetActive(true);
        GameStateManager.Instance.SetState(State.GameOver);
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
