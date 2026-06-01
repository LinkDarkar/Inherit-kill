using Godot;
using System;

public partial class Hitbox : Area2D
{
    [Export]
    private PlayerCharacter playerCharacter;

    public override void _Ready()
    {
        base._Ready();
        this.BodyEntered += this.OnHitboxBodyEntered;
    }

    private async void OnHitboxBodyEntered(Node2D body)
    {
        if (body is BaseNpc npc && this.playerCharacter.model.currentMove.moveType == MOVES.ATTACKING)
        {
            await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
            npc.PlayDyingEffects();
        }
        else if (body is BaseNpc npc1 && this.playerCharacter.model.currentMove.moveType == MOVES.INTERACTING)
        {
            npc1.PlayInteractionEffects();
        }
    }
}
