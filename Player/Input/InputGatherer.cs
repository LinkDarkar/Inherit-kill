using Godot;
using System;

public partial class InputGatherer : Node
{
    private InputPackage inputPackage;

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
            this.inputPackage.actions.Add(MOVES.MOVING);
        }
        this.inputPackage.saveDirection();

        return this.inputPackage;
    }

}
