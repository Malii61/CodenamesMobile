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
                infoText.text = "Hostun oyunu baþlatmasý bekleniyor. Bu sýrada takýmýný seçebilirsin.";
                break;
            case State.BlueOperativesGuessing:
                infoText.text = "Mavi takýmýn casuslarý tahmin ediyor...";
                break;
            case State.BlueSpymasterGivesClue:
                infoText.text = "Mavi takýmýn þefi ipucu veriyor...";
                break;
            case State.RedOperativesGuessing:
                infoText.text = "Kýrmýzý takýmýn casuslarý tahmin ediyor...";
                break;
            case State.RedSpymasterGivesClue:
                infoText.text = "Kýrmýzý takýmýn þefi ipucu veriyor...";
                break;
            case State.GameOver:
                infoText.text = "Oyun bitti.";
                break;
        }
    }
}
