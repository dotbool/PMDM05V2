using UnityEngine;

public class JumpingState : IState
{
    private PlayerController player;
    AudioClip soundClick;

    public JumpingState(PlayerController player)
    {
        this.player = player;
        soundClick = Resources.Load<AudioClip>("Sounds/jump-clip");

    }
    public void Enter()
    {
        player.Animator.SetBool("IsJumping", true);
        player.Animator.SetBool("IsGrounded", false);
        player.PlaySound(soundClick);

    }

    public void Exit()
    {

        player.Animator.SetBool("IsJumping", false);
    }

    //Cuando el estado IsJumping se produce un impulse en el move del player
    //por lo que hago la transición inmediatamente
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
        //player.IsJumping = false;
        //player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.ascendingState);


    }
}
