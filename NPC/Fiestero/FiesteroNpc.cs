using Godot;
using System;

public partial class FiesteroNpc : BaseNpc
{
    public override void _Ready()
    {
        base._Ready();
        
        this.upperTimeLimit = 20;
        this.lowerTimeLimit = 5;
    }

    protected override void HandleAnimations()
    {
        if (this.changeAnim == true)
        {
            // or maybe... TODO add exclusive SPY counter to make sure there is not too much time
            // between spy exclusive behaviour
            int newAnimIndex = GD.RandRange(0, this.animationNames.Length - 1);
            this.currentAnim = this.animationNames[newAnimIndex];
            this.changeAnim = false;
        }
    }
}
