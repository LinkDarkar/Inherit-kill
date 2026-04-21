using Godot;
using Godot.Collections;

public partial class InputPackage : Node
{
    public Vector2 movementDirection = new Vector2();
    public Array<MOVES> actions = new Array<MOVES>();

    public DIRECTION lastDirection = DIRECTION.DOWN;

    public override void _Ready()
    {
        base._Ready();
    }

    public void saveDirection()
    {
        if (Mathf.Abs(this.movementDirection.X) > Mathf.Abs(this.movementDirection.Y))
        {
            if (this.movementDirection.X > 0)
            {
                lastDirection = DIRECTION.RIGHT;
            }
            else
            {
                lastDirection = DIRECTION.LEFT;
            }
        }
        else
        {
            if (this.movementDirection.Y > 0)
            {
                lastDirection = DIRECTION.DOWN;
            }
            else
            {
                lastDirection = DIRECTION.UP;
            }
        }
    }
}