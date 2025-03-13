using Unity.VisualScripting;
using UnityEngine;

public class ClimbingState: IState
{
    private PlayerController player;

    public ClimbingState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Tick()
    {
    }
}
