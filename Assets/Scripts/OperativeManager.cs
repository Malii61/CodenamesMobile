using System;
using UnityEngine;
using UnityEngine.UI;

public class OperativeManager : MonoBehaviour
{
    public static OperativeManager Instance { get; private set; }

    public event EventHandler<OnGuessedWordEventArgs> OnGuessedRedWord;
    public event EventHandler<OnGuessedWordEventArgs> OnGuessedBlueWord;
    public event EventHandler<OnGuessedWordEventArgs> OnGuessedNonColorWord;

    private bool isGuessable;
    private Button currentButton;
    private void Awake()
    {
        Instance = this;
    }
    public class OnGuessedWordEventArgs : EventArgs
    {
        public Button button;
    }
    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += GameStateManager_OnStateChanged;
    }
    private void GameStateManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameStateManager.Instance.CanLocalPlayerGuess())
        {
            isGuessable = true;
        }
        else
        {
            isGuessable = false;
        }
    }
    public void OnClick_WordSelected(Button btn)
    {
        if (!isGuessable)
            return;
        currentButton = btn;
        Debug.Log(btn.GetHashCode());
        Debug.Log(btn.GetInstanceID());
        Debug.Log("operative manager: kelime seçildi");
        Side side = CodenamesGameMultiplayer.Instance.GetPlayerData().side;
        SideColor localSideColor;
        if (side == Side.BlueSideOperative)
            localSideColor = SideColor.Blue;
        else
            localSideColor = SideColor.Red;
        CodenamesGameManager.Instance.OnPlayerGuessed(btn, localSideColor);
    }
    public void CheckGuessedButtonColor(SideColor btnColor)
    {
        if (btnColor == SideColor.Blue)
            OnGuessedBlueWord?.Invoke(this, new OnGuessedWordEventArgs { button = currentButton });
        else if (btnColor == SideColor.Red)
            OnGuessedRedWord?.Invoke(this, new OnGuessedWordEventArgs { button = currentButton });
        else
            OnGuessedNonColorWord?.Invoke(this, new OnGuessedWordEventArgs { button = currentButton });

    }
}
