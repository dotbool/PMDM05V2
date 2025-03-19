using UnityEngine;

public class IdleState : IState
{
    private PlayerController player;

    public IdleState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.Animator.SetBool("IsIdle", true);
        player.Animator.SetBool("IsGrounded", true);

    }

    public void Tick()
    {

        if ((player.IsPushLeft  || player.IsPushRight)  && player.IsGrounded)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.runningState);
        }
        else if (player.IsJumping)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.jumpingState);
        }
        else if (player.IsFalling)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.fallingState);
        }
    }

    public void Exit()
    {
        player.Animator.SetBool("IsIdle", false);

    }
}
