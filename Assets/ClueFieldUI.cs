using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueFieldUI : MonoBehaviour
{
    [SerializeField] private Button giveClueButton;
    [SerializeField] private TextMeshProUGUI clueText;
    [SerializeField] private TMP_Dropdown clueCountDropdown;
    [SerializeField] private ClueDisplayPanelUI clueDisplay;
    // Start is called before the first frame update
    private void Awake()
    {
        giveClueButton.onClick.AddListener(() =>
        {
            OnClick_GiveClueButton(clueText.text, clueCountDropdown.value + 1);
        });
    }
    void Start()
    {
        GameStateManager.Instance.OnStateChanged += GameStateManager_OnStateChanged;
        gameObject.SetActive(false);
    }
    private void GameStateManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameStateManager.Instance.CanLocalPlayerGiveClue())
        {
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

    public void OnClick_GiveClueButton(string clueText, int clueCount)
    {
        clueDisplay.ShowClue(clueText, clueCount.ToString());
    }
}
