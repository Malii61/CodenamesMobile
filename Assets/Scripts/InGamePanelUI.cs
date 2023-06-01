using UnityEngine;
using UnityEngine.UI;

public class InGamePanelUI : MonoBehaviour
{
    [SerializeField] Sprite redTurnSprite;
    [SerializeField] Sprite blueTurnSprite;
    private Image backgroundImage;
    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
    }
    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += GameStateManager_OnStateChanged;
    }

    private void GameStateManager_OnStateChanged(object sender, System.EventArgs e)
    {
        State state = GameStateManager.Instance.GetState();
        if (state == State.RedSpymasterGivesClue)
            backgroundImage.sprite = redTurnSprite;
        else if (state == State.BlueSpymasterGivesClue)
            backgroundImage.sprite = blueTurnSprite;
    }


}
