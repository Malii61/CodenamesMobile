using System;
using UnityEngine;
using UnityEngine.UI;

public class OperativeManager : MonoBehaviour
{
    public static OperativeManager Instance { get; private set; }

    public event EventHandler OnGuessedRedWord;
    public event EventHandler OnGuessedBlueWord;
    public event EventHandler<OnSelectedBlackWordEventArgs> OnGuessedBlackWord;
    public event EventHandler OnGuessedNonColorWord;

    [SerializeField] private Button finishGuessButton;
    private bool isGuessable;
    public class OnSelectedBlackWordEventArgs : EventArgs
    {
        public SideColor sideClr;
    }
    private void Awake()
    {
        Instance = this;
        finishGuessButton.onClick.AddListener(() => { OnClick_FinishGuessButton(); });
        finishGuessButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        GameStateManager.Instance.OnStateChanged += GameStateManager_OnStateChanged;
    }
    private void GameStateManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameStateManager.Instance.CanLocalPlayerGuess() && GameStateManager.Instance.GetState() != State.GameOver)
        {
            isGuessable = true;
        }
        else
        {
            isGuessable = false;
        }
        finishGuessButton.gameObject.SetActive(isGuessable);
    }
    public void OnClick_WordSelected(Button btn)
    {
        if (!isGuessable)
            return;
        Side side = CodenamesGameMultiplayer.Instance.GetPlayerData().side;
        SideColor localSideColor;
        if (side == Side.BlueSideOperative)
            localSideColor = SideColor.Blue;
        else
            localSideColor = SideColor.Red;
        CodenamesGameManager.Instance.OnPlayerGuessed(btn, localSideColor);
    }

    private void OnClick_FinishGuessButton()
    {
        GameStateManager.Instance.ChangeState();
    }
    public void CheckGuessedButtonColor(SideColor btnColor)
    {
        if (btnColor == SideColor.Blue)
            OnGuessedBlueWord?.Invoke(this, EventArgs.Empty);
        else if (btnColor == SideColor.Red)
            OnGuessedRedWord?.Invoke(this, EventArgs.Empty);
        else if (btnColor == SideColor.Grey)
            OnGuessedNonColorWord?.Invoke(this, EventArgs.Empty);
    }
    public void OnSelectedBlackWord(SideColor playerSideColor)
    {
        OnGuessedBlackWord?.Invoke(this, new OnSelectedBlackWordEventArgs { sideClr = playerSideColor });
    }
}
