using Godot;
using System;
using System.Transactions;

public partial class FedericoNpc : BaseNpc
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
        base.HandleAnimations();
    }
}
