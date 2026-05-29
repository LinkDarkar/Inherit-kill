using Godot;
using System;

public partial class TeacherNpc : BaseNpc
{
    public override void _Ready()
    {
        base._Ready();
        
        this.upperTimeLimit = 10;
        this.lowerTimeLimit = 3;
    }

    protected override void HandleAnimations()
    {
        // manejar animaciones profesor
    }

}
