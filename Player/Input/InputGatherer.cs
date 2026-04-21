using Godot;
using System;

public partial class InputGatherer : Node
{
    private InputPackage inputPackage;
    private InputPackage.LOOK_DIRECTION lastDirection;

    public override void _Ready()
    {
        this.inputPackage = new InputPackage();
    }

    public InputPackage GatherInputs()
    {
        this.inputPackage = new InputPackage();

        this.inputPackage.movementDirection = Input.GetVector("player_move_left", "player_move_right", "player_move_forward", "player_move_backward");
        if (this.inputPackage.movementDirection != Vector2.Zero)
        {
            this.inputPackage.saveDirection();
            this.lastDirection = this.inputPackage.lastDirection;
            this.inputPackage.actions.Add(MOVES.MOVING);
        }
        else
        {
            this.inputPackage.lastDirection = this.lastDirection;
        }

        return this.inputPackage;
    }

}
