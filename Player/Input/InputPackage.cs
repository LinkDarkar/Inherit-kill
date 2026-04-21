
using Godot;
using Godot.Collections;

public partial class InputPackage : Node
{
    public Vector2 movementDirection = new Vector2();
    public Array<MOVES> actions = new Array<MOVES>();

    public enum LOOK_DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    public LOOK_DIRECTION lastDirection = LOOK_DIRECTION.DOWN;

    public override void _Ready()
    {
        base._Ready();
    }

    public void saveDirection()
    {
        if (movementDirection == Vector2.Zero)
        {
            lastDirection = LOOK_DIRECTION.DOWN;
        }
        else
        {
            if (Mathf.Abs(this.movementDirection.X) > Mathf.Abs(this.movementDirection.Y))
            {
                if (this.movementDirection.X > 0)
                {
                    lastDirection = LOOK_DIRECTION.RIGHT;
                }
                else
                {
                    lastDirection = LOOK_DIRECTION.LEFT;
                }
            }
            else
            {
                if (this.movementDirection.Y > 0)
                {
                    lastDirection = LOOK_DIRECTION.DOWN;
                }
                else
                {
                    lastDirection = LOOK_DIRECTION.UP;
                }
            }
        }
    }
}