using Godot;
using System;

public partial class Hitbox : Area2D
{
    public override void _Ready()
    {
        base._Ready();
        this.BodyEntered += this.OnHitboxBodyEntered;
    }

    private async void OnHitboxBodyEntered(Node2D body)
    {
        if (body is Npc npc)
        {
            await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
            npc.PlayDyingEffects();
            // ((Npc) body).PlayDyingEffects();
        }
    }
}
