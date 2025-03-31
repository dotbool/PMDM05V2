using UnityEngine;

public class JumpingState : IState
{
    private PlayerController player;
    AudioClip soundClip;

    public JumpingState(PlayerController player)
    {
        this.player = player;
        soundClip = Resources.Load<AudioClip>("Sounds/jump-clip");

    }
    public void Enter()
    {
        player.Animator.SetBool("IsJumping", true);
        player.Animator.SetBool("IsGrounded", false);
        player.PlaySound(soundClip);

    }

    public void Exit()
    {

        player.Animator.SetBool("IsJumping", false);
    }

    //Cuando el estado IsJumping se produce un impulse en el move del player
    //por lo que hago la transición inmediatamente
    public void Tick()
    {
        if (player.IsHurt)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hurtState);
        }
        else if ((!player.IsPushRight || !player.IsPushLeft) && player.IsGrounded)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }
        else if ((player.IsPushLeft || player.IsPushRight ) && player.IsGrounded)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.runningState);
        }
        else if (player.IsFalling)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.fallingState);
        }

    }
}
