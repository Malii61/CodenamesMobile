using System;
using Unity.Netcode;
public enum State
{
    WaitingToStart,
    GamePlaying,
    RedSpymasterGivesClue,
    RedOperativesGuessing,
    BlueSpymasterGivesClue,
    BlueOperativesGuessing,
    GameOver,
}
public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnPlayerOrderChanged;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private void Awake()
    {
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    public void SetState(State _state)
    {
        SetStateServerRpc(_state);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetStateServerRpc(State _state)
    {
        state.Value = _state;
    }

    public State GetState()
    {
        return state.Value;
    }
    public void ChangeState()
    {
        switch (state.Value)
        {
            case State.RedSpymasterGivesClue:
                SetState(State.RedOperativesGuessing);
                break;
            case State.RedOperativesGuessing:
                SetState(State.BlueSpymasterGivesClue);
                break;
            case State.BlueSpymasterGivesClue:
                SetState(State.BlueOperativesGuessing);
                break;
            case State.BlueOperativesGuessing:
                SetState(State.RedSpymasterGivesClue);
                break;
        }
    }
    public bool CanLocalPlayerGuess()
    {
        if (state.Value == State.BlueOperativesGuessing &&
                 CodenamesGameMultiplayer.Instance.GetPlayerData().side == Side.BlueSideOperative ||
                 state.Value == State.RedOperativesGuessing &&
                 CodenamesGameMultiplayer.Instance.GetPlayerData().side == Side.RedSideOperative)
            return true;
        return false;
    }
    public bool CanLocalPlayerGiveClue()
    {
        if (state.Value == State.BlueSpymasterGivesClue &&
                CodenamesGameMultiplayer.Instance.GetPlayerData().side == Side.BlueSideSpymaster ||
                state.Value == State.RedSpymasterGivesClue &&
                CodenamesGameMultiplayer.Instance.GetPlayerData().side == Side.RedSideSpymaster)
            return true;
        return false;
    }
}
