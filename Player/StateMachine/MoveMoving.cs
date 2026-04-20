using Godot;
using System;

public partial class MoveMoving : MoveBase
{
    private float speed = 110f;

    public override void _Ready()
    {
        this.moveType = MOVES.MOVING;
        base._Ready();
    }

    public override MOVES TransitionLogic(InputPackage inputPackage)
    {
        if (inputPackage.actions.Contains(MOVES.INTERACTING) == true)
        {
            return MOVES.INTERACTING;
        }
        else if (inputPackage.actions.Contains(MOVES.MOVING) == true)
        {
            return MOVES.MOVING;
        }

        return MOVES.IDLE;
    }

    public override void Update(InputPackage inputPackage, double delta)
    {
        this.player.Velocity = new Vector2(inputPackage.movementDirection.X * this.speed, inputPackage.movementDirection.Y * this.speed);
        this.player.MoveAndSlide();
        GD.Print($"player velocity {this.player.Velocity}");
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }

    private Vector2 VelocityByInput(InputPackage inputPackage, float delta)
    {
        return Vector2.Zero;
    }
}
