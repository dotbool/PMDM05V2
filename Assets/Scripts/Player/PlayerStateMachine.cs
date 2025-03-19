using System;
using UnityEngine;

public class PlayerStateMachine
{
    public IState CurrentState { get; private set; }
    public IdleState idleState;
    public RunningState runningState;
    public JumpingState jumpingState;
    public FallingState fallingState;
    public HurtState hurtState;

    public event Action<IState> StateChanged;

    public PlayerStateMachine(PlayerController player)
    {
        idleState = new IdleState(player);
        runningState = new RunningState(player);
        jumpingState = new JumpingState(player);
        fallingState = new FallingState(player);
        hurtState = new HurtState(player);
    }

    public void Initialize(IState state)
    {
        CurrentState = state;
        state.Enter();

        // notificamos el cambio de estado
        StateChanged?.Invoke(state);
    }

    // exit this state and enter another
    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();

        // notify other objects that state has changed
        StateChanged?.Invoke(nextState);
    }

    // Ejecutamos el estado. Método llamado en el update del player
    // para que se ejecute el currentState en cada frame
    public void Execute()
    {
        if (CurrentState != null)
        {
            Debug.Log(CurrentState);
            CurrentState.Tick();
        }
    }

}
