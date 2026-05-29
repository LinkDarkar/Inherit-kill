using Godot;
using System;

public partial class FiesteroNpc : BaseNpc
{
    public override void _Ready()
    {
        base._Ready();
        
        this.upperTimeLimit = 15;
        this.lowerTimeLimit = 5;
    }

    protected override void HandleAnimations()
    {
        // manejar animaciones profesor
    }
}
