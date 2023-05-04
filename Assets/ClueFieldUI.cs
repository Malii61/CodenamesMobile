using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueFieldUI : MonoBehaviour
{
    [SerializeField] private Button giveClueButton;
    [SerializeField] private TextMeshProUGUI clueText;
    [SerializeField] private TextMeshProUGUI clueCountText;
    [SerializeField] private ClueDisplayPanelUI clueDisplay;
    // Start is called before the first frame update
    private void Awake()
    {
        giveClueButton.onClick.AddListener(() => {
            OnClick_GiveClueButton(clueText.text,clueCountText.text);
        });
    }
    void Start()
    {
        GameStateManager.Instance.OnStateChanged += GameStateManager_OnStateChanged;
        gameObject.SetActive(false);
    }

    private void GameStateManager_OnStateChanged(object sender, System.EventArgs e)
    {
        Debug.Log(CodenamesGameMultiplayer.Instance.GetPlayerData().side);
        if (GameStateManager.Instance.GetState() == State.BlueSpymasterGivesClue &&
            CodenamesGameMultiplayer.Instance.GetPlayerData().side == Side.BlueSideSpymaster ||
            GameStateManager.Instance.GetState() == State.RedSpymasterGivesClue && 
            CodenamesGameMultiplayer.Instance.GetPlayerData().side == Side.RedSideSpymaster
            )
        {
            Debug.Log("obje aktif ediliyoor");
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnStateChanged -= GameStateManager_OnStateChanged;
    }

    public void OnClick_GiveClueButton(string clueText, string clueCountText)
    {
        clueDisplay.ShowClue(clueText,clueCountText);
    }
}
