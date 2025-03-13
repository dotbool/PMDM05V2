using System.Collections;
using UnityEngine;


public class RunningState : IState
{
    private PlayerController player;
    AudioClip soundClick = Resources.Load<AudioClip>("Sounds/run-clip");
    float timeClip;
    public RunningState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.Animator.SetBool("IsRunning", true);
        player.Animator.SetBool("IsGrounded", true);

    }

    public void Exit()
    {
        player.Animator.SetBool("IsRunning", false);
        player.StopSound();

    }

    public void Tick()
    {
        timeClip -= Time.deltaTime;

        if (timeClip <= 0)
        {
            player.PlaySound(soundClick);
            timeClip = soundClick.length;
        }

        if (!player.IsPushRight && !player.IsPushLeft && player.IsGrounded)
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
