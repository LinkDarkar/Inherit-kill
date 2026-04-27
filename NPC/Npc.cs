using Godot;
using System;

public partial class Npc : CharacterBody2D
{
    private AnimationPlayer deathAnimationPlayer;
    private AudioStreamPlayer2D audioDyingSound;

    public override void _Ready()
    {
        base._Ready();
        this.deathAnimationPlayer = GetNode<Sprite2D>("Sprite2D").GetNode<AnimationPlayer>("AnimationPlayer");
        this.audioDyingSound = GetNode<AudioStreamPlayer2D>("AudioDyingSound");

    }

    private void PlayDyingSound()
    {
        this.audioDyingSound.Play();
    }

    private void PlayVanishEffect()
    {
        this.deathAnimationPlayer.Play("dying");
    }

    private void OnDeathAnimationFinished(StringName animationName)
    {
        this.QueueFree();
    }

    public void PlayDyingEffects()
    {
        this.PlayDyingSound();
        this.PlayVanishEffect();
        this.deathAnimationPlayer.AnimationFinished += this.OnDeathAnimationFinished;
    }
}
