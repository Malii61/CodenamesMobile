using System;
using Unity.Netcode;
using UnityEngine;
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
        Debug.Log(newValue);
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    public void SetState(State _state)
    {
        state.Value = _state;
    }
    public State GetState()
    {
        return state.Value;
    }
}
