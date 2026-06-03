using Godot;
using System;
using System.ComponentModel;

public partial class JokerMusicianNpc : BaseNpc
{
    [Export] protected PlayerCharacter player = null;

    private float distance = 0f;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // check distance to player
        if (player == null)
        {
            return;
        }

        this.distance = this.GlobalPosition.DistanceTo(this.player.GlobalPosition);
        GD.Print($" dist {this.distance}");
    }

    protected override void HandleAnimations()
    {
        // base.HandleAnimations();
        if (this.animationNames == null)
		{
			GD.Print($"{this.Name} has no animations to choose from");
			return;
		}

        if (this.distance <= 30f && this.isSpy == false)
        {
            this.currentAnim = "playing_music_down";
        }
    }

}
