using Godot;
using System;

public partial class MoveAttacking : MoveBase
{
    private Area2D attackArea;

    public override void _Ready()
    {
        this.moveType = MOVES.ATTACKING;

        this.attackArea = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player").GetNode<Area2D>("Area2DAttack");
        this.attackArea.Visible = false;
        this.attackArea.Monitoring = false;
    }

    public override MOVES TransitionLogic(InputPackage inputPackage)
    {
        return MOVES.ATTACKING;
    }

    public override void Update(InputPackage inputPackage, double delta)
    {
    }

    public override void OnEnterState()
    {
        this.attackArea.Visible = true;
        this.attackArea.Monitoring = true;
        base.OnEnterState();
    }

    public override void OnExitState()
    {
        this.attackArea.Visible = false;
        this.attackArea.Monitoring = false;
        base.OnExitState();
    }
}
