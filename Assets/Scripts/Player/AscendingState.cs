using UnityEngine;

public class AscendingState : IState
{

    private PlayerController player;

    public AscendingState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.Animator.SetBool("IsAscending", true);
        player.Animator.SetBool("IsGrounded", false);

    }

    public void Exit()
    {
        player.Animator.SetBool("IsAscending", false);

    }

    public void Tick()
    {

        if (!player.IsPushRight && !player.IsPushLeft && player.IsGrounded)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }
        else if (player.IsFalling)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.fallingState);
        }

    }
}
