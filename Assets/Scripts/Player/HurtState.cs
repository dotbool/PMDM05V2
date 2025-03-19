using Unity.VisualScripting;
using UnityEngine;

public class HurtState : IState
{

    private PlayerController player;
    AudioClip soundClick;


    public HurtState(PlayerController player)
    {
        this.player = player;
        soundClick = Resources.Load<AudioClip>("Sounds/hurt-clip");

    }
    public void Enter()
    {
        player.Animator.SetBool("IsHurt", true);
        player.PlaySound(soundClick);
    }

    public void Exit()
    {
        player.Animator.SetBool("IsHurt", false);
    }

    public void Tick()
    {
        if (!player.IsHurt)
        {
            if ((player.IsPushLeft || player.IsPushRight) && player.IsGrounded)
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.runningState);
            }
            else if (!player.IsPushRight && !player.IsPushLeft && player.IsGrounded)
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
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
    }
}
