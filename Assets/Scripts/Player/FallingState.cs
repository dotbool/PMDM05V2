using UnityEngine;

public class FallingState: IState
{
    private PlayerController player;

    public FallingState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.Animator.SetBool("IsFalling", true);
        player.Animator.SetBool("IsGrounded", false);

    }

    public void Exit()
    {
        player.Animator.SetBool("IsFalling", false);
        player.Animator.SetBool("IsGrounded", true);

    }
    public void Tick()
    {
        if (player.IsHurt)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hurtState);
        }
        else if (!player.IsPushRight && !player.IsPushLeft && player.IsGrounded)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }
        else if ((player.IsPushRight || player.IsPushLeft) && player.IsGrounded)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.runningState);

        }
    }
}
