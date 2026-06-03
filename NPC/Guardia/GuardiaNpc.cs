using Godot;
using System;

public partial class GuardiaNpc : BaseNpc
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
        if (this.changeAnim == true)
        {
            // this only works if the animation number is not too high
            // or maybe... TODO add exclusive SPY counter to make sure there is not too much time
            // between spy exclusive behaviour
            int newAnimIndex = GD.RandRange(0, this.animationNames.Length - 1);
            this.currentAnim = this.animationNames[newAnimIndex];
            GD.Print($"current anim {this.currentAnim}");
            this.changeAnim = false;
        }

    }
}
