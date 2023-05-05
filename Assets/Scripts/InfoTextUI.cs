using TMPro;
using UnityEngine;

public class InfoTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += GameStateManager_OnStateChanged;
    }

    private void GameStateManager_OnStateChanged(object sender, System.EventArgs e)
    {
        SetInfoText(GameStateManager.Instance.GetState());
    }
    private void SetInfoText(State state)
    {
        switch (state)
        {
            case State.WaitingToStart:
                infoText.text = "Hostun oyunu ba�latmas� bekleniyor. Bu s�rada tak�m�n� se�ebilirsin.";
                break;
            case State.BlueOperativesGuessing:
                infoText.text = "Mavi tak�m�n casuslar� tahmin ediyor...";
                break;
            case State.BlueSpymasterGivesClue:
                infoText.text = "Mavi tak�m�n �efi ipucu veriyor...";
                break;
            case State.RedOperativesGuessing:
                infoText.text = "K�rm�z� tak�m�n casuslar� tahmin ediyor...";
                break;
            case State.RedSpymasterGivesClue:
                infoText.text = "K�rm�z� tak�m�n �efi ipucu veriyor...";
                break;
            case State.GameOver:
                infoText.text = "Oyun bitti.";
                break;
        }
    }
}
