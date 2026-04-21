using Godot;
using System;

public partial class MoveIdle : MoveBase
{
    public override void _Ready()
    {
        this.moveType = MOVES.IDLE;
        base._Ready();
    }


    public override MOVES TransitionLogic(InputPackage inputPackage)
    {
        // Add if to check for inputs and to react accordingly
        if (inputPackage.actions.Contains(MOVES.INTERACTING) == true)
        {
            return MOVES.INTERACTING;
        }
        else if (inputPackage.actions.Contains(MOVES.MOVING) == true)
        {
            return MOVES.MOVING;
        }

        return base.TransitionLogic(inputPackage);
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        this.animationPlayer.Stop();
    }

    public override void Update(InputPackage inputPackage, double delta)
    {
        base.Update(inputPackage, delta);
        if (inputPackage.lastDirection == InputPackage.LOOK_DIRECTION.RIGHT)
        {
            this.animationPlayer.Play("idle_right");
        }
        else if (inputPackage.lastDirection == InputPackage.LOOK_DIRECTION.LEFT)
        {
            this.animationPlayer.Play("idle_left");
        }
        else if (inputPackage.lastDirection == InputPackage.LOOK_DIRECTION.DOWN)
        {
            this.animationPlayer.Play("idle_down");
        }
        else if (inputPackage.lastDirection == InputPackage.LOOK_DIRECTION.UP)
        {
            this.animationPlayer.Play("idle_up");
        }
    }
}
