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
        else if (inputPackage.actions.Contains(MOVES.ATTACKING) == true)
        {
            return MOVES.ATTACKING;
        }

        return base.TransitionLogic(inputPackage);
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
    }

    public override void Update(InputPackage inputPackage, double delta)
    {
        base.Update(inputPackage, delta);
    }
}
