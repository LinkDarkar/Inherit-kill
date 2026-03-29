
using Godot;
using Godot.Collections;

public partial class InputPackage : Node
{
    public Vector2 movementDirection = new Vector2();
    public Array<MOVES> actions = new Array<MOVES>();

    public override void _Ready()
    {
        base._Ready();
    }
}