using Godot;
using System;

public partial class MoveAttacking : MoveBase
{
    public override void _Ready()
    {
        this.moveType = MOVES.ATTACKING;
        base._Ready();
    }

    public override MOVES TransitionLogic(InputPackage inputPackage)
    {
        return MOVES.ATTACKING;
    }

    public override void Update(InputPackage inputPackage, double delta)
    {
        // this.PlayAnimation(inputPackage);
    }

    public override void OnEnterState()
    {
        GD.Print("enters attack");
        base.OnEnterState();
    }

    public override void OnExitState()
    {
        GD.Print("exits attack");
        base.OnExitState();
    }
}
