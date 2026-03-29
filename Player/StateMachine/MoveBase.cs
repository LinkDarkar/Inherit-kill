using Godot;
using Godot.Collections;

public partial class MoveBase : Node
{
    public MOVES moveType;
    public CharacterBody2D player;
    public static Dictionary<MOVES, int> movesPriority = new Godot.Collections.Dictionary<MOVES, int>
    {
        {MOVES.IDLE, 1},
        {MOVES.MOVING, 2},
        {MOVES.INTERACTING, 10}
    };

    // TODO give inputs
    public virtual void Update(InputPackage input, double delta)
    {
    }

    // give input
    public virtual MOVES TransitionLogic(InputPackage inputPackage)
    {
        return MOVES.IDLE;
    }

    public virtual void OnEnterState()
    {
        return;
    }

    public virtual void OnExitState()
    {
        return;
    }

}